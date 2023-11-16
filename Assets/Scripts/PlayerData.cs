using System;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public enum KnowledgeID
    {
        DASH,
        WALL_SLIDE,
        GROUND_SLIDE,
        DOUBLE_JUMP,
        AOE_ATTACK,
        AIR_ATTACK,
        COMBO_ATTACK,
        DEFAULT
    }

    public enum AvalaibleKnowledgePosition
    {
        NOT_AVALAIBLE,
        POSITION1,
        POSITION2,
        POSITION3,
        POSITION4
    }
    
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private int orderUpgradeValue;
        [SerializeField] private int playerBaseDamage;
        [SerializeField] private Knowledge[] knowledgeBank;
        
        public delegate void ParametersLessDelegate();
        public ParametersLessDelegate OnDeadDelegate;
        
        public ParametersLessDelegate OnHitDelegate;
        
        
        public Dictionary<KnowledgeID, bool> KnownKnowledgeDictionary { get; set; }
        public Dictionary<KnowledgeID, AvalaibleKnowledgePosition> AvalaibleKnowledgeDictionary;
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
        public int WeaponUpgdrade         {
            get { return weaponUpgrade;}
            set { weaponUpgrade = value; }
        }

        private int armorUpgrade;
        public int ArmorUpgrade         {
            get { return armorUpgrade;}
            set { armorUpgrade = value; }
        }

        public void InitializeData()
        {
            KnownKnowledgeDictionary = new Dictionary<KnowledgeID, bool>();
            AvalaibleKnowledgeDictionary = new Dictionary<KnowledgeID, AvalaibleKnowledgePosition>();
            EveryKnowledgeDictionary = new Dictionary<KnowledgeID, Knowledge>();

            for (int i = 0; i < Enum.GetNames(typeof(KnowledgeID)).Length; i++)
            {
                KnowledgeID currentID = (KnowledgeID)i;
                
                KnownKnowledgeDictionary.Add(currentID , false);
                AvalaibleKnowledgeDictionary.Add(currentID, AvalaibleKnowledgePosition.NOT_AVALAIBLE);
                
                for (int j = 0; j < knowledgeBank.Length; j++)
                {
                    if (knowledgeBank[j].ID == currentID)
                    {
                        EveryKnowledgeDictionary.Add(currentID, knowledgeBank[j]);
                    }
                }
            }
        }

        public void TakeDamage(int dmg)
        {
            actualOrder -= (dmg - ArmorUpgrade);

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

        int GetPlayerDamage(Knowledge usedKnowledge)
        {
            return usedKnowledge != null
                ? usedKnowledge.damageBoost + playerBaseDamage + weaponUpgrade
                : playerBaseDamage + weaponUpgrade;
        }
    }
}
