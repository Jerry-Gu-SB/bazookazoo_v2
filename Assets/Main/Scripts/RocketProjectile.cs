using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class RocketProjectile : NetworkBehaviour
    {
        public float speed = 20f;
        [SerializeField]
        private int rocketDamage = 20;
        [SerializeField]
        private int rocketKnockBack = 40;

        public CircleCollider2D explosionHitbox;
        
        public NetworkVariable<ulong> ownerId;
        
        [HideInInspector] public GameObject owner;

        private Rigidbody2D _rb;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = transform.right * speed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out NetworkObject otherNetObj))
            {
                if (otherNetObj.NetworkObjectId == ownerId.Value) return;  // Don't collide with rocket owner
                
                if (collision.CompareTag("Player"))
                {
                    PlayerManager collisionPlayerManager = collision.GetComponent<PlayerManager>();
                    collisionPlayerManager.playerHeath -= rocketDamage;
                
                    Rigidbody2D collisionRigidBody2D = collision.GetComponent<Rigidbody2D>();
                    collisionRigidBody2D.AddForce(transform.right * rocketKnockBack, ForceMode2D.Impulse);

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
            yield return new WaitForSeconds(.01f);
            if (IsServer)
            {
                Destroy(gameObject);
            }
            
        }
    }
}