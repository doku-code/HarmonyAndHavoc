using Cinemachine;
using System;
using System.Collections;
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
        END,
    }
    public class GameManager : MonoBehaviour
    {
        private MapManager currentMapManager;
        [SerializeField] public GameObject player;
        [SerializeField] private FadeInFadeOutScreen loadingScreen;
        [NonSerialized] public string actualMap = "MainMenu";
        [NonSerialized] public ParametersLessDelegate OnLoadMapDelegate;

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

        public void LoadNextMap(string mapToLoad, SpawnerPosition spawnPosition)
        {
            StartCoroutine(LoadYourAsyncScene(mapToLoad, () =>
                {
                    GetCurrentMapManager();                                    

                    actualMap = mapToLoad;
                    
                    if (mapToLoad != "MainMenu")
                    {
                        PlacePlayer(spawnPosition);                        
                        LoadSceneMenu();                        
                    }                    

                    if (OnLoadMapDelegate is not null)
                    {
                        OnLoadMapDelegate();
                    }
                }
            ));
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
            GameObject playerGO = Instantiate(player);
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = playerGO.transform;

            switch (spawnPosition)
            {
                case SpawnerPosition.BEGIN:
                    playerGO.transform.position =
                        currentMapManager.spawnerBegin.transform.position;
                    break;
                case SpawnerPosition.END:
                    playerGO.transform.position =
                        currentMapManager.spawnerEnd.transform.position;
                    break;
            }
        }

        public void GetCurrentMapManager()
        {
            currentMapManager = FindObjectOfType<MapManager>();
        }

        public IEnumerator LoadYourAsyncScene(string sceneName, ParametersLessDelegate callback)
        {
            loadingScreen.FadeInFadeOut();
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
