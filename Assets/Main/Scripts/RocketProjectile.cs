using System.Collections;
using Main.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class RocketProjectile : NetworkBehaviour
    {
        public NetworkVariable<ulong> ownerId;

        [Header("Rocket properties")]
        public float speed = 20f;
        public int selfRocketDamage = 20;
        public int enemyRocketDamage = 40;
        public int rocketMinKnockBack = 5;
        public int rocketMaxKnockBack = 40;
        public float explosionRadius = 3f;

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
            if (_exploded) return;
            if (collision.TryGetComponent(out NetworkObject otherNetObj))
            {
                if (collision.CompareTag("Player") && otherNetObj.NetworkObjectId != ownerId.Value)
                {
                    PlayerManager collisionPlayerManager = collision.GetComponent<PlayerManager>();
                    collisionPlayerManager.playerHeath -= selfRocketDamage;

                    Rigidbody2D enemyRigidBody2D = collision.GetComponent<Rigidbody2D>();
                    enemyRigidBody2D.AddForce(transform.right * rocketMaxKnockBack, ForceMode2D.Impulse);
                    
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

            Vector2 explosionCenter = transform.position;

            Collider2D[] hits = Physics2D.OverlapCircleAll(explosionCenter, explosionRadius, LayerMask.GetMask("Player"));

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out NetworkObject netObj)) continue;
                if (!hit.CompareTag("Player")) continue;
                Rigidbody2D rb2d = hit.GetComponent<Rigidbody2D>();
                if (!rb2d) continue;

                Vector2 direction = (hit.transform.position - transform.position).normalized;
                var distance = Vector2.Distance(hit.transform.position, transform.position);
                var forceFactor = 1 - (distance / explosionRadius);
                var finalForce = Mathf.Lerp(rocketMinKnockBack, rocketMaxKnockBack, forceFactor);
                rb2d.AddForce(direction * finalForce, ForceMode2D.Impulse);
                
                PlayerManager playerManager = hit.GetComponent<PlayerManager>();
                playerManager.playerHeath -= enemyRocketDamage * forceFactor;  // TODO: add a min/max clamp for these
            }
            yield return new WaitForSeconds(.05f);
            
            if (IsServer) Destroy(gameObject);
        }
    }
}