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
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
