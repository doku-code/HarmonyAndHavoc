using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class AirborneState : PlayerState
    {
        public AirborneState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.AIRBORNE;
        }

        public override void Enter()
        {            
            animator.SetBool("IsFalling", true);
            base.Enter();
        }

        public override void Update()
        {

            player.SetHighestAirborneY();

            if (player.CanTurn())
            {
                player.Turn();
            }

            // When the Player is in a corner, all normals are the same, so I use in addition a raycast.
            /*if (
                (
                    player.IsEventGrounded && 
                    (player.IsVerticalDirection(player.GroundDirection) || player.IsVerticalDirection(player.GroundDirection2))
                )
                || 
                (
                    !player.Raycast(false, player.LadderLayer, Vector2.zero, 0.0f) && 
                    player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, 0.2f)
                )
            )*/
            //if(player.IsGrounded(false))
            Vector2 v;
            //Debug.Log($"Before: {player.stairsSide}");
            if (player.stairsSide != 0)
            {
                v = player.stairsSide == -1 ? -Vector2.right : Vector2.right;
            }
            else
            {
                v = Vector2.zero;
            }
            int foundStairsBeneath = player.FindStairsBeneath();
            bool stairsAreRightSide = foundStairsBeneath == 1;
            player.stairsSide = foundStairsBeneath;
            //Debug.Log($"After: {player.stairsSide}");
            if (player.IsCastGrounded(false) || foundStairsBeneath != 0)
            {
                if (player.WillLand())
                {
                    player.ChangeState(player.landingState);
                }
                else
                {
                    if (player.MoveInput.x != 0.0f && foundStairsBeneath != 0)
                    {
                        if(stairsAreRightSide == player.IsFacingRight)
                        {
                            player.ChangeState(player.stairsClimbingUpState);
                        }
                        else
                        {
                            player.ChangeState(player.stairsClimbingDownState);
                        }
                    }
                    else
                    {
                        // To rectify
                        // The Player actually "waits" in idle after having fallen
                        player.idleState.nFrames = 3;

                        player.ChangeState(player.idleState);
                    }
                }
                return;
            }

            /*float angle = 45 * Mathf.Deg2Rad;
            float side = player.IsFacingRight ? 1.0f : -1.0f;
            float distance = player.StairsGroundDistance;
            Vector2 vec = new Vector2(side * Mathf.Cos(angle), -Mathf.Sin(angle));

            if (!player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * distance, Vector2.down, false, false) &&
                player.Raycast(false, player.GroundLayer, Vector2.right * side * (player.ColliderSize.x / 2), distance, vec, false, false) &&
                player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.down * player.StairsDownMinHeight * 2.5f, 0.1f, Vector2.down, false, false)
            )
            {
                if (player.MoveInput.x != 0.0f)
                {
                    player.ChangeState(player.stairsClimbingUpState);
                }
                else
                {
                    player.ChangeState(player.idleState);
                }
                return;
            }
            vec = new Vector2(-side * Mathf.Cos(angle), -Mathf.Sin(angle));
            if (!player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, Mathf.Sin(angle) * distance, Vector2.down, false, false) && 
                player.Raycast(false, player.GroundLayer, Vector2.right * -side * (player.ColliderSize.x / 2), distance, vec, false, false) &&
                player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.down * player.StairsDownMinHeight * 2.5f, 0.1f, Vector2.down, false, false)
            )
            {
                if (player.MoveInput.x != 0.0f)
                {
                    player.ChangeState(player.stairsClimbingDownState);
                }
                else
                {
                    player.ChangeState(player.idleState);
                }
                return;
            }*/

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
        }

        public override void Exit()
        {
            animator.SetBool("IsFalling", false);
            base.Exit();
        }
    }
}