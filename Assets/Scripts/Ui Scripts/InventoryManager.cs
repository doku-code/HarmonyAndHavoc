using static UnityEngine.InputSystem.InputAction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Serialization;

namespace AF
{
    public class InventoryManager : MonoBehaviour
    {
        // Faire un custom editor pour mettre les knowledges et les textures de ces derniers en deux colonnes.
        [Header("PlayerData"), Tooltip("The PlayerData ScriptableObject")]
        [SerializeField] private PlayerData playerData;
        [Space]
        [Header("Knowledge Buttons"), Tooltip("All the Knowledge Button from the inventory UI")]
        [SerializeField] private Button[] knowledges;
        [SerializeField] private Sprite[] knowledgeSprites;
        [Space]
        [SerializeField] private Sprite disabledKnowledgeSprite;
        [Space]
        [Header("Knowledges Equipped")]
        [SerializeField] private Button[] knowledgesEquippedInventory;
        [SerializeField] private Image[] knowledgesAvailableInHUD;
        [Space]
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject PauseMenuPanel;
        private bool isInventoryMenuOpen = false;

        private void Start()
        {
            InitializeKnowledgeSprites();
        }

        public void InventoryButtonCallback(CallbackContext value)
        {
            if (value.performed)
            {
                OpenInventoryMenu();
            }
        }

        private void OpenInventoryMenu() 
        {
            if (!isInventoryMenuOpen)
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayAClip(1);
                PauseMenuPanel.SetActive(false);
                InventoryPanel.SetActive(true);
                Debug.Log("Opening the inventory menu");
                isInventoryMenuOpen = true;
                InitializeKnowledgeSprites();
            }
            else
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayAClip(1);
                InventoryPanel.SetActive(false);
                isInventoryMenuOpen = false;
            }
        }

        private void InitializeKnowledgeSprites()
        {
            for(int i = 0; i < knowledgesEquippedInventory.Length; i++)
            {                
                knowledgesEquippedInventory[i].GetComponent<Image>().sprite = disabledKnowledgeSprite;
                knowledgesAvailableInHUD[i].sprite = disabledKnowledgeSprite;

            }

            for (int i = 0; i < Enum.GetNames(typeof(KnowledgeID)).Length; i++)
            {
                KnowledgeID currentID = (KnowledgeID)i;
      
                if(playerData.KnownKnowledgeDictionary[currentID])
                {
                    knowledges[i].GetComponent<Image>().sprite = knowledgeSprites[i];
                }
                else
                {
                    knowledges[i].GetComponent<Image>().sprite = disabledKnowledgeSprite;
                }

                AvailableKnowledgePosition position = playerData.AvailableKnowledgeDictionary[currentID];
                if(position != AvailableKnowledgePosition.NOT_AVAILABLE)
                {
                    knowledgesEquippedInventory[(int)position - 1].GetComponent<Image>().sprite = knowledgeSprites[i];
                    knowledgesAvailableInHUD[(int)position - 1].sprite = knowledgeSprites[i];
                }                                
            }
        }

        public void InteractEquipped(Transform tr)
        {
            for (int i = 0; i < knowledgesEquippedInventory.Length; i++)
            {
                if (knowledgesEquippedInventory[i] == tr)
                {
                   // playerData.AvailableKnowledgeDictionary.FirstOrDefault((x) => x.Value == ((AvailableKnowledgePosition)(i + 1))).Key;
                }
            }
        }

        public void InteractKnowledgeKnown(Transform tr)
        {
            
        }
    }
}