using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject soundPanel;

    void Awake()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        soundPanel.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        soundPanel.SetActive(false);
    }

    public void OpenSoundMenu()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        soundPanel.SetActive(true);
    }

    public void MakeNewGame()
    {
        //a reajuster une foi le system de sauvegarde fait
        StartCoroutine(LoadYourAsyncScene("Village"));
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    IEnumerator LoadYourAsyncScene(string sceneName)
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
    }
}