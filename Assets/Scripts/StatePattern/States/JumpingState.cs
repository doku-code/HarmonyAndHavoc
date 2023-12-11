using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "JumpingState", menuName = "States/Jumping")]
    public class JumpingState : PlayerState
    {
        private int nFrames;
        private bool stopForce;
        public override void Enter()
        {
            player.animator.SetTrigger("Jump");
            player.animator.SetBool("IsAirborne", true);
            player.Jump();
            player.inputTriggers["Jump"] = false;
            nFrames = 0;

            stopForce = false;

            base.Enter();
        }

        public override void Update()
        {
            player.SetHighestAirborneY();

            if (player.IsGrounded(player.GroundLayer) && player.rb.velocity.y < -0.001f)
            {
                if (player.WillLand())
                {
                    player.StateMachine.ChangeState(player.States[STATE.LAND]);
                }
                else
                {                    
                    player.StateMachine.ChangeState(player.States[STATE.IDLE]);
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
                LadderClimbingState state = (LadderClimbingState)player.States[STATE.LADDER];
                state.targetX = player.GetBeneathObjectPosition().x + 0.5f - player.ColliderOffset.x;
                player.StateMachine.ChangeState(state);
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.DASH).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.DASH);
                return;
            }

            if (player.rb.velocity.y < -0.001f)
            {
                AirborneState state = (AirborneState)player.States[STATE.AIRBORNE];
                state.wasGrounded = false;
                player.StateMachine.ChangeState(state);
                return;
            }

            if (player.CanTurn())
            {
                player.Turn();
            }

            // Add force but limit speed
            if (player.MoveInput.x != 0.0f && player.rb.velocity.magnitude < player.AirSpeed && !stopForce)
            {
                player.rb.AddForce((player.IsFacingRight ? Vector3.right : -Vector3.right) * /*player.AirSpeed * */player.AirAcceleration * Time.fixedDeltaTime);

                if (player.rb.velocity.magnitude > player.AirSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.AirSpeed;
                }

                if (player.rb.velocity.magnitude == 0.0f)
                {
                    stopForce = true;
                }
            }

            if (player.WillAttack())
            {
                player.StateMachine.ChangeState(player.States[STATE.BASIC_ATTACK]);
                return;
            }

            if (player.WillJump())
            {
                player.Jump();
                player.inputTriggers["Jump"] = false;
            }

            /*if (player.inputTriggers["Inventory"])
            {
                player.StateMachine.ChangeState(player.States[STATE.PAUSE]);
                return;
            }*/

            //Debug.Log($"totalForces={player.rb.totalForce} vel={player.rb.velocity}");

            nFrames++;
        }

        public override void Exit()
        {
            player.animator.ResetTrigger("Jump");
            player.animator.SetBool("IsAirborne", false);
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}