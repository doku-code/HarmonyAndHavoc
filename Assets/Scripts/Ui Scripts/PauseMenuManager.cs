using AF;
using TMPro;
using Unity.VisualScripting;
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
        [SerializeField] private GameObject exitPromptPanel;

        [Header("Event System Object"), Tooltip("It is use to change the main selected object in eventSystem GameObject")]
        [SerializeField] private GameObject pauseMenuFirstObj;
        [SerializeField] private GameObject settingFirstObj;

        [Header("Display Settings UI"), Tooltip("Different UI element to change display/Sound")]
        [SerializeField] private TMP_Dropdown windowModePreset;
        [SerializeField] private TMP_Dropdown windowSizePreset;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider fxSlider;
        [SerializeField] private Slider masterSlider;

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
                        SoundManager.Instance.PlayFxClip(1);

                    OpenPauseMenu();

                    
                    if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
                    {
                        EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
                    }
                }
                else
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayFxClip(1);

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
            exitPromptPanel.SetActive(false);
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
        }

        public void ContinueGame()
        {
            isPauseMenuOpen = false;
            ClosePauseMenu();
            Time.timeScale = 1;
            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(pauseMenuFirstObj);
            }
        }

        public void ExitPrompt()
        {
            pausePanel.SetActive(false);
            exitPromptPanel.SetActive(true);
        }
        public void ExitToMainMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.LoadNextMap("MainMenu", SpawnerPosition.END);
        }
        
        public void OpenSettingsMenu()
        {
            SoundManager.Instance.PlayFxClip(1);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
            if (Gamepad.current != null && Mouse.current == null && Keyboard.current == null)
            {
                EventSystem.current.SetSelectedGameObject(settingFirstObj);
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
                    Cursor.lockState = CursorLockMode.None;
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
            if (SoundManager.Instance != null)
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