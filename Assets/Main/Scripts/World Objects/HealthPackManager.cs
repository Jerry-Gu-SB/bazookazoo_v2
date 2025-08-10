using Main.Scripts.Player;
using UnityEngine;

namespace Main.Scripts.World_Objects
{
    public class HealthPackManager : MonoBehaviour
    {
        public int healthValue = 50;

        [SerializeField]
        private Transform healthPackTransform;
        [SerializeField]
        private SpriteRenderer healthPackSpriteRenderer;
        [SerializeField]
        private BoxCollider2D healthPackCollider;
        
        private bool _goingUp = true;
        private float _initialHeight;
        private const float MaxHeight = .1f;
        private const float HoverSpeed = .1f;
        
        private bool _respawning = false;
        private float _respawnTimer = 0f;
        private const int RespawnTime = 3;

        private void Start()
        {
            _initialHeight = healthPackTransform.localPosition.y;
        }
        private void Update()
        {
            if (_goingUp)
            {
                Vector3 newPosition = healthPackTransform.localPosition + new Vector3(0, Time.deltaTime * HoverSpeed, 0);
                healthPackTransform.position = newPosition;
                if (healthPackTransform.localPosition.y > _initialHeight + MaxHeight)
                {
                    _goingUp = false;
                }
            }
            else
            {
                Vector3 newPosition = healthPackTransform.localPosition + new Vector3(0, Time.deltaTime * -HoverSpeed, 0);
                healthPackTransform.position = newPosition;
                if (healthPackTransform.localPosition.y < _initialHeight)
                {
                    _goingUp = true;
                }
            }

            if (_respawning)
            {
                _respawnTimer += Time.deltaTime;
                healthPackSpriteRenderer.enabled = false;
                healthPackCollider.enabled = false;
            }
            if (_respawnTimer >= RespawnTime)
            {
                _respawning = false;
                _respawnTimer = 0;
                healthPackSpriteRenderer.enabled = true;
                healthPackCollider.enabled = true;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            PlayerManager playerManager = collision.GetComponent<PlayerManager>();
            
            if (playerManager.playerHeath < playerManager.maxHealth - healthValue)
            {
                playerManager.playerHeath += healthValue;
            }
            else
            {
                playerManager.playerHeath = playerManager.maxHealth;
            }
            _respawning = true;
        }
    }
}
