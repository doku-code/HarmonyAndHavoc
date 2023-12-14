using JFM;
using System;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Serialization;

namespace AF
{
    public class Despawner : MonoBehaviour
    {
        [SerializeField] private SpawnerPosition spawnerPosition;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                switch (spawnerPosition)
                {
                    case SpawnerPosition.BEGIN:
                        GameManager.Instance.LoadNextMap(MapManager.Instance.previousMap, SpawnerPosition.END);
                        break;
                    case SpawnerPosition.PORTAL:

                        other.GetComponent<PlayerController>().Vanish();
                        
                        if (GameManager.Instance.currentMap != "Village")
                            GameManager.Instance.PortalToVillage();
                        else
                            GameManager.Instance.PortalToMap();
                        break;
                    case SpawnerPosition.END:
                        GameManager.Instance.LoadNextMap(MapManager.Instance.nextMap, SpawnerPosition.BEGIN);
                        break;
                }
            }
        }
    }
}