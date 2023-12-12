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

        public int animatorLayerIndex = 2;
        public string[] airAttackMotionNames;
        public string[] groundAttackMotionNames;
        private bool hasSubscribedToAnimatorObserver;

        // Fail-safe to avoid being stuck when subscribing to the wrong state
        public string[] exitingMotionNames;
        private int nFrames;
        public int frameCountBeforeCanExitByMotionName = 2;

        public override void Enter()
        {
            hasSubscribedToAnimatorObserver = false;
            // If airborne, use air attack animation
            if (isAirborned = !player.IsGrounded())
            {
                player.animator.SetBool("IsAirborne", true);
            }            

            player.animator.SetInteger("AttackIndex", Random.Range(1, 4));
            player.animator.SetTrigger("IsAttacking");
            
            player.inputTriggers["BasicAttack"] = false;
            //player.rb.velocity = Vector2.zero;
            startTime = Time.time;
            player.Attack();

            SubscribeToAnimatorObserver(!isAirborned);

            nFrames = 0;

            base.Enter();
        }

        public override void Update()
        {
            bool grounded = player.IsGroundedSlope();
            
            if (!hasSubscribedToAnimatorObserver)
            {
                SubscribeToAnimatorObserver(grounded);
            }         

            player.animator.SetBool("IsAirborne", !grounded);
            player.animator.SetBool("IsFalling", !grounded);

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

            float speed = player.WalkSpeed;
            float acceleration = player.WalkAcceleration;

            if (grounded)
            {
                speed = player.AirSpeed;
                acceleration = player.AirAcceleration;
            }

            // Add force but limit speed
            if (player.MoveInput.x != 0.0f && player.rb.velocity.magnitude < speed)
            {
                Vector2 v = (player.IsFacingRight ? Vector2.right : -Vector2.right) * /*speed **/ acceleration * Time.fixedDeltaTime;
                player.rb.AddForce(v, ForceMode2D.Force);

                //Debug.Log($"AddForce() player.rb.velocity.magnitude={player.rb.velocity.magnitude} v={v}");
                //player.rb.velocity += v;
                if (player.rb.velocity.magnitude > speed)
                {
                    player.rb.velocity = player.rb.velocity.normalized * speed;
                }
            }
            else
            {
                //player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            }

            // Fail-safe to avoid being stuck when subscribing to the wrong state.
            // Waiting nFrames prevents bailing out beforehand being already on "No Motion"
            // motion.
            if (nFrames >= frameCountBeforeCanExitByMotionName)
            {
                foreach (string motionName in exitingMotionNames)
                {
                    if (player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(motionName))
                    {
                        OnLeaveState();
                        return;
                    }
                }
            }

            nFrames++;
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

        public void SubscribeToAnimatorObserver(bool isGrounded)
        {
            if (isGrounded)
            {
                foreach (string name in groundAttackMotionNames)
                {
                    if (player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(name))
                    {
                        SubscribeToAnimatorObserver(groundAttackAnimatorObserverName);
                        hasSubscribedToAnimatorObserver = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (string name in airAttackMotionNames)
                {
                    if (player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(name))
                    {
                        SubscribeToAnimatorObserver(airAttackAnimatorObserverName);
                        hasSubscribedToAnimatorObserver = true;
                        break;
                    }
                }
            }
        }


    }
}