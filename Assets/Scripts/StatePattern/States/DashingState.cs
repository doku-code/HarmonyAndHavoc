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
        //private bool becameAirborne;
        //private bool enteredGrounded;
        private int nFrames;

        public DashingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.DASH;
        }

        public override void Enter()
        {
            animator.SetTrigger("Dash");
            player.Dash();
            //Debug.Log($"Mathf.Abs(player.rb.velocity.x)={Mathf.Abs(player.rb.velocity.x)}");
            player.SetInputTriggersFromKnowledge(AF.KnowledgeID.DASH, false);
            startTime = Time.time;
            //enteredGrounded = player.IsCastGrounded(false);
            //becameAirborne = !enteredGrounded;

            nFrames = 0;

            //         lastKnowledge = player.Data.Knowledges.find_if()

            if (animationClipLength == 0.0f)
            {
                animationClipLength = animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
            }

            base.Enter();
        }

        public override void Update()
        {
            //Debug.Log($"Mathf.Abs(player.rb.velocity.x)={Mathf.Abs(player.rb.velocity.x)}");

            bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
            bool grounded = player.IsCastGrounded(true, player.GroundLayer | player.LadderLayer, Vector2.zero, player.GroundDistance * 2.0f, false) || (foundSlopeBeneath && Mathf.Abs(slope) > player.StairsUpMinSlope && Mathf.Abs(slope) < player.StairsUpMaxSlope);
            //becameAirborne |= !grounded;
            
            float elapsedTime = Time.time - startTime;        
            /*if ((becameAirborne || elapsedTime > animationClipLength * player.GroundDashBailOutNormalizedTime) && grounded )//&& player.rb.velocity.y < 0)// player.MoveInput.y < 0)
            {
                Debug.Log($"becameAirborne={becameAirborne}");

                //player.idleState.nFrames = 3;
                //player.ChangeState(player.idleState);
                //return;
            }*/

            if (player.WillGripToWall())// && player.inputTriggers["Move"])
            {
                player.ChangeState(player.wallGrippingState);
                return;
            }

            if (player.DashDirection.y != 0.0f && player.rb.velocity.y < -0.01f && !grounded)
            {
                player.ChangeState(player.airborneState);
                return;
            }
            
            if (player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }
            
            if (player.WillClimbUpStairs())
            {
                player.ChangeState(player.stairsClimbingUpState);
                return;
            }
            else
            {
                //Debug.Log($"nFrames={nFrames}");
                //Debug.Break();
            }

            //Debug.Log($"Mathf.Abs(player.rb.velocity.x)={Mathf.Abs(player.rb.velocity.x)}");
            if ((elapsedTime > animationClipLength || (Mathf.Abs(player.rb.velocity.x) < 0.0005f && grounded)) && nFrames > 1)
            {
                player.ChangeState(player.idleState);
                return;
            }
            // Let rigidbody have a little deceleration when dashing on the ground
            else if (nFrames > 1)
            {                
                player.rb.AddForce(Vector2.right * -player.rb.velocity.x * player.GroundDashDeceleration * Time.fixedDeltaTime, ForceMode2D.Force);
            }
            else
            {
                player.ContinueDash();
            }

            nFrames++;
        }

        public override void Exit()
        {
            animator.ResetTrigger("Dash");
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            base.Exit();
        }
    }
}