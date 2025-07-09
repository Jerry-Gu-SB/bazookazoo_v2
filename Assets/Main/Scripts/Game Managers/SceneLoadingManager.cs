using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Main.Scripts.Game_Managers
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        public static void TransitionScenes(string currentScene, string newScene)
        {
            Scene currentSceneObject = SceneManager.GetSceneByName(currentScene);
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
        

        public void LoadNewSceneAdditive(string sceneName)
        {
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}