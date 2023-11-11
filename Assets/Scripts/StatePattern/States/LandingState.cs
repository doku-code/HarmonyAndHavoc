using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace JFM
{ 
    public class LandingState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private int animatorLayer = 0;

        public LandingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.LAND;
        }

        public override void Enter()
        {
            animator.SetBool("IsLanding", true);
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;
            player.rb.gravityScale = 0.0f;
            if(animationClipLength == 0.0f)
            {
                animationClipLength = animator.GetCurrentAnimatorStateInfo(animatorLayer).length;                       
            }

            base.Enter();
        }

        public override void Update()
        {
            //Debug.Break();
            if (Time.time - startTime >= animationClipLength)
            {
                player.ChangeState(player.idleState);
                return;
            }            
        }

        public override void Exit()
        {
            player.rb.gravityScale = player.DefaultGravityScale;
            animator.SetBool("IsLanding", false);
            base.Exit();
        }
    }
}