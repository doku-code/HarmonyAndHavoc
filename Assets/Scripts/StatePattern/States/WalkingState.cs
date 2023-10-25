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
            animator.SetBool("IsRunning", true);
            base.Enter();
        }

        public override void Update()
        {
            //if (!(player.IsEventGrounded && (player.IsVerticalDirection(player.GroundDirection) || player.IsVerticalDirection(player.GroundDirection2))) && !player.Raycast(true, player.GroundLayer | player.LadderLayer, Vector2.zero, 0.2f))
            if(!player.IsGrounded(false))
            {
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

            if (player.CanTurn())
            {
                player.Turn();                
            }
            
            if(player.WillClimbLadder())
            {
                player.ChangeState(player.ladderClimbingState);
                return;
            }

            if (!player.Raycast(true, player.LadderLayer, Vector2.up * 0.2f, 0.0f) &&
                player.Raycast(true, player.LadderLayer, Vector2.zero, 0.0f) && 
                player.MoveInput.x != 0.0f)
            {

                //player.rb.velocity = new Vector2(player.rb.velocity.x, player.LadderPushUpForce);
                //player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);

                player.rb.velocity = new Vector2(player.rb.velocity.x, 0.0f);
                player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                float y = Mathf.Floor(player.HitInfo.hit.point.y) + 1;
                player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);

                //return;
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
            animator.SetBool("IsRunning", false);
            base.Exit();
        }
    }
}