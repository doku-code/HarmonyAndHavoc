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
        [SerializeField] public GameObject doorToNextLevel;

        public static MapManager Instance;

        void Awake()
        {
            if(SoundManager.Instance != null)
                SoundManager.Instance.PlayAmbientClip(levelAudio.LevelAmbient);
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void UnlockDoor()
        {
            doorToNextLevel.SetActive(false);
        }
    }
}