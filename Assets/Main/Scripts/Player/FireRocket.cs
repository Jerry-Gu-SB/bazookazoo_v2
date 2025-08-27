using Main.Scripts.World_Objects;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Player
{
    public class FireRocket : NetworkBehaviour
    {
        [SerializeField]
        private Transform firePoint;
        [SerializeField]
        private PlayerManager playerManager;
        [SerializeField]
        private GameObject rocketPrefab;
        [SerializeField]
        private SpriteRenderer bazookaSpriteRenderer;
        private float _fireRate = .5f;
        private float _fireTimer = 0;
        private bool _hasFired = false;

        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetMouseButtonDown(0) && !_hasFired)
            {
                _hasFired = true;
                if (playerManager.isDead) return;
                FireRocketServerRpc(firePoint.position, firePoint.rotation);
            }
            CalculateFiredTimer();
        }

        [ServerRpc]
        private void FireRocketServerRpc(Vector3 position, Quaternion rotation)
        {
            GameObject rocket = Instantiate(rocketPrefab, position, rotation);
            NetworkObject netObj = rocket.GetComponent<NetworkObject>();
            netObj.Spawn(true);

            RocketProjectile rp = rocket.GetComponent<RocketProjectile>();
            rp.shooterNetworkID.Value = this.GetComponent<NetworkObject>().NetworkObjectId;
        }
        private void CalculateFiredTimer()
        {
            if (_hasFired)
            {
                _fireTimer += Time.deltaTime;
                if  (_fireTimer >= _fireRate)
                {
                    _fireTimer = 0;
                    _hasFired = false;
                }
            }
        }

        public void SetFireRate(float fireRate)
        {
            _fireRate = fireRate;
        }

        public void SetBazookaSprite(Sprite bazookaSprite)
        {
            bazookaSpriteRenderer.sprite = bazookaSprite;
        }

        public float GetFireRate()
        {
            return _fireRate;
        }

        public SpriteRenderer GetBazookaSpriteRenderer()
        {
            return bazookaSpriteRenderer;
        }
    }
}