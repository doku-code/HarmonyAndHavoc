using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

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
        [Space]
        [SerializeField] private Sprite disabledKnowledgeSprite;
        [Space]
        [Header("Knowledges Equipped")]
        [SerializeField] private Button[] knowledgesEquippedInventory;
        [SerializeField] private Image[] knowledgesAvailableInHUD;

        [Space]
        [Header("Misc")]
        [SerializeField] private TMP_Text armorUpgradeText;
        [SerializeField] private TMP_Text swordUpgradeText;
        [SerializeField] private TMP_Text quarterOrderCountText;
        [SerializeField] private TMP_Text goldCountText;
        [SerializeField] private TMP_Text maxCapacityKnowledgeText;
        [SerializeField] private GameObject[] quarterOrderPieces;
        [Space]
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject PauseMenuPanel;
        private bool isInventoryMenuOpen = false;
        private int currentCapacityKnowledge;
        private int maxCapacityKnowledge = 20;

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
                    SoundManager.Instance.PlayFxClip(1);

                PauseMenuPanel.SetActive(false);
                InventoryPanel.SetActive(true);
                Debug.Log("Opening the inventory menu");
                isInventoryMenuOpen = true;
                InitializeKnowledgeSprites();
                InitializeMisc();

                Time.timeScale = 0;
            }
            else
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayFxClip(1);
                InventoryPanel.SetActive(false);
                isInventoryMenuOpen = false;

                Time.timeScale = 1;
            }
        }

        private void InitializeMisc()
        {
            //Armor Upgrade
            armorUpgradeText.text = playerData.ArmorUpgrade.ToString();
            //Sword Upgrade
            swordUpgradeText.text = playerData.WeaponUpgrade.ToString();
            //Heart Upgrade
            quarterOrderCountText.text = playerData.OrderFragments.ToString();
            //Gold Amount
            goldCountText.text = playerData.Gold.ToString();
            //Calculate knowledge cost 
            currentCapacityKnowledge = CalculateCurrentKnowledgeCost();
            //Heart Pieces Actualization
            for (int i = 0; i < quarterOrderPieces.Length; i++)
            {
                quarterOrderPieces[i].SetActive(false);
            }
            for (int i = 0; i < playerData.OrderFragments; i++)
            {
                quarterOrderPieces[i].SetActive(true);
            }

        }

        private void InitializeKnowledgeSprites()
        {
            for (int i = 0; i < knowledgesEquippedInventory.Length; i++)
            {
                knowledgesEquippedInventory[i].GetComponent<Image>().sprite = disabledKnowledgeSprite;
                knowledgesAvailableInHUD[i].sprite = disabledKnowledgeSprite;
                knowledgesEquippedInventory[i].GetComponent<Button>().enabled = false;
            }

            for (int i = 0; i < Enum.GetNames(typeof(KnowledgeID)).Length; i++)
            {
                KnowledgeID currentID = (KnowledgeID)i;

                if (playerData.KnownKnowledgeDictionary[currentID])
                {
                    knowledgesKnown[i].GetComponent<Button>().enabled =
                        playerData.AvailableKnowledgeDictionary[currentID] == AvailableKnowledgePosition.NOT_AVAILABLE;

                    knowledgesKnown[i].GetComponent<Image>().sprite = playerData.knowledgeImgBank[i];
                }
                else
                {
                    knowledgesKnown[i].GetComponent<Button>().enabled = false;
                    knowledgesKnown[i].GetComponent<Image>().sprite = disabledKnowledgeSprite;
                }

                AvailableKnowledgePosition position = playerData.AvailableKnowledgeDictionary[currentID];
                if (position != AvailableKnowledgePosition.NOT_AVAILABLE)
                {
                    knowledgesEquippedInventory[(int)position - 1].GetComponent<Image>().sprite = playerData.knowledgeImgBank[i];
                    knowledgesAvailableInHUD[(int)position - 1].sprite = playerData.knowledgeImgBank[i];
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
                    playerData.EveryKnowledgeDictionary[knowledge].Deactivate();

                    knowledgesKnown[(int)knowledge].GetComponent<Button>().enabled = true;

                    InitializeKnowledgeSprites();
                    currentCapacityKnowledge = CalculateCurrentKnowledgeCost();

                    maxCapacityKnowledgeText.text = $"Max capacity {currentCapacityKnowledge} / {maxCapacityKnowledge}";

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
                    if ((position = GetNextKnowledgeSlot()) != AvailableKnowledgePosition.NOT_AVAILABLE)
                    {
                        KnowledgeID knowledge = (KnowledgeID)i;
                        if (CanEquipKnowledge(knowledge))
                        {

                            playerData.AvailableKnowledgeDictionary[knowledge] = position;
                            playerData.EveryKnowledgeDictionary[knowledge].Activate();

                            knowledgesKnown[i].GetComponent<Button>().enabled = false;
                            currentCapacityKnowledge += playerData.EveryKnowledgeDictionary[knowledge].slotCost;
                            maxCapacityKnowledgeText.text = $"Max capacity {currentCapacityKnowledge} / {maxCapacityKnowledge}";

                            InitializeKnowledgeSprites();
                        }
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
        private bool CanEquipKnowledge(KnowledgeID knowledgeID)
        {
            currentCapacityKnowledge = CalculateCurrentKnowledgeCost();
            return (currentCapacityKnowledge + playerData.EveryKnowledgeDictionary[knowledgeID].slotCost <= maxCapacityKnowledge);
        }

        private int CalculateCurrentKnowledgeCost()
        {
            int currentCapacityKnowledge = 0;

            for (int i = 0; i < playerData.AvailableKnowledgeDictionary.Count; i++)
            {
                if (playerData.AvailableKnowledgeDictionary[(KnowledgeID)i] != AvailableKnowledgePosition.NOT_AVAILABLE)
                {
                    currentCapacityKnowledge += playerData.EveryKnowledgeDictionary[(KnowledgeID)i].slotCost;
                }
            }
            return currentCapacityKnowledge;
        }
    }
}