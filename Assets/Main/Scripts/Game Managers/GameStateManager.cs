using System;
using System.Collections;
using System.Collections.Generic;
using Main.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace Main.Scripts.Game_Managers
{
    public class GameStateManager : NetworkBehaviour
    {
	    public static GameStateManager instance;
	    
        [SerializeField] private SceneLoadingManager sceneLoadingManager;
		[SerializeField] private SpawnPointManager spawnPointManager;
        
        private HashSet<GameObject> _playerGameObjects =  new HashSet<GameObject>();
        
        private void Awake()
        {
	        if (instance == null) instance = this;
	        else Destroy(gameObject);
        }
        
        public void StartLobby()
        {
	        StartCoroutine(LobbyLoad());
	        DestroyAllRockets();
        }

        private IEnumerator LobbyLoad()
        {
	        sceneLoadingManager.LoadNewSceneAdditive("Lobby");
	        float timeout = 10f;
	        float elapsed = 0f;

	        while (!(spawnPointManager = FindFirstObjectByType<SpawnPointManager>()))
	        {
		        if (elapsed >= timeout)
		        {
			        Debug.LogError("SpawnPointManager not found within timeout.");
			        yield break;
		        }

		        yield return null;
		        elapsed += Time.deltaTime;
	        }
	        Debug.Log("found spawnpointmanager: " + spawnPointManager.name);
        }

        public void StartGame(string mapName)
        {
	        StartCoroutine(LoadGameScene(mapName));
	        DestroyAllRockets();
	        ResetPlayers();
        }

        private IEnumerator LoadGameScene(string mapName)
        {
	        sceneLoadingManager.TransitionScenes(mapName);
	        float timeout = 10f;
	        float elapsed = 0f;
	        
	        spawnPointManager = null;
	        while (!(spawnPointManager = FindFirstObjectByType<SpawnPointManager>()))
	        {
		        if (elapsed >= timeout)
		        {
			        Debug.LogError("SpawnPointManager not found within timeout.");
			        yield break;
		        }

		        yield return null;
		        elapsed += Time.deltaTime;
	        }
	        Debug.Log("found spawnpointmanager: " + spawnPointManager.name);
        }
        
        private void ResetPlayers()
        {
	        if (!IsServer) return;
	        
	        foreach (GameObject player in _playerGameObjects)
	        {
		        PlayerManager playerManager = player.GetComponent<PlayerManager>();
		        playerManager.playerScore = 0;
		        RespawnPlayer(player);
	        }
        }

        public void RespawnPlayer(GameObject player)
        {
	        if (!IsServer) return;

	        PlayerManager playerManager = player.GetComponent<PlayerManager>();
	        playerManager.playerHeath = 100;
	        Rigidbody2D playerRigidbody2D = player.GetComponent<Rigidbody2D>();
	        playerRigidbody2D.linearVelocity = Vector2.zero;
	        spawnPointManager.RespawnPlayer(player.GetComponent<Transform>());
        }

        private void DestroyAllRockets()
        {
	        if (!IsServer) return;
	        
	        foreach (RocketProjectile rocket in FindObjectsByType<RocketProjectile>(FindObjectsSortMode.None))
	        {
		        Destroy(rocket.gameObject);
	        }
        }

        public void RegisterPlayer(GameObject player)
        {
	        _playerGameObjects.Add(player);
        }

        public void UnregisterPlayer(GameObject player)
        {
	        _playerGameObjects.Remove(player);
        }
    }
}
