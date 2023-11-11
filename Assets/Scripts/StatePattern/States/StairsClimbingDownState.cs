using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace JFM
{
    /* * * * * * * * * * * * * * * * * * * * 
     * 
     * It seems that, in order to make raycasts work with irregular collider shapes
     * (like stairs for instance), you must set the Geometry Type of the Composite Collider 2D to
     * "Outlines".
     * 
     * A bug sometimes prevent Unity from updating the "Custom Physics Shapes" in the scene. To work around this,
     * select the problematic tilemap and uncheck and re-check "Used by composite" (or uncheck and re-check the 
     * TilemapCollider2D component).
     * 
     * * * * * * * * * * * * * * * * * * * */
    public class StairsClimbingDownState : PlayerState
    {        
        private int nFrames;
        private float stairsSlope;
        private bool addForceNotWorking;

        public StairsClimbingDownState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.STAIRS_DOWN;
        }

        public override void Enter()
        {
            nFrames = 0;
            animator.SetBool("IsRunning", true);           
            player.rb.velocity = Vector2.zero;
            player.rb.totalForce = Vector2.zero;
            addForceNotWorking = false;

            base.Enter();
        }

        public override void Update()
        {
            float angle = 45 * Mathf.Deg2Rad;
            float side = player.IsFacingRight ? 1.0f : -1.0f;
            float distance = player.StairsGroundDistance;
            Vector2 vec;
            vec = new Vector2(-side * Mathf.Cos(angle), -Mathf.Sin(angle));
            
            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;

            bool foundStairsBeneath;
            foundStairsBeneath =  player.FindSlopeBeneath(out float slope, Vector2.up * 0.02f + v * player.StairsDownGroundX, -v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, player.StairsDownHeight, false, (player.IsFacingRight ? -1.0f : 1.0f) * 45.0f);            

            if (foundStairsBeneath && nFrames == 0)
            {
                stairsSlope = slope;
                //Debug.Log($"stairsSlope={stairsSlope}");
                //Debug.Break();
            }

            //player.stairsSide = foundStairsBeneath;
            if (!player.IsCastGrounded(false) && !foundStairsBeneath)
            {
                player.ChangeState(player.airborneState);
                //Debug.Log("Going Airborne");
                //Debug.Break();
                return;
            }

            if (player.MoveInput.x == 0.0f)
            {
                player.ChangeState(player.idleState);
                return;
            }

            /*if (                
                player.Raycast(false, player.GroundLayer, Vector2.right * side * (player.ColliderSize.x / 2), player.StairsGroundDistance, vec) && //, false, true) &&
                player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * player.StairsGroundDistance, Vector2.down) && //, false, true) &&
                nFrames > 1                
            )*/
            if(Mathf.Abs(slope) < player.StairsUpMinSlope && nFrames > 1)
            {
                //Debug.Log($"slope={slope}");
                //Debug.Break();
                player.ChangeState(player.walkingState);
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

            if (player.CanTurn())
            {
                player.Turn();
            }

            if (player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }            

            if (player.WillJump())
            {
                player.ChangeState(player.jumpingState);
                return;
            }

            float usedSlope = 0.0f;
            //if (Mathf.Abs(slope) < player.StairsUpMinSlope && Mathf.Abs(slope2) < player.StairsUpMinSlope)
            if (Mathf.Abs(stairsSlope) < player.StairsUpMinSlope )
            {
                angle = 0.0f;
            }
            else
            {
                //angle = player.StairsDownAngle * Mathf.Deg2Rad;
                usedSlope = stairsSlope;
                angle = Mathf.Atan(-usedSlope);
                //Debug.Log($"angle={angle * Mathf.Rad2Deg}");
            }
            float angleRange = (62.0f * Mathf.Deg2Rad) - Mathf.PI / 4.0f;
            float lerp = Mathf.Lerp(1.0f, 0.4f, (Mathf.Abs(Mathf.Max(angle, Mathf.PI / 4.0f)) - Mathf.PI / 4.0f) / angleRange);
            float speed = player.StairsSpeed * lerp;
            //Debug.Log($"lerp={lerp}");

            if(lerp == 1.0f)
            {
                //Debug.Log($"slope={slope}");
                //Debug.Break();
            }

            // Add force but limit speed
            if (player.rb.velocity.magnitude < speed /** player.StairsDownDeceleration*/)
            {
                
                //f = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), -Mathf.Sin(angle)) * speed * player.StairsAcceleration * player.StairsDownDeceleration * Time.fixedDeltaTime;

                Vector3 f = new Vector3(player.IsFacingRight ? 1.0f : -1.0f, 0.0f) * speed * player.StairsAcceleration * player.StairsDownDeceleration * Time.fixedDeltaTime;


                player.rb.AddForce(f, ForceMode2D.Force);
                //Debug.Log($"AddForce() player.rb.velocity.magnitude={player.rb.velocity.magnitude} speed = {speed} f={f}");

                /*if(player.rb.velocity.magnitude == 0.0f || addForceNotWorking)
                {
                    addForceNotWorking = true;
                    player.rb.velocity += (v * player.StairsAcceleration * player.StairsDownDeceleration * Time.fixedDeltaTime) / player.rb.mass;
                    Debug.Log($"Passage a la course.");
                    //player.ChangeState(player.walkingState);
                    //return;
                }*/

                if (player.rb.velocity.magnitude > speed /** player.StairsDownDeceleration*/)
                {
                    player.rb.velocity = player.rb.velocity.normalized * speed /** player.StairsDownDeceleration*/;
                }
            }

            nFrames++;
        }

        public override void Exit()
        {
            animator.SetBool("IsRunning", false);
            base.Exit();
        }
    }
}