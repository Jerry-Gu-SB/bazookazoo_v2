using System;
using System.Linq;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Main.Scripts
{
    public class NetworkUIManager : MonoBehaviour
    {
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private TMP_InputField ipAddressInputField;
        [SerializeField] private TMP_InputField portInputField;
        [SerializeField] private TMP_Text localIPDisplayText;

        private UnityTransport _transport;

        private void Awake()
        {
            if (FindAnyObjectByType<EventSystem>()) return;
            Type inputType = typeof(StandaloneInputModule);
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
        }

        private void Start()
        {
            _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);

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
            DeactivateUI();
        }

        private void StartHost()
        {
            ApplyConnectionData();
            NetworkManager.Singleton.StartHost();
            DeactivateUI();
        }

        private void DeactivateUI()
        {
            startClientButton.gameObject.SetActive(false);
            startHostButton.gameObject.SetActive(false);
            ipAddressInputField.gameObject.SetActive(false);
            portInputField.gameObject.SetActive(false);
            localIPDisplayText.gameObject.SetActive(false);
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
