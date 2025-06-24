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
        
        private void LateUpdate()
        {
            playerHealthText.text = "Health: " + playerManager.playerHeath.ToString("F1");
        }
    }
}
