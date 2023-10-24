using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{ 
    public class LadderClimbingState : PlayerState
    {
        private float oldGravityScale;
        private bool isCentering;
        private float targetX;
        private float playerSide; 

        public LadderClimbingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.LADDER;
        }

        public override void Enter()
        {
            animator.SetFloat("MotionSpeed", 1);
            animator.SetInteger("Ladder", Mathf.FloorToInt(player.MoveInput.y));
            oldGravityScale = player.rb.gravityScale;
            player.rb.gravityScale = 0.0f;
            player.rb.velocity = Vector2.zero;

            isCentering = true;           
            targetX = player.GetBeneathObjectPosition().x - player.ColliderOffset.x + 0.5f;
            playerSide = Mathf.Sign(targetX - player.transform.position.x);
            
            base.Enter();
        }

        public override void Update()
        {            
            if (player.CanTurn())
            {
                player.Turn();
            }

            if (isCentering && player.MoveInput.y != 0.0f)
            {
                float diff = targetX - player.transform.position.x;
                
                if (diff * playerSide <= 0.1f)
                {
                    isCentering = false;
                    player.transform.position = new Vector3(targetX, player.transform.position.y, player.transform.position.z);
                    player.rb.velocity = Vector2.zero;
                }
                else
                {
                    player.rb.AddForce(Vector2.right * diff * (player.WalkAcceleration * Mathf.Abs(diff) * 10.0f) * Time.fixedDeltaTime, ForceMode2D.Force);
                }
            }
            else if (player.MoveInput.x != 0.0f)
            {                
                player.ChangeState(player.airborneState);
                return;
            }

            if (player.inputTriggers["Jump"])
            {
                player.ChangeState(player.airborneState);
                return;
            }

            if(!player.CanClimbLadder())
            {
                player.ChangeState(player.idleState);
                
                return;
            }

            if (player.inputTriggers["Move"] && player.MoveInput.y != 0.0f)
            {
                animator.SetFloat("MotionSpeed", 1);
                animator.SetInteger("Ladder", Mathf.FloorToInt(player.MoveInput.y));
                
                // Add force but limit speed
                if (player.rb.velocity.y < player.LadderSpeed)
                {
                    player.rb.AddForce(Vector2.up * player.MoveInput.y * player.LadderSpeed * player.LadderAcceleration * Time.fixedDeltaTime, ForceMode2D.Force);

                    if (Mathf.Abs(player.rb.velocity.y) > player.LadderSpeed)
                    {
                        player.rb.velocity = new Vector2(player.rb.velocity.x, Mathf.Sign(player.rb.velocity.y) * player.LadderSpeed);
                    }
                }

                if (!player.Raycast(true, player.LadderLayer, Vector2.zero, 0.0f) && player.MoveInput.y > 0.0f)
                {
                    player.rb.velocity = new Vector2(player.rb.velocity.x, player.ColliderSize.y / 2.0f);
                    player.MoveInput = new Vector2(player.MoveInput.x, 0.0f);
                    player.ChangeState(player.idleState);

                    return;
                }

                // Don't want to test the rest.
                return;
            }
            else
            {
                animator.SetFloat("MotionSpeed", 0);
                player.rb.velocity = Vector2.zero;                
            }
            

            if (player.IsCastGrounded(true, player.GroundLayer))
            {
                player.ChangeState(player.idleState);
                return;
            }
        }

        public override void Exit()
        {
            animator.SetInteger("Ladder", 0);
            player.rb.gravityScale = oldGravityScale;
            base.Exit();
        }
    }
}