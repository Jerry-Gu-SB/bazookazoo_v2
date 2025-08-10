using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Main.Scripts.Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerManager playerManager;
        [SerializeField]
        private TMP_Text playerHealthText;
        [SerializeField] 
        private TMP_Text playerUsernameText;
        [SerializeField]
        private TMP_Text respawnTimerText;
        [SerializeField] 
        private Image redTint;
        
        
        private void LateUpdate()
        {
            playerUsernameText.text = playerManager.username.Value.ToString();
            playerHealthText.text = "Health: " + playerManager.playerHeath.ToString("F1");
            respawnTimerText.text = "Respawning in: " + playerManager.respawnTimer.ToString("F3");

            playerUsernameText.enabled = !playerManager.isDead;
            
            respawnTimerText.enabled = playerManager.isDead;
            redTint.enabled = playerManager.isDead;
        }
    }
}
