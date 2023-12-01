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

            player.rb.velocity = Vector2.zero;

            resetAnimatorParams = true;

            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsGrounded())
            {
                player.ChangeState(player.states[STATE.AIRBORNE]);
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
                    player.ChangeState(player.states[STATE.WALK]);
                }
                else
                {
                    IdleState state = (IdleState)player.states[PlayerState.STATE.IDLE];
                    state.waitNFrames = 1;
                    player.ChangeState(state);
                }
                return;
            }            

            if (player.WillAttack() && player.MoveInput.y < 0.0f)
            {
                resetAnimatorParams = false;
                player.ChangeState(player.states[STATE.CROUCH_ATTACK]);
                return;
            }

            if (player.WillClimbDownLadder(out Vector2 ladderPoint))
            {
                float ladderX = Mathf.Floor(ladderPoint.x) + 0.5f - player.ColliderOffset.x;
                Debug.Log($"Climbing ladder... ladderX={ladderX}");
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);

                LadderClimbingState state = (LadderClimbingState)player.states[STATE.LADDER];
                state.targetX = ladderX;
                player.ChangeState(state);
                player.ChangeState(player.states[STATE.LADDER]);
                return;
            }
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