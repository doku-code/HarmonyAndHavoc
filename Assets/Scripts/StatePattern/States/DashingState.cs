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
     * IMPORTANT NOTICE: Use 'Continuous' Collision Detection setting in Rigidbody2D, else
     * it will have a chance of passing through some walls.
     * 
     * * * * * * * * * * * * * * */
    public class DashingState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private int animatorLayer = 0;

        public DashingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.DASH;
        }

        public override void Enter()
        {
            animator.SetTrigger("Dash");
            player.Dash();
            player.SetInputTriggersFromKnowledge(AF.KnowledgeID.DASH, false);
            startTime = Time.time;

            //         lastKnowledge = player.Data.Knowledges.find_if()

            if (animationClipLength == 0.0f)
            {
                animationClipLength = animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
            }

            base.Enter();
        }

        public override void Update()
        {
            float elapsedTime = Time.time - startTime;        
            if ((player.DashDirection.y == 0.0f && elapsedTime > animationClipLength * player.GroundDashBailOutNormalizedTime) && player.IsCastGrounded() )//&& player.rb.velocity.y < 0)// player.MoveInput.y < 0)
            {
                player.idleState.nFrames = 3;
                player.ChangeState(player.idleState);
                return;
            }

            if (player.WillGripToWall())// && player.inputTriggers["Move"])
            {
                player.ChangeState(player.wallGrippingState);
                return;
            }

            if (player.DashDirection.y != 0.0f && player.rb.velocity.y < -0.5f)
            {
                player.ChangeState(player.airborneState);
                return;
            }
            
            if (player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (elapsedTime > animationClipLength)
            {
                player.ChangeState(player.idleState);
                return;
            }
            // Let rigidbody have a little deceleration when dashing on the ground
            else if (player.DashDirection.y == 0.0f)
            {
                player.rb.AddForce(-player.rb.velocity * player.GroundDashDeceleration * Time.fixedDeltaTime, ForceMode2D.Force);
                return;
            }

            player.ContinueDash();
        }

        public override void Exit()
        {
            animator.ResetTrigger("Dash");
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            base.Exit();
        }
    }
}