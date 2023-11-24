using UnityEngine;

namespace Fineallday.StatePattern
{
    public class LandingState : PlayerState
    {
        public LandingState(PlayerController player) : base(player)
        {
        }

        public override void OnEnterState()
        {
         //   playerC._animator.SetBool("Fall", true);
        }

        public override void OnUpdateState()
        {

            if (playerC.isGrounded)
            {
                playerC.ChangeState(playerC._idleState);
            }
        }

        public override void OnExitState()
        {
            //playerC._animator.SetBool("Fall", false);
        }
    }
}