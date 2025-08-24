using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.UI_Scripts
{
    public class ScoreboardManager : NetworkBehaviour
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

        public static void SetDeaths(ulong playerID, int deaths)
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                _instance.SetDeathsServerRpc(playerID, deaths);
            }
            else
            {
                _instance.SetDeathsInternal(playerID, deaths);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetDeathsServerRpc(ulong playerID, int deaths)
        {
            SetDeathsClientRpc(playerID, deaths);
            SetDeathsInternal(playerID, deaths);
        }

        [ClientRpc]
        private void SetDeathsClientRpc(ulong playerID, int deaths)
        {
            SetDeathsInternal(playerID, deaths);
        }

        private void SetDeathsInternal(ulong playerID, int deaths)
        {
            _playerCards[playerID].SetDeaths(deaths);
        }
        
        public static void SetKills(ulong playerID, int kills)
        {
            // Client wants to request update from server
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                _instance.SetKillsServerRpc(playerID, kills);
            }
            else
            {
                // If server, just apply directly
                _instance.SetKillsInternal(playerID, kills);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetKillsServerRpc(ulong playerID, int kills)
        {
            // Update all clients
            SetKillsClientRpc(playerID, kills);
        
            // Apply on server too
            SetKillsInternal(playerID, kills);
        }

        [ClientRpc]
        private void SetKillsClientRpc(ulong playerID, int kills)
        {
            SetKillsInternal(playerID, kills);
        }

        private void SetKillsInternal(ulong playerID, int kills)
        {
            _playerCards[playerID].SetKills(kills);
        }
    }
}
