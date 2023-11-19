using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "JumpingState", menuName = "States/Jumping")]
    public class JumpingState : PlayerState
    {
        private int nFrames;       

        public override void Enter()
        {
            player.animator.SetTrigger("Jump");
            player.Jump();
            player.inputTriggers["Jump"] = false;
            nFrames = 0;

            base.Enter();
        }

        public override void Update()
        {
            player.SetHighestAirborneY();

            if (player.IsGrounded(player.GroundLayer) && player.rb.velocity.y < -0.001f)
            {
                if (player.WillLand())
                {
                    player.ChangeState(player.states[STATE.LAND]);
                }
                else
                {                    
                    player.ChangeState(player.states[STATE.IDLE]);
                }
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.WALL_SLIDE).WillUse()) // && nFrames > 0)
            {
                player.UseKnowledge(AF.KnowledgeID.WALL_SLIDE);                
                return;
            }

            if (player.WillClimbLadder())
            {
                LadderClimbingState state = (LadderClimbingState)player.states[STATE.LADDER];
                state.targetX = player.GetBeneathObjectPosition().x + 0.5f - player.ColliderOffset.x;
                player.ChangeState(state);
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.DASH).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.DASH);
                return;
            }

            if (player.rb.velocity.y < 0.0f)
            {
                AirborneState state = (AirborneState)player.states[STATE.AIRBORNE];
                state.wasGrounded = false;
                player.ChangeState(state);
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
            player.animator.ResetTrigger("Jump");
            base.Exit();
        }
    }
}