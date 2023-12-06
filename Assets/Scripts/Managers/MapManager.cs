using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] public string previousMap;
        [SerializeField] public string nextMap;
        [SerializeField] public Spawner despawnerBegin;
        [SerializeField] public Spawner despawnerEnd;
        [SerializeField] public GameObject spawnerBegin;
        [SerializeField] public GameObject spawnerEnd;
        [SerializeField] public GameObject savingSpot;
        [SerializeField] public LevelAudio levelAudio;
    }
}