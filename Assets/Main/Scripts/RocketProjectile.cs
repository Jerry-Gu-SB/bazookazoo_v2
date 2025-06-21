using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class RocketProjectile : NetworkBehaviour
    {
        public float speed = 20f;
        public NetworkVariable<ulong> ownerId;

        public int rocketDamage = 20;
        public int rocketKnockBack = 40;
        
        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;
        private bool exploded = false;
        
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
                    if (!exploded)
                    {
                        PlayerManager collisionPlayerManager = collision.GetComponent<PlayerManager>();
                        collisionPlayerManager.playerHeath -= rocketDamage;
                
                        Rigidbody2D collisionRigidBody2D = collision.GetComponent<Rigidbody2D>();
                        collisionRigidBody2D.AddForce(transform.right * rocketKnockBack, ForceMode2D.Impulse);
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
            exploded = true;
            _renderer.enabled = false;
            yield return new WaitForSeconds(.05f);
            if (IsServer)
            {
                Destroy(gameObject);
            }
            
        }
    }
}