using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Scripts.World_Objects
{
    public class SpawnPointManager : MonoBehaviour
    {
        private List<Transform> _spawnPoints = new();
        private List<SpawnPoint> _spawnPointScripts = new();
        private readonly HashSet<int> _usedSpawnIndices = new();
        
        private void Awake()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>(true).Select(sp => sp.transform).ToList();
            _spawnPointScripts = _spawnPoints.Select(t => t.GetComponent<SpawnPoint>()).ToList();
        }

        public void RespawnPlayer(Transform playerTransform, bool requireUnique = false)
        {
            for (int i = 0; i < 20; i++)
            {
                int index = Random.Range(0, _spawnPoints.Count);

                if (requireUnique && _usedSpawnIndices.Contains(index))
                    continue;

                if (!requireUnique && _spawnPointScripts[index].playerInCloseProximity)
                    continue;

                playerTransform.position = _spawnPoints[index].position;

                if (requireUnique)
                    _usedSpawnIndices.Add(index);
                return;
            }
            playerTransform.position = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
        }
    }
}