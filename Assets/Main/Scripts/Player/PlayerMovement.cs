using System;
using System.Collections.Generic;
using Main.Scripts.Utilities;
using Unity.Netcode;
using UnityEngine;

// Netcode from https://www.youtube.com/watch?v=-lGsuCEWkM0
namespace Main.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Physics parameters")]
        public float moveForce = 30f;
        public float maxControlSpeed = 10f;
        public float controlInAirMultiplier = 0.75f;
        public float fallingMultiplier = 15f;
        
        [Header("Ground Layer")]
        public LayerMask groundLayer;
        
        [Header("Player Manager")]
        [SerializeField]
        private PlayerManager playerManager;
        
        private Rigidbody2D _rb;
        private NetworkTimer _timer;
        
        // Netcode
        private const float KServerTickRate = 60f; // 60 FPS
        private const int KBufferSize = 1024;
        
        // Client
        private CircularBuffer<StatePayload> _clientStateBuffer;
        private CircularBuffer<InputPayLoad> _clientInputBuffer;
        private StatePayload _lastServerState;
        private StatePayload _lastProcessedState;
        
        // Server
        private CircularBuffer<StatePayload> _serverStateBuffer;
        private Queue<InputPayLoad> _serverInputQueue;
        [SerializeField] float reconciliationThreshold = 10f;
        
        private void Awake()
        {
            _timer = new NetworkTimer(KServerTickRate);
            _clientStateBuffer = new CircularBuffer<StatePayload>(KBufferSize);
            _clientInputBuffer = new CircularBuffer<InputPayLoad>(KBufferSize);
            _serverStateBuffer = new CircularBuffer<StatePayload>(KBufferSize);
            _serverInputQueue = new Queue<InputPayLoad>();
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public struct InputPayLoad : INetworkSerializable
        {
            public int Tick;
            public Vector2 InputVector;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Tick);
                serializer.SerializeValue(ref InputVector);
            }
        }

        public struct StatePayload : INetworkSerializable
        {
            public int Tick;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector2 Velocity;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Tick);
                serializer.SerializeValue(ref Position);
                serializer.SerializeValue(ref Rotation);
                serializer.SerializeValue(ref Velocity);
            }
        }

        private void Update()
        {
            _timer.Update(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            if (!IsOwner) return;
            while (_timer.ShouldTick())
            {
                HandleClientTick();
                HandleServerTick();
            }
        }

        private void HandleServerTick()
        {
            int bufferIndex = -1;
            while (_serverInputQueue.Count > 0)
            {
                InputPayLoad inputPayload = _serverInputQueue.Dequeue();
                bufferIndex = inputPayload.Tick %  KBufferSize;
                
                StatePayload statePayload = SimulateMovement(inputPayload);
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }

            if (bufferIndex == -1) return;
            SendToClientRpc(_serverStateBuffer.Get(bufferIndex));
        }

        StatePayload SimulateMovement(InputPayLoad input)
        {
            Physics.simulationMode = SimulationMode.Script;
            
            HandleMovement(input.InputVector);
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.simulationMode = SimulationMode.FixedUpdate;

            return new StatePayload()
            {
                Tick = input.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = _rb.linearVelocity,
            };
        }
        [ClientRpc]
        private void SendToClientRpc(StatePayload statePayload)
        {
            if (!IsOwner) return;
            _lastServerState = statePayload;
        }

        private void HandleClientTick()
        {
            if (!IsClient) return;

            int currentTick = _timer.CurrentTick;
            int bufferIndex = currentTick % KBufferSize;

            InputPayLoad clientInput = new InputPayLoad()
            {
                Tick = currentTick,
                InputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            };
            
            _clientInputBuffer.Add(clientInput, bufferIndex);
            SendToServerRpc(clientInput);
            
            StatePayload statePayload = ProcessMovement(clientInput);
            _clientStateBuffer.Add(statePayload,  bufferIndex);
            
            HandleServerReconciliation();
        }

        private void HandleServerReconciliation()
        {
            if (!ShouldReconcile()) return;

            var bufferIndex = _lastServerState.Tick % KBufferSize;
            if (bufferIndex - 1 < 0) return;  // Not enough information to reconcile

            // Host RPCs execute immediately, so we can use the last server state
            var rewindState = IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState;
            var positionError = Vector3.Distance(rewindState.Position, _clientStateBuffer.Get(bufferIndex).Position);

            if (positionError > reconciliationThreshold)
            {
                ReconcileState(rewindState);
            }
        }

        private void ReconcileState(StatePayload rewindState)
        {
            transform.position = rewindState.Position;
            transform.rotation = rewindState.Rotation;
            _rb.linearVelocity = rewindState.Velocity;

            if (!rewindState.Equals(_lastServerState)) return;
            _clientStateBuffer.Add(rewindState, rewindState.Tick);
            
            // Replay all inputs from rewind to current state
            int tickToReplay = _lastServerState.Tick;

            while (tickToReplay < _timer.CurrentTick)
            {
                int bufferIndex =  tickToReplay % KBufferSize;
                StatePayload statePayload = ProcessMovement(_clientInputBuffer.Get(bufferIndex));
                _clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        private bool ShouldReconcile()
        {
            bool isNewServerState = !_lastServerState.Equals(null);
            bool isLastStateUndefinedOrDifferent = _lastProcessedState.Equals(null)
                                                   || !_lastProcessedState.Equals(_lastServerState);
            return isNewServerState && isLastStateUndefinedOrDifferent;
        }

        [ServerRpc]
        private void SendToServerRpc(InputPayLoad clientInput)
        {
            _serverInputQueue.Enqueue(clientInput);
        }
        
        private StatePayload ProcessMovement(InputPayLoad input)
        {
            HandleMovement(input.InputVector);

            return new StatePayload()
            {
                Tick = input.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = _rb.linearVelocity
            };
        }

        private void HandleMovement(Vector2 input)
        {
            if (playerManager.isDead) return;
            
            bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, .6f, groundLayer);
            if (_rb.linearVelocityY < 0 && !isGrounded)
            {
                _rb.linearVelocityY -= fallingMultiplier * _timer.MinTimeBetweenTicks / (1f / Time.deltaTime);
            }
            
            if (Mathf.Abs(input.x) < 0.01f) return;
            
            float controlMultiplier = isGrounded ? 1f : controlInAirMultiplier;  // Decrease air horizontal control

            // Only add force if we're under max control speed in input direction
            if (Mathf.Approximately(Mathf.Sign(input.x), Mathf.Sign(_rb.linearVelocityX)) &&
                !(Mathf.Abs(_rb.linearVelocityX) < maxControlSpeed)) return;
            
            float velocityInInputDir = _rb.linearVelocityX * input.x;
            float forceScale = Mathf.Clamp01(1f - (velocityInInputDir / maxControlSpeed));
            
            // I won't lie gpt cooked this weird equation, but it feels good so we move.
            _rb.AddForce(Vector2.right * input * (moveForce * forceScale * controlMultiplier), ForceMode2D.Force);
        }
    }
}