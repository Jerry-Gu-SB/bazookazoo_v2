using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Main.Scripts.Game_Managers
{
    public class GameStateManager : NetworkBehaviour
    {
        [SerializeField] private SceneLoadingManager sceneLoadingManager;

        public static UnityEvent<string> switchMaps;
        public static UnityEvent startLobby;
        public static UnityEvent startGame;

        private void Awake()
        {
            if (switchMaps == null) switchMaps = new UnityEvent<string>();
            if (startLobby == null) startLobby = new UnityEvent();
            if (startGame == null) startGame = new UnityEvent();
        }
    }
}
