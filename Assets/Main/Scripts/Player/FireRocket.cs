using System;
using System.Collections;
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

        private BazookaTypes _currentBazookaType = BazookaTypes.Default;
        
        private float _fireRate = .5f;
        private float _fireTimer = 0;
        private bool _hasFired = false;
        
        private bool _isReloading = false;
        private float _reloadSpeed = 2f;
        private int _clipMaxSize = 4;
        private int _currentClip = 4;
        private int _maxAmmoStock = 20;
        private int _currentAmmoStock = 20;

        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetMouseButtonDown(0) && !_hasFired && !_isReloading && _currentClip > 0)
            {
                _hasFired = true;
                _currentClip -= 1;
                if (playerManager.isDead) return;
                FireRocketServerRpc(firePoint.position, firePoint.rotation);
            }

            if (Input.GetButtonDown("Fire2") && !_isReloading && _currentClip < _clipMaxSize && _currentAmmoStock > 0)
            {
                StartCoroutine(ReloadBazooka());
            }

            if (!_isReloading && _currentClip <= 0 && _currentAmmoStock > 0)
            {
                StartCoroutine(ReloadBazooka());
            }
            CalculateFiredTimer();
        }

        private IEnumerator ReloadBazooka()
        {
            _isReloading = true;
            // TODO: Play animation here
            yield return new WaitForSeconds(_reloadSpeed);
            
            var neededAmmo = _clipMaxSize - _currentClip;
            var ammoToLoad = Math.Min(neededAmmo, _currentAmmoStock);
            _currentClip += ammoToLoad;
            _currentAmmoStock -= ammoToLoad;
            _isReloading = false;
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

        public int GetCurrentAmmoStock()
        {
            return _currentAmmoStock;
        }
        public int GetMaxAmmoStock()
        {
            return _maxAmmoStock;
        }
        public int GetCurrentClip()
        {
            return _currentClip;
        }

        public int GetMaxClipSize()
        {
            return _clipMaxSize;
        }
        public BazookaTypes GetCurrentBazookaType()
        {
            return _currentBazookaType;
        }
        public void SetBazookaType(BazookaTypes type)
        {
            _currentBazookaType = type;
        }
        public void SetFireRate(float fireRate)
        {
            _fireRate = fireRate;
        }
        public void SetBazookaSprite(Sprite bazookaSprite)
        {
            bazookaSpriteRenderer.sprite = bazookaSprite;
        }
        public void SetReloadSpeed(float newSpeed)
        {
            _reloadSpeed = newSpeed;
        }

        public void SetCurrentClipSize(int newCurrentClipSize)
        {
            _currentClip = newCurrentClipSize;
        }
        public void SetClipMaxSize(int newClipSize)
        {
            _clipMaxSize = newClipSize;
        }
        public void SetCurrentAmmoStock(int newAmmoStock)
        {
            _currentAmmoStock = newAmmoStock;
        }
        public void SetMaxAmmoStock(int newAmmoStock)
        {
            _maxAmmoStock = newAmmoStock;
        }
        public void SetRocketPrefab(GameObject newRocketPrefab)
        {
            this.rocketPrefab = newRocketPrefab;
        }
    }
}