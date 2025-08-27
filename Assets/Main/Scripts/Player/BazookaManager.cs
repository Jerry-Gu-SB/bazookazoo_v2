using System;
using UnityEngine;

namespace Main.Scripts.Player
{
    public class BazookaManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerManager playerManager;
        [SerializeField]
        private SpriteRenderer barrelSpriteRenderer;
        [SerializeField]
        private SpriteRenderer firePointSpriteRenderer;

        private void Update()
        {
            barrelSpriteRenderer.enabled = !playerManager.isDead;
            // firePointSpriteRenderer.enabled = !playerManager.isDead;
        }
    }
}
