using static UnityEngine.InputSystem.InputAction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace AF
{
    public class InventoryManager : MonoBehaviour
    {
        // Faire un custom editor pour mettre les knowledges et les textures de ces derniers en deux colonnes.
        [Header("PlayerData"), Tooltip("The PlayerData ScriptableObject")]
        [SerializeField] private PlayerData playerData;
        [Space]
        [Header("Knowledge Buttons"), Tooltip("All the Knowledge Button from the inventory UI")]
        [SerializeField] private GameObject[] knowledgesKnown;
        [SerializeField] private Sprite[] knowledgeKnownSprites;
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
                knowledgesEquippedInventory[i].GetComponent<Button>().enabled = false;
            }

            for (int i = 0; i < Enum.GetNames(typeof(KnowledgeID)).Length; i++)
            {
                KnowledgeID currentID = (KnowledgeID)i;
      
                if(playerData.KnownKnowledgeDictionary[currentID])
                {
                    knowledgesKnown[i].GetComponent<Button>().enabled = 
                        playerData.AvailableKnowledgeDictionary[currentID] == AvailableKnowledgePosition.NOT_AVAILABLE;
                    
                    knowledgesKnown[i].GetComponent<Image>().sprite = knowledgeKnownSprites[i];
                }
                else
                {
                    knowledgesKnown[i].GetComponent<Button>().enabled = false;
                    knowledgesKnown[i].GetComponent<Image>().sprite = disabledKnowledgeSprite;
                }

                AvailableKnowledgePosition position = playerData.AvailableKnowledgeDictionary[currentID];
                if(position != AvailableKnowledgePosition.NOT_AVAILABLE)
                {
                    knowledgesEquippedInventory[(int)position - 1].GetComponent<Image>().sprite = knowledgeKnownSprites[i];
                    knowledgesAvailableInHUD[(int)position - 1].sprite = knowledgeKnownSprites[i];
                    knowledgesEquippedInventory[(int)position - 1].GetComponent<Button>().enabled = true;
                }                
            }
        }

        public void InteractEquipped(Transform tr)
        {            
            for (int i = 0; i < knowledgesEquippedInventory.Length; i++)
            {
                if (knowledgesEquippedInventory[i].transform == tr)
                {                    
                    AvailableKnowledgePosition position = (AvailableKnowledgePosition)i + 1;
                    KnowledgeID knowledge = playerData.AvailableKnowledgeDictionary.FirstOrDefault(x => x.Value == position).Key;

                    playerData.AvailableKnowledgeDictionary[knowledge] = AvailableKnowledgePosition.NOT_AVAILABLE;
                    
                    knowledgesKnown[(int)knowledge].GetComponent<Button>().enabled = true;
                    
                    InitializeKnowledgeSprites();
                                        
                    break;
                }
            }
        }

        public void InteractKnowledgeKnown(Transform tr)
        {
            AvailableKnowledgePosition position;
            for (int i = 0; i < knowledgesKnown.Length; i++)
            {
                if (knowledgesKnown[i].transform == tr)  
                {
                    if((position = GetNextKnowledgeSlot()) != AvailableKnowledgePosition.NOT_AVAILABLE)
                    {
                        KnowledgeID knowledge = (KnowledgeID)i;
                        playerData.AvailableKnowledgeDictionary[knowledge] = position;

                        knowledgesKnown[i].GetComponent<Button>().enabled = false;
                        
                        InitializeKnowledgeSprites();

                        break;
                    }
                }
            }
        }

        private AvailableKnowledgePosition GetNextKnowledgeSlot()
        {
            for (int i = 0; i < knowledgesEquippedInventory.Length; i++)
            {
                if (!knowledgesEquippedInventory[i].GetComponent<Button>().enabled)
                {
                    return (AvailableKnowledgePosition)(i + 1);
                }
            }

            return AvailableKnowledgePosition.NOT_AVAILABLE;
        }
    }
}