using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JFM
{
    public class WalkingState : PlayerState
    {
        private float oldGravityScale;
        private bool resetGravityScale;
        public bool willBreak;
        public float newGravityScale;
        public bool resetGravityScaleWithOther;
        public float otherGravityScale;

        public WalkingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.WALK;
        }

        public override void Enter()
        {
            animator.SetBool("IsRunning", true);
            oldGravityScale = player.rb.gravityScale;
            //Debug.Log($"player.rb.gravityScale(1)={player.rb.gravityScale}");
            player.rb.gravityScale = newGravityScale;
            //Debug.Log($"player.rb.gravityScale(2)={player.rb.gravityScale}");
            player.rb.isKinematic = false;

            resetGravityScale = true;
            resetGravityScaleWithOther = true;
            otherGravityScale = player.DefaultGravityScale;
            base.Enter();
        }

        public override void Update()
        {
            if (willBreak)
            {
                Debug.Log($"rb.totalForce={player.rb.totalForce}");
                Debug.Break();
            }

            if (player.rb.isKinematic)
            {
                player.rb.isKinematic = false;
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

            if (player.WillClimbDownStairs())
            {
                player.ChangeState(player.stairsClimbingDownState);
                return;
            }

            if (player.MoveInput.y < 0.0f)
            {
                player.ChangeState(player.crouchedState);
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

            bool grounded = player.IsCastGrounded(false);
            //Debug.Log($"grounded={grounded} player.groundedDistance={player.groundedDistance}");
            player.Raycast(false, player.LadderLayer | player.GroundLayer, Vector2.zero, 1.0f, Vector2.down);//, false, true);
            if ((!grounded && player.HitInfo.hit.distance > player.GroundDistance * 25.0f) || !player.HitInfo.hasHit )
            {
                //Debug.Log($"d={player.HitInfo.hit.distance - player.GroundDistance}");
                //Debug.Log($"rb.totalForce={player.rb.totalForce} rb.velocity={player.rb.velocity}");
                //Debug.Break();
                
                player.ChangeState(player.airborneState);
                return;
            }
            else
            {
                if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down) && //, false, true) &&
                player.MoveInput.x != 0.0f)
                {
                    //Debug.Log("Adjusting for ladders");

                    Vector2 force = (player.IsFacingRight ? Vector2.right : -Vector2.right) * player.WalkAcceleration * Time.fixedDeltaTime;

                    player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f) + force / player.rb.mass;
                    if (Mathf.Abs(player.rb.velocity.x) > player.WalkSpeed)
                    {
                        player.rb.velocity = new Vector2(player.WalkSpeed * Mathf.Sign(player.rb.velocity.x), 0.0f);
                    }
                    player.rb.totalForce = Vector2.zero;
                    player.rb.gravityScale = 0.0f;
                    //player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                    float y = Mathf.Floor(player.rb.position.y) + 0.007519f;//player.rb.gravityScale * -Physics2D.gravity.y * player.LadderPushUpForce * Time.fixedDeltaTime;
                    

                    //player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                    player.rb.isKinematic = true;
                    player.rb.MovePosition(new Vector2(player.transform.position.x + player.rb.velocity.x * Time.fixedDeltaTime, y));                    

                    return;
                }
            }

            if (player.WillClimbDownLadder())
            {
                float ladderX = Mathf.Floor(player.HitInfo.probePoint.x) + 0.5f - player.ColliderOffset.x;
                Debug.Log($"Climbing ladder... ladderX={ladderX}");
                player.transform.position = new Vector3(ladderX, player.transform.position.y, player.transform.position.z);
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (player.MoveInput.x == 0.0f)
            {
                if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
                    )
                {
                    resetGravityScale = false;
                    player.idleState.resetGravityScaleWithOther = true;
                    player.idleState.otherGravityScale = player.DefaultGravityScale;
                    player.rb.gravityScale = 0.0f;
                    //Debug.Log("idling on ladder");
                }
                else
                {
                    //Debug.Log("not idling on ladder");
                }

                player.ChangeState(player.idleState);
                return;
            }
                          
            // Add force but limit speed
            if (player.rb.velocity.magnitude < player.WalkSpeed)
            {
                player.rb.AddForce((player.IsFacingRight ? Vector2.right : -Vector2.right) * player.WalkSpeed * player.WalkAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);
                
                if (player.rb.velocity.magnitude > player.WalkSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.WalkSpeed;
                }
            }

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
            animator.SetBool("IsRunning", false);
            willBreak = false;
            base.Exit();
        }
    }
}