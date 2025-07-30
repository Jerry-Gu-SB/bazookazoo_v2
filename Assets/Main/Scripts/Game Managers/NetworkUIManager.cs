using System;
using System.Linq;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Main.Scripts.Game_Managers;

namespace Main.Scripts.Game_Managers
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
            GameObject eventSystem = new("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(transform);
        }

        private void Start()
        {
            _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);
            dustyButton.onClick.AddListener(() =>
            {
                GameStateManager.Instance.SetSelectedMap(MapNames.Dusty);
                GameStateManager.Instance.TransitionToState(GameState.MapLoading);
            });

            DisplayLocalIPAddress();
            HandleGameStateChanged(GameStateManager.Instance.CurrentState);
        }

        private void OnEnable()
        {
            GameStateManager.GameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameStateManager.GameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState state)
        {
            ShowConnectingUI(state is GameState.Idle or GameState.Connecting);
            ShowMapSelectionUI(state == GameState.LobbyReady);
        }

        private void StartClient()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartClient();
            GameStateManager.Instance.TransitionToState(GameState.Connecting);
            ShowConnectingUI(false);
            ShowMapSelectionUI(false);
        }

        private void StartHost()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartHost();
            GameStateManager.Instance.TransitionToState(GameState.LobbyLoading);
        }

        private void ApplyConnectionData()
        {
            string ip = string.IsNullOrEmpty(ipAddressInputField.text) ? "127.0.0.1" : ipAddressInputField.text;
            ushort port = ushort.TryParse(portInputField.text, out ushort p) ? p : (ushort)7777;
            _transport.SetConnectionData(ip, port);
        }

        private void ShowConnectingUI(bool show)
        {
            startClientButton.gameObject.SetActive(show);
            startHostButton.gameObject.SetActive(show);
            ipAddressInputField.gameObject.SetActive(show);
            portInputField.gameObject.SetActive(show);
            localIPDisplayText.gameObject.SetActive(show);
        }

        private void ShowMapSelectionUI(bool show)
        {
            selectMapText.gameObject.SetActive(show);
            dustyButton.gameObject.SetActive(show);
        }

        private void DisplayLocalIPAddress()
        {
            string ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            localIPDisplayText.text = $"Your IP: {ip}\nRecommended port: 7777";
        }
    }
}