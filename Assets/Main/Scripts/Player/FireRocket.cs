using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Player
{
    public class FireRocket : NetworkBehaviour
    {
        public GameObject rocketPrefab;
        public Transform firePoint;

        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetMouseButtonDown(0))
            {
                FireRocketServerRpc(firePoint.position, firePoint.rotation);
            }
        }

        [ServerRpc]
        private void FireRocketServerRpc(Vector3 position, Quaternion rotation)
        {
            GameObject rocket = Instantiate(rocketPrefab, position, rotation);
            NetworkObject netObj = rocket.GetComponent<NetworkObject>();
            netObj.Spawn(true);

            RocketProjectile rp = rocket.GetComponent<RocketProjectile>();
            rp.ownerId.Value = this.GetComponent<NetworkObject>().NetworkObjectId;
        }
    }
}