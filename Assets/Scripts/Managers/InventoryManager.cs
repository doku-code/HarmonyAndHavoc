using static UnityEngine.InputSystem.InputAction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace AF
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("PlayerData"), Tooltip("The PlayerData ScriptableObject")] [SerializeField]
        private PlayerData playerData;

        [Space]
        [Header("Knowledge Buttons"), Tooltip("All the Knowledge Button from the inventory UI")]
        [SerializeField] private Button[] knowledges;
        [Space] 
        [SerializeField] private Image disabledKnowledgeImage;
        [Space]
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject PauseMenuPanel;
        private bool isInventoryMenuOpen = false;
        
        public void OpenInventoryMenu(CallbackContext value)
        {
            if (value.performed)
            {
                if (!isInventoryMenuOpen)
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayAClip(1);
                    PauseMenuPanel.SetActive(false);
                    InventoryPanel.SetActive(true);
                    Debug.Log("Opening the inventory menu");
                    isInventoryMenuOpen = true;
                }
                else
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayAClip(1);
                    InventoryPanel.SetActive(false);
                    isInventoryMenuOpen = false;
                }
            }
        }

        private void InitializeKnowledgeTextures()
        {
            
        }
    }
}