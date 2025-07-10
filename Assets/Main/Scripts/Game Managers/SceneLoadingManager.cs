using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace Main.Scripts.Game_Managers
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        public string currentScene;

        private void Awake()
        {
            GameStateManager.startLobby.AddListener(() => LoadNewSceneAdditive("Lobby"));
            GameStateManager.switchMaps.AddListener(TransitionScenes);
        }
        private void TransitionScenes(string newScene)
        {
            if (currentScene.IsNullOrEmpty())
            {
                Debug.LogError("Current scene is not set");
            }
            
            Scene currentSceneObject = SceneManager.GetSceneByName(currentScene);
            currentScene = newScene;

            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
            return;

            void OnSceneEvent(SceneEvent sceneEvent)
            {
                if (sceneEvent.SceneName != newScene ||
                    sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

                if (currentSceneObject.IsValid())
                {
                    NetworkManager.Singleton.SceneManager.UnloadScene(currentSceneObject);
                }
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
        }
        
        private void LoadNewSceneAdditive(string sceneName)
        {
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            currentScene = sceneName;
        }
    }
}