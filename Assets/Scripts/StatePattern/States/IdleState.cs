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
        public int waitNFrames;
        private bool resetGravityScale;
        public bool resetGravityScaleWithOther;
        public float otherGravityScale;
        public bool overrideOldGravityScale;
        private bool startingOnLadder;

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
            if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
            )
            {
                //Debug.Log("Ladders on idle.Enter().");
                player.rb.velocity = Vector2.zero;
                player.rb.totalForce = Vector2.zero;
                player.rb.gravityScale = 0.0f;
                startingOnLadder = true;
            }
            else
            {
                startingOnLadder = false;
            }
            
            player.rb.isKinematic = false;

            resetGravityScale = true;
            base.Enter();
        }

        public override void Update()
        {
            if (waitNFrames > 0)
            {
                waitNFrames--;
                return;
            }
            //Debug.Log($"gravityScale = {player.rb.gravityScale}");

            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;
                        
            // Could reuse this raycast for player.WillClimbUpStairs() below
            bool foundSlopeInFront = (player.FindSlopeAtPoint(out float slopeFront, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, (player.IsFacingRight ? 1.0f : -1.0f) * 45.0f), player.StairsDownHeight));//, true))

            bool foundSlopeBehind = (player.FindSlopeAtPoint(out float slopeBack, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, (player.IsFacingRight ? -1.0f : 1.0f) * 45.0f), player.StairsDownHeight));//, true))

            if (!player.IsCastGrounded(false))
            {
                if (!foundSlopeInFront && !foundSlopeBehind)
                {
                    player.ChangeState(player.airborneState);
                    return;
                }
                //Debug.Log("!player.IsCastGrounded()");
            }
            else if((foundSlopeInFront && Mathf.Abs(slopeFront) > 0.001f) || (foundSlopeBehind && Mathf.Abs(slopeBack) > 0.001f)) 
            {
                //Debug.Log("OK!!!!!");
                //player.rb.AddForce(Vector2.up * -Physics2D.gravity.y * player.rb.gravityScale * Time.fixedDeltaTime, ForceMode2D.Force);
                player.rb.velocity = Vector2.zero;
                player.rb.totalForce = Vector2.zero;
                player.rb.gravityScale = 0.0f;
            }
            else if(!startingOnLadder)
            {
                //Debug.Log("!startingOnLadder");
                player.rb.gravityScale = player.DefaultGravityScale;
            }

            if (player.CanTurn())
            {
                player.Turn();
            }

            if (player.WillClimbUpStairs())
            {                
                player.ChangeState(player.stairsClimbingUpState);
                return;
            }

            if (player.WillClimbDownStairs())// || (foundSlopeInFront && Mathf.Abs(slopeFront) > player.StairsUpMinSlope) && Mathf.Abs(player.rb.velocity.y) <= 0.01f && ((player.MoveInput.x > 0.0f && slopeFront < 0) || (player.MoveInput.x > 0.0f && slopeFront < 0)))
            {
                player.ChangeState(player.stairsClimbingDownState);
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f && player.MoveInput.y == 0.0f)
            {
                // Could I reuse these raycasts for WillClimbDownLadder() below ?
                if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
                    )
                {
                    resetGravityScale = false;
                    player.walkingState.newGravityScale = 0.0f;
                    player.walkingState.resetGravityScaleWithOther = true;
                    player.walkingState.otherGravityScale = oldGravityScale;
                    player.rb.gravityScale = 0.0f;
                    player.rb.velocity = Vector2.zero;
                    player.rb.totalForce = Vector2.zero;
                    //Debug.Log("walking on ladder");
                }
                else
                {
                    //Debug.Log("not walking on ladder");
                }
                player.ChangeState(player.walkingState);
                return;
            }
            
            if (player.WillClimbDownLadder())
            {
                float ladderX = Mathf.Floor(player.HitInfo.probePoint.x) + 0.5f - player.ColliderOffset.x;
                //Debug.Log($"Climbing ladder... ladderX={ladderX}");
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

            //player.rb.velocity = Vector2.zero;
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