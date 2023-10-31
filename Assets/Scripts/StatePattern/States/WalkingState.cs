using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class WalkingState : PlayerState
    {
        private float oldGravityScale;
        public bool willBreak;

        public WalkingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.WALK;
        }

        public override void Enter()
        {
            animator.SetBool("IsRunning", true);
            oldGravityScale = player.rb.gravityScale;

            base.Enter();
        }

        public override void Update()
        {
            if(willBreak)
            {
                Debug.Log($"rb.totalForce={player.rb.totalForce}");
                Debug.Break();
            }

            if (player.CanTurn())
            {
                player.Turn();
            }

            if (player.WillClimbUpStairs())
            {
                player.ChangeState(player.stairsClimbingUpState);
                return;
            }

            if (player.WillClimbDownStairs())
            {
                player.ChangeState(player.stairsClimbingDownState);
                return;
            }

            //if (!player.IsGrounded(false))
            if (!player.IsCastGrounded(false))
            {
                Debug.Log($"rb.totalForce={player.rb.totalForce}");
                //Debug.Break();
                player.ChangeState(player.airborneState);
                return;
            }

            if (player.MoveInput.x == 0.0f)
            {
                player.ChangeState(player.idleState);
                return;
            }

            if(player.MoveInput.y < 0.0f)
            {
                player.ChangeState(player.crouchedState);
                return;
            }

            if (player.WillDash())
            {
                player.ChangeState(player.dashingState);
                return;
            }
            
            if(player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            // Adjust for walking on ladders
            if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.01f, Vector2.up) && //, false, true) &&
                player.MoveInput.x != 0.0f)
            {
                //Debug.Log("Adjusting for ladders");
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                float y = Mathf.Floor(player.HitInfo.hit.point.y) + 1 + player.rb.gravityScale * -Physics2D.gravity.y * player.LadderPushUpForce * Time.fixedDeltaTime;
                player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);

                player.rb.gravityScale = 0.0f;

                Debug.Log("OH NOOOOOO!");
            }
            else
            {
                player.rb.gravityScale = oldGravityScale;
            }

            if (player.WillJump())
            {
                player.ChangeState(player.jumpingState);
                return;
            }
            
            // Add force but limit speed
            if (player.rb.velocity.magnitude < player.WalkSpeed)
            {
                player.rb.AddForce((player.IsFacingRight ? Vector2.right : -Vector2.right) * player.WalkSpeed * player.WalkAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);
                
                if (player.rb.velocity.magnitude > player.WalkSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.WalkSpeed;
                }
            }                      
        }

        public override void Exit()
        {
            player.rb.gravityScale = oldGravityScale;
            animator.SetBool("IsRunning", false);
            willBreak = false;
            base.Exit();
        }
    }
}