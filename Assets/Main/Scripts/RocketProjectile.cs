using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts
{
    public class RocketProjectile : NetworkBehaviour
    {
        public float speed = 20f;
        [HideInInspector] public GameObject owner;

        private Rigidbody2D _rb;

        private void Start()
        {
            if (!IsServer)
            {
                GetComponent<Rigidbody2D>().simulated = false;
                return;
            }

            _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = transform.right * speed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;
            
            if (collision.gameObject == owner) return;

            if (collision.CompareTag("Player") && collision.gameObject != owner)
            {
                Explode();
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                Explode();
            }

            if (collision.GetComponent<RocketProjectile>())
            {
                Explode();
            }
        }

        private void Explode()
        {
            Destroy(gameObject);
        }
    }
}