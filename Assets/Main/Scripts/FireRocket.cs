using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class FireRocket : NetworkBehaviour
    {
        public GameObject rocketPrefab;
        public Transform firePoint;

        void Update()
        {
            if (!IsOwner) return;

            if (Input.GetMouseButtonDown(0))
            {
                FireRocketServerRpc(firePoint.position, firePoint.rotation);
            }
        }

        [ServerRpc]
        void FireRocketServerRpc(Vector3 position, Quaternion rotation)
        {
            GameObject rocket = Instantiate(rocketPrefab, position, rotation);
            NetworkObject netObj = rocket.GetComponent<NetworkObject>();
            netObj.Spawn(true);

            RocketProjectile rp = rocket.GetComponent<RocketProjectile>();
            rp.owner = this.gameObject;
        }
    }
}