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
        private float animationClipLength;
        [SerializeField] private int animatorLayer = 0;

        public override void Enter()
        {
            player.animator.SetBool("IsLanding", true);
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;
            player.rb.gravityScale = 0.0f;
            if(animationClipLength == 0.0f)
            {
                animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;                       
            }

            base.Enter();
        }

        public override void Update()
        {
            //Debug.Break();
            if (Time.time - startTime >= animationClipLength)
            {
                player.ChangeState(player.states[STATE.IDLE]);
                return;
            }            
        }

        public override void Exit()
        {
            player.rb.gravityScale = player.DefaultGravityScale;
            player.animator.SetBool("IsLanding", false);
            base.Exit();
        }
    }
}