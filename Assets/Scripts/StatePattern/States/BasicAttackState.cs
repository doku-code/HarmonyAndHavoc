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
       
        public override void Enter()
        {
            // If airborne, use air attack animation
            if (isAirborned = !player.IsGrounded())
            {
                player.animator.SetBool("IsAirborne", true);
                SubscribeToAnimatorObserver("AirAttacks");
            }
            else
            {
                SubscribeToAnimatorObserver("GroundAttacks");
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
            Debug.Log("OnLeaveState()");

            if (!player.IsGrounded())
            {
                player.ChangeState(player.states[STATE.AIRBORNE]);                
            }
            else
            {
                player.animator.SetBool("IsAirborne", false);
                player.ChangeState(player.states[STATE.IDLE]);
            }
        }
    }
}