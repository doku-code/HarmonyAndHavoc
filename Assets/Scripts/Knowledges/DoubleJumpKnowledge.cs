using AF;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "DoubleJumpKnowledge", menuName = "Knowledges/Double Jump")]
    public class DoubleJumpKnowledge : Knowledge
    {
        public override void Activate()
        {
            player.BaseNumJumps = 2;
        }

        public override void Deactivate()
        {
            player.BaseNumJumps = 1;
        }

        public override void Enter()
        {
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        public override bool WillUse()
        {
            return true;
        }

        public override void OnLeave()
        {
        }
    }
}