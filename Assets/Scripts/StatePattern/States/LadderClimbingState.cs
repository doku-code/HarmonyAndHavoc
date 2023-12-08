//#define _DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace JFM
{
    [CreateAssetMenu(fileName = "LadderClimbingState", menuName = "States/LadderClimbing")]
    public class LadderClimbingState : PlayerState
    {
        private float oldGravityScale;
        private bool resetGravityScale;

        private bool isCentering;
        [NonSerialized] public float targetX;
        private float playerSide; 
        private float lastPositionX;
        
        public override void Enter()
        {
            int inputY = Mathf.FloorToInt(player.MoveInput.y);
            if(inputY == 0 ) 
            {
                inputY = 1;
            }

            // In case Crouch state was triggered right before going down the ladder,
            // put the Player back in Idle state (it'll be overriden since it's an
            // animator sub-layer
            player.animator.SetBool("IsIdle", true);
            
            player.animator.SetFloat("MotionSpeed", 1);
            player.animator.SetInteger("Ladder", inputY);
            oldGravityScale = player.rb.gravityScale;
            player.rb.gravityScale = 0.0f;
            player.rb.velocity = Vector2.zero;

            isCentering = true;          
            
            //targetX = player.GetBeneathObjectPosition().x - player.ColliderOffset.x + 0.5f;
            playerSide = Mathf.Sign(targetX - player.transform.position.x);

            resetGravityScale = true;

            lastPositionX = player.transform.position.x;
            //Debug.Log($"targetX={targetX} transform.position={player.transform.position}");
            //Debug.Break();

            base.Enter();
        }

        public override void Update()
        {
            //player.SetHighestAirborneY(true);

            if (player.GetBeneathObject() is null && !player.IsAboveLadder())
            {
                //Debug.Break();
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }

            if (player.CanTurn())
            {
                player.Turn();
            }

            if (player.rb.isKinematic)
            {
                player.rb.isKinematic = false;
                player.rb.gravityScale = player.DefaultGravityScale;
#if _DEBUG
                Debug.Log($"player.rb.position={player.rb.position}");
                //Debug.Break();
#endif
            }

            if (isCentering && player.MoveInput.y != 0.0f)
            {
                float diff = targetX - player.transform.position.x;
                
                if (diff * playerSide <= 0.1f)
                {
                    isCentering = false;
                    player.rb.isKinematic = true;
                    player.rb.MovePosition(new Vector3(targetX, player.transform.position.y, player.transform.position.z));
                    player.rb.velocity = Vector2.zero;
                    player.rb.totalForce = Vector2.zero;
                    player.rb.gravityScale = 0.0f;
#if _DEBUG
                    Debug.Log($"(1) player.rb.position={player.rb.position}");
#endif
                }
                else
                {                    
                    player.rb.AddForce(Vector2.right * Mathf.Sign(diff) * (Mathf.Abs(diff) * player.LadderCenteringSpeed) * Time.fixedDeltaTime, ForceMode2D.Force);
                    if(Mathf.Abs(player.rb.velocity.x) < 0.01f)
                    {
                        isCentering = false;
                        player.rb.isKinematic = true;
                        player.rb.MovePosition(new Vector2(targetX, player.transform.position.y));
                        player.rb.velocity = Vector2.zero;
                        player.rb.totalForce = Vector2.zero;
                        player.rb.gravityScale = 0.0f;
                    }
                    lastPositionX = player.transform.position.x;
#if _DEBUG
                    Debug.Log($"(2) player.rb.position={player.rb.position} targetX={targetX}");
#endif
                }
            }
            else if (player.MoveInput.x != 0.0f)
            {                
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }

            if (player.inputTriggers["Jump"])
            {
                player.inputTriggers["Jump"] = false;
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }
            /*
            if(!player.CanClimbLadder())
            {
#if _DEBUG
                Debug.Log($"Cannot climb ladder.");
#endif
                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                
                return;
            }*/

            if (player.IsGrounded(player.GroundLayer))
            {                
                if(player.MoveInput.y < 0.0f)
                {
                    player.StateMachine.ChangeState(player.States[STATE.CROUCH]);
                    return;
                }
                else if(player.MoveInput.y == 0.0f)
                {
#if _DEBUG
                    Debug.Log($"Won't climb ladder.");
#endif
                    player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                    return;
                }                                
            }

            //Debug.Log($"isCentering={isCentering}");
            if (player.inputTriggers["Move"] && player.MoveInput.y != 0.0f && !isCentering)
            {
                player.animator.SetFloat("MotionSpeed", 1);
                player.animator.SetInteger("Ladder", Mathf.FloorToInt(player.MoveInput.y));
                
                // Add force but limit speed
                if (player.rb.velocity.y < player.LadderSpeed)
                {
                    player.rb.AddForce(Vector2.up * player.MoveInput.y * player.LadderSpeed * player.LadderAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);

                    if (Mathf.Abs(player.rb.velocity.y) > player.LadderSpeed)
                    {
                        player.rb.velocity = new Vector2(player.rb.velocity.x, Mathf.Sign(player.rb.velocity.y) * player.LadderSpeed);
                    }
                }

                //if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.02f, 0.25f, Vector2.up, false, true) && player.MoveInput.y > 0.0f)
                if( !player.CanClimbLadder() && player.MoveInput.y > 0.0f)
                {                    
                    player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                    float y;
                    if (player.Raycast(false, player.LadderLayer, Vector2.up * 0.02f, 0.25f, Vector2.up, false, true))
                    {
                        y = Mathf.Round(player.HitInfo.hit.point.y);// + 0.007519f;
                    }
                    else
                    {
                        y = Mathf.Round(player.HitInfo.probePoint.y);// + 0.007519f;
                    }
                    player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                    
                    IdleState state = (IdleState)player.States[PlayerState.STATE.IDLE];
                    state.otherGravityScale = player.DefaultGravityScale;
                    state.resetGravityScaleWithOther = true;
                    state.overrideOldGravityScale = true;
                    state.oldGravityScale = player.rb.gravityScale;

                    player.rb.gravityScale = 0.0f;
                    player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                    player.rb.totalForce = new Vector2(player.rb.totalForce.x, 0.0f); 
                    resetGravityScale = false;
                    //Debug.Log($"No ladder found. hasHit={player.HitInfo.hasHit}");
#if _DEBUG
                    Debug.Log($"No ladder found. beneathObject={player.GetBeneathObject()}");
                    //Debug.Break();
#endif
                    player.StateMachine.ChangeState(state);
                    
                    return;
                }

                // Don't want to test the rest.
                return;
            }
            else
            {
                player.animator.SetFloat("MotionSpeed", 0);
                player.rb.velocity = Vector2.zero;  
                player.rb.totalForce = Vector2.zero;
                player.rb.gravityScale = 0.0f;
            }            
        }

        public override void Exit()
        {
            player.animator.SetBool("IsIdle", false);
            player.animator.SetInteger("Ladder", 0);
            if (resetGravityScale)
            {
                player.rb.gravityScale = oldGravityScale;
            }
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}