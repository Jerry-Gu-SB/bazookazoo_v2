using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Object = System.Object;

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
            ReorderScoreboard();
        }

        private void ReorderScoreboard()
        {
            var sorted = _playerCards
                .OrderByDescending(pair => pair.Value.GetScore())
                .ToList();

            int index = 0;
            foreach (var kvp in sorted)
            {
                kvp.Value.transform.SetSiblingIndex(index);
                index++;
            }
        }

        public static void PlayerJoined(ulong playerOwnerClientID, string username)
        {
            PlayerCardManager newCard = Instantiate(_instance.playerCardPrefab, _instance.playerCardParent)
                .GetComponent<PlayerCardManager>();
            _instance._playerCards.Add(playerOwnerClientID, newCard);
            newCard.Initialize(username);
        }

        public static void PlayerLeft(ulong playerOwnerClientID)
        {
            if (_instance._playerCards.TryGetValue(playerOwnerClientID, out PlayerCardManager playerCard))
            {
                Destroy(playerCard.gameObject);
                _instance._playerCards.Remove(playerOwnerClientID);
            }
        }

        public static void SetScore(ulong playerOwnerClientID, int score)
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                _instance.SetScoreServerRpc(playerOwnerClientID, score);
            }
            else
            {
                _instance.SetScoreInternal(playerOwnerClientID, score);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetScoreServerRpc(ulong playerOwnerClientID, int score)
        {
            SetScoreClientRpc(playerOwnerClientID, score);
            SetScoreInternal(playerOwnerClientID, score);
        }

        [ClientRpc]
        private void SetScoreClientRpc(ulong playerOwnerClientID, int score)
        {
            SetScoreInternal(playerOwnerClientID, score);
        }

        private void SetScoreInternal(ulong playerID, int score)
        {
            _playerCards[playerID].SetScore(score);
        }
        
        public static void SetDeaths(ulong playerOwnerClientID, int deaths)
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                _instance.SetDeathsServerRpc(playerOwnerClientID, deaths);
            }
            else
            {
                _instance.SetDeathsInternal(playerOwnerClientID, deaths);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetDeathsServerRpc(ulong playerOwnerClientID, int deaths)
        {
            SetDeathsClientRpc(playerOwnerClientID, deaths);
            SetDeathsInternal(playerOwnerClientID, deaths);
        }

        [ClientRpc]
        private void SetDeathsClientRpc(ulong playerOwnerClientID, int deaths)
        {
            SetDeathsInternal(playerOwnerClientID, deaths);
        }

        private void SetDeathsInternal(ulong playerID, int deaths)
        {
            _playerCards[playerID].SetDeaths(deaths);
        }

        public static void SetKills(ulong playerOwnerClientID, int kills)
        {
            // Client wants to request update from server
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                _instance.SetKillsServerRpc(playerOwnerClientID, kills);
            }
            else
            {
                // If server, just apply directly
                _instance.SetKillsInternal(playerOwnerClientID, kills);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetKillsServerRpc(ulong playerOwnerClientID, int kills)
        {
            // Update all clients
            SetKillsClientRpc(playerOwnerClientID, kills);

            // Apply on server too
            SetKillsInternal(playerOwnerClientID, kills);
        }

        [ClientRpc]
        private void SetKillsClientRpc(ulong playerOwnerClientID, int kills)
        {
            SetKillsInternal(playerOwnerClientID, kills);
        }

        private void SetKillsInternal(ulong playerOwnerClientID, int kills)
        {
            _playerCards[playerOwnerClientID].SetKills(kills);
        }

        public static void SetUsername(ulong playerOwnerClientID, string username)
        {
            // Client wants to request update from server
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                _instance.SetUsernameServerRpc(playerOwnerClientID, username);
            }
            else
            {
                // If server, just apply directly
                _instance.SetUsernameInternal(playerOwnerClientID, username);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetUsernameServerRpc(ulong playerOwnerClientID, string username)
        {
            // Update all clients
            SetUsernameClientRpc(playerOwnerClientID, username);

            // Apply on server too
            SetUsernameInternal(playerOwnerClientID, username);
        }

        [ClientRpc]
        private void SetUsernameClientRpc(ulong playerOwnerClientID, string username)
        {
            SetUsernameInternal(playerOwnerClientID, username);
        }

        private void SetUsernameInternal(ulong playerOwnerClientID, string username)
        {
            _playerCards[playerOwnerClientID].SetUsername(username);
        }
    }
}
