using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{ 
    public class LadderClimbingState : PlayerState
    {
        private float oldGravityScale;

        public LadderClimbingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.LADDER;
        }

        public override void Enter()
        {
            //animator.SetTrigger("isIdle");
            oldGravityScale = player.rb.gravityScale;
            player.rb.gravityScale = 0.0f;
            player.rb.velocity = Vector2.zero;

            base.Enter();
        }

        public override void Update()
        {
            if (player.CanTurn())
            {
                //Debug.Log("Can Turn");
                player.Turn();
            }

            if (player.MoveInput.x != 0.0f)
            {
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

            if (player.inputTriggers["Jump"])
            {
                player.ChangeState(player._airborneState);
                return;
            }

            if(!player.CanClimbLadder())
            {
                if (player.IsInFrontOfObjectLayer(new Vector2(0.0f, -0.5f), player.LadderLayer))
                {
                    player.ChangeState(player._ledgeClimbingState);
                }
                else
                {                    
                    player.ChangeState(player._airborneState);
                }
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.y != 0.0f)
            {

                //Debug.Log("OKKKKKKK!!!!!");
                // Add force but limit speed
                if (player.rb.velocity.y < player.LadderSpeed)
                {
                    player.rb.AddForce(Vector2.up * player.MoveInput.y * player.LadderSpeed * player.LadderAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);

                    if (Mathf.Abs(player.rb.velocity.y) > player.LadderSpeed)
                    {
                        player.rb.velocity = new Vector2(player.rb.velocity.x, Mathf.Sign(player.rb.velocity.y) * player.LadderSpeed);
                    }
                }

                // Don't want to test the rest.
                return;
            }
            else
            {
                player.rb.velocity = Vector2.zero;
                //Debug.Log("YEAAAAAAAAAH!!!!!");
            }
            

            if (player.IsGrounded())
            {
                player.ChangeState(player._idleState);
                return;
            }
        }

        public override void Exit()
        {
            //animator.ResetTrigger("isIdle");
            player.rb.gravityScale = oldGravityScale;
            base.Exit();
        }
    }
}