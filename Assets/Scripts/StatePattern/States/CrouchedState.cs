using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class CrouchedState : PlayerState
    {
        public CrouchedState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.CROUCH;
        }

        public override void Enter()
        {
            animator.SetBool("IsCrouched", true);
            animator.SetBool("IsIdle", true);

            player.rb.velocity = Vector2.zero;

            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsCastGrounded())
            {
                player.ChangeState(player.airborneState);
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
                    player.ChangeState(player.walkingState);
                }
                else
                {
                    player.idleState.waitNFrames = 1;
                    player.ChangeState(player.idleState);
                }
                return;
            }            

            if (player.inputTriggers["BasicAttack"] && player.MoveInput.y < 0.0f)
            {
                player.ChangeState(player.crouchedAttackState);
                return;
            }

            if (player.WillClimbDownLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }
        }

        public override void Exit()
        {            
            animator.SetBool("IsCrouched", false);
            animator.SetBool("IsIdle", false);
            base.Exit();
        }
    }
}