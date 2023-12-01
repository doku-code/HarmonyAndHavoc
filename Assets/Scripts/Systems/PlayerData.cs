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
        public SingleParameterDelegate OnOrderDelegate;
        public ParametersLessDelegate OnChaosDelegate;

        public Dictionary<KnowledgeID, bool> KnownKnowledgeDictionary { get; set; }
        // Put a protection (range, 4 maximum possible knowledges at the same time).
        public Dictionary<KnowledgeID, AvailableKnowledgePosition> AvailableKnowledgeDictionary;
        public Dictionary<KnowledgeID, Knowledge> EveryKnowledgeDictionary;

        [SerializeField] private int knowledgeSlots;
        public int KnowledgeSlots
        {
            get { return knowledgeSlots;}
            set { knowledgeSlots = value; }
        }

        [SerializeField] private int actualOrder;
        public int ActualOrder         
        {
            get { return actualOrder;}
            
            // Todo: Limit actual order value
            set { actualOrder = value; }
        }

        [SerializeField] private int maxOrder;
        public int MaxOrder         {
            get { return maxOrder;}
            set { maxOrder = value; }
        }

        [SerializeField] private int actualChaos;
        public int ActualChaos         {
            get { return actualChaos;}
            set { actualChaos = value; }
        }

        [SerializeField] private int maxChaos;
        public int MaxChaos         {
            get { return maxChaos;}
            set { maxChaos = value; }
        }

        [SerializeField] private int gold;
        public int Gold         {
            get { return gold;}
            set { gold = value; }
        }

        [SerializeField] private int orderFragments;
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

        [SerializeField] private int weaponUpgrade;
        public int WeaponUpgrade         {
            get { return weaponUpgrade;}
            set { weaponUpgrade = value; }
        }

        [SerializeField] private int armorUpgrade;
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

            Debug.Log($"actualOrder {actualOrder} = maxOrder {maxOrder}");
            actualOrder = maxOrder;
            actualChaos = maxChaos;
        }

        public void TakeDamage(int dmg)
        {
            int actualDmg = Mathf.Max(dmg - ArmorUpgrade, 0);
            actualOrder -= actualDmg;

            if (actualOrder <= 0)
            {
                OnDeadDelegate();
            }
            else
            {
                OnOrderDelegate(-actualDmg);
            }
        }

        public int GetPlayerDamage(Knowledge usedKnowledge)
        {
            return usedKnowledge != null
                ? usedKnowledge.damageBoost + playerBaseDamage + weaponUpgrade
                : playerBaseDamage + weaponUpgrade;
        }

        public void HealPlayer(int value)
        {
            OnOrderDelegate(value);

            actualOrder = Mathf.Min(value + actualOrder, maxOrder);
        }

        public bool UseChaos(int value)
        {
            if(actualChaos >= value)
            {
                actualChaos -= value;

                if (OnChaosDelegate is not null)
                {
                    OnChaosDelegate();
                }

                return true;
            }

            return false;
        }        
    }
}
