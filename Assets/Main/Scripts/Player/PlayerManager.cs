using Unity.Netcode;
using UnityEngine;
using System.Collections;
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
        public static UnityEvent ResetPlayer;
        public static UnityEvent PlayerRespawn;
        public static UnityEvent KilledPlayer;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerCanvas.enabled = IsLocalPlayer;
            AddUnityEventListeners();
        }
        
        private void Start()
        {
            ResetPlayerProperties();
        }

        private void Update()
        {
            if (playerHeath <= 0)
            {
                Respawn();
            }
        }

        private void ResetPlayerProperties()
        {
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
            // Wait until the SpawnPointManager is available
            SpawnPointManager spawnPointManager;
            while (!(spawnPointManager = FindFirstObjectByType<SpawnPointManager>()))
            {
                yield return null; // Wait one frame
            }
            spawnPointManager.RespawnPlayer(transform);
        }

        private void UpdateScore()
        {
            playerScore += 1;
            Debug.Log("Score: " + playerScore);
        }
        
        private void AddUnityEventListeners()
        {
            if (PlayerRespawn == null) PlayerRespawn = new UnityEvent();
            PlayerRespawn.AddListener(Respawn);

            if (KilledPlayer == null) KilledPlayer = new UnityEvent();
            KilledPlayer.AddListener(UpdateScore);
            
            if (ResetPlayer == null) ResetPlayer = new UnityEvent();
            ResetPlayer.AddListener(ResetPlayerProperties);
        }
    }
}
