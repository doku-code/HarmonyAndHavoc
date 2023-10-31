using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private InputActionReference[] actionsToRebind;

    [Header("Panel")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject controlPanel;

    [Header("Event System Object")]
    [SerializeField] private GameObject mainMenuFirstObj;
    [SerializeField] private GameObject settingFirstObj;
    [SerializeField] private GameObject soundSettingsFirstObj;
    [SerializeField] private GameObject creditFirstObj;
    [SerializeField] private GameObject displayFirstObj;
    [SerializeField] private GameObject controlFirstObj;

    [Header("Display Settings UI ")]
    [SerializeField] private Toggle toggleBtnVSYNC;
    [SerializeField] private TMP_Dropdown qualityPreset;
    [SerializeField] private TMP_Dropdown windowModePreset;
    [SerializeField] private TMP_Dropdown windowSizePreset;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider masterSlider;

    [Header("Keyboard Rebinding button")]
    [SerializeField] private Button[] actionBtn;

    [Header("Keyboard Rebinding text")]
    [SerializeField] private TMP_Text[] actionBtnText;

    void Awake()
    {
        OpenMainMenu();
    }
    public void OpenMainMenu()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        soundPanel.SetActive(false);
        creditPanel.SetActive(false);
        displayPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstObj);
    }
    #region settingFunc
    public void OpenSettingsMenu()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        soundPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingFirstObj);
    }

    public void CloseSettingsMenu()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        soundPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstObj);
    }
    #endregion
    #region soundFunc
    public void OpenSoundMenu()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        soundPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(soundSettingsFirstObj);
    }
    public void CloseSoundMenu()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        soundPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingFirstObj);
    }
    #endregion
    #region creditfunc
    public void OpenCreditMenu()
    {
        mainPanel.SetActive(false);
        creditPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditFirstObj);
    }
    public void CloseCreditMenu()
    {
        mainPanel.SetActive(true);
        creditPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstObj);
    }
    #endregion
    #region displayFunc
    public void OpenDisplayPanel()
    {
        settingsPanel.SetActive(false);
        displayPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(displayFirstObj);
    }
    public void CloseDisplayPanel()
    {
        settingsPanel.SetActive(true);
        displayPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingFirstObj);
    }
    #endregion
    #region controlFunc 
    public void OpenControlSetting()
    {
        settingsPanel.SetActive(false);
        controlPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(controlFirstObj);
    }
    public void CloseControlSetting()
    {
        settingsPanel.SetActive(true);
        controlPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(settingFirstObj);
    }
    #endregion
    #region displaySetting
    public void toggleVsyncOn()
    {
        //https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html
        if (toggleBtnVSYNC == true)
        {
            QualitySettings.vSyncCount = 4;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void ChangeQuality()
    {
        switch (qualityPreset.value)
        {
            case 0:
                QualitySettings.SetQualityLevel(qualityPreset.value, true);
                break;
            case 1:
                QualitySettings.SetQualityLevel(qualityPreset.value, true);
                break;
            case 2:
                QualitySettings.SetQualityLevel(qualityPreset.value, true);
                break;
        }
    }
    public void ChangeWindowMode()
    {
        switch (windowModePreset.value)
        {
            case 0:
                ;
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }
    public void ChangeWindowSize()
    {
        switch (windowSizePreset.value)
        {
            case 0:
                Screen.SetResolution(3840, 2160, false);
                break;
            case 1:
                Screen.SetResolution(2560, 1440, false);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, false);
                break;
            case 3:
                Screen.SetResolution(1600, 900, false);
                break;
            case 4:
                Screen.SetResolution(1440, 900, false);
                break;
            case 5:
                Screen.SetResolution(1366, 768, false);
                break;
            case 6:
                Screen.SetResolution(1280, 720, false);
                break;
            case 7:
                Screen.SetResolution(1280, 1024, false);
                break;
            case 8:
                Screen.SetResolution(1024, 768, false);
                break;
            case 9:
                Screen.SetResolution(800, 600, false);
                break;
        }
    }
    #endregion
    #region soundVolumeFunc
    public void SetAmbientVolume(float volume)
    {
        audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
    }

    public void SetFXVolume(float volume)
    {
        audioMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
    #endregion

    public void MakeNewGame()
    {
        //a reajuster une fois le systeme de sauvegarde fait
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