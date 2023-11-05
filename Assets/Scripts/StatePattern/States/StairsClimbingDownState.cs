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
     * "Polygons".
     * 
     * A bug sometimes prevent Unity from updating the "Custom Physics Shapes" in the scene. To work around this,
     * select the problematic tilemap and check and uncheck "Used by composite".
     * 
     * * * * * * * * * * * * * * * * * * * */
    public class StairsClimbingDownState : PlayerState
    {        
        private int nFrames;

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

            base.Enter();
        }

        public override void Update()
        {
            float angle = 45 * Mathf.Deg2Rad;
            float side = player.IsFacingRight ? 1.0f : -1.0f;
            float distance = player.StairsGroundDistance;
            Vector2 vec;
            vec = new Vector2(-side * Mathf.Cos(angle), -Mathf.Sin(angle));
            /*if (!player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * distance, Vector2.down, false, false) &&
                !player.Raycast(false, player.GroundLayer, Vector2.right * -side * (player.ColliderSize.x / 2), distance, vec, false, false) &&
                !player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.down * player.StairsDownMinHeight * 2.5f, 0.1f, Vector2.down, false, false)
            )
            {
                player.ChangeState(player.airborneState);
                return;
            }*/
            Vector2 v = player.IsFacingRight ? -Vector2.right : Vector2.right;

            bool foundStairsBeneath = player.FindSlopeBeneath(out float slope, Vector2.up * 0.02f + v * player.StairsDownGroundX, -v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight);
            //player.stairsSide = foundStairsBeneath;
            if (!player.IsCastGrounded(false) && !foundStairsBeneath)
            {
                player.ChangeState(player.airborneState);
                //Debug.Break();
                return;
            }

            if (player.MoveInput.x == 0.0f)
            {
                player.ChangeState(player.idleState);
                return;
            }

            if (                
                player.Raycast(false, player.GroundLayer, Vector2.right * side * (player.ColliderSize.x / 2), player.StairsGroundDistance, vec) && //, false, true) &&
                player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * player.StairsGroundDistance, Vector2.down) && //, false, true) &&
                nFrames > 1                
            )
            {
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

            // Add force but limit speed
            if (player.rb.velocity.magnitude < player.StairsSpeed * player.StairsDownDeceleration)
            {
                if (Mathf.Abs(slope) < player.StairsUpMinSlope)
                {
                    angle = 0.0f;
                }
                else
                { 
                    angle = player.StairsDownAngle * Mathf.Deg2Rad;
                }
                v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), -Mathf.Sin(angle)) * player.StairsSpeed * player.StairsAcceleration * player.StairsDownDeceleration * Time.fixedDeltaTime;

                player.rb.AddForce(v, ForceMode2D.Force);

                if (player.rb.velocity.magnitude > player.StairsSpeed * player.StairsDownDeceleration)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.StairsSpeed * player.StairsDownDeceleration;
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