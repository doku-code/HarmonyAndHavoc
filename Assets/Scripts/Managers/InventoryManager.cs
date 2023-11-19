using static UnityEngine.InputSystem.InputAction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

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
        [Header("Knowledges available")]
        [SerializeField] private Button[] knowledgesAvailableInMenu;
        [SerializeField] private Image[] knowledgesAvailableInGame;
        [Space]
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject PauseMenuPanel;
        private bool isInventoryMenuOpen = false;

        private void Awake()
        {
            OpenInventoryMenu();
            OpenInventoryMenu();
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
            for(int i = 0; i < knowledgesAvailableInMenu.Length; i++)
            {                
                knowledgesAvailableInMenu[i].GetComponent<Image>().sprite = disabledKnowledgeSprite;
                knowledgesAvailableInGame[i].sprite = disabledKnowledgeSprite;

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
                    knowledgesAvailableInMenu[(int)position - 1].GetComponent<Image>().sprite = knowledgeSprites[i];
                    knowledgesAvailableInGame[(int)position - 1].sprite = knowledgeSprites[i];
                }                                
            }
        }
    }
}