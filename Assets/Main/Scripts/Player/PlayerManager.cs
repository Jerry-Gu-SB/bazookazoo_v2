using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Main.Scripts.Game_Managers;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        [Header("Player Properties")] 
        public float maxHealth = 100f;
        public float playerHeath;
        public int playerScore = 0;

        [SerializeField] private Canvas playerCanvas;
        [SerializeField] private Rigidbody2D playerRigidbody2D;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerHeath = maxHealth;
            playerCanvas.enabled = IsLocalPlayer;
            GameStateManager.GameStateChanged += HandleGameState;
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
                Respawn();
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
            playerScore = 0;
            Respawn();
        }

        private void Respawn()
        {
            playerHeath = maxHealth;
            playerRigidbody2D.linearVelocity = Vector2.zero;
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
            spawnPointManager.RespawnPlayer(transform);
        }
    }
}
