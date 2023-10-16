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
            //animator.SetTrigger("isIdle");
            player.Turn();
            //player.inputTriggers["Jump"] = false;
            base.Enter();
        }

        public override void Update()
        {

            //Debug.Log($"player.MoveInput.y={player.MoveInput.y}");
            if (player.IsGrounded())
            {
                player.ChangeState(player._idleState);
                return;
            }

            //Debug.Log($"player.CanJump() = {player.CanJump()} && player.inputTriggers[\"Jump\"] = {player.inputTriggers["Jump"]}");
            if (player.CanJump() && player.inputTriggers["Jump"])
            {
                //Debug.Log("OUIIIIIII");
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
            //animator.ResetTrigger("isIdle");
            base.Exit();
        }
    }
}