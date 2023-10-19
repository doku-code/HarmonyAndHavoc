using System;
using System.Collections.Generic;
using AF;
using JetBrains.Annotations;
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
    
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
    public class PlayerData : ScriptableObject
    {
        
        [SerializeField] private int orderUpgradeValue;
        [SerializeField] private int playerBaseDamage;
        [SerializeField] private Knowledge[] knowledgeBank;
        public delegate void ParametersLessDelegate();
        public ParametersLessDelegate OnDeadDelegate;

        public Dictionary<KnowledgeID, bool> KnownKnowledgeDictionary;
        public Dictionary<KnowledgeID, bool> AvalaibleKnowledgeDictionary;
        public Dictionary<KnowledgeID, Knowledge> EveryKnowledgeDictionary;
        
        
        private int knowledgeSlots;
        public int KnowledgeSlots { get; set; }

        private int actualOrder;
        public int ActualOrder { get; set; }

        private int maxOrder;
        public int MaxOrder { get; set; }

        private int actualChaos;
        public int ActualChaos { get; set; }

        private int maxChaos;
        public int MaxChaos { get; set; }

        private int gold;
        public int Gold { get; set; }

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
        public int WeaponUpgdrade { get; set; }

        private int armorUpgrade;
        public int ArmorUpgrade { get; set; }

        private void Awake()
        {
            for (int i = 0; i < Enum.GetNames(typeof(KnowledgeID)).Length; i++)
            {
                KnowledgeID currentID = (KnowledgeID)i;
                
                KnownKnowledgeDictionary.Add(currentID , false);
                AvalaibleKnowledgeDictionary.Add(currentID, false);
                
                for (int j = 0; j < knowledgeBank.Length; j++)
                {
                    if (knowledgeBank[j].ID == currentID)
                    {
                        EveryKnowledgeDictionary.Add(currentID, knowledgeBank[j]);
                    }
                }
            }
        }

        void TakeDamage(int dmg)
        {
            actualOrder = (dmg - ArmorUpgrade);

            if (actualOrder <= 0)
            {
                OnDeadDelegate();
            }
        }

        int GetPlayerDamage(Knowledge usedKnowledge)
        {
            return usedKnowledge != null
                ? usedKnowledge.damageBoost + playerBaseDamage + weaponUpgrade
                : playerBaseDamage + weaponUpgrade;
        }
    }
}
