using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] public string previousMap;
        [SerializeField] public string nextMap;
        [SerializeField] public Spawner spawnerBegin;
        [SerializeField] public Spawner spawnerEnd;
    }
}