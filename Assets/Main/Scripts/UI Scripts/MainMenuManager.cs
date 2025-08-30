using System.Linq;
using System.Net;
using Main.Scripts.Game_Managers;
using Main.Scripts.Utilities;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.Scripts.UI_Scripts
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Connection UI Elements")]
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_Text localIPDisplayText;
        
        [Header("Map Selection UI Elements")]
        [SerializeField] private TMP_Text selectMapText;
        [SerializeField] private Button dustyButton;
        [SerializeField] private Button forestButton;

        [Header("Game Mode Selection UI Elements")]
        [SerializeField] private TMP_Text gameModeText;
        [SerializeField] private Button deathmatchButton;
        
        [Header("Public Variables")]
        public static string LocalPlayerUsername { get; private set; } = "Player";

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

            SetNetworkButtonListeners();
            SetGameModeButtonListeners();
            SetMapButtonListeners();
            
            ShowGameModeUI(false);
            ShowMapSelectionUI(false);

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
        }
        
        private void SetNetworkButtonListeners()
        {
            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);
        }
        
        private void SetGameModeButtonListeners()
        {
            deathmatchButton.onClick.AddListener(() => SetGameMode(GameMode.Deathmatch));
        }

        private void SetGameMode(GameMode mode)
        {
            GameStateManager.Instance.SetGameModeServerRpc(mode);
            ShowGameModeUI(false);
            ShowMapSelectionUI(true);
        }
        
        private void SetMapButtonListeners()
        {
            dustyButton.onClick.AddListener(() =>
            {
                GameStateManager.Instance.SetSelectedMap(MapNames.Dusty);
                GameStateManager.Instance.TransitionToState(GameState.MapLoading);
                ShowMapSelectionUI(false);
            });
            forestButton.onClick.AddListener(() =>
            {
                GameStateManager.Instance.SetSelectedMap(MapNames.Forest);
                GameStateManager.Instance.TransitionToState(GameState.MapLoading);
                ShowMapSelectionUI(false);
            });
        }
        
        private void StartHost()
        {
            ApplyConnectionData();
            ApplyPlayerUsername();
            NetworkManager.Singleton.StartHost();
            GameStateManager.Instance.TransitionToState(GameState.LobbyLoading);
            ShowGameModeUI(true);
        }
        
        private void StartClient()
        {
            ApplyConnectionData();
            ApplyPlayerUsername();
            NetworkManager.Singleton.StartClient();
            GameStateManager.Instance.TransitionToState(GameState.Connecting);
            ShowConnectingUI(false);
            ShowMapSelectionUI(false);
            ShowGameModeUI(false);
        }

        private void ApplyConnectionData()
        {
            string ip = string.IsNullOrEmpty(ipAddressInputField.text) ? "127.0.0.1" : ipAddressInputField.text;
            ushort port = ushort.TryParse(portInputField.text, out ushort p) ? p : (ushort)7777;
            _transport.SetConnectionData(ip, port);
        }

        private void ApplyPlayerUsername()
        {
            LocalPlayerUsername = string.IsNullOrEmpty(usernameInputField.text)
                ? "Player"
                : usernameInputField.text;
        }

        private void ShowConnectingUI(bool show)
        {
            startClientButton.gameObject.SetActive(show);
            startHostButton.gameObject.SetActive(show);
            ipAddressInputField.gameObject.SetActive(show);
            portInputField.gameObject.SetActive(show);
            localIPDisplayText.gameObject.SetActive(show);
            usernameInputField.gameObject.SetActive(show);
        }

        private void ShowGameModeUI(bool show)
        {
            gameModeText.gameObject.SetActive(show);
            deathmatchButton.gameObject.SetActive(show);
        }

        private void ShowMapSelectionUI(bool show)
        {
            selectMapText.gameObject.SetActive(show);
            dustyButton.gameObject.SetActive(show);
            forestButton.gameObject.SetActive(show);
        }

        private void DisplayLocalIPAddress()
        {
            string ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            localIPDisplayText.text = $"Your IP: {ip}\nRecommended port: 7777";
        }
    }
}