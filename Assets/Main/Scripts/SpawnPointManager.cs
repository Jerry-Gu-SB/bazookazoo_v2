using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace Main.Scripts
{
    public class SpawnPointManager : MonoBehaviour
    {
        private List<Transform> _spawnPoints = new();
        private List<SpawnPoint> _spawnPointScripts = new();

        private void Awake()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>(true).Select(sp => sp.transform).ToList();
            _spawnPointScripts = _spawnPoints.Select(t => t.GetComponent<SpawnPoint>()).ToList();
        }

        public void RespawnPlayer(Transform playerTransform)
        {
            for (int i = 0; i < 20; i++)
            {
                int index = Random.Range(0, _spawnPoints.Count);
                if (_spawnPointScripts[index].playerInCloseProximity) continue;
                playerTransform.position = _spawnPoints[index].position;
                return;
            }
            playerTransform.position = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
        }
    }
}