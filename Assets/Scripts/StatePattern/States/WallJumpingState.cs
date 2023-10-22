using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    /* * * * * * * * * * * * * * * 
     * 
     * This state is meant to prevent the player to go in opposite direction 
     * as his input will be in direction of the wall that initiated the walljump.
     * 
     * * * * * * * * * * * * * * */
    public class WallJumpingState : PlayerState
    {
        private float startTime;
        
        public WallJumpingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.WALLJUMP;
        }

        public override void Enter()
        {
            animator.SetTrigger("Dash");
            player.Jump();
            player.inputTriggers["Jump"] = false;
            startTime = Time.time;

            base.Enter();
        }

        public override void Update()
        {

            if (player.IsCastGrounded())
            {
                player.ChangeState(player._idleState);
                return;
            }

            if (player.CanGripToWall() && player.inputTriggers["Move"])
            {
                player.ChangeState(player._wallGrippingState);
                return;
            }                  

            if (player.WillJump())
            {
                player.ChangeState(player._jumpingState);
                return;
            }

            if(Time.time - startTime > player.WallJumpDuration)
            {
                player.ChangeState(player._airborneState);
                return;
            }
        }

        public override void Exit()
        {
            animator.ResetTrigger("Dash");
            base.Exit();
        }
    }
}