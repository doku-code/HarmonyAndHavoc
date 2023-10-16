using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class WalkingState : PlayerState
    {
        public WalkingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.WALK;
        }

        public override void Enter()
        {
            //animator.SetTrigger("isIdle");
            base.Enter();
        }

        public override void Update()
        {

            if (!player.IsGrounded())
            {
                player.ChangeState(player._airborneState);
                return;
            }

            //Debug.Log($"player.inputTriggers[\"Move\"]={player.inputTriggers["Move"]}");
            if (player.MoveInput.x == 0.0f)
            {
                player.ChangeState(player._idleState);
                return;
            }

            if (player.CanDash())
            {
                player.ChangeState(player._dashingState);
                return;
            }

            if (player.CanTurn())
            {
                //Debug.Log("Can Turn");
                player.Turn();
            }
            
            if(player.CanClimbLadder())
            {
                //Debug.Log("YESSSSS");
                player.ChangeState(player._ladderClimbingState);
                return;
            }

            if (player.CanJump() && player.inputTriggers["Jump"])
            {
                player.ChangeState(player._jumpingState);
                return;
            }

            // Add force but limit speed
            if (player.rb.velocity.magnitude < player.WalkSpeed)
            {                
                //Debug.Log($"(player.IsFacingRight ? Vector3.right : -Vector3.right) * player.WalkSpeed * Time.deltaTime = {(player.IsFacingRight ? Vector3.right : -Vector3.right) * player.WalkSpeed * Time.deltaTime}");
                player.rb.AddForce((player.IsFacingRight ? Vector2.right : -Vector2.right) * player.WalkSpeed * player.WalkAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);

                if (Mathf.Abs(player.rb.velocity.magnitude) > player.WalkSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.WalkSpeed;
                }
            }

            
        }

        public override void Exit()
        {
            //animator.ResetTrigger("isIdle");
            base.Exit();
        }
    }
}