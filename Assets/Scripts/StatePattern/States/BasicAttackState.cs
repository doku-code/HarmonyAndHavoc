using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "BasicAttackState", menuName = "States/BasicAttack")]
    public class BasicAttackState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private bool usingAirAnimation;
        [SerializeField] private int animatorLayer = 2;
        [SerializeField] private string motionName = "Player_Attack_1";
        [SerializeField] private string airMotionName = "Player_Air_Attack_1";

        public override void Enter()
        {
            // If airborne, use air attack animation
            if (usingAirAnimation = !player.IsGrounded())
            {
                player.animator.SetBool("IsAirborne", true);
            }
            
            player.animator.SetBool("IsAttacking", true);            

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

            if (animationClipLength == 0.0f)
            {
                if (player.animator.GetCurrentAnimatorStateInfo(animatorLayer).IsName(motionName))
                {
                    animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
                    //Debug.Log($"Testing crouchedattack animationClipLength = {animationClipLength}");
                }
            }
            else if (Time.time - startTime >= animationClipLength)
            {
                if (!player.IsGrounded())
                {
                    player.ChangeState(player.states[STATE.AIRBORNE]);                 
                }
                else
                {
                    player.ChangeState(player.states[STATE.IDLE]);
                }
                return;
            }
        }

        public override void Exit()
        {
            player.rb.gravityScale = player.DefaultGravityScale;
            player.animator.SetBool("IsAttacking", false);
            if(usingAirAnimation)
            {
                player.animator.SetBool("IsAirborne", false);
            }
            base.Exit();
        }
    }
}