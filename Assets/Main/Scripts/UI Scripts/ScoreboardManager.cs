using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.UI_Scripts
{
    public class ScoreboardManager : MonoBehaviour
    {
        private static ScoreboardManager _instance;

        [SerializeField] 
        private GameObject playerCardPrefab;
        [SerializeField] 
        private Transform playerCardParent;
        [SerializeField]
        private GameObject scoreboard;
        
        private Dictionary<ulong, PlayerCardManager> _playerCards = new Dictionary<ulong, PlayerCardManager>();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            _instance = this;
            scoreboard.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                scoreboard.SetActive(true);
            if (Input.GetKeyUp(KeyCode.Tab))
                scoreboard.SetActive(false);                
        }
        
        public static void PlayerJoined(ulong playerID, string userName)
        {
            PlayerCardManager newCard = Instantiate(_instance.playerCardPrefab, _instance.playerCardParent).GetComponent<PlayerCardManager>();
            _instance._playerCards.Add(playerID, newCard);
            newCard.Initialize(userName);
            
        }

        public static void PlayerLeft(ulong playerID)
        {
            if (_instance._playerCards.TryGetValue(playerID, out PlayerCardManager playerCard))
            {
                Destroy(playerCard.gameObject);
                _instance._playerCards.Remove(playerID);
            }
        }
    }
}
