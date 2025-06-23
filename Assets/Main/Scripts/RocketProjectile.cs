using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class RocketProjectile : NetworkBehaviour
    {
        public float speed = 20f;
        public NetworkVariable<ulong> ownerId;

        public int selfRocketDamage = 20;
        public int enemyRocketDamage = 40;
        public int rocketKnockBack = 40;
        
        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;
        private bool _exploded = false;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _rb.linearVelocity = transform.right * speed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out NetworkObject otherNetObj))
            {
                if (otherNetObj.NetworkObjectId == ownerId.Value) return;  // Don't collide with rocket owner
                
                if (collision.CompareTag("Player"))
                {
                    if (!_exploded)
                    {
                        PlayerManager collisionPlayerManager = collision.GetComponent<PlayerManager>();
                        collisionPlayerManager.playerHeath -= selfRocketDamage;
                    }
                    
                    StartCoroutine(Explode());
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                StartCoroutine(Explode());
            }

            if (collision.GetComponent<RocketProjectile>())
            {
                StartCoroutine(Explode());
            }
        }

        private IEnumerator Explode()
        {
            _exploded = true;
            _renderer.enabled = false;

            float explosionRadius = 3f;
            Vector2 explosionCenter = transform.position;

            Collider2D[] hits = Physics2D.OverlapCircleAll(explosionCenter, explosionRadius, LayerMask.GetMask("Player"));

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out NetworkObject netObj)) continue;
                if (!hit.CompareTag("Player")) continue;

                // Apply knockback force
                Rigidbody2D rb2d = hit.GetComponent<Rigidbody2D>();
                if (!rb2d) continue;

                Vector2 direction = (hit.transform.position - transform.position).normalized;
                var distance = Vector2.Distance(hit.transform.position, transform.position);
                var forceFactor = 1 - (distance / explosionRadius);
                var finalForce = rocketKnockBack * forceFactor;
                
                // Apply damage
                PlayerManager playerManager = hit.GetComponent<PlayerManager>();
                playerManager.playerHeath -= enemyRocketDamage * forceFactor;

                rb2d.AddForce(direction * finalForce, ForceMode2D.Impulse);
            }


            yield return new WaitForSeconds(.05f);
            if (IsServer)
            {
                Destroy(gameObject);
            }


        }

    }
}