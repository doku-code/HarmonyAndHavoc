using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    public class StairsClimbingUpState : PlayerState
    {
        private int nFrames;
        private float stairsEndY;
        private bool stairsEndIsSet;
        private bool reachedTop;
        private bool finishing;
        private Vector2 finishPosition;
        private Vector2 finishTranslate;
        private int finishNSteps;

        public StairsClimbingUpState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.STAIRS_UP;
        }

        public override void Enter()
        {
            nFrames = 0;
            animator.SetBool("IsRunning", true);
            stairsEndIsSet = false;
            reachedTop = false;
            finishing = false;

            base.Enter();
        }

        public override void Update()
        {
            float angle = 45 * Mathf.Deg2Rad;
            float side = player.IsFacingRight ? 1.0f : -1.0f;
            Vector2 vec = new Vector2(side * Mathf.Cos(angle), -Mathf.Sin(angle));

            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;


            if(finishing)
            {
                if (finishNSteps > 0)
                {
                    finishPosition += finishTranslate;
                    player.rb.MovePosition(finishPosition);
                    finishNSteps--;                    
                }
                else
                {
                    player.ChangeState(player.walkingState);                                        
                }
                return;
            }


            /*if (                
                (player.Raycast(false, player.GroundLayer, Vector2.right * side * (player.ColliderSize.x / 2), player.StairsGroundDistance, vec) &&//, false, true) &&
                player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * player.StairsGroundDistance, Vector2.down) &&//, false, true) &&
                nFrames > 1) || reachedTop
            )*/
            if (reachedTop)
            {
                Debug.Log($"reachedTop={reachedTop}");
                player.rb.velocity = Vector2.zero;// new Vector2(player.rb.velocity.x, 0.0f);
                //player.transform.position = new Vector3(player.transform.position.x, stairsEndY, player.transform.position.z);
                player.rb.totalForce = Vector2.zero;
                player.rb.isKinematic = true;
                //player.rb.MovePosition(new Vector2(player.transform.position.x + v.x * player.StairsUpFinishTranslate.x, stairsEndY));// + player.StairsUpFinishTranslate.y));
                Vector2 goalPosition = new Vector2(player.transform.position.x + v.x * player.StairsUpFinishTranslate.x, stairsEndY);
                
                finishNSteps = 5;
                finishTranslate = (goalPosition - player.rb.position) / finishNSteps;
                finishPosition = player.rb.position;

                finishPosition += finishTranslate;
                player.rb.MovePosition(finishPosition);
                finishNSteps--;

                //player.rb.totalForce = Vector2.zero;
                //player.walkingState.willBreak = true;
                Debug.Log($"stairsEndY = {stairsEndY}");
                //Debug.Break();
                //player.rb.isKinematic = false;
                //player.inputTriggers["Move"] = false;
                //player.rb.gravityScale = 0.0f;
                player.rb.velocity = Vector2.zero;// new Vector2(player.rb.velocity.x, 0.0f);                
                player.rb.totalForce = Vector2.zero;
                //player.rb.AddForce(v * player.WalkSpeed * player.WalkAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);
                //player.ChangeState(player.walkingState);
                
                finishing=true;
                
                return;

            }



            /*if (!player.Raycast(false, player.GroundLayer, Vector2.right * side * (player.ColliderSize.x / 2), player.StairsGroundDistance, vec) && //, false, true) &&
                !player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * player.StairsGroundDistance, Vector2.down)) //, false, true))
            {
                player.ChangeState(player.airborneState);
                return;
            }*/

            
            //bool foundStairsBeneath = player.FindSlopeBeneath(out float slope, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, v * player.StairsUpDistanceHigh2 + Vector2.up * player.StairsUpHeight2);
            bool foundStairsInFront = player.FindSlopeAtPoint(out float slope, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, v);
            //player.stairsSide = foundStairsBeneath;
            //Debug.Log($"After: {player.stairsSide}");
            if (!player.IsCastGrounded(false) && !foundStairsInFront && reachedTop)
            {
                //Debug.Break();
                
                player.ChangeState(player.airborneState);
                return;
            }

            if (player.MoveInput.x == 0.0f)
            {
                Debug.Log("Here.");
                player.ChangeState(player.idleState);
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
            float speedFactor = 1.0f;

            if (!player.Raycast(false, player.GroundLayer, Vector2.up * player.ColliderSize.y / 2.0f + (player.IsFacingRight ? Vector2.right : -Vector2.right) * 2.5f * player.ColliderSize.x, 0.01f, (player.IsFacingRight ? Vector2.right : -Vector2.right), false, false) && !stairsEndIsSet)
            {
                //Debug.Log("Starting deceleration!");

                RaycastHit2D hit = Physics2D.Raycast(player.HitInfo.probePoint, Vector2.down, 2.0f, player.GroundLayer);

                if (hit.collider is not null)
                {
                    stairsEndIsSet = true;
                    stairsEndY = hit.point.y;
                    Debug.Log($"stairsEndY={stairsEndY}");
                }
            }

            if (stairsEndIsSet)
            {
                speedFactor = Mathf.Min(1 / Mathf.Max((player.HitInfo.probePoint.y - stairsEndY) * player.StairsUpDeceleration, 0.001f), 1.0f);                
            }

            float stairsSpeed = player.StairsSpeed;// * speedFactor;

            // Add force but limit speed
            if (player.rb.velocity.magnitude < stairsSpeed)
            {                
                angle = player.StairsUpAngle * Mathf.Deg2Rad;                
                v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * stairsSpeed * player.StairsAcceleration * Time.fixedDeltaTime;

                player.rb.AddForce(v, ForceMode2D.Force);

                if (player.rb.velocity.magnitude > stairsSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * stairsSpeed;
                }
            }

            Vector2 newPosition = new Vector2(player.transform.position.x, player.transform.position.y) + player.rb.velocity * Time.fixedDeltaTime;
            Debug.Log($"{newPosition.y}  {stairsEndY}");
            if (newPosition.y > stairsEndY && stairsEndIsSet)
            {
                //player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - (newPosition.y - stairsEndY), player.transform.position.z);
                //player.rb.MovePosition(new Vector2(player.transform.position.x, player.transform.position.y - (newPosition.y - stairsEndY)));
                //player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                reachedTop = true;
                Debug.Log("Testtttttttt");
            }

            nFrames++;
        }

        public override void Exit()
        {
            animator.SetBool("IsRunning", false);
            player.rb.isKinematic = false;
            player.rb.velocity = new Vector2((player.IsFacingRight ? 1.0f : -1.0f) * player.WalkSpeed, 0.0f);
            base.Exit();
        }
    }
}