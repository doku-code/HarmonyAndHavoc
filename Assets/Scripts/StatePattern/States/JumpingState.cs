using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class JumpingState : PlayerState
    {
        private int nFrames;

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
            nFrames = 0;

            base.Enter();
        }

        public override void Update()
        {
            player.SetHighestAirborneY();

            //if (player.IsGrounded())
            if (player.IsCastGrounded(false) && player.rb.velocity.y < -0.001f)
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
            
            if(player.WillGripToWall() && nFrames > 0)
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

            // Add force but limit speed
            if (player.MoveInput.x != 0.0f && player.rb.velocity.magnitude < player.WalkSpeed)
            {
                player.rb.AddForce((player.IsFacingRight ? Vector3.right : -Vector3.right) * player.WalkSpeed * player.AirAcceleration * Time.fixedDeltaTime);

                if (player.rb.velocity.magnitude > player.WalkSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.WalkSpeed;
                }
            }

            if (player.WillJump())
            {
                player.Jump();
            }  
            
            nFrames++;
        }

        public override void Exit()
        {
            animator.ResetTrigger("Jump");
            base.Exit();
        }
    }
}