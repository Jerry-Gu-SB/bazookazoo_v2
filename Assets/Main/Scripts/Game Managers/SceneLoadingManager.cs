using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts.Game_Managers
{
    public class SceneLoadingManager : NetworkBehaviour
    {
        public string currentScene;

        public void LoadScene(string sceneName, Action onLoaded)
        {
            var oldScene = currentScene;
            currentScene = sceneName;

            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

            void HandleSceneEvent(SceneEvent e)
            {
                if (e.SceneName != sceneName || e.SceneEventType != SceneEventType.LoadComplete) return;

                StartCoroutine(DelayCallback());

                if (!string.IsNullOrEmpty(oldScene))
                {
                    var old = SceneManager.GetSceneByName(oldScene);
                    if (old.IsValid())
                    {
                        NetworkManager.Singleton.SceneManager.UnloadScene(old);
                    }
                }
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
            }

            IEnumerator DelayCallback()
            {
                yield return null;
                yield return null;
                onLoaded?.Invoke();
            }
        }
    }
}