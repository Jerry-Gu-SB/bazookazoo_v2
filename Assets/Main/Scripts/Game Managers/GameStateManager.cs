using Main.Scripts.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Main.Scripts.Game_Managers
{
    public class GameStateManager : NetworkBehaviour
    {
        [SerializeField] private SceneLoadingManager sceneLoadingManager;
        public void StartLobby()
        {
            sceneLoadingManager.LoadNewSceneAdditive("Lobby");
            RespawnAllPlayers();
        }

        public void StartMatch(string mapSceneName)
        {
            SceneLoadingManager.TransitionScenes("Lobby", mapSceneName);
            DestroyAllRockets();
            RespawnAllPlayers();
            ResetAllPlayers();
        }

        private static void RespawnAllPlayers()
        {
            foreach (PlayerManager player in FindObjectsByType<PlayerManager>(FindObjectsSortMode.None))
            {
                if (player.IsOwner || player.IsLocalPlayer)
                {
                    PlayerManager.PlayerRespawn.Invoke();
                }
            }
        }

        private static void ResetAllPlayers()
        {
            foreach (PlayerManager player in FindObjectsByType<PlayerManager>(FindObjectsSortMode.None))
            {
                if (player.IsOwner || player.IsLocalPlayer)
                {
                    PlayerManager.ResetPlayer.Invoke();
                }
            }
        }

        private void DestroyAllRockets()
        {
            foreach (RocketProjectile rocket in FindObjectsByType<RocketProjectile>(FindObjectsSortMode.None))
            {
                if (IsServer)
                {
                    Destroy(rocket.gameObject);
                }
            }
        }
    }
}
