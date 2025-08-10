using UnityEngine;
using TMPro;

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
        
        private void LateUpdate()
        {
            playerUsernameText.text = playerManager.username.Value.ToString();
            playerHealthText.text = "Health: " + playerManager.playerHeath.ToString("F1");
        }
    }
}
