using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Main.Scripts.Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        [Header("Player Components")]
        [SerializeField]
        private PlayerManager playerManager;
        [SerializeField]
        private FireRocket fireRocket;
        
        [Header("Screen UI")]
        [SerializeField]
        private TMP_Text playerHealthText;
        [SerializeField] 
        private TMP_Text respawnTimerText;
        [SerializeField] 
        private Image redTint;
        [SerializeField]
        private TMP_Text currentClip;
        [SerializeField]
        private TMP_Text currentAmmoStock;
        
        [Header("World space UI")]
        [SerializeField]
        private TMP_Text playerUsernameText;
        [SerializeField] 
        private Image invincibilityAura;
        
        private void LateUpdate()
        {
            playerUsernameText.text = playerManager.username.Value.ToString();
            playerHealthText.text = "Health: " + playerManager.playerHeath.ToString("F1");
            respawnTimerText.text = "Respawning in: " + playerManager.respawnTimer.ToString("F3");

            currentClip.text = fireRocket.GetCurrentClip().ToString();
            currentAmmoStock.text = fireRocket.GetCurrentAmmoStock().ToString();
            
            playerUsernameText.enabled = !playerManager.isDead;
            
            respawnTimerText.enabled = playerManager.isDead;
            redTint.enabled = playerManager.isDead;
            
            invincibilityAura.enabled = playerManager.isInvincible;
        }
    }
}
