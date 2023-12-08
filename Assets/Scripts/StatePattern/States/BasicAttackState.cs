using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "BasicAttackState", menuName = "States/BasicAttack")]
    public class BasicAttackState : PlayerState
    {
        private float startTime;
        private bool isAirborned;
        public string airAttackAnimatorObserverName = "AirAttacks";
        public string groundAttackAnimatorObserverName = "GroundAttacks";

        public override void Enter()
        {
            // If airborne, use air attack animation
            if (isAirborned = !player.IsGrounded())
            {
                player.animator.SetBool("IsAirborne", true);
                SubscribeToAnimatorObserver(airAttackAnimatorObserverName);
            }
            else
            {
                SubscribeToAnimatorObserver(groundAttackAnimatorObserverName);
            }

            player.animator.SetInteger("AttackIndex", Random.Range(1, 4));
            player.animator.SetTrigger("IsAttacking");
            
            player.inputTriggers["BasicAttack"] = false;
            //player.rb.velocity = Vector2.zero;
            startTime = Time.time;
            player.Attack();
            
            base.Enter();
        }

        public override void Update()
        {
            if (
                !player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
            )
            {                
                player.rb.gravityScale = 0.0f;
                player.rb.velocity = Vector2.zero;
                player.rb.totalForce = Vector2.zero;
                //Debug.Log("walking on ladder");
            }

            // Add force but limit speed
            if (player.MoveInput.x != 0.0f && player.rb.velocity.magnitude < player.WalkSpeed)
            {
                Vector2 v = (player.IsFacingRight ? Vector2.right : -Vector2.right) * /*player.WalkSpeed **/ player.WalkAcceleration * Time.fixedDeltaTime;
                player.rb.AddForce(v, ForceMode2D.Force);

                //Debug.Log($"AddForce() player.rb.velocity.magnitude={player.rb.velocity.magnitude} v={v}");
                //player.rb.velocity += v;
                if (player.rb.velocity.magnitude > player.WalkSpeed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * player.WalkSpeed;
                }
            }
            else
            {
                player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            }
        }

        public override void Exit()
        {
            player.rb.gravityScale = player.DefaultGravityScale;
            player.animator.ResetTrigger("IsAttacking");
            if (isAirborned)
            {
                //player.animator.SetBool("IsAirborne", false);                
            }

            UnsubscribeToAnimatorObserver();
            base.Exit();
        }

        public override void OnLeaveState()
        {
            //Debug.Log("OnLeaveState()");

            if (!player.IsGrounded())
            {
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);                
            }
            else
            {
                player.animator.SetBool("IsAirborne", false);
                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
            }
        }
    }
}