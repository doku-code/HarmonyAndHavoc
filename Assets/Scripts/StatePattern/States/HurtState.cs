using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "HurtState", menuName = "States/Hurt")]
    public class HurtState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private int animatorLayer = 0;

        public override void Enter()
        {
            player.animator.SetTrigger("IsHit");
            startTime = Time.time;
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
            player.animator.ResetTrigger("IsHit");
            base.Exit();
        }
    }
}