namespace Fineallday.StatePattern
{
    public class RunningState : PlayerState
    {
        public RunningState(PlayerController player) : base(player)
        {
        }

        public override void OnEnterState()
        {
           playerC._animator.SetBool("Run",true); 
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

            if (!playerC.InputTriggers["Run"] || !playerC.InputTriggers["Walk"])
            {
                playerC.ChangeState(playerC._walkingState);
            }
            
        }

        public override void OnExitState()
        {
            playerC._animator.SetBool("Run",false); 
        }
    }
}
