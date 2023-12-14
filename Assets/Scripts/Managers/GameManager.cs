using Cinemachine;
using System;
using System.Collections;
using JFM;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public delegate void ParametersLessDelegate();
    public delegate void IntegerParameterDelegate(int value);

    public enum SpawnerPosition
    {
        BEGIN,
        SAVING_SPOT,
        PORTAL,
        END,
    }
    public class GameManager : MonoBehaviour
    {
        [NonSerialized] public ParametersLessDelegate OnLoadMapDelegate;
        [NonSerialized] public ParametersLessDelegate OnReadyToLoadMapDelegate;
        [SerializeField] public GameObject player;
        [NonSerialized] public GameObject playerGO;
        [NonSerialized] public PlayerData data;
        private MapManager currentMapManager;
        private FadeInFadeOutScreen loadingScreen;
        private string nextMapToLoad;
        [NonSerialized] public string currentMap = "MainMenu";
        private Transform portalMapPosition;
        private string portalMap;
        private SpawnerPosition nextSpawnPosition;

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

                OnReadyToLoadMapDelegate += OnReadyToLoadMap;
            }

            data = player.GetComponent<PlayerController>().Data;
            loadingScreen = GetComponentInChildren<FadeInFadeOutScreen>();
        }

        private void OnReadyToLoadMap()
        {
            StartCoroutine(LoadYourAsyncScene(nextMapToLoad, () =>
            {
                GetCurrentMapManager();

                currentMap = nextMapToLoad;

                if (nextMapToLoad != "MainMenu" && nextMapToLoad != "Ending")
                {
                    PlacePlayer(nextSpawnPosition);
                    LoadSceneMenu();
                    CheckCurrentProgression();
                }

                if (OnLoadMapDelegate is not null)
                {
                    OnLoadMapDelegate();
                }
            }
            ));
        }

        public void LoadNextMap(string mapToLoad, SpawnerPosition spawnPosition)
        {
            nextMapToLoad = mapToLoad;
            nextSpawnPosition = spawnPosition; 

            loadingScreen.FadeInFadeOut();            
        }

        public void LoadGame()
        {
            LoadNextMap("Village", SpawnerPosition.END);
        }

        public void LoadSceneMenu()
        {
            SceneManager.LoadScene("InGameUI", LoadSceneMode.Additive);
        }

        public void PlacePlayer(SpawnerPosition spawnPosition)
        {
            playerGO = Instantiate(player);
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = playerGO.transform;

            switch (spawnPosition)
            {
                case SpawnerPosition.BEGIN:
                    playerGO.transform.position =
                        currentMapManager.spawnerBegin.transform.position;
                    break;
                case SpawnerPosition.SAVING_SPOT:
                    playerGO.transform.position = 
                        currentMapManager.savingSpot.transform.position;
                    break;
                case SpawnerPosition.PORTAL:
                    playerGO.transform.position = portalMapPosition.position;
                    break;
                case SpawnerPosition.END:
                    playerGO.transform.position =
                        currentMapManager.spawnerEnd.transform.position;
                    break;
            }
        }

        void CheckCurrentProgression()
        {
            if (!data.CurrentPlayerMapProgression.ContainsKey(currentMap))
            {
                data.CurrentPlayerMapProgression.Add(currentMap, false);
            }
            else if (data.CurrentPlayerMapProgression[currentMap])
            {
                currentMapManager.UnlockDoor();
            }
        }

        public void GetCurrentMapManager()
        {
            currentMapManager = FindObjectOfType<MapManager>();
        }

        void PortalToVillage()
        {
            portalMap = currentMap;
            portalMapPosition = playerGO.transform;
            LoadNextMap("Village", SpawnerPosition.END);
        }

        void PortalToMap()
        {
            
        }

        public IEnumerator LoadYourAsyncScene(string sceneName, ParametersLessDelegate callback)
        {
            AsyncOperation aSyncLoad = SceneManager.LoadSceneAsync(sceneName);
            aSyncLoad.allowSceneActivation = false;

            while (!aSyncLoad.isDone)
            {
                if (aSyncLoad.progress >= 0.90f)
                {
                    aSyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }

            if (callback is not null)
            {
                callback();
            }
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
