using JFM;
using System;
using UnityEngine;

namespace AF
{
    [Serializable]
    //[CreateAssetMenu(fileName = "Knowledge", menuName = "PlayerData")] // To put in my children
    public abstract class Knowledge : ScriptableObject
    {
        public int damageBoost;
        public KnowledgeID ID;

        protected PlayerController player;
        
        public void Initialize(PlayerController player)
        {
            this.player = player;
        }

        public abstract bool WillUseKnowledge();

        public abstract void Activate();
        public abstract void Deactivate();

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
