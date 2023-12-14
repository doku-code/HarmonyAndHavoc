using JFM;
using UnityEngine;

namespace AF
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] public string previousMap;
        [SerializeField] public string nextMap;
        [SerializeField] public Despawner despawnerBegin;
        [SerializeField] public Despawner despawnerEnd;
        [SerializeField] public GameObject spawnerBegin;
        [SerializeField] public GameObject spawnerEnd;
        [SerializeField] public GameObject savingSpot;
        [SerializeField] public LevelAudio levelAudio;
        [SerializeField] public GameObject doorToNextLevel;
        [SerializeField] public GameObject portalPrefab;
        [SerializeField] public GameObject villagePortalSpot;

        public static MapManager Instance;

        private PlayerData data;

        void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            
            if(SoundManager.Instance != null)
                SoundManager.Instance.PlayAmbientClip(levelAudio.LevelAmbient);
            
        }

        public void UnlockDoor()
        {
            doorToNextLevel.SetActive(false);
            
            GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap] = true;
        }

        public void MakePortal(Transform tr)
        {
            GameObject portalGO = Instantiate(portalPrefab,tr.position,Quaternion.identity);
        }
    }
}