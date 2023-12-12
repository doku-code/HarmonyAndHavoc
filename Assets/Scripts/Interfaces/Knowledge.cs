using JFM;
using System;
using UnityEngine;

namespace AF
{
    //[Serializable]
    //[CreateAssetMenu(fileName = "Knowledge", menuName = "PlayerData")] // To put in my children
    public abstract class Knowledge : ScriptableObject
    {
        public int damageBoost;
        public KnowledgeID ID;
        public bool isUtility;
        public int chaosCost;
        public int slotCost;
        public float cooldown;
        protected float activationTime;

        protected PlayerController player;
        
        public void Initialize(PlayerController player)
        {            
            activationTime = Time.time - cooldown;
            this.player = player;
        }

        public bool CanUse()
        {
            return Time.time - activationTime >= cooldown && player.Data.ActualChaos >= chaosCost;
        }

        public void Use()
        {
            activationTime = Time.time;
            player.Data.UseChaos(chaosCost);
        }

        public abstract bool WillUse();

        public abstract void Activate();
        public abstract void Deactivate();

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        public abstract void OnLeave();
    }
}
