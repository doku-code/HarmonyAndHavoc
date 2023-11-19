using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace AF
{
    public enum KnowledgeID
    {
        DASH,
        DOUBLE_JUMP,
        WALL_SLIDE,
        GROUND_SLIDE,                
        AIR_ATTACK,
        COMBO_ATTACK,
        AOE_ATTACK        
    }

    public enum AvailableKnowledgePosition
    {
        NOT_AVAILABLE,
        POSITION1,
        POSITION2,
        POSITION3,
        POSITION4
    }
    
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
    public class PlayerData : ScriptableObject
    {        
        [SerializeField] private int orderUpgradeValue;
        public int OrderUpgradeValue
        { 
          get { return orderUpgradeValue; } 
          set { orderUpgradeValue = value; } 
        }

        [SerializeField] private int playerBaseDamage;
        public int PlayerBaseDamage
        {
            get { return playerBaseDamage; }
            set { playerBaseDamage = value; }
        }

        [SerializeField] private Knowledge[] knowledgeBank;

        public ParametersLessDelegate OnDeadDelegate;
        public ParametersLessDelegate OnHitDelegate;
        public ParametersLessDelegate OnChaosDelegate;

        public Dictionary<KnowledgeID, bool> KnownKnowledgeDictionary { get; set; }
        // Put a protection (range, 4 maximum possible knowledges at the same time).
        public Dictionary<KnowledgeID, AvailableKnowledgePosition> AvailableKnowledgeDictionary;
        public Dictionary<KnowledgeID, Knowledge> EveryKnowledgeDictionary;
        
        private int knowledgeSlots;
        public int KnowledgeSlots
        {
            get { return knowledgeSlots;}
            set { knowledgeSlots = value; }
        }

        private int actualOrder;
        public int ActualOrder         
        {
            get { return actualOrder;}
            
            // Todo: Limit actual order value
            set { actualOrder = value; }
        }

        private int maxOrder;
        public int MaxOrder         {
            get { return maxOrder;}
            set { maxOrder = value; }
        }

        private int actualChaos;
        public int ActualChaos         {
            get { return actualChaos;}
            set { actualChaos = value; }
        }

        private int maxChaos;
        public int MaxChaos         {
            get { return maxChaos;}
            set { maxChaos = value; }
        }

        private int gold;
        public int Gold         {
            get { return gold;}
            set { gold = value; }
        }

        private int orderFragments;
        public int OrderFragments
        {
            get { return orderFragments; }
            set
            {
                orderFragments = value;
                orderFragments = orderFragments == 4 ? 0 : orderFragments;
                maxOrder += orderUpgradeValue;
            }
        }

        private int weaponUpgrade;
        public int WeaponUpgrade         {
            get { return weaponUpgrade;}
            set { weaponUpgrade = value; }
        }

        private int armorUpgrade;
        public int ArmorUpgrade         {
            get { return armorUpgrade;}
            set { armorUpgrade = value; }
        }

        public Knowledge GetKnowledgeByID(KnowledgeID id)
        {
            return EveryKnowledgeDictionary[id];
        }

        public void InitializeData()
        {
            KnownKnowledgeDictionary = new Dictionary<KnowledgeID, bool>();
            AvailableKnowledgeDictionary = new Dictionary<KnowledgeID, AvailableKnowledgePosition>();
            EveryKnowledgeDictionary = new Dictionary<KnowledgeID, Knowledge>();

            for (int i = 0; i < Enum.GetNames(typeof(KnowledgeID)).Length; i++)
            {
                KnowledgeID currentID = (KnowledgeID)i;
                
                KnownKnowledgeDictionary.Add(currentID , false);
                AvailableKnowledgeDictionary.Add(currentID, AvailableKnowledgePosition.NOT_AVAILABLE);
                
                for (int j = 0; j < knowledgeBank.Length; j++)
                {
                    if (knowledgeBank[j].ID == currentID)
                    {
                        EveryKnowledgeDictionary.Add(currentID, knowledgeBank[j]);
                    }
                }
            }

            actualOrder = maxOrder;
            actualChaos = maxChaos;
        }

        public void TakeDamage(int dmg)
        {
            actualOrder -= Mathf.Max(dmg - ArmorUpgrade, 0);

            if (actualOrder <= 0)
            {
                OnDeadDelegate();
            }
            else
            {
                OnHitDelegate();
            }

            Debug.Log(actualOrder);
        }

        public int GetPlayerDamage(Knowledge usedKnowledge)
        {
            return usedKnowledge != null
                ? usedKnowledge.damageBoost + playerBaseDamage + weaponUpgrade
                : playerBaseDamage + weaponUpgrade;
        }

        public void HealPlayer(int value)
        {
            // To do: delegate to inform HUDManager

            actualOrder = Mathf.Min(value + actualOrder, maxOrder);
        }

        public bool UseChaos(int value)
        {
            if(actualChaos >= value)
            {
                actualChaos -= value;
                
                OnChaosDelegate();

                return true;
            }

            return false;
        }        
    }
}
