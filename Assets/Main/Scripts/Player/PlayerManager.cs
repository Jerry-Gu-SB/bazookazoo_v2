using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Main.Scripts.Game_Managers;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        [Header("Player Properties")]
        public float playerHeath = 100f;
        public int playerScore = 0;

        [SerializeField] private Canvas playerCanvas;
        [SerializeField] private Rigidbody2D playerRigidbody2D;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerCanvas.enabled = IsLocalPlayer;
            GameStateManager.GameStateChanged += HandleGameState;
        }

        public override void OnDestroy()
        {
            GameStateManager.GameStateChanged -= HandleGameState;
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
            if (state == GameState.GameReady)
            {
                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            playerHeath = 100f;
            playerScore = 0;
            Respawn();
        }

        private void Respawn()
        {
            playerRigidbody2D.linearVelocity = Vector2.zero;
            StartCoroutine(WaitForSpawnerAndRespawn());
        }

        private IEnumerator WaitForSpawnerAndRespawn()
        {
            float timeout = 5f;
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

        public static void SpawnAllPlayers(System.Action callback)
        {
            foreach (var player in FindObjectsByType<PlayerManager>(FindObjectsSortMode.None))
            {
                player.ResetPlayer();
            }
            callback?.Invoke();
        }
    }
}