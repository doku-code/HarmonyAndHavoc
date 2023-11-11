using AF;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace charles
{
    public class PauseMenuManager : MonoBehaviour
    {
        private bool isPauseMenuOpen = false;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private InputActionReference[] actionsToRebind;

        [Header("Panel"), Tooltip("This is where all the panel goes")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject soundPanel;
        [SerializeField] private GameObject displayPanel;
        [SerializeField] private GameObject controlPanel;

        [Header("Event System Object"), Tooltip("It is use to change the main selected object in eventSystem GameObject")]
        [SerializeField] private GameObject pauseMenuFirstObj;
        [SerializeField] private GameObject settingFirstObj;
        [SerializeField] private GameObject soundSettingsFirstObj;
        [SerializeField] private GameObject displayFirstObj;
        [SerializeField] private GameObject controlFirstObj;

        [Header("Display Settings UI"), Tooltip("Different UI element to change display/Sound")]
        [SerializeField] private Toggle toggleBtnVSYNC;
        [SerializeField] private TMP_Dropdown qualityPreset;
        [SerializeField] private TMP_Dropdown windowModePreset;
        [SerializeField] private TMP_Dropdown windowSizePreset;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider fxSlider;
        [SerializeField] private Slider masterSlider;

        [Header("Keyboard Rebinding button"), Tooltip("A array of button that action will go in")]
        [SerializeField] private Button[] actionBtn;

        [Header("Keyboard Rebinding text"), Tooltip("Text array for the button mapping")]
        [SerializeField] private TMP_Text[] actionBtnText;

        void Start()
        {
            InitializeMixerAtStart();
            DontDestroyOnLoad(this);
            EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
            Application.targetFrameRate = -1;
        }
        void InitializeMixerAtStart()
        {
            SoundManager.Instance.LoadSoundSetting(masterSlider, SoundManager.Instance.sMasterVolume);
            SoundManager.Instance.LoadSoundSetting(fxSlider, SoundManager.Instance.sFXVolume);
            SoundManager.Instance.LoadSoundSetting(ambientSlider, SoundManager.Instance.sAmbientVolume);
            SoundManager.Instance.SetMasterVolume(masterSlider.value);
            SoundManager.Instance.SetFXVolume(fxSlider.value);
            SoundManager.Instance.SetAmbientVolume(ambientSlider.value);
        }
        #region PauseFunc
        public void PauseMenu(CallbackContext value)
        {
            if (value.performed)
            {
                if (!isPauseMenuOpen)
                {
                    SoundManager.Instance.PlayAClip(1);
                    isPauseMenuOpen = true;
                    pausePanel.SetActive(true);
                    settingsPanel.SetActive(false);
                    soundPanel.SetActive(false);
                    displayPanel.SetActive(false);
                    controlPanel.SetActive(false);
                    Time.timeScale = 0;
                   EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
                }
                else
                {
                    SoundManager.Instance.PlayAClip(1);
                    isPauseMenuOpen = false;
                    pausePanel.SetActive(false);
                    settingsPanel.SetActive(false);
                    soundPanel.SetActive(false);
                    displayPanel.SetActive(false);
                    controlPanel.SetActive(false);
                    Time.timeScale = 1;
                   EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
                }
            }
        }
        public void ContinueGame()
        {
            isPauseMenuOpen = false;
            pausePanel.SetActive(false);
            settingsPanel.SetActive(false);
            soundPanel.SetActive(false);
            displayPanel.SetActive(false);
            controlPanel.SetActive(false);
            Time.timeScale = 1;
            EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
        }
        public void ExitToMainMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        #endregion
        #region settingFunc
        public void OpenSettingsMenu()
        {
            SoundManager.Instance.PlayAClip(1);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
            soundPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingFirstObj);
            Time.timeScale = 0;
        }

        public void CloseSettingsMenu()
        {
            SoundManager.Instance.PlayAClip(0);
            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);
            soundPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
            Time.timeScale = 0;
        }
        #endregion
        #region soundFunc
        public void OpenSoundMenu()
        {
            SoundManager.Instance.PlayAClip(1);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(false);
            soundPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(soundSettingsFirstObj);
            Time.timeScale = 0;
        }
        public void CloseSoundMenu()
        {
            SoundManager.Instance.PlayAClip(0);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
            soundPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingFirstObj);
            Time.timeScale = 0;
        }
        #endregion
        #region displayFunc
        public void OpenDisplayPanel()
        {
            SoundManager.Instance.PlayAClip(1);
            settingsPanel.SetActive(false);
            displayPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(displayFirstObj);
        }
        public void CloseDisplayPanel()
        {
            SoundManager.Instance.PlayAClip(0);
            settingsPanel.SetActive(true);
            displayPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingFirstObj);
        }
        #endregion
        #region controlFunc 
        public void OpenControlSetting()
        {
            SoundManager.Instance.PlayAClip(1);
            settingsPanel.SetActive(false);
            controlPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(controlFirstObj);
            Time.timeScale = 0;
        }
        public void CloseControlSetting()
        {
            SoundManager.Instance.PlayAClip(0);
            settingsPanel.SetActive(true);
            controlPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingFirstObj);
            Time.timeScale = 0;
        }
        #endregion
        #region displaySetting
        public void ChangeWindowMode()
        {
            SoundManager.Instance.PlayAClip(0);
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
            SoundManager.Instance.PlayAClip(0);
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
        public void SetAmbientVolume()
        {
            SoundManager.Instance.SetAmbientVolume(ambientSlider.value);
        }

        public void SetFXVolume()
        {
            SoundManager.Instance.SetFXVolume(fxSlider.value);
        }

        public void SetMasterVolume()
        {
            SoundManager.Instance.SetMasterVolume(masterSlider.value);
        }
        #endregion

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