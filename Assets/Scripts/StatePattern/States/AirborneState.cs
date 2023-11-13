using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class AirborneState : PlayerState
    {
        private bool wasOnLadder;
        private int ladderX;

        public AirborneState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.AIRBORNE;
        }

        public override void Enter()
        {            
            animator.SetBool("IsFalling", true);

            if (player.CanClimbLadder())
            {
                //Debug.Log("Ladders on airborne.Enter().");
                wasOnLadder = true;
                ladderX = Mathf.FloorToInt(player.GetBeneathObjectPosition().x);
            }
            else
            {
                wasOnLadder = false;
            }

            base.Enter();
        }

        public override void Update()
        {

            player.SetHighestAirborneY();

            if (player.CanTurn())
            {
                player.Turn();
            }
            
            Vector2 v;
            
            bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
            bool stairsAreRightSide = slope > 0;
            //player.stairsSide = foundSlopeBeneath;
            //Debug.Log($"After: {player.stairsSide}");
            bool grounded = player.IsCastGrounded(false);            

            if (grounded && player.WillLand() && (!foundSlopeBeneath || Mathf.Abs(slope) < player.StairsUpMaxSlope))
            {
                //Debug.Log($"foundSlopeBeneath={foundSlopeBeneath} grounded={grounded}");
                player.ChangeState(player.landingState);
                return;
            }
            //Debug.Log($"1 << player.groundedLayer={1 << player.groundedLayer} wasOnLadder={wasOnLadder} ladder={ladderX}");

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
                                //player.ChangeState(player.stairsClimbingUpState);
                                player.ChangeState(player.walkingState);
                            }
                            else
                            {
                                //player.ChangeState(player.stairsClimbingDownState);
                                player.ChangeState(player.walkingState);
                            }
                        }
                        else
                        {
                            player.ChangeState(player.wallGrippingState);
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
                                player.walkingState.resetGravityScaleWithOther = true;
                                player.walkingState.otherGravityScale = player.rb.gravityScale;
                                player.walkingState.newGravityScale = 0.0f;
                                player.rb.gravityScale = 0.0f;
                                player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                                player.rb.totalForce = Vector2.zero;

                                float y = Mathf.Floor(player.HitInfo.hit.point.y) + 1 + 0.007519f;// - player.ColliderSize.y + player.ColliderOffset.y;
                                player.rb.isKinematic = true;
                                player.rb.MovePosition(new Vector2(player.transform.position.x, y));
                                //Debug.Log($"Bon! y={y}");
                            }
                        }
                        //Debug.Log($"MoveInput.x != 0.0f!!! grounded={grounded} slope={slope} foundSlopeBeneath={foundSlopeBeneath} player.groundedLayer={player.groundedLayer}");
                        //Debug.Break();
                        player.ChangeState(player.walkingState);
                        return;
                    }
                    // If grounded to Ground layer, or to Ladder layer IF we didn't start this Airborne state in front 
                    // of a ladder or we started in front of a ladder of another X coordinate.
                    else if (grounded
                            &&
                            (
                                (1 << player.groundedLayer == (int)player.GroundLayer)
                                ||
                                (
                                    (1 << player.groundedLayer == (int)player.LadderLayer)
                                    &&
                                    (
                                        !wasOnLadder
                                        ||
                                        ladderX != Mathf.FloorToInt(player.rb.position.x + player.ColliderOffset.x)
                                    )
                                )
                            )
                            )
                    {
                        //Debug.Log($"Didn't make a case (1)... grounded={grounded} slope={slope} foundSlopeBeneath={foundSlopeBeneath} player.groundedLayer={player.groundedLayer}");
                        player.ChangeState(player.walkingState);
                        return;
                    }
                }
                else if (grounded && (Mathf.Abs(slope) > player.StairsUpMinSlope && foundSlopeBeneath))
                {

                    // Adjust for walking on ladders
                    if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                        player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down) )
                    {
                        //Debug.Log("Adjusting for ladders");
                        player.rb.velocity = Vector2.zero;
                        player.rb.totalForce = Vector2.zero;
                        player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                        float y = Mathf.Floor(player.HitInfo.hit.point.y) + 1 + 0.007519f;// + player.rb.gravityScale * -Physics2D.gravity.y * player.LadderPushUpForce * Time.fixedDeltaTime;
                        //player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                        player.rb.isKinematic = true;
                        player.rb.MovePosition(new Vector2(player.transform.position.x, y));
                        //player.rb.isKinematic = false;
                        player.idleState.otherGravityScale = player.rb.gravityScale;
                        player.idleState.resetGravityScaleWithOther = true;
                        player.idleState.overrideOldGravityScale = true;
                        player.idleState.oldGravityScale = player.rb.gravityScale;
                        player.rb.gravityScale = 0.0f;
                            
                        //Debug.Log($"Adjust for walking on ladders! y={y}");
                    }

                    //Debug.Log($"MoveInput.x = 0 grounded={grounded} || ({Mathf.Abs(slope) > player.StairsUpMinSlope} && {foundSlopeBeneath})");
                    // To rectify
                    // The Player actually "waits" in idle after having fallen
                    player.idleState.waitNFrames = 3;

                    player.ChangeState(player.idleState);
                    return;
                }
                // If grounded to Ground layer, or to Ladder layer IF we didn't start this Airborne state in front 
                // of a ladder or we started in front of a ladder of another X coordinate.
                else if (grounded 
                        && 
                        (
                            (1 << player.groundedLayer == (int)player.GroundLayer) 
                            || 
                            (
                                (1 << player.groundedLayer == (int)player.LadderLayer) 
                                && 
                                (
                                    !wasOnLadder 
                                    || 
                                    ladderX != Mathf.FloorToInt(player.rb.position.x + player.ColliderOffset.x)
                                )
                            )
                        )
                        ) 
                {                    
                    //Debug.Log($"Didn't make a case (2)... grounded={grounded} slope={slope} foundSlopeBeneath={foundSlopeBeneath} player.groundedLayer={player.groundedLayer}");

                    if(1 << player.groundedLayer == (int)player.LadderLayer)
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
                        player.rb.velocity = Vector2.zero;
                        player.rb.totalForce = Vector2.zero;
                        player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                        
                        //player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
                        player.rb.isKinematic = true;
                        player.rb.MovePosition(new Vector2(player.transform.position.x, y));
                        //player.rb.isKinematic = false;
                        player.idleState.otherGravityScale = player.rb.gravityScale;
                        player.idleState.resetGravityScaleWithOther = true;
                        player.idleState.overrideOldGravityScale = true;
                        player.idleState.oldGravityScale = player.rb.gravityScale;
                        player.rb.gravityScale = 0.0f;

                    }

                    player.ChangeState(player.idleState);
                    return;
                }
                
                //Debug.Log($"Detected stairs or ground. slope was {slope} player.MoveInput.x={player.MoveInput.x} foundSlopeBeneath={foundSlopeBeneath} && Mathf.Abs(slope) > player.StairsUpMinSlope={Mathf.Abs(slope) > player.StairsUpMinSlope}");                
            }                       

            if (player.WillGripToWall())
            {
                player.ChangeState(player.wallGrippingState);
                return;
            }

            if (player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (player.WillDash())
            {
                player.ChangeState(player.dashingState);
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
                wasOnLadder = true;
                ladderX = Mathf.FloorToInt(player.GetBeneathObjectPosition().x);
            }
            else
            {
                wasOnLadder = false;
            }
        }

        public override void Exit()
        {
            animator.SetBool("IsFalling", false);
            base.Exit();
        }
    }
}