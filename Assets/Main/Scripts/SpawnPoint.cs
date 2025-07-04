using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Main.Scripts
{
    public class SpawnPoint : MonoBehaviour
    {
        public bool playerInCloseProximity;
        [SerializeField] private Collider2D col2d;
        private void Update()
        {
            playerInCloseProximity = col2d.IsTouchingLayers(LayerMask.GetMask("Player"));
        }
    }
}
