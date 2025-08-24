using TMPro;
using UnityEngine;

namespace Main.Scripts.UI_Scripts
{
    public class PlayerCardManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerUsername;
        [SerializeField] 
        private TMP_Text playerKills;
        [SerializeField] 
        private TMP_Text playerDeaths;

        public void Initialize(string username)
        {
            playerUsername.text = username;
            playerKills.text = "0";
            playerDeaths.text = "0";
        }
        public void SetKills(int kills)
        {
            playerKills.text = kills.ToString();
        }
        public void SetDeaths(int deaths)
        {
            playerDeaths.text = deaths.ToString();
        }

        public void SetUsername(string username)
        {
            playerUsername.text = username;
        }
    }
}
