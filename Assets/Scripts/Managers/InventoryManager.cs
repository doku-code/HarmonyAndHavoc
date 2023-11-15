using static UnityEngine.InputSystem.InputAction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace AF
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("PlayerData"), Tooltip("The PlayerData ScriptableObject")] 
        [SerializeField] private PlayerData playerData;
        [Space]
        [Header("Knowledge Buttons"), Tooltip("All the Knowledge Button from the inventory UI")]
        [SerializeField] private Button knowledge1;
        [SerializeField] private Button knowledge2;
        [SerializeField] private Button knowledge3;
        [SerializeField] private Button knowledge4;
        [SerializeField] private Button knowledge5;
        [SerializeField] private Button knowledge6;
        [SerializeField] private Button knowledge7;
        [Space]
        [SerializeField] private Image disabledKnowledgeImage;
        [SerializeField] private GameObject InventoryPanel;
        private bool isInventoryMenuOpen = false;
        
        public static InventoryManager Instance { get; private set; }

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

        public void OpenInventory()
        {
            
            
        }
        
        public void InventoryMenu(CallbackContext value)
        {
            if (value.performed)
            {
                if (!isInventoryMenuOpen)
                {
                    //SoundManager.Instance.PlayAClip(1);
                    InventoryPanel.SetActive(true);
                    isInventoryMenuOpen = true;
                }
                else
                {
                    //SoundManager.Instance.PlayAClip(1);
                    InventoryPanel.SetActive(false);
                    isInventoryMenuOpen = false;
                }
            }
        }
    }
}
