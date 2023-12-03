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
     * "Outlines".
     * 
     * A bug sometimes prevent Unity from updating the "Custom Physics Shapes" in the scene. To work around this,
     * select the problematic tilemap and uncheck and re-check "Used by composite" (or uncheck and re-check the 
     * TilemapCollider2D component or, as a last resort, remove and re-create that component).
     * 
     * * * * * * * * * * * * * * * * * * * */
    [CreateAssetMenu(fileName = "StairsClimbingUpState", menuName = "States/StairsClimbingUp")]
    public class StairsClimbingUpState : PlayerState
    {
        private int nFrames;
        private float stairsEndY;
        private bool stairsEndIsSet;
        private bool reachedTop;        
        private bool hasTurned;
        private float yEndDistance;  

        public override void Enter()
        {
            nFrames = 0;
            player.animator.SetBool("IsRunning", true);
            stairsEndIsSet = false;
            reachedTop = false;
            hasTurned = false;
            base.Enter();
        }

        public override void Update()
        {
            float angle = 45 * Mathf.Deg2Rad;
            float side = player.IsFacingRight ? 1.0f : -1.0f;
            Vector2 vec = new Vector2(side * Mathf.Cos(angle), -Mathf.Sin(angle));

            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;            

            float stairsSpeed = player.StairsSpeed;
            bool foundStairsInFront = Raycast2DHelper.FindSlopeAtPoint(player.rb.position, out float slope, v * player.StairsUpDistanceHigh + Vector2.up * player.StairsUpHeight, Platformer2DUtilities.RotateVector2(Vector2.down, (player.IsFacingRight ? 1.0f : -1.0f) * 45.0f), player.StairsDownHeight, player.GroundLayer);//, true);

            if (reachedTop)
            {
                float yDiff = stairsEndY - player.rb.position.y;                
                stairsSpeed = Mathf.Lerp(0.0f, player.StairsSpeed, Mathf.Abs(yDiff) / yEndDistance);

                if(yDiff < 0.001f)
                {
                    player.StateMachine.ChangeState(player.States[STATE.WALK]);
                    return;
                }            
            }            

            if (Mathf.Abs(slope) < player.StairsUpMinSlope)
            {
                if (!player.Raycast(false, player.GroundLayer, Vector2.up * player.ColliderSize.y / 2.0f + (player.IsFacingRight ? Vector2.right : -Vector2.right) * 2.5f * player.ColliderSize.x, 0.01f, (player.IsFacingRight ? Vector2.right : -Vector2.right), false, false) && !stairsEndIsSet)
                {
                    //Debug.Log("Starting deceleration!");

                    RaycastHit2D hit = Physics2D.Raycast(player.HitInfo.probePoint, Vector2.down, 2.0f, player.GroundLayer);

                    if (hit.collider is not null)
                    {
                        stairsEndIsSet = true;
                        stairsEndY = hit.point.y;
                        //Debug.Log($"stairsEndY={stairsEndY}");
                    }
                    else
                    {
                        stairsEndIsSet = true;
                        stairsEndY = player.HitInfo.probePoint.y;
                    }

                    float yDiff = stairsEndY - player.rb.position.y;
                    yEndDistance = Mathf.Abs(yDiff);                    
                }                
            }
            
            if (!player.IsGrounded() && !foundStairsInFront && (reachedTop || hasTurned))
            {
                //Debug.Break();
                
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }

            if (player.MoveInput.x == 0.0f)
            {
                //Debug.Log("Here.");
                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
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

            if (player.WillClimbLadder())
            {
                LadderClimbingState state = (LadderClimbingState)player.States[STATE.LADDER];
                state.targetX = player.GetBeneathObjectPosition().x + 0.5f - player.ColliderOffset.x;
                player.StateMachine.ChangeState(state);
                return;
            }
           
            if (player.WillJump())
            {
                player.StateMachine.ChangeState(player.States[STATE.JUMP]);
                return;
            }     

            // Add force but limit speed
            if (player.rb.velocity.magnitude < stairsSpeed)
            {                
                angle = player.StairsUpAngle * Mathf.Deg2Rad;                
                v = new Vector2((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * stairsSpeed * player.StairsAcceleration * Time.fixedDeltaTime;

                player.rb.AddForce(v, ForceMode2D.Force);
                //player.rb.velocity += v;

                if (player.rb.velocity.magnitude > stairsSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * stairsSpeed;
                }
            }

            Vector2 newPosition = new Vector2(player.transform.position.x, player.transform.position.y) + player.rb.velocity * Time.fixedDeltaTime;
            //Debug.Log($"{newPosition.y}  {stairsEndY}");
            if (newPosition.y > stairsEndY && stairsEndIsSet)
            {
                reachedTop = true;                
            }

            if (player.CanTurn())
            {
                player.Turn();
                hasTurned = true;
            }

            nFrames++;
        }

        public override void Exit()
        {
            player.animator.SetBool("IsRunning", false);
            player.rb.isKinematic = false;
            player.rb.velocity = new Vector2((player.IsFacingRight ? 1.0f : -1.0f) * player.WalkSpeed, 0.0f);
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}