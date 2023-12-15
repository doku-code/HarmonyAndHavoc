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
            
            if (GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap])
            {
                UnlockDoor();
            }
        }

        public void UnlockDoor()
        {
            doorToNextLevel.SetActive(false);

            Debug.Log($"UnlockDoor(): GameManager.Instance.currentMap={GameManager.Instance.currentMap}");
            GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap] = true;
        }

        public void MakePortal(Vector3 position)
        {            
            GameObject portalGO = Instantiate(portalPrefab, position - Vector3.up * portalPrefab.GetComponent<BoxCollider2D>().offset.y, Quaternion.identity);
        }
    }
}