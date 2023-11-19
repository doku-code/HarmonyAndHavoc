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

        [Space]
        [Header("Other UI menus")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject HUDPanel;

        void Start()
        {
            InitializeMixerAtStart();
            EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
            Application.targetFrameRate = -1;
        }

        void InitializeMixerAtStart()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.LoadSoundSetting(masterSlider, SoundManager.Instance.sMasterVolume);
                SoundManager.Instance.LoadSoundSetting(fxSlider, SoundManager.Instance.sFXVolume);
                SoundManager.Instance.LoadSoundSetting(ambientSlider, SoundManager.Instance.sAmbientVolume);
                SoundManager.Instance.SetMasterVolume(masterSlider.value);
                SoundManager.Instance.SetFXVolume(fxSlider.value);
                SoundManager.Instance.SetAmbientVolume(ambientSlider.value);
            }
        }

        public void PauseMenu(CallbackContext value)
        {
            if (value.performed)
            {

                if (!isPauseMenuOpen)
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayAClip(1);

                    OpenPauseMenu();

                    
                    if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
                    {
                        EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
                    }
                }
                else
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayAClip(1);

                    ClosePauseMenu();

                    
                    if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
                    {
                        EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
                    }
                }
                isPauseMenuOpen = !isPauseMenuOpen;
            }
        }

        // A very good idea would be to create a method that takes a GameObject (panel) in parameter and opens (sets active) it and
        // closes all other panel GOs (would have to have a list)
        public void OpenPauseMenu()
        {
            pausePanel.SetActive(true);
            inventoryPanel.SetActive(false);
            HUDPanel.SetActive(false);
            CloseOtherPanels();

            //Pause the Game
            Time.timeScale = 0;
        }

        public void ClosePauseMenu()
        {
            pausePanel.SetActive(false);
            HUDPanel.SetActive(true);
            CloseOtherPanels();

            //Unpause the Game
            Time.timeScale = 1;
        }

        private void CloseOtherPanels()
        {
            settingsPanel.SetActive(false);
            soundPanel.SetActive(false);
            displayPanel.SetActive(false);
            controlPanel.SetActive(false);
        }

        public void ContinueGame()
        {
            isPauseMenuOpen = false;
            ClosePauseMenu();

            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
            }
        }

        public void ExitToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.LoadNextMap("MainMenu", SpawnerPosition.END);
        }
        
        public void OpenSettingsMenu()
        {
            SoundManager.Instance.PlayAClip(1);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
            soundPanel.SetActive(false);
            displayPanel.SetActive(false);
            controlPanel.SetActive(false);
            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(settingFirstObj);
            }
        }        

        public void OpenSoundMenu()
        {
            SoundManager.Instance.PlayAClip(1);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(false);
            soundPanel.SetActive(true);
            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(soundSettingsFirstObj);
            }
        }        
       
        public void OpenDisplayPanel()
        {
            SoundManager.Instance.PlayAClip(1);
            settingsPanel.SetActive(false);
            displayPanel.SetActive(true);
            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(displayFirstObj);
            }
        }        

        public void OpenControlSetting()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayAClip(1);
            settingsPanel.SetActive(false);
            controlPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(controlFirstObj);
        }

        public void ChangeWindowMode()
        {
            if (SoundManager.Instance != null)
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
            if (SoundManager.Instance != null)
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

        public void SetAmbientVolume()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.SetAmbientVolume(ambientSlider.value);
        }

        public void SetFXVolume()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.SetFXVolume(fxSlider.value);
        }

        public void SetMasterVolume()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.SetMasterVolume(masterSlider.value);
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