using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class JumpingState : PlayerState
    {
        public JumpingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.JUMP;
        }

        public override void Enter()
        {
            animator.SetTrigger("Jump");
            player.Jump();
            player.inputTriggers["Jump"] = false;
            base.Enter();
        }

        public override void Update()
        {
            player.SetHighestAirborneY();

            if (player.IsGrounded())
            {
                if (player.WillLand())
                {
                    player.ChangeState(player.landingState);
                }
                else
                {                    
                    player.ChangeState(player.idleState);
                }
                return;
            }
            
            if(player.WillGripToWall())
            {
                player.ChangeState(player.wallGrippingState);
                return;
            }

            if (player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (player.WillDash())
            {
                player.ChangeState(player.dashingState);
                return;
            }

            if(player.rb.velocity.y < 0.0f)
            {
                player.ChangeState(player.airborneState);
                return;
            }

            if (player.CanTurn())
            {
                player.Turn();
            }

            if (player.MoveInput.x != 0.0f)
            {
                player.rb.AddForce((player.IsFacingRight ? Vector3.right : -Vector3.right) * player.WalkSpeed * player.AirSpeedMultiplier * Time.fixedDeltaTime);
            }

            if (player.WillJump())
            {                
                player.Jump();
            }
        }

        public override void Exit()
        {
            animator.ResetTrigger("Jump");
            base.Exit();
        }
    }
}