using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace Main.Scripts
{
    public class SpawnPointManager : MonoBehaviour
    {
        private int _numberOfSpawnPoints = -1;
        private List<GameObject> _spawnPoints = new List<GameObject>();
        private readonly List<SpawnPoint> _spawnPointScripts = new List<SpawnPoint>();
        private readonly List<Transform> _spawnPointTransforms = new List<Transform>();
        
        private void Awake()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>(true)
                .Select(sp => sp.gameObject)
                .ToList();
            _numberOfSpawnPoints = _spawnPoints.Count;
        }

        private void Start()
        {
            for (int i = 0; i < _numberOfSpawnPoints; i++)
            {
                _spawnPointScripts.Add(_spawnPoints[i].GetComponent<SpawnPoint>());
            }

            for (int i = 0; i < _numberOfSpawnPoints; i++)
            {
                _spawnPointTransforms.Add(_spawnPoints[i].GetComponent<Transform>());
            }
            if (_spawnPointScripts.Count != _numberOfSpawnPoints) Debug.LogError("Spawn Point Scripts Error");
            if (_spawnPointTransforms.Count != _numberOfSpawnPoints) Debug.LogError("Spawn Point Transforms Error");
        }

        public void RespawnPlayer(Transform playerTransform)
        {
            // Finds a spawner that doesn't have a player close
            for (int i = 0; i < 20; i++)
            {
                int spawnerIndex = Random.Range(0, _numberOfSpawnPoints);
                
                if (_spawnPointScripts[spawnerIndex].playerInCloseProximity) continue;
                
                playerTransform.position = _spawnPointTransforms[spawnerIndex].position;
                return;
            }
            // If failed to get a position after 20 attempts, just spawn in a random spot
            playerTransform.position = _spawnPointTransforms[Random.Range(0, _numberOfSpawnPoints)].position;
        }
    }
}
