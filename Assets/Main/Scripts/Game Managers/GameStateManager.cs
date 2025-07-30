using System;
using Main.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Game_Managers
{
    public enum GameState
    {
        Idle,
        Connecting,
        LobbyLoading,
        LobbyReady,
        MapLoading,
        GameReady,
        GameRunning
    }

    public class GameStateManager : NetworkBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public static event Action<GameState> GameStateChanged;

        [SerializeField] private SceneLoadingManager sceneLoader;
        private string _selectedMap = MapNames.Lobby;

        public GameState CurrentState { get; private set; } = GameState.Idle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetSelectedMap(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
            {
                Debug.LogWarning("Map name is null or empty. Defaulting to Lobby.");
                _selectedMap = MapNames.Lobby;
            }
            else
            {
                _selectedMap = mapName;
            }
        }

        public void TransitionToState(GameState newState)
        {
            Debug.Log($"[GameStateManager] Transitioning from {CurrentState} to {newState}");
            CurrentState = newState;
            GameStateChanged?.Invoke(CurrentState);
            HandleState(CurrentState);
        }

        private void HandleState(GameState state)
        {
            switch (state)
            {
                case GameState.Idle:
                    Debug.Log($"[GameStateManager] Received Idle GameState from {CurrentState}");
                    break;
                case GameState.Connecting:
                    Debug.Log($"[GameStateManager] Received Connecting GameState from {CurrentState}");
                    break;
                case GameState.LobbyLoading:
                    sceneLoader.LoadScene(MapNames.Lobby, () => TransitionToState(GameState.LobbyReady));
                    break;
                case GameState.LobbyReady:
                    Debug.Log($"[GameStateManager] Received LobbyReady GameState from {CurrentState}");
                    break;
                case GameState.MapLoading:
                    if (string.IsNullOrEmpty(_selectedMap)) _selectedMap = MapNames.Lobby;
                    sceneLoader.LoadScene(_selectedMap, () => TransitionToState(GameState.GameReady));
                    break;
                case GameState.GameReady:
                    Debug.Log($"[GameStateManager] Received GameReady GameState from {CurrentState}");
                    break;
                case GameState.GameRunning:
                    Debug.Log("[GameStateManager] Game is now running.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
