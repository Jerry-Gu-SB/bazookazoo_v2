using Main.Scripts.Game_Managers;
using Unity.Netcode;
using UnityEngine;

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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerCanvas.enabled = IsLocalPlayer;
            if (IsServer)
            {
                GameStateManager.instance.RegisterPlayer(gameObject);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            GameStateManager.instance.UnregisterPlayer(gameObject);
        }
        
        private void Update()
        {
            if (playerHeath <= 0)
            {
                GameStateManager.instance.RespawnPlayer(gameObject);
            }
        }

        private void UpdateScore()
        {
            if (!IsOwner || !IsLocalPlayer) return;
            playerScore += 1;
            Debug.Log("Score: " + playerScore);
        }
    }
}
