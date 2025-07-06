using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        private string _mapSceneName;
        private Scene _loadedScene;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            }

            base.OnNetworkSpawn();
        }
        
        private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                {
                    // We want to handle this for only the server-side
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // Keep track of the loaded scene, you need this to unload it
                        _loadedScene = sceneEvent.Scene;
                    }
                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on " +
                              $"{clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
                case SceneEventType.UnloadComplete:
                {
                    Debug.Log($"Unloaded the {sceneEvent.SceneName} scene on " +
                              $"{clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
                case SceneEventType.LoadEventCompleted:
                case SceneEventType.UnloadEventCompleted:
                {
                    var loadUnload = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
                    Debug.Log($"{loadUnload} event completed for the following client " +
                              $"identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0)
                    {
                        Debug.LogWarning($"{loadUnload} event timed out for the following client " +
                                         $"identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }
                    break;
                }
                
                case SceneEventType.Load:
                case SceneEventType.Unload:
                case SceneEventType.Synchronize:
                case SceneEventType.ReSynchronize:
                case SceneEventType.SynchronizeComplete:
                case SceneEventType.ActiveSceneChanged:
                case SceneEventType.ObjectSceneChanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LoadScene(string sceneName)
        {
            if (IsServer && string.IsNullOrEmpty(sceneName)) return;
            
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
        public void UnloadScene()
        {
            if (!IsServer || !IsSpawned || _loadedScene.IsValid() || _loadedScene.isLoaded)
            {
                return;
            }
            SceneEventProgressStatus status = NetworkManager.SceneManager.UnloadScene(_loadedScene);
            CheckStatus(status, false);
        }
        
        private void CheckStatus(SceneEventProgressStatus status, bool isLoading = true)
        {
            var sceneEventAction = isLoading ? "load" : "unload";
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to {sceneEventAction} {_mapSceneName} with" +
                                 $" a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
}