
using UnityEngine;

namespace Fineallday.StatePattern
{
    public class FallingState : PlayerState
    {
        public FallingState(PlayerController player) : base(player)
        {
        }

        public override void OnEnterState()
        {
            playerC._animator.SetBool("Fall", true); 
            //Debug.Break();
        }

        public override void OnUpdateState()
        {
            Debug.Log("Updating Falling State");
            
            bool isCloseToGround = Physics.Raycast(playerC.transform.position + new Vector3(0,0.2f,0), Vector3.down, out RaycastHit hit, 1.5f);

            if (isCloseToGround) // && vélocité vers la bas
            {
                playerC.ChangeState(playerC._landingState);
            }
        }
        public override void OnExitState()
        {
            playerC._animator.SetBool("Fall", false);
        }
    }
}
