using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace Main.Scripts.Game_Managers
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        public string currentScene;

        private void Start()
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

                StartCoroutine(DelaySceneReady());
                
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
        
        private static IEnumerator DelaySceneReady()
        {
            // Let a few frames pass so all Start() / Awake() calls run
            yield return null;
            yield return null;
            GameStateManager.onSceneReady?.Invoke();
        }

    }
}