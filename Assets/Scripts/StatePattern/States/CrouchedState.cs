using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "CrouchedState", menuName = "States/Crouched")]
    public class CrouchedState : PlayerState
    {
        private bool resetAnimatorParams;

        public override void Enter()
        {
            player.animator.SetBool("IsCrouched", true);
            //player.animator.SetBool("IsIdle", true);

            // Stop the player
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);

            resetAnimatorParams = true;

            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsGrounded())
            {
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }

            if (player.CanTurn())
            {
                player.Turn();
            }            

            if (player.MoveInput.y >= 0.0f)
            {
                if (player.MoveInput.x != 0.0f)
                {
                    player.StateMachine.ChangeState(player.States[STATE.WALK]);
                }
                else
                {
                    IdleState state = (IdleState)player.States[PlayerState.STATE.IDLE];
                    state.waitNFrames = 1;
                    player.StateMachine.ChangeState(state);
                }
                return;
            }            

            if (player.WillAttack() && player.MoveInput.y < 0.0f)
            {
                resetAnimatorParams = false;
                player.StateMachine.ChangeState(player.States[STATE.CROUCH_ATTACK]);
                return;
            }

            if (player.WillClimbDownLadder(out Vector2 ladderPoint))
            {
                float ladderX = Mathf.Floor(ladderPoint.x) + 0.5f - player.ColliderOffset.x;
                //Debug.Log($"Climbing ladder... ladderX={ladderX}");
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);

                LadderClimbingState state = (LadderClimbingState)player.States[STATE.LADDER];
                state.targetX = ladderX;
                player.StateMachine.ChangeState(state);
                player.StateMachine.ChangeState(player.States[STATE.LADDER]);
                return;
            }

            // Stop the player
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            player.rb.totalForce = Vector2.zero;
        }

        public override void Exit()
        {
            //player.animator.SetBool("IsIdle", false);
            if (resetAnimatorParams)
            {
                player.animator.SetBool("IsCrouched", false);                
            }
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}