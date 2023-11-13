using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace JFM
{ 
    public class LadderClimbingState : PlayerState
    {
        private float oldGravityScale;
        private bool resetGravityScale;

        private bool isCentering;
        private float targetX;
        private float playerSide; 

        public LadderClimbingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.LADDER;
        }

        public override void Enter()
        {
            int inputY = Mathf.FloorToInt(player.MoveInput.y);
            if(inputY == 0 ) 
            {
                inputY = 1;
            }
            animator.SetFloat("MotionSpeed", 1);
            animator.SetInteger("Ladder", inputY);
            oldGravityScale = player.rb.gravityScale;
            player.rb.gravityScale = 0.0f;
            player.rb.velocity = Vector2.zero;

            isCentering = true;          
            if(player.GetBeneathObject() is null)
            {
                //Debug.Break();
            }
            targetX = player.GetBeneathObjectPosition().x - player.ColliderOffset.x + 0.5f;
            playerSide = Mathf.Sign(targetX - player.transform.position.x);

            resetGravityScale = true;

            //Debug.Log($"targetX={targetX} transform.position={player.transform.position}");
            //Debug.Break();
            
            base.Enter();
        }

        public override void Update()
        {            
            if (player.CanTurn())
            {
                player.Turn();
            }

            if (player.rb.isKinematic)
            {
                player.rb.isKinematic = false;
            }

            if (isCentering && player.MoveInput.y != 0.0f)
            {
                float diff = targetX - player.transform.position.x;
                
                if (diff * playerSide <= 0.1f)
                {
                    isCentering = false;
                    player.rb.isKinematic = true;
                    player.transform.position = new Vector3(targetX, player.transform.position.y, player.transform.position.z);
                    player.rb.velocity = Vector2.zero;
                }
                else
                {
                    //Debug.Log($"player.rb.position={player.rb.position}");
                    player.rb.AddForce(Vector2.right * Mathf.Sign(diff) * (Mathf.Abs(diff) * player.LadderCenteringSpeed) * Time.fixedDeltaTime, ForceMode2D.Force);
                }
            }
            else if (player.MoveInput.x != 0.0f)
            {                
                player.ChangeState(player.airborneState);
                return;
            }

            if (player.inputTriggers["Jump"])
            {
                player.inputTriggers["Jump"] = false;
                player.ChangeState(player.airborneState);
                return;
            }
            /*
            if(!player.CanClimbLadder())
            {
                Debug.Log($"Cannot climb ladder.");
                player.ChangeState(player.idleState);
                
                return;
            }*/

            if (player.IsCastGrounded(true, player.GroundLayer))
            {                
                if(player.MoveInput.y < 0.0f)
                {
                    player.ChangeState(player.crouchedState);
                    return;
                }
                else if(player.MoveInput.y == 0.0f)
                {
                    Debug.Log($"Won't climb ladder.");
                    player.ChangeState(player.idleState);
                    return;
                }                                
            }

            if (player.inputTriggers["Move"] && player.MoveInput.y != 0.0f && !isCentering)
            {
                animator.SetFloat("MotionSpeed", 1);
                animator.SetInteger("Ladder", Mathf.FloorToInt(player.MoveInput.y));
                
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
                        y = Mathf.Floor(player.HitInfo.hit.point.y) + 0.007519f;
                    }
                    else
                    {
                        y = Mathf.Floor(player.HitInfo.probePoint.y) + 0.007519f;
                    }
                    player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);

                    player.idleState.otherGravityScale = player.DefaultGravityScale;
                    player.idleState.resetGravityScaleWithOther = true;
                    player.idleState.overrideOldGravityScale = true;
                    player.idleState.oldGravityScale = player.rb.gravityScale;

                    player.rb.gravityScale = 0.0f;
                    player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                    player.rb.totalForce = new Vector2(player.rb.totalForce.x, 0.0f); 
                    resetGravityScale = false;
                    //Debug.Log($"No ladder found. hasHit={player.HitInfo.hasHit}");
                    Debug.Log($"No ladder found. beneathObject={player.GetBeneathObject()}");
                    player.ChangeState(player.idleState);

                    return;
                }

                // Don't want to test the rest.
                return;
            }
            else
            {
                animator.SetFloat("MotionSpeed", 0);
                player.rb.velocity = Vector2.zero;                
            }            
        }

        public override void Exit()
        {
            animator.SetInteger("Ladder", 0);
            if (resetGravityScale)
            {
                player.rb.gravityScale = oldGravityScale;
            }
            base.Exit();
        }
    }
}