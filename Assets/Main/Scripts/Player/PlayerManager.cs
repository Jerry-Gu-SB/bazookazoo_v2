using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Main.Scripts.Game_Managers;
using Main.Scripts.UI_Scripts;
using Main.Scripts.World_Objects;
using Unity.Collections;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        [Header("Player Properties")]
        public NetworkVariable<FixedString32Bytes> username = new NetworkVariable<FixedString32Bytes>(
            "",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
        public float maxHealth = 100f;
        public float playerHeath;
        public int playerKills = 0;
        public int playerDeaths = 0;
        public bool isDead = false;
        public float respawnTimer = 0;
        public bool isInvincible;

        [Header("Player Components")]
        [SerializeField] private Canvas playerCanvas;
        [SerializeField] private Rigidbody2D playerRigidbody2D;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        
        // Player Death Properties
        private const int RespawnTime = 3;
        private int _playerLayer;
        private int _terrainLayer;
        
        // Player Respawn Properties
        private const int InvincibilityTime = 2;
        private float _invincibilityTimer = 0;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerHeath = maxHealth;
            playerCanvas.enabled = IsLocalPlayer;
            GameStateManager.GameStateChanged += HandleGameState;
            
            if (IsClient && GameStateManager.Instance != null)
            {
                HandleGameState(GameStateManager.Instance.CurrentState);
            }
            if (IsOwner)
            {
                username.Value = NetworkUIManager.LocalPlayerUsername; 
            }
        }
        private void Awake()
        {
            _playerLayer = LayerMask.NameToLayer("Player");
            _terrainLayer = LayerMask.NameToLayer("Terrain");
        }
        public override void OnDestroy()
        {
            GameStateManager.GameStateChanged -= HandleGameState;
            base.OnDestroy();
        }

        private void Update()
        {
            if (playerHeath <= 0)
            {
                isDead = true;
            }
            if (isDead)
            {
                respawnTimer += Time.deltaTime;

                playerSpriteRenderer.enabled = false;
                SetDeadCollisions(true);
                
                if (!(respawnTimer >= RespawnTime)) return;
                playerSpriteRenderer.enabled = true;
                SetDeadCollisions(false);
                isDead = false;
                respawnTimer = 0;
                Respawn();
            }

            if (isInvincible)
            {
                _invincibilityTimer += Time.deltaTime;
                playerHeath = maxHealth;
                if (!(_invincibilityTimer >= InvincibilityTime)) return;
                isInvincible = false;
                _invincibilityTimer = 0;
            }
        }

        private void HandleGameState(GameState state)
        {
            if (state is GameState.GameReady or GameState.LobbyReady)
            {
                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            playerHeath = maxHealth;
            playerKills = 0;
            Respawn();
        }

        private void Respawn()
        {
            playerHeath = maxHealth;
            playerRigidbody2D.linearVelocity = Vector2.zero;
            isInvincible = true;
            StartCoroutine(WaitForSpawnerAndRespawn());
        }

        private IEnumerator WaitForSpawnerAndRespawn()
        {
            const float timeout = 5f;
            float timer = 0f;
            SpawnPointManager spawnPointManager = null;

            while (!(spawnPointManager = FindFirstObjectByType<SpawnPointManager>()))
            {
                timer += Time.deltaTime;
                if (timer > timeout)
                {
                    transform.position = Vector3.zero;
                    yield break;
                }
                yield return null;
            }

            bool isGameStart = GameStateManager.Instance.CurrentState == GameState.GameReady;
            spawnPointManager.RespawnPlayer(transform, requireUnique: isGameStart);
        }
        private void SetDeadCollisions(bool ignore)
        {
            for (int i = 0; i < 32; i++) // Unity supports up to 32 layers
            {
                if (i == _terrainLayer) continue;
                Physics2D.IgnoreLayerCollision(_playerLayer, i, ignore);
            }
        }
    }
}
