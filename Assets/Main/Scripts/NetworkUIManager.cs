using System;
using System.Linq;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Main.Scripts
{
    public class NetworkUIManager : MonoBehaviour
    {
        [Header("Connection UI Elements")]
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text localIPDisplayText;
        
        [Header("Map Selection UI Elements")]
        [SerializeField] private TMP_Text selectMapText;
        [SerializeField] private Button dustyButton;

        private UnityTransport _transport;

        private void Awake()
        {
            if (FindAnyObjectByType<EventSystem>()) return;
            Type inputType = typeof(StandaloneInputModule);
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
            
            ActivateConnectingUI();
            DeactivateMapSelectionUI();
        }

        private void Start()
        {
            _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);
            dustyButton.onClick.AddListener(() => LoadMapFromLobby("Dusty"));
            
            DisplayLocalIPAddress();
        }

        private void ApplyConnectionData()
        {
            string ip = ipAddressInputField.text;
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

            string portStr = portInputField.text;
            ushort port = 7777;
            if (!string.IsNullOrEmpty(portStr) && ushort.TryParse(portStr, out ushort parsedPort))
            {
                port = parsedPort;
            }

            _transport.SetConnectionData(ip, port);
        }

        private void StartClient()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartClient();
            DeactivateConnectingUI();
        }

        private void StartHost()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
            DeactivateConnectingUI();
            ActivateMapSelectionUI();
        }
        
        private void LoadMapFromLobby(string mapSceneName)
        {
            Scene lobbyScene = SceneManager.GetSceneByName("Lobby");

            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(mapSceneName, LoadSceneMode.Additive);
            return;
            
            // Checks if we have finished loading into the actual map unloads the lobby
            void OnSceneEvent(SceneEvent sceneEvent)
            {
                if (sceneEvent.SceneName != mapSceneName ||
                    sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
                
                if (lobbyScene.IsValid())
                {
                    NetworkManager.Singleton.SceneManager.UnloadScene(lobbyScene);
                }

                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;

                DeactivateMapSelectionUI();
            }
        }
        
        private void ActivateConnectingUI()
        {
            startClientButton.gameObject.SetActive(true);
            startHostButton.gameObject.SetActive(true);
            ipAddressInputField.gameObject.SetActive(true);
            portInputField.gameObject.SetActive(true);
            localIPDisplayText.gameObject.SetActive(true);
            
        }
        private void DeactivateConnectingUI()
        {
            startClientButton.gameObject.SetActive(false);
            startHostButton.gameObject.SetActive(false);
            ipAddressInputField.gameObject.SetActive(false);
            portInputField.gameObject.SetActive(false);
            localIPDisplayText.gameObject.SetActive(false);
        }

        private void ActivateMapSelectionUI()
        {
            selectMapText.gameObject.SetActive(true);
            dustyButton.gameObject.SetActive(true);
        }

        private void DeactivateMapSelectionUI()
        {
            selectMapText.gameObject.SetActive(false);
            dustyButton.gameObject.SetActive(false);
        }

        private void DisplayLocalIPAddress()
        {
            string ip = GetLocalIPv4();
            localIPDisplayText.text = $"Your IP: {ip} \nRecommended port: 7777";
        }

        private static string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }
    }
}
