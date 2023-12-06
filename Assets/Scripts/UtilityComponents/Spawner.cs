using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AF
{
    public class Spawner : MonoBehaviour
    {
        private MapManager mapManager;
        [SerializeField] private SpawnerPosition spawnerPosition;

        void Awake()
        {
            mapManager = transform.parent.gameObject.GetComponent<MapManager>();
        }
        
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