//#define _DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "AirborneState", menuName = "States/Airborne")]
    public class AirborneState : PlayerState
    {
        //private bool wasOnLadder;
        private int ladderX;
        [SerializeField] private float coyoteTime = 0.2f;
        private float coyoteTimeCounter;
        [NonSerialized] public bool wasGrounded = true;
        private bool hasDepletedJumps;

        public override void Enter()
        {            
            player.animator.SetBool("IsFalling", true);

            if (player.CanClimbLadder())
            {
                //Debug.Log("Ladders on airborne.Enter().");
                //wasOnLadder = true;
                ladderX = Mathf.FloorToInt(player.GetBeneathObjectPosition().x);
            }
            else
            {
                //wasOnLadder = false;
            }

            if (wasGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                //player.rb.gravityScale = 0.0f;
            }
            else
            {
                coyoteTimeCounter = 0.0f;
            }

            hasDepletedJumps = false;

            base.Enter();
        }

        public override void Update()
        {

            player.SetHighestAirborneY();

            if (player.CanTurn())
            {
                player.Turn();
            }
            
            bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
            bool stairsAreRightSide = slope > 0;
            //player.stairsSide = foundSlopeBeneath;
            //Debug.Log($"After: {player.stairsSide}");
            bool grounded = player.IsGrounded();            

            if (grounded && player.WillLand() && (!foundSlopeBeneath || Mathf.Abs(slope) < player.StairsUpMaxSlope))
            {
                //Debug.Log($"foundSlopeBeneath={foundSlopeBeneath} grounded={grounded}");
                player.ChangeState(player.states[STATE.LAND]);
                return;
            }
#if _DEBUG
            Debug.Log($"grounded={grounded} 1 << player.groundedLayer={1 << player.groundedLayer} == {(int)player.GroundLayer} wasOnLadder ={wasOnLadder} ladder={ladderX}");
#endif
            bool noLadderAbove = false;
            if (grounded || foundSlopeBeneath)
            {                               
                if (player.MoveInput.x != 0.0f)
                {
                    if (foundSlopeBeneath && Mathf.Abs(slope) > player.StairsUpMinSlope)
                    {
                        if (Mathf.Abs(slope) < player.StairsUpMaxSlope)
                        {
                            if (stairsAreRightSide == player.IsFacingRight)
                            {
                                //player.ChangeState(player.states[STATE.STAIRS_UP]);
                                player.ChangeState(player.states[STATE.WALK]);
                            }
                            else
                            {
                                //player.ChangeState(player.states[STATE.STAIRS_DOWN]);
                                player.ChangeState(player.states[STATE.WALK]);
                            }
                        }
                        else
                        {
                            player.UseKnowledge(AF.KnowledgeID.WALL_SLIDE);
                        }

                        return;
                    }
                    else if (grounded && (Mathf.Abs(slope) > player.StairsUpMinSlope && foundSlopeBeneath))
                    {
                        //Debug.Log($"{1 << player.groundedLayer} == {(int)player.LadderLayer}");
                        if (1 << player.groundedLayer == (int)player.LadderLayer)
                        {
                            if (player.Raycast(false, player.LadderLayer, Vector2.zero, player.GroundDistance + 1.0f, Vector2.down))
                            {
                                WalkingState state = (WalkingState)player.states[STATE.WALK];
                                state.resetGravityScaleWithOther = true;
                                state.otherGravityScale = player.rb.gravityScale;
                                state.newGravityScale = 0.0f;
                                player.rb.gravityScale = 0.0f;
                                player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                                player.rb.totalForce = Vector2.zero;

                                float y = Mathf.Floor(player.HitInfo.hit.point.y) + 1 + 0.007519f;// - player.ColliderSize.y + player.ColliderOffset.y;
                                player.rb.isKinematic = true;
                                player.rb.MovePosition(new Vector2(player.transform.position.x, y));
                                //Debug.Log($"Bon! y={y}");
                            }
                        }
#if _DEBUG
                        Debug.Log($"MoveInput.x != 0.0f!!! grounded={grounded} slope={slope} foundSlopeBeneath={foundSlopeBeneath} player.groundedLayer={player.groundedLayer}");
                        //Debug.Break();
#endif
                        player.ChangeState(player.states[STATE.WALK]);
                        return;
                    }
                    // If grounded to Ground layer, or to Ladder layer IF we didn't start this Airborne state in front 
                    // of a ladder or we started in front of a ladder of another X coordinate.
                    else if (grounded
                            &&
                            (
                                (1 << player.groundedLayer == (int)player.GroundLayer)
                                ||
                                (1 << player.groundedLayer == (int)player.LadderLayer)                                                                    
                            )
                            )
                    {
#if _DEBUG
                        Debug.Log($"Didn't make a case (1)... grounded={grounded} noLadderAbove={noLadderAbove} slope={slope} foundSlopeBeneath={foundSlopeBeneath} player.groundedLayer={player.groundedLayer}");
#endif
                        player.ChangeState(player.states[STATE.WALK]);
                        return;
                    }
                }
                else if (grounded && (Mathf.Abs(slope) > player.StairsUpMinSlope && foundSlopeBeneath))
                {
                    IdleState state = (IdleState)player.states[PlayerState.STATE.IDLE];
                    // Adjust for walking on ladders
                    if (player.WillClimbLadder())
                    {
                        noLadderAbove = Physics2D.CircleCast(player.rb.position + Vector2.up * 0.6f, player.GroundedRadius, Vector2.up, 0.8f, player.LadderLayer).collider is null;
                        //Debug.Log("Adjusting for ladders");
                        if (noLadderAbove)
                        {
                            player.rb.velocity = Vector2.zero;
                            player.rb.totalForce = Vector2.zero;
                            player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                            float y = Mathf.Floor(player.HitInfo.hit.point.y) + 1 + 0.007519f;// + player.rb.gravityScale * -Physics2D.gravity.y * player.LadderPushUpForce * Time.fixedDeltaTime;
                                                                                              //player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                            player.rb.isKinematic = true;
                            player.rb.MovePosition(new Vector2(player.transform.position.x, y));
                            //player.rb.isKinematic = false;

                            state.otherGravityScale = player.rb.gravityScale;
                            state.resetGravityScaleWithOther = true;
                            state.overrideOldGravityScale = true;
                            state.oldGravityScale = player.rb.gravityScale;
                            player.rb.gravityScale = 0.0f;

                            //Debug.Log($"Adjust for walking on ladders! y={y}");
                        }
                    }
#if _DEBUG
                    Debug.Log($"MoveInput.x = 0 grounded={grounded} || ({Mathf.Abs(slope) > player.StairsUpMinSlope} && {foundSlopeBeneath})");
#endif
                    if ((1 << player.groundedLayer == (int)player.LadderLayer) && noLadderAbove || (1 << player.groundedLayer == (int)player.GroundLayer))
                    {
                        // To rectify
                        // The Player actually "waits" in idle after having fallen
                        state.waitNFrames = 3;

                        player.ChangeState(state);
                        return;
                    }
                }
                // If grounded to Ground layer, or to Ladder layer IF we didn't start this Airborne state in front 
                // of a ladder or we started in front of a ladder of another X coordinate.
                else if (grounded 
                        && 
                        (
                            (1 << player.groundedLayer == (int)player.GroundLayer) 
                            || 
                            (1 << player.groundedLayer == (int)player.LadderLayer)                                                             
                        )
                        ) 
                {
#if _DEBUG
                    Debug.Log($"Didn't make a case (2)... grounded={grounded} slope={slope} foundSlopeBeneath={foundSlopeBeneath} player.groundedLayer={player.groundedLayer}");
#endif
                    if (1 << player.groundedLayer == (int)player.LadderLayer)
                    {
                        bool hit = player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down);
                        float y;
                        if ( hit )
                        {
                            y = Mathf.Floor(player.HitInfo.hit.point.y) + 1 + 0.007519f;// + player.rb.gravityScale * -Physics2D.gravity.y * player.LadderPushUpForce * Time.fixedDeltaTime;
                        }
                        else
                        {
                            y = Mathf.Floor(player.HitInfo.probePoint.y) + 1 + 0.007519f;
                        }

                        noLadderAbove = Physics2D.CircleCast(new Vector2(player.rb.position.x, y) + Vector2.up * 0.6f, player.GroundedRadius, Vector2.up, 0.8f, player.LadderLayer).collider is null;
#if _DEBUG
                        Debug.Log($"noLadderAbove={noLadderAbove}");
#endif
                        if (noLadderAbove)
                        {
                            player.rb.velocity = Vector2.zero;
                            player.rb.totalForce = Vector2.zero;
                            player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);

                            //player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                            player.rb.isKinematic = true;
                            player.rb.MovePosition(new Vector2(player.transform.position.x, y));
                            //player.rb.isKinematic = false;
                            IdleState state = (IdleState)player.states[PlayerState.STATE.IDLE];
                            state.otherGravityScale = player.rb.gravityScale;
                            state.resetGravityScaleWithOther = true;
                            state.overrideOldGravityScale = true;
                            state.oldGravityScale = player.rb.gravityScale;
                            player.rb.gravityScale = 0.0f;
#if _DEBUG
                            Debug.Log($"y={y}");
                            //Debug.Break();
#endif
                        }
                    }

                    if ((1 << player.groundedLayer == (int)player.LadderLayer) && noLadderAbove || (1 << player.groundedLayer == (int)player.GroundLayer))
                    {
                        player.ChangeState(player.states[PlayerState.STATE.IDLE]);
                        return;
                    }
                }
                
                //Debug.Log($"Detected stairs or ground. slope was {slope} player.MoveInput.x={player.MoveInput.x} foundSlopeBeneath={foundSlopeBeneath} && Mathf.Abs(slope) > player.StairsUpMinSlope={Mathf.Abs(slope) > player.StairsUpMinSlope}");                
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.WALL_SLIDE).WillUseKnowledge())
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

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.DASH).WillUseKnowledge())
            {
                player.UseKnowledge(AF.KnowledgeID.DASH);
                return;
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

            if (player.CanClimbLadder())
            {
                //Debug.Log("Ladders on airborne");
                //wasOnLadder = true;
                ladderX = Mathf.FloorToInt(player.GetBeneathObjectPosition().x);
            }
            else
            {
                //wasOnLadder = false;
            }

            coyoteTimeCounter -= Time.fixedDeltaTime;

            if(coyoteTimeCounter <= 0.0f && !hasDepletedJumps && wasGrounded) 
            {
                //player.rb.gravityScale = player.DefaultGravityScale;
                player.DepleteJumps();
                hasDepletedJumps = true;
            }
        }

        public override void Exit()
        {
            wasGrounded = true;
            //player.rb.gravityScale = player.DefaultGravityScale;
            player.animator.SetBool("IsFalling", false);
            base.Exit();
        }
    }
}