using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace Main.Scripts
{
    public class SpawnPointManager : MonoBehaviour
    {
        private List<GameObject> _spawnPoints = new List<GameObject>();
        [SerializeField]
        private int numberOfSpawnPoints = -1;
        
        private void Awake()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>(true)
                .Select(sp => sp.gameObject)
                .ToList();
            numberOfSpawnPoints = _spawnPoints.Count;
        }
        
        public void RespawnPlayer(Transform playerTransform)
        {
            // Finds a spawner that doesn't have a player close
            for (int i = 0; i < 20; i++)
            {
                int spawnerIndex = Random.Range(0, numberOfSpawnPoints);
                
                if (_spawnPoints[spawnerIndex].GetComponent<SpawnPoint>().playerInCloseProximity) continue;
                
                playerTransform.position = _spawnPoints[spawnerIndex].GetComponent<Transform>().position;
                
                return;
            }
            // If failed to get a position after 20 attempts, just spawn in a random spot
            playerTransform.position = _spawnPoints[Random.Range(0, numberOfSpawnPoints)].transform.position;
        }
    }
}
