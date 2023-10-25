using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    /* * * * * * * * * * * * * * * * * * * * 
     * 
     * You may have to set the default sprite displayed by the SpriteRenderer to something like an idle frame, 
     * because when having a multiple-layered animator, it can sometimes display this default sprite between two animations.
     *       
     * * * * * * * * * * * * * * * * * * * */

    public class IdleState : PlayerState
    {
        private float oldGravityScale;
        public int nFrames;
       
        public IdleState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.IDLE;
        }

        public override void Enter()
        {           
            animator.SetBool("IsIdle", true);
            player.rb.velocity = Vector2.zero;
            oldGravityScale = player.rb.gravityScale;
            player.rb.gravityScale = 0.0f;
            base.Enter();
        }

        public override void Update()
        {
            if(nFrames > 0)
            {
                nFrames--;
                return;
            }
      
            if (!player.IsGrounded(false))
            {
                player.ChangeState(player.airborneState);
                return;
            }
            
            if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f && player.MoveInput.y == 0.0f)
            {
                player.ChangeState(player.walkingState);
                return;
            }

            if (player.WillClimbDownLadder())
            {
                float ladderX = Mathf.Floor(player.HitInfo.probePoint.x) + 0.5f - player.ColliderOffset.x;
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.y < 0.0f)
            {
                player.ChangeState(player.crouchedState);
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

            if (player.WillJump())
            {
                player.ChangeState(player.jumpingState);
                return;
            }

            if (player.inputTriggers["BasicAttack"])
            {
                player.ChangeState(player.basicAttackState);
                return;
            }

            player.rb.velocity = Vector2.zero;
        }

        public override void Exit()
        {
            player.rb.gravityScale = oldGravityScale;
            animator.SetBool("IsIdle", false);
            base.Exit();
        }
    }
}