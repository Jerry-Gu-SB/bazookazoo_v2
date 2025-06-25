using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.Scripts
{
    public class NetworkUIManager : MonoBehaviour
    {
        [SerializeField]
        private Button startHostButton;
        [SerializeField]
        private Button startClientButton;
        private void Awake()
        {
            if (FindAnyObjectByType<EventSystem>()) return;
            Type inputType = typeof(StandaloneInputModule);
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
        }

        // Start is called before the first frame update
        private void Start()
        {
            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);
        }
        
        private void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            DeactivateButtons();
        }

        private void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            DeactivateButtons();
        }

        private void DeactivateButtons()
        {
            startClientButton.gameObject.SetActive(false);
            startHostButton.gameObject.SetActive(false);
        }
    }
}