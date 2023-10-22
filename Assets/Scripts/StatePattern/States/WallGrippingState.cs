using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{ 
    public class WallGrippingState : PlayerState
    {
        public WallGrippingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.WALLGRIP;
        }

        public override void Enter()
        {
            animator.SetBool("IsWallSliding", true);
            player.Turn();
            player.inputTriggers["Jump"] = false;
            base.Enter();
        }

        public override void Update()
        {
            player.SetHighestAirborneY(true);

            // When the Player is in a corner, all normals are the same, so I use in addition a raycast.
            if (((player.IsEventGrounded && (player.IsVerticalDirection(player.GroundDirection) || player.IsVerticalDirection(player.GroundDirection2))) || player.Raycast(true, player.GroundLayer | player.LadderLayer, Vector2.zero, 0.2f)) && player.rb.velocity.y >= 0.0f)
            {
                player.ChangeState(player._idleState);
                return;
            }

            if(!player.IsGrippingToWall())
            {
                player.ChangeState(player._airborneState);
                return;
            }

            if (player.WillJump())
            {
                player.ChangeState(player._wallJumpingState);
                return;
            }

            if (!player.inputTriggers["Move"] || !((player.MoveInput.x > 0 && !player.IsFacingRight) || (player.MoveInput.x < 0 && player.IsFacingRight)))
            {
                player.ChangeState(player._airborneState);
                return;
            }

            player.rb.AddForce(Vector2.up * -player.rb.velocity.y * player.WallGripForce * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        public override void Exit()
        {
            animator.SetBool("IsWallSliding", false);
            base.Exit();
        }
    }
}