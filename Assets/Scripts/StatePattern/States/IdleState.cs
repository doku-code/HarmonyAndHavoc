using System;
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
    [CreateAssetMenu(fileName = "IdleState", menuName = "States/Idle")]
    public class IdleState : PlayerState
    {
        [NonSerialized] public float oldGravityScale;
        [NonSerialized] public int waitNFrames;
        private bool resetGravityScale;
        [NonSerialized] public bool resetGravityScaleWithOther;
        [NonSerialized] public float otherGravityScale;
        [NonSerialized] public bool overrideOldGravityScale;
        private bool startingOnLadder;       

        public override void Enter()
        {           
            player.animator.SetBool("IsIdle", true);
            player.animator.SetBool("IsCrouched", false);

            //player.rb.velocity = Vector2.zero;
            if (player.rb.velocity.magnitude > 0.0f)
            {                
                player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            }

            if (!overrideOldGravityScale)
            {
                oldGravityScale = player.rb.gravityScale;
            }
            else
            {
                overrideOldGravityScale = false;
            }
            if (player.WillClimbLadder() || player.IsAboveLadder())
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
            bool foundSlopeInFront = (Raycast2DHelper.FindSlopeAtPoint(player.rb.position, out float slopeFront, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, (player.IsFacingRight ? 1.0f : -1.0f) * 45.0f), player.StairsDownHeight, player.GroundLayer));//, true));

            bool foundSlopeBehind = (Raycast2DHelper.FindSlopeAtPoint(player.rb.position, out float slopeBack, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, (player.IsFacingRight ? -1.0f : 1.0f) * 45.0f), player.StairsDownHeight, player.GroundLayer));//, true));

            foundSlopeInFront &= (Mathf.Abs(slopeFront) >= player.StairsUpMinSlope || Mathf.Abs(slopeFront) >= player.StairsDownMinSlope) &&
                (Mathf.Abs(slopeFront) <= player.StairsUpMaxSlope || Mathf.Abs(slopeFront) <= player.StairsDownMaxSlope);
            foundSlopeBehind &= (Mathf.Abs(slopeBack) >= player.StairsUpMinSlope || Mathf.Abs(slopeBack) >= player.StairsDownMinSlope) &&
                (Mathf.Abs(slopeBack) <= player.StairsUpMaxSlope || Mathf.Abs(slopeBack) <= player.StairsDownMaxSlope);

            if (!player.IsGrounded())
            {
                if (!foundSlopeInFront && !foundSlopeBehind)
                {
                    player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                    return;
                }
                Debug.Log($"!player.IsGrounded() {foundSlopeInFront} && {foundSlopeBehind} {slopeBack}");
                player.rb.velocity = Vector2.zero;
                player.rb.totalForce = Vector2.zero;
                player.rb.gravityScale = 0.0f;
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
                player.StateMachine.ChangeState(player.States[STATE.STAIRS_UP]);
                return;
            }

            if (player.WillClimbDownStairs())// || (foundSlopeInFront && Mathf.Abs(slopeFront) > player.StairsUpMinSlope) && Mathf.Abs(player.rb.velocity.y) <= 0.01f && ((player.MoveInput.x > 0.0f && slopeFront < 0) || (player.MoveInput.x > 0.0f && slopeFront < 0)))
            {
                player.StateMachine.ChangeState(player.States[STATE.STAIRS_DOWN]);
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f && player.MoveInput.y >= 0.0f)
            {
                // Could I reuse these raycasts for WillClimbDownLadder() below ?
                if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
                    )
                {
                    resetGravityScale = false;
                    WalkingState state = (WalkingState)player.States[STATE.WALK];
                    state.newGravityScale = 0.0f;
                    state.resetGravityScaleWithOther = true;
                    state.otherGravityScale = oldGravityScale;
                    player.rb.gravityScale = 0.0f;
                    player.rb.velocity = Vector2.zero;
                    player.rb.totalForce = Vector2.zero;
                    //Debug.Log("walking on ladder");
                }
                else
                {
                    //Debug.Log("not walking on ladder");
                }
                player.StateMachine.ChangeState(player.States[STATE.WALK]);
                return;
            }
            
            if (player.WillClimbDownLadder(out Vector2 ladderPoint))
            {
                float ladderX = Mathf.Floor(ladderPoint.x) + 0.5f - player.ColliderOffset.x;
                //Debug.Log($"Climbing ladder... ladderX={ladderX}");
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);
                //Debug.Break();
                LadderClimbingState state = (LadderClimbingState)player.States[STATE.LADDER];
                state.targetX = ladderX;
                player.StateMachine.ChangeState(state);
                return;
            }
                        
            if (player.WillClimbLadder())
            {
                LadderClimbingState state = (LadderClimbingState)player.States[STATE.LADDER];
                state.targetX = player.GetBeneathObjectPosition().x + 0.5f - player.ColliderOffset.x;
                player.StateMachine.ChangeState(state);
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.y < 0.0f)
            {
                player.StateMachine.ChangeState(player.States[STATE.CROUCH]);
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.DASH).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.DASH);                
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.GROUND_SLIDE).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.GROUND_SLIDE);
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.COMBO_ATTACK).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.COMBO_ATTACK);
                return;
            }
            
            if (player.WillJump())
            {
                player.StateMachine.ChangeState(player.States[STATE.JUMP]);
                return;
            }
            /*
            if (player.inputTriggers["Inventory"])
            {
                player.StateMachine.ChangeState(player.States[STATE.PAUSE]);
                return;
            }*/

            if (player.WillAttack())
            {
                player.StateMachine.ChangeState(player.States[STATE.BASIC_ATTACK]);
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
            player.animator.SetBool("IsIdle", false);
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}