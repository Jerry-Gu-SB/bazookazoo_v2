using System.Collections;
using Main.Scripts.Game_Managers;
using Main.Scripts.Player;
using Main.Scripts.UI_Scripts;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.World_Objects
{
    public class RocketProjectile : NetworkBehaviour
    {
        public NetworkVariable<ulong> ownerId;

        private const float Speed = 20f;
        private const int RocketMinKnockBack = 5;
        private const int RocketMaxKnockBack = 40;
        private const float ExplosionRadius = 3f;

        private const int RocketDamage = 60;
        private const int RocketMinDamage = 10;
        private const int RocketMaxDamage = 50;

        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;
        private bool _exploded = false;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _rb.linearVelocity = transform.right * Speed;
            
            GameStateManager.GameStateChanged += HandleGameState;
        }   

        private void HandleGameState(GameState state)
        {
            if ((state is 
                    GameState.Connecting or 
                    GameState.MapLoading or 
                    GameState.LobbyReady or 
                    GameState.LobbyLoading)
                && IsServer)
            {
                DestroySelf();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_exploded) return;
            if (collision.TryGetComponent(out NetworkObject otherNetObj))
            {
                if (collision.CompareTag("Player") && otherNetObj.NetworkObjectId != ownerId.Value)
                {
                    ApplyForceToTarget(collision);
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

        private void ApplyForceToTarget(Collider2D collision)
        {
            Rigidbody2D enemyRigidBody2D = collision.GetComponent<Rigidbody2D>();
            enemyRigidBody2D.AddForce(transform.right * RocketMaxKnockBack, ForceMode2D.Impulse);
        }

        private IEnumerator Explode()
        {
            _exploded = true;
            _renderer.enabled = false;
            ResolveBlastRadius();
            yield return new WaitForSeconds(.05f);
            
            DestroySelf();
        }

        private void ResolveBlastRadius()
        {
            Vector2 explosionCenter = transform.position;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                explosionCenter,
                ExplosionRadius,
                LayerMask.GetMask("Player")
            );

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out NetworkObject hitNetworkObject)) continue;
                Rigidbody2D rb2d = hit.GetComponent<Rigidbody2D>();
                if (!rb2d) continue;

                var forceFactor = ApplyPlayerForce(hit, rb2d);

                PlayerManager hitPlayerManager = hit.GetComponent<PlayerManager>();
                if (!hitPlayerManager) continue;

                float oldHealth = hitPlayerManager.playerHeath;

                hitPlayerManager.playerHeath -= Mathf.Clamp(
                    Mathf.RoundToInt(RocketDamage * forceFactor),
                    RocketMinDamage,
                    RocketMaxDamage
                );

                if (oldHealth > 0 && hitPlayerManager.playerHeath <= 0)
                {
                    AwardKillToOwner();
                    hitPlayerManager.AddDeath();
                }
            }
        }

        private void AwardKillToOwner()
        {
            if (!IsServer) return;

            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ownerId.Value, out NetworkObject ownerNetObj))
                return;
            
            PlayerManager ownerPlayerManager = ownerNetObj.GetComponent<PlayerManager>();
            if (ownerPlayerManager)
            {
                ownerPlayerManager.AddKill();
            }
        }


        private float ApplyPlayerForce(Collider2D hit, Rigidbody2D rb2d)
        {
            Vector2 direction = (hit.transform.position - transform.position).normalized;
            var distance = Vector2.Distance(hit.transform.position, transform.position);
            var forceFactor = 1 - (distance / ExplosionRadius);
            var finalForce = Mathf.Lerp(RocketMinKnockBack, RocketMaxKnockBack, forceFactor);
            rb2d.AddForce(direction * finalForce, ForceMode2D.Impulse);
            return forceFactor;
        }

        private void DestroySelf()
        {
            if (!IsServer || !this) return;
            this.enabled = false;
            Destroy(gameObject);
        }
    }
}