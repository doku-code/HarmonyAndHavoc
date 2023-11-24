using UnityEngine;

namespace Fineallday.StatePattern
{
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController player) : base(player)
        {
        }

        public override void OnEnterState()
        {
            
        }

        public override void OnUpdateState()
        {
            Debug.Log("Updating Idle State");  
           
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

            if (playerC.InputTriggers["Walk"])
            {
                playerC.ChangeState(playerC._walkingState);
            }
         
        }

        public override void OnExitState()
        {
           
        }
    }
}
