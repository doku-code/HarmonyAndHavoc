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

            // When the Player is in a corner, all normals are the same, so I use in addition a raycast.
            if (
                    (
                        player.IsEventGrounded && 
                        (player.IsVerticalDirection(player.GroundDirection) || player.IsVerticalDirection(player.GroundDirection2))
                    )
                    || 
                    (
                        !player.Raycast(true, player.LadderLayer, Vector2.zero, 0.0f) && 
                        player.Raycast(true, player.GroundLayer | player.LadderLayer, Vector2.zero, 0.2f)
                    )
                )
            {
                if (player.WillLand())
                {
                    player.ChangeState(player.landingState);
                }
                else
                {
                    Debug.Log("test 2.");
                    player.idleState.nFrames = 3;
                    player.ChangeState(player.idleState);
                }
                return;
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

            if (player.CanTurn())
            {                                
                player.Turn();
            }

            if (player.MoveInput.x != 0.0f)
            {
                player.rb.AddForce((player.IsFacingRight ? Vector3.right : -Vector3.right) * player.WalkSpeed * player.AirSpeedMultiplier * Time.fixedDeltaTime);
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