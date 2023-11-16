using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "KnowledgeState", menuName = "States/Knowledge")]
    public class KnowledgeState : PlayerState
    {
        public Knowledge knowledge;
        
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
        }
    }
}