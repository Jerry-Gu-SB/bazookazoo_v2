using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    [RequireComponent(typeof(Renderer))]
    public class SetPlayerColor : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetColorBasedOnOwner();
        }
        
        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            SetColorBasedOnOwner();
        }

        private void SetColorBasedOnOwner()
        {
            Random.InitState((int) OwnerClientId);
            GetComponent<Renderer>().material.color = Random.ColorHSV();
        }
    }
}
