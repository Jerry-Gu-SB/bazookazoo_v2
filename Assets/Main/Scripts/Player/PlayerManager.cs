using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Main.Scripts.Game_Managers;
using UnityEngine.Events;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        [Header("Player Properties")]
        public float playerHeath = 100f;
        public int playerScore = 0;
        
        [Header("Player Component References")]
        [SerializeField]
        private Canvas playerCanvas;
        [SerializeField]
        private Rigidbody2D playerRigidbody2D;
        
        [Header("Unity Events")]
        public static UnityEvent killedPlayer;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerCanvas.enabled = IsLocalPlayer;
            AddUnityEventListeners();
        }

        private void Update()
        {
            if (playerHeath <= 0)
            {
                Respawn();
            }
        }

        private void ResetPlayer()
        {
            Respawn();
            playerHeath = 100f;
            playerScore = 0;
        }
        
        private void Respawn()
        {
            playerRigidbody2D.linearVelocity = Vector2.zero;
            playerHeath = 100f;
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
                    Debug.LogWarning("Timeout: Could not find SpawnPointManager. Spawning at (0,0).");
                    transform.position = Vector3.zero;
                    yield break;
                }
                yield return null;
            }

            spawnPointManager.RespawnPlayer(transform);
        }


        private void UpdateScore()
        {
            // TODO: this is bugged, somehow only the killed player is getting score
            if (!IsOwner && !IsLocalPlayer) return;
            playerScore += 1;
            Debug.Log("Score: " + playerScore);
        }
    
        
        private void AddUnityEventListeners()
        {
            if (killedPlayer == null) killedPlayer = new UnityEvent();
            killedPlayer.AddListener(UpdateScore);
            
            GameStateManager.onSceneReady.AddListener(ResetPlayer);
        }
    }
}
