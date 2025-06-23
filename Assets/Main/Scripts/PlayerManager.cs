using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class PlayerManager : NetworkBehaviour
    {
        public float playerHeath;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerHeath = 100f;
        }
    }
}
