//#define _DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "WalkingState", menuName = "States/Walking")]
    public class WalkingState : PlayerState
    {
        private float oldGravityScale;
        private bool resetGravityScale;
        [NonSerialized] public bool willBreak;
        [NonSerialized] public float newGravityScale;
        [NonSerialized] public bool resetGravityScaleWithOther;
        [NonSerialized] public float otherGravityScale;
        private Vector2 lastposition;

        public override void Enter()
        {
            player.animator.SetBool("IsRunning", true);
            oldGravityScale = player.rb.gravityScale;
            //Debug.Log($"player.rb.gravityScale(1)={player.rb.gravityScale}");
            player.rb.gravityScale = newGravityScale;
            //Debug.Log($"player.rb.gravityScale(2)={player.rb.gravityScale}");
            player.rb.isKinematic = false;

            resetGravityScale = true;
            resetGravityScaleWithOther = true;
            otherGravityScale = player.DefaultGravityScale;

            if ((player.WillClimbLadder() || player.IsAboveLadder()) &&
                player.MoveInput.x != 0.0f)
            {
                player.rb.totalForce = Vector2.zero;
                player.rb.gravityScale = 0.0f;
            }

            // Adding Vector2.right just so it's different than player.rb.position
            lastposition = player.rb.position + Vector2.right;

            base.Enter();
        }

        public override void Update()
        {
            //Debug.Log($"gravityScale = {player.rb.gravityScale}");

            if (willBreak)
            {
                Debug.Log($"rb.totalForce={player.rb.totalForce}");
                Debug.Break();
            }

            if (player.rb.isKinematic)
            {
                player.rb.isKinematic = false;                
            }

            bool hasTurned = false;
            if (player.CanTurn())
            {
                player.Turn();
                hasTurned = true;
            }
            
            if (player.WillClimbUpStairs())
            {
                player.StateMachine.ChangeState(player.States[STATE.STAIRS_UP]);
                return;
            }

            if (player.WillClimbDownStairs())
            {
                //Debug.Break();
                player.StateMachine.ChangeState(player.States[STATE.STAIRS_DOWN]);
                return;
            }

            if (player.MoveInput.y < 0.0f)
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

            if (player.WillJump())
            {
                player.StateMachine.ChangeState(player.States[STATE.JUMP]);
                return;
            }

            bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
            bool grounded = player.IsGrounded(player.GroundLayer | player.LadderLayer, Vector2.zero, player.GroundDistance, true);
            //Debug.Log($"grounded={grounded} slope ={slope} foundSlopeBeneath={foundSlopeBeneath}");
            if (!grounded && (!foundSlopeBeneath || slope == 0.0f))
            {
#if _DEBUG
                Debug.Log($"Walking not grounded!");
#endif
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }
            else
            {
               
                bool playerIsAboveLadder = player.IsAboveLadder();
                if ((player.WillClimbLadder() || playerIsAboveLadder) && 
                player.MoveInput.x != 0.0f && player.rb.gravityScale != 0.0f &&
                player.GetBeneathObject() is null)
                {
#if _DEBUG
                    Debug.Log($"Walking on ladder");
#endif
                    Vector2 force = (player.IsFacingRight ? Vector2.right : -Vector2.right) * player.WalkAcceleration * Time.fixedDeltaTime;

                    player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f) + force / player.rb.mass;
                    if (Mathf.Abs(player.rb.velocity.x) > player.WalkSpeed)
                    {
                        player.rb.velocity = new Vector2(player.WalkSpeed * Mathf.Sign(player.rb.velocity.x), 0.0f);
                    }
                    player.rb.totalForce = Vector2.zero;
                    //player.rb.gravityScale = 0.0f;
                    //player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                    float y;
                    if (playerIsAboveLadder)
                    {
                        y = Mathf.Round(player.groundedY) + player.GroundDistance;//0.1f;// 0.007519f;                        
                        //Debug.Log($"y={y}");
                    }
                    else
                    {
                        float adjust = 1.0f;
                        if (Platformer2DUtilities.AreNearlyEqual(player.GetBeneathObjectPosition().y - Mathf.Floor(player.GetBeneathObjectPosition().y), 0.0f))
                        {
                            adjust = 0.0f;
                        }
                        y = Mathf.Floor(player.GetBeneathObjectPosition().y) + adjust + 0.007519f;//player.rb.gravityScale * -Physics2D.gravity.y * player.LadderPushUpForce * Time.fixedDeltaTime;                    
                    }
                    //player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                    player.rb.isKinematic = true;
                    player.rb.MovePosition(new Vector2(player.transform.position.x + player.rb.velocity.x * Time.fixedDeltaTime, y));

                    //Debug.Log($"Adjusting for walking on ladders y={y} player.HitInfo.hit.point.y={player.HitInfo.hit.point.y}");
                    player.rb.gravityScale = 0.0f;
                   
                    return;
                }
                else
                {                    
                    player.rb.gravityScale = player.DefaultGravityScale;                    
                }
            }

            if (player.WillClimbDownLadder(out Vector2 ladderPoint))
            {
                float ladderX = Mathf.Floor(ladderPoint.x) + 0.5f - player.ColliderOffset.x;
                //Debug.Log($"Climbing ladder... ladderX={ladderX}");
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);
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

            if (player.MoveInput.x == 0.0f)
            {
                if(player.IsAboveLadder())
                {
                    resetGravityScale = false;
                    IdleState state = (IdleState)player.States[PlayerState.STATE.IDLE];
                    state.resetGravityScaleWithOther = true;
                    state.otherGravityScale = player.DefaultGravityScale;
                    player.rb.gravityScale = 0.0f;
                    player.rb.totalForce = Vector2.zero;
                    player.rb.velocity = Vector2.zero;
#if _DEBUG
                    Debug.Log("Walking on ladder while MoveInput.x = 0");
#endif
                }
                else
                {
#if _DEBUG
                    Debug.Log("Not walking on ladder while MoveInput.x = 0");
#endif
                }

                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                return;
            }

            if (player.WillAttack())
            {
                player.StateMachine.ChangeState(player.States[STATE.BASIC_ATTACK]);
                return;
            }

            /*if (player.inputTriggers["Inventory"])
            {
                player.StateMachine.ChangeState(player.States[STATE.PAUSE]);
                return;
            }*/

            // Add force but limit speed
            if ((player.rb.velocity.magnitude < player.WalkSpeed && !Platformer2DUtilities.AreNearlyEqual(lastposition.x, player.rb.position.x)) || hasTurned)
            {
                Vector2 v = (player.IsFacingRight ? Vector2.right : -Vector2.right) * /*player.WalkSpeed **/ player.WalkAcceleration * Time.fixedDeltaTime;
                player.rb.AddForce(v, ForceMode2D.Force);

                //Debug.Log($"AddForce() player.rb.velocity.magnitude={player.rb.velocity.magnitude} v={v}");
                //player.rb.velocity += v;
                if (player.rb.velocity.magnitude > player.WalkSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.WalkSpeed;
                }

                lastposition = player.rb.position;
            }
            //Debug.Log($"player.rb.velocity.magnitude={player.rb.velocity.magnitude} player.rb.gravityScale={player.rb.gravityScale}");
            //Debug.Log($"player.rb.gravityScale(3)={player.rb.gravityScale}");   
        }

        public override void Exit()
        {
            if (resetGravityScale)
            {
                if (resetGravityScaleWithOther)
                {
                    player.rb.gravityScale = otherGravityScale;
                    resetGravityScaleWithOther = false;
                    //Debug.Log($"resetGravityScaleWithOther = True otherGravityScale={otherGravityScale}");
                }
                else
                {
                    player.rb.gravityScale = oldGravityScale;
                    //Debug.Log($"resetGravityScaleWithOther = False oldGravityScale={oldGravityScale}");
                }
            }
            newGravityScale = oldGravityScale;
            player.animator.SetBool("IsRunning", false);
            willBreak = false;
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}