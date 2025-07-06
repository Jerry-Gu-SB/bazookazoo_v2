using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public float playerHeath = 100f;
        [SerializeField]
        private Canvas playerCanvas;
        [SerializeField]
        private Rigidbody2D playerRigidbody2D;
        private UnityEvent _respawn;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerCanvas.enabled = IsLocalPlayer;
            if (_respawn == null)
                _respawn = new UnityEvent();

            _respawn.AddListener(Respawn);
        }

        private void Start()
        {
            playerHeath = 100f;
        }

        private void Update()
        {
            if (!IsServer) return;
            
            if (playerHeath <= 0)
            {
                Respawn();
            }
        }
        
        private void Respawn()
        {
            StartCoroutine(WaitForSpawnerAndRespawn());
        }

        private IEnumerator WaitForSpawnerAndRespawn()
        {
            // Wait until the SpawnPointManager is available
            SpawnPointManager manager = null;
            while (!(manager = FindFirstObjectByType<SpawnPointManager>()))
            {
                yield return null; // Wait one frame
            }

            playerRigidbody2D.linearVelocity = Vector2.zero;
            playerHeath = 100f;
            manager.RespawnPlayer(transform);
        }

    }
}
