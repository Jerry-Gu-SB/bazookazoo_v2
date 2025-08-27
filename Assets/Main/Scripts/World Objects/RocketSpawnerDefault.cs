using System;
using Main.Scripts.Player;
using UnityEngine;

namespace Main.Scripts.World_Objects
{
    public class RocketSpawnerDefault : MonoBehaviour
    {
        [Header("Weapon Change Components")]
        private BazookaTypes _currentBazookaType = BazookaTypes.Default;
        [SerializeField] private GameObject rocketPrefab;
        private const float FireRate = .01f;
        private const float ReloadSpeed = 2f;
        private const int ClipSize = 4;
        private const int MaxAmmoStock = 20;
        
        private bool _goingUp = true;
        private float _initialHeight;
        private const float MaxHeight = .1f;
        private const float HoverSpeed = .1f;
        
        [Header("Respawning Components")]
        private bool _respawning = false;
        private float _respawnTimer = 0f;
        private const int RespawnTime = 3;
        
        [SerializeField]
        private Transform bazookaSpriteTransform;
        [SerializeField]
        private SpriteRenderer bazookaSpriteRenderer;
        [SerializeField]
        private BoxCollider2D bazookaSpriteCollider;

        private void Start()
        {
            _initialHeight = bazookaSpriteTransform.position.y;
        }
        private void Update()
        {
            if (_goingUp)
            {
                Vector3 newPosition = bazookaSpriteTransform.position + new Vector3(0, Time.deltaTime * HoverSpeed, 0);
                bazookaSpriteTransform.position = newPosition;
                if (bazookaSpriteTransform.position.y > _initialHeight + MaxHeight)
                {
                    _goingUp = false;
                }
            }
            else
            {
                Vector3 newPosition = bazookaSpriteTransform.position + new Vector3(0, Time.deltaTime * -HoverSpeed, 0);
                bazookaSpriteTransform.position = newPosition;
                if (bazookaSpriteTransform.position.y < _initialHeight)
                {
                    _goingUp = true;
                }
            }

            if (_respawning)
            {
                _respawnTimer += Time.deltaTime;
                bazookaSpriteRenderer.enabled = false;
                bazookaSpriteCollider.enabled = false;
            }
            if (_respawnTimer >= RespawnTime)
            {
                _respawning = false;
                _respawnTimer = 0;
                bazookaSpriteRenderer.enabled = true;
                bazookaSpriteCollider.enabled = true;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                FireRocket fireRocket = collision.gameObject.GetComponent<FireRocket>();
                if (fireRocket.GetCurrentBazookaType() == _currentBazookaType)
                {
                    fireRocket.SetCurrentAmmoStock(Math.Min(fireRocket.GetCurrentAmmoStock() + fireRocket.GetMaxAmmoStock() / 2, MaxAmmoStock));
                }
                else
                {
                    SetFireRocketProperties(fireRocket);
                }
                _respawning = true;
            }
        }
        private void SetFireRocketProperties(FireRocket fireRocket)
        {
            fireRocket.SetBazookaSprite(bazookaSpriteRenderer.sprite);
            fireRocket.SetFireRate(FireRate);
            fireRocket.SetMaxAmmoStock(MaxAmmoStock);
            fireRocket.SetClipMaxSize(ClipSize);
            fireRocket.SetFireRate(FireRate);
            fireRocket.SetReloadSpeed(ReloadSpeed);
            fireRocket.SetRocketPrefab(rocketPrefab);
        }
    }
}
