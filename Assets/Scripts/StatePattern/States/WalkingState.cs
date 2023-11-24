using UnityEngine;

namespace Fineallday.StatePattern
{
    public class WalkingState : PlayerState
    {
        public WalkingState(PlayerController player) : base(player)
        {
        }

        public override void OnEnterState()
        {
          playerC._animator.SetBool("Walk",true);  
        }

        public override void OnUpdateState()
        {
            if (!playerC.isGrounded)
            {
                playerC.ChangeState(playerC._fallingState);
                return;
            }
            
            if (playerC.InputTriggers["Jump"])
            {
                playerC.ChangeState(playerC._jumpingState);
                return;
            }
            
            if (!playerC.InputTriggers["Walk"])
            {
                playerC.ChangeState(playerC._idleState);
                return;
            }

            if (playerC.InputTriggers["Run"])
            {
                playerC.ChangeState(playerC._runningState);
            }
        
        }

        public override void OnExitState()
        {
            playerC._animator.SetBool("Walk",false);  
        }
    }
}
