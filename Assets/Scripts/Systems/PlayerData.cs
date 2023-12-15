using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public delegate void KnowledgeIDParameterDelegate(KnowledgeID id);
        
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
        [SerializeField] public Sprite[] knowledgeImgBank;
        
        public KnowledgeIDParameterDelegate OnLearningKnowledge;
        public ParametersLessDelegate OnDeadDelegate;
        public IntegerParameterDelegate OnOrderDelegate;
        public IntegerParameterDelegate OnChaosDelegate;

        //CurrentProgression
        public Dictionary<string, bool> CurrentPlayerMapProgression;
        public string lastSafeSpotMap;
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
            set 
            {
                int change = value - actualOrder;
                actualOrder = Mathf.Min(value, maxOrder);
                if (OnOrderDelegate is not null)
                {
                    OnOrderDelegate(change);
                }
            }
        }

        [SerializeField] private const int initialMaxOrder = 200;

        [SerializeField] private int maxOrder;
        public int MaxOrder         
        {
            get { return maxOrder;}
            set { maxOrder = value; }
        }

        [SerializeField] private int actualChaos;
        public int ActualChaos         
        {
            get { return actualChaos;}
            set 
            {
                int change = value - actualOrder;
                actualChaos = Mathf.Min(value, maxChaos);
                if (OnChaosDelegate is not null)
                {
                    OnChaosDelegate(change);
                }
            }
        }

        [SerializeField] private int maxChaos;
        public int MaxChaos         
        {
            get { return maxChaos;}            
        }

        [SerializeField] private int gold;
        public int Gold         
        {
            get { return gold;}
            set 
            {
                totalGold += Mathf.Max(value - gold, 0);
                gold = value;                 
            }
        }

        [SerializeField] private int totalGold;
        public int TotalGold
        {
            get { return totalGold;}
        }

        [SerializeField] private int orderFragments;
        public int OrderFragments
        {
            get { return orderFragments; }
            set
            {
                orderFragments = value;

                if (orderFragments == 4)
                {
                    maxOrder += orderUpgradeValue;
                    orderFragments = 0;
                }                
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

        [SerializeField] private int totalKills;
        public int TotalKills
        {             
            get { return totalKills;}
            set { totalKills = value; }
        }

        [SerializeField] private int totalHeals;
        public int TotalHeals
        {
            get { return totalHeals; }
        }

        public Knowledge GetKnowledgeByID(KnowledgeID id)
        {
            return EveryKnowledgeDictionary[id];
        }

        public void InitializeData()
        {
            lastSafeSpotMap = "";
            KnownKnowledgeDictionary = new Dictionary<KnowledgeID, bool>();
            AvailableKnowledgeDictionary = new Dictionary<KnowledgeID, AvailableKnowledgePosition>();
            EveryKnowledgeDictionary = new Dictionary<KnowledgeID, Knowledge>();
            CurrentPlayerMapProgression = new Dictionary<string, bool>();

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

            CurrentPlayerMapProgression.Clear();
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                Debug.Log($"scene.name={sceneName}");
                CurrentPlayerMapProgression.Add(sceneName, false);
            }

            maxOrder = initialMaxOrder;
            ActualOrder = maxOrder;
            ActualChaos = maxChaos;
            totalKills = 0;
            totalHeals = 0;
            totalGold = 0;
            Gold = 0;
            OrderFragments = 0;
            WeaponUpgrade = 0;
            ArmorUpgrade = 0;
        }

        public void TakeDamage(int dmg)
        {
            int actualDmg = Mathf.Max(dmg - ArmorUpgrade, 0);
            ActualOrder -= actualDmg;

            if (actualOrder <= 0)
            {
                OnDeadDelegate();
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
            ActualOrder = Mathf.Min(value + actualOrder, maxOrder);

            totalHeals++;
        }

        public bool UseChaos(int value)
        {
            if(ActualChaos >= value)
            {
                ActualChaos -= value;

                return true;
            }

            return false;
        }

        public void LearnKnowledge(KnowledgeID id)
        {
            KnownKnowledgeDictionary[id] = true;
            OnLearningKnowledge(id);
        }

        public void DebugDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            foreach (var keyValue in dictionary)
            {
                Debug.Log($"{keyValue.Key} -> {keyValue.Value}");
            }
        }
    }
}
