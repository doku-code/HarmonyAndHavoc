using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        public float oldGravityScale;
        public int nFrames;
        private bool resetGravityScale;
        public bool resetGravityScaleWithOther;
        public float otherGravityScale;
        public bool overrideOldGravityScale;
        
        public IdleState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.IDLE;
        }

        public override void Enter()
        {           
            animator.SetBool("IsIdle", true);
            player.rb.velocity = Vector2.zero;
            if (!overrideOldGravityScale)
            {
                oldGravityScale = player.rb.gravityScale;
            }
            else
            {
                overrideOldGravityScale = false;
            }
            player.rb.gravityScale = 0.0f;

            player.rb.isKinematic = false;

            resetGravityScale = true;
            base.Enter();
        }

        public override void Update()
        {
            if (nFrames > 0)
            {
                nFrames--;
                return;
            }

            Vector2 v;
            //Debug.Log($"Before: {player.stairsSide}");
            /*if (player.stairsSide != 0)
             {
                 v = player.stairsSide == -1 ? -Vector2.right : Vector2.right;
             }
             else
             {
                 v = Vector2.zero;
             }*/
            //bool foundStairsBeneath = player.FindSlopeBeneath(out float slope);
            //player.stairsSide = foundStairsBeneath;

            v = player.IsFacingRight ? -Vector2.right : Vector2.right;

            if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f && player.MoveInput.y == 0.0f)
            {
                if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
                    )
                {
                    resetGravityScale = false;
                    player.walkingState.newGravityScale = 0.0f;
                    player.walkingState.resetGravityScaleWithOther = true;
                    player.walkingState.otherGravityScale = oldGravityScale;
                    player.rb.gravityScale = 0.0f;
                    Debug.Log("walking on ladder");
                }
                else
                {
                    Debug.Log("not walking on ladder");
                }
                player.ChangeState(player.walkingState);
                return;
            }

            bool foundStairsBeneath = player.FindSlopeBeneath(out float slope, Vector2.up * 0.02f + v * 0.0f /*player.ColliderSize.x / 2.0f*/ + v * 0.2f, -v * 0.3f + Vector2.up * player.StairsUpHeight, player.StairsDownHeight);//, true);

            if (!player.IsCastGrounded(false) && !foundStairsBeneath)
            {
                player.ChangeState(player.airborneState);
                return;
            }

            if(player.CanTurn())
            {
                player.Turn();
            }

            if (player.WillClimbUpStairs())
            {
                player.ChangeState(player.stairsClimbingUpState);
                return;
            }

            if (player.WillClimbDownStairs() || (foundStairsBeneath && Mathf.Abs(slope) > player.StairsUpMinSlope) && Mathf.Abs(player.rb.velocity.y) <= 0.01f && ((player.MoveInput.x > 0.0f && slope < 0) || (player.MoveInput.x > 0.0f && slope < 0)))
            {
                player.ChangeState(player.stairsClimbingDownState);
                return;
            }            

            if (player.WillClimbDownLadder())
            {
                float ladderX = Mathf.Floor(player.HitInfo.probePoint.x) + 0.5f - player.ColliderOffset.x;
                Debug.Log($"Climbing ladder... ladderX={ladderX}");
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);
                //Debug.Break();
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
            if(resetGravityScale)
            {
                if (resetGravityScaleWithOther)
                {
                    player.rb.gravityScale = otherGravityScale;
                    resetGravityScaleWithOther = false;
                }
                else
                {
                    player.rb.gravityScale = oldGravityScale;
                }
            }
            animator.SetBool("IsIdle", false);
            base.Exit();
        }
    }
}