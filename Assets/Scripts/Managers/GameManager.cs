using System;
using UnityEngine;

namespace AF
{
    public class GameManager : MonoBehaviour
    {
        [NonSerialized] public string actualLevel;

        public static GameManager Instance { get; private set; }
        
        
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
