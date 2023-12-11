using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "LandingState", menuName = "States/Landing")]
    public class LandingState : PlayerState
    {
        private float startTime;
        public int animatorLayerIndex = 0;
        public string motionName = "Player_landing";
        private bool hasSubscribedToAnimatorObserver;
        public string animatorObserverName = "Movement";

        public string[] exitingMotionNames;
            
        public override void Enter()
        {
            player.animator.SetBool("IsLanding", true);
            
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
            
            startTime = Time.time;
            player.rb.gravityScale = 0.0f;

            player.Land(true);

            hasSubscribedToAnimatorObserver = false;
            if(player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(motionName))
            {
                SubscribeToAnimatorObserver(animatorObserverName);
                hasSubscribedToAnimatorObserver = true;
            }

            base.Enter();
        }

        public override void Update()
        {
            // It seems while in landing Enter() method execution the animator might still be in 
            // 'Player_Fall' motion, so subscribing at that time is wrong and we have to wait
            // to do so.
            if (!hasSubscribedToAnimatorObserver 
                && player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(motionName))
            {
                SubscribeToAnimatorObserver(animatorObserverName);
                hasSubscribedToAnimatorObserver = true;                
            }

            foreach (string motionName in exitingMotionNames)
            {
                if (player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(motionName))
                {
                    OnLeaveState();
                    break;
                }
            }
        }

        public override void Exit()
        {            
            UnsubscribeToAnimatorObserver();
            player.rb.gravityScale = player.DefaultGravityScale;
            player.animator.SetBool("IsLanding", false);
            base.Exit();
        }

        public override void OnLeaveState() 
        {
            player.StateMachine.ChangeState(player.States[STATE.IDLE]);
        }
    }
}