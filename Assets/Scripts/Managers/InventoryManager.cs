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
        [SerializeField]
        private Button knowledge1;

        [SerializeField] private Button knowledge2;
        [SerializeField] private Button knowledge3;
        [SerializeField] private Button knowledge4;
        [SerializeField] private Button knowledge5;
        [SerializeField] private Button knowledge6;
        [SerializeField] private Button knowledge7;
        [Space] [SerializeField] private Image disabledKnowledgeImage;
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject PauseMenuPanel;
        private bool isInventoryMenuOpen = false;


        public void InventoryMenu(CallbackContext value)
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
    }
}