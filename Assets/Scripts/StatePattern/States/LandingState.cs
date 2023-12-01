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
        
        public override void Enter()
        {
            player.animator.SetBool("IsLanding", true);
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;
            player.rb.gravityScale = 0.0f;

            SubscribeToAnimatorObserver("Movement");

            base.Enter();
        }

        public override void Update()
        {            
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
            player.ChangeState(player.states[STATE.IDLE]);
        }
    }
}