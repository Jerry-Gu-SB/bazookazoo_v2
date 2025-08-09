using System;
using Main.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Game_Managers
{
    public enum GameState
    {
        Idle,
        Connecting,
        LobbyLoading,
        LobbyReady,
        MapLoading,
        GameReady,
        GameRunning
    }

    public class GameStateManager : NetworkBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public static event Action<GameState> GameStateChanged;

        [SerializeField] private SceneLoadingManager sceneLoader;
        private string _selectedMap = MapNames.Lobby;

        public GameState CurrentState { get; private set; } = GameState.Idle;
        
        private readonly NetworkVariable<GameState> _networkedGameState = new(
            GameState.Idle,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;
            _networkedGameState.OnValueChanged += OnGameStateChanged;   
            OnGameStateChanged(GameState.Idle, _networkedGameState.Value); // Catch up to current state
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetSelectedMap(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
            {
                Debug.LogWarning("Map name is null or empty. Defaulting to Lobby.");
                _selectedMap = MapNames.Lobby;
            }
            else
            {
                _selectedMap = mapName;
            }
        }

        public void TransitionToState(GameState newState)
        {
            if (!IsServer) return;

            Debug.Log($"[GameStateManager] Transitioning from {CurrentState} to {newState}");

            _networkedGameState.Value = newState; // Triggers OnValueChanged on clients

            HandleState(newState);

            GameStateChanged?.Invoke(newState);
        }
        
        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            CurrentState = newState;

            if (IsServer) return;
            Debug.Log($"[GameStateManager] Client received GameState: {newState}");
            GameStateChanged?.Invoke(newState);
        }


        private void HandleState(GameState state)
        {
            switch (state)
            {
                case GameState.Idle:
                case GameState.Connecting:
                case GameState.GameRunning:
                case GameState.GameReady:
                case GameState.LobbyReady:
                    Debug.Log($"[GameStateManager] State: {state}");
                    break;

                case GameState.LobbyLoading:
                    if (!IsServer) return;
                    sceneLoader.LoadScene(MapNames.Lobby, () => TransitionToState(GameState.LobbyReady));
                    break;

                case GameState.MapLoading:
                    if (!IsServer) return;
                    if (string.IsNullOrEmpty(_selectedMap)) _selectedMap = MapNames.Lobby;
                    sceneLoader.LoadScene(_selectedMap, () => TransitionToState(GameState.GameReady));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
