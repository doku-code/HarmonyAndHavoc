using JFM;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AF
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private InputActionReference[] actionsToRebind;

        [Header("Panel"), Tooltip("This is where all the panel goes")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditPanel;

        [Header("Event System Object"), Tooltip("It is use to change the main selected object in eventSystem GameObject")]
        [SerializeField] private GameObject mainMenuFirstObj;
        [SerializeField] private GameObject settingFirstObj;
        [SerializeField] private GameObject creditFirstObj;

        [Header("Display Settings UI"), Tooltip("Different UI element to change display/Sound")]
        [SerializeField] private TMP_Dropdown windowModePreset;
        [SerializeField] private TMP_Dropdown windowSizePreset;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider fxSlider;
        [SerializeField] private Slider masterSlider;

        [Header("Keyboard Rebinding button"), Tooltip("A array of button that action will go in")]
        [SerializeField] private Button[] actionBtn;

        [Header("Keyboard Rebinding text"), Tooltip("Text array for the button mapping")]
        [SerializeField] private TMP_Text[] actionBtnText;
        [Header("Music")]
        [SerializeField] private AudioClip mainMenuMusic;

        void Start()
        {
            InitializeMixerAtStart();
            OpenMainMenu();
            Application.targetFrameRate = -1;

            if(SoundManager.Instance != null)
            SoundManager.Instance.PlayAmbientClip(mainMenuMusic);
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
        
        public void OpenMainMenu()
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
            creditPanel.SetActive(false);

            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuFirstObj);
            }
        }

        public void OpenSettingsMenu()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
            creditPanel.SetActive(false);
            SoundManager.Instance.PlayFxClip(1);

            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(settingFirstObj);
            }
        }
        
        public void OpenCreditMenu()
        {
            mainPanel.SetActive(false);
            creditPanel.SetActive(true);
            settingsPanel.SetActive(false);
            SoundManager.Instance.PlayFxClip(1);

            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(creditFirstObj);
            }
        }

        public void ChangeWindowMode()
        {
            SoundManager.Instance.PlayFxClip(0);

            switch (windowModePreset.value)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Cursor.visible = true;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
            }
        }
        
        public void ChangeWindowSize()
        {
            SoundManager.Instance.PlayFxClip(0);

            switch (windowSizePreset.value)
            {
                case 0:
                    Screen.SetResolution(3840, 2160, true);
                    break;
                case 1:
                    Screen.SetResolution(2560, 1440, true);
                    break;
                case 2:
                    Screen.SetResolution(1920, 1080, true);
                    break;
                case 3:
                    Screen.SetResolution(1600, 900, true);
                    break;
                case 4:
                    Screen.SetResolution(1440, 900, true);
                    break;
                case 5:
                    Screen.SetResolution(1366, 768, true);
                    break;
                case 6:
                    Screen.SetResolution(1280, 720, true);
                    break;
                case 7:
                    Screen.SetResolution(1280, 1024, true);
                    break;
                case 8:
                    Screen.SetResolution(1024, 768, true);
                    break;
                case 9:
                    Screen.SetResolution(800, 600, true);
                    break;
            }
        }
        
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

        public void MakeNewGame()
        {
            GameManager.Instance.player.GetComponent<PlayerController>().Data.InitializeData();
            GameManager.Instance.LoadGame();
            ChaosOrderSystem.Instance.ResetChaosLevel();            
        }

        public void ContinueGame()
        {
            GameManager.Instance.LoadGame();
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