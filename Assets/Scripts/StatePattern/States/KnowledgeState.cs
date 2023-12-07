using AF;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "KnowledgeState", menuName = "States/Knowledge")]
    public class KnowledgeState : PlayerState
    {
        [NonSerialized] public Knowledge knowledge;
        
        public override void Enter()
        {
            knowledge.Enter();
            base.Enter();
        }

        public override void Update()
        {
            knowledge.Update();
        }

        public override void Exit()
        {
            knowledge.Exit();
            base.Exit();

            if (!knowledge.isUtility)
            {
                player.ResetLastAttackKnowledgeUsed();
            }
        }

        public override void OnLeaveState() 
        {
            knowledge.OnLeave();
        }
    }
}