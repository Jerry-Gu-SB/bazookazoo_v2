using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class PlayerManager : NetworkBehaviour
    {
        public int playerHeath;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            playerHeath = 100;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
