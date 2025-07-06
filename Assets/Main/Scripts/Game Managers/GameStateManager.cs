using Main.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Game_Managers
{
    public class GameStateManager : NetworkBehaviour
    {
        [SerializeField] private SceneLoadingManager sceneLoadingManager;

        public void StartLobby()
        {
            sceneLoadingManager.LoadScene("Lobby");
            RespawnAllPlayers();
        }

        public void StartMapFromLobby(string mapSceneName)
        {
            sceneLoadingManager.UnloadScene();
            sceneLoadingManager.LoadScene(mapSceneName);
            RespawnAllPlayers();
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
    }
}
