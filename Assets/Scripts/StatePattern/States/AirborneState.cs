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
            //animator.SetTrigger("isIdle");
            
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

            if (player.CanGripToWall())
            {
                player.ChangeState(player._wallGrippingState);
                return;
            }

            if (player.CanClimbLadder())
            {
                Debug.Log("YESSSSS");
                player.ChangeState(player._ladderClimbingState);
                return;
            }

            if (player.CanDash())
            {
                player.ChangeState(player._dashingState);
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

            if (player.CanJump() && player.inputTriggers["Jump"])
            {                
                player.Jump();
            }
        }

        public override void Exit()
        {
            //animator.ResetTrigger("isIdle");
            base.Exit();
        }
    }
}