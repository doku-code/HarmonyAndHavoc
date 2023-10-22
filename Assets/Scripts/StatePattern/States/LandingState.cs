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
            
            if(animationClipLength == 0.0f)
            {
                animationClipLength = animator.GetCurrentAnimatorStateInfo(animatorLayer).length;                       
            }

            base.Enter();
        }

        public override void Update()
        {
            if (Time.time - startTime >= animationClipLength)
            {
                player.ChangeState(player._idleState);
                return;
            }            
        }

        public override void Exit()
        {
            animator.SetBool("IsLanding", false);
            base.Exit();
        }
    }
}