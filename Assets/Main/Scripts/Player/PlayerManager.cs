using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public float playerHeath = 100f;
        [SerializeField]
        private Canvas playerCanvas;
        [SerializeField]
        private Rigidbody2D playerRigidbody2D;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerCanvas.enabled = IsLocalPlayer;
        }

        private void Start()
        {
            playerHeath = 100f;
        }

        private void Update()
        {
            if (playerHeath <= 0)
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            playerRigidbody2D.linearVelocity = Vector2.zero;
            playerHeath = 100f;
            SpawnPointManager manager = FindFirstObjectByType<SpawnPointManager>();
            if (manager)
            {
                manager.RespawnPlayer(transform);
            }
        }
    }
}
