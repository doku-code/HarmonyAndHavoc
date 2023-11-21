using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AF
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        [SerializeField] private SpawnerPosition spawnerPosition;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                switch (spawnerPosition)
                {
                    case SpawnerPosition.BEGIN:
                        GameManager.Instance.LoadNextMap(mapManager.previousMap, SpawnerPosition.END);
                        break;
                    case SpawnerPosition.END:
                        GameManager.Instance.LoadNextMap(mapManager.nextMap, SpawnerPosition.BEGIN);
                        break;
                }
            }
        }
    }
}