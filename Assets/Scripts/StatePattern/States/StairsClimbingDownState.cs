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
        private float oldGravityScale;
        private bool isCentering;
        private float targetX;
        private float playerSide;
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

            base.Enter();
        }

        public override void Update()
        {
            float angle = 45 * Mathf.Deg2Rad;                        
            float side = player.IsFacingRight ? 1.0f : -1.0f;
            Vector2 vec = new Vector2(-side * Mathf.Cos(angle), -Mathf.Sin(angle));
            if (!player.Raycast(false, player.GroundLayer, Vector2.right * -side * (player.ColliderSize.x / 2), player.StairsGroundDistance, vec) && //, false, true) &&
                !player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * player.StairsGroundDistance, Vector2.down) //, false, true) 
                )
            {
                player.ChangeState(player.airborneState);
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
            if (player.rb.velocity.magnitude < player.StairsSpeed)
            {
                angle = player.StairsDownAngle * Mathf.Deg2Rad;
                Vector3 v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), -Mathf.Sin(angle)) * player.StairsSpeed * player.StairsAcceleration * Time.fixedDeltaTime;

                player.rb.AddForce(v, ForceMode2D.Force);

                if (player.rb.velocity.magnitude > player.StairsSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.StairsSpeed;
                }
            }

            nFrames++;
        }

        public override void Exit()
        {
            animator.SetBool("IsRunning", false);
            //player.rb.gravityScale = oldGravityScale;
            base.Exit();
        }
    }
}