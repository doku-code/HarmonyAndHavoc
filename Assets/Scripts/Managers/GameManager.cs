using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public enum SpawnerPosition
    {
        BEGIN,
        END,
    }
    public class GameManager : MonoBehaviour
    {
        private MapManager currentMapManager;
        private GameObject player;
        [NonSerialized] public string actualMap;
        
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
            StartCoroutine(LoadYourAsyncScene(mapToLoad, spawnPosition));
        }

        public void LoadGame()
        {
            LoadNextMap("Village", SpawnerPosition.END);
            LoadSceneMenu();
        }

        public void LoadSceneMenu()
        {
            SceneManager.LoadScene("InGameUI", LoadSceneMode.Additive);
        }

        public void UnloadSceneMenu()
        {
            SceneManager.UnloadSceneAsync("InGameUI");
        }

        public void PlacePlayer(SpawnerPosition spawnPosition)
        {
            player = GameObject.FindWithTag("Player");
            
            switch (spawnPosition)
            {
                case SpawnerPosition.BEGIN:
                    player.transform.position = 
                        currentMapManager.spawnerBegin.transform.position;
                    break;
                case SpawnerPosition.END:
                    player.transform.position = 
                        currentMapManager.spawnerEnd.transform.position;
                    break;
            }
        }

        public void GetCurrentMapManager()
        {
            currentMapManager = FindObjectOfType<MapManager>();
        }

        public IEnumerator LoadYourAsyncScene(string sceneName, SpawnerPosition spawnPosition)
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
            
            GetCurrentMapManager();
            PlacePlayer(spawnPosition);
            actualMap = sceneName;
            
            if(sceneName == "InGameUI")
                UnloadSceneMenu();
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
