using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class IdleState : PlayerState
    {
        public IdleState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.IDLE;
        }

        public override void Enter()
        {
            //animator.SetTrigger("isIdle");
            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsGrounded())
            {
                player.ChangeState(player._airborneState);
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f)
            {
                player.ChangeState(player._walkingState);
                return;
            }

            if (player.CanClimbLadder())
            {
                //Debug.Log("YESSSSS");
                player.ChangeState(player._ladderClimbingState);
                return;
            }

            if (player.CanDash())
            {
                player.ChangeState(player._dashingState);
                return;
            }

            if (player.CanJump() && player.inputTriggers["Jump"])
            {
                player.ChangeState(player._jumpingState);
                return;
            }

            player.rb.velocity = Vector2.zero;
        }

        public override void Exit()
        {
            //animator.ResetTrigger("isIdle");
            base.Exit();
        }
    }
}