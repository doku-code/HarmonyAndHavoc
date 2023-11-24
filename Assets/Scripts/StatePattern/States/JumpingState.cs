
using UnityEngine;

namespace Fineallday.StatePattern
{
  
    public class JumpingState : PlayerState
    {
        private float jumpForce = 5;
        public JumpingState(PlayerController player) : base(player)
        {
        }

        public override void OnEnterState()
        {
            playerC._animator.SetBool("Jump",true);
            playerC.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        public override void OnUpdateState()
        {
            if (!playerC.isGrounded)
            {
                playerC.ChangeState(playerC._fallingState);
                return;
            }
        }

        public override void OnExitState()
        {
            playerC._animator.SetBool("Jump",false);
        }
    }
}
