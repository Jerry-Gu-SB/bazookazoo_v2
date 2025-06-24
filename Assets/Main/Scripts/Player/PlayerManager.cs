using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public float playerHeath = 100f;
        [SerializeField]
        private Canvas playerCanvas;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerHeath = 100f;
            playerCanvas.enabled = IsLocalPlayer;
        }
    }
}
