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
            animator.SetBool("IsJumping", true);
            player.Jump();
            player.inputTriggers["Jump"] = false;
            base.Enter();
        }

        public override void Update()
        {
            player.SetHighestAirborneY();

            if (player.IsEventGrounded && player.rb.velocity.y < -0.001f)
            {
                if (player.WillLand())
                {
                    player.ChangeState(player._landingState);
                }
                else
                {
                    player.ChangeState(player._idleState);
                }
                return;
            }
            
            if(player.CanGripToWall())
            {
                player.ChangeState(player._wallGrippingState);
                return;
            }

            if (player.WillClimbLadder())
            {
                player.ChangeState(player._ladderClimbingState);
                return;
            }

            if (player.WillDash())
            {
                player.ChangeState(player._dashingState);
                return;
            }

            if(player.rb.velocity.y < 0.0f)
            {
                player.ChangeState(player._airborneState);
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
            animator.SetBool("IsJumping", false);
            base.Exit();
        }
    }
}