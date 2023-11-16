using System;
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
        [SerializeField] private int animatorLayer = 0;
        [SerializeField] private string motionName = "Player_Get_Hit";
        [SerializeField] private float clipLengthAdjustment = -0.02f;
        [SerializeField] private float throwBackImpulse = 0.5f;
        [NonSerialized] public Vector2 pushDirection;

        public override void Enter()
        {
            player.animator.SetTrigger("IsHit");
            startTime = Time.time;
            /*if(animationClipLength == 0.0f)
            {
                animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;                       
            }*/

            player.rb.AddForce(pushDirection * throwBackImpulse, ForceMode2D.Impulse);

            base.Enter();
        }

        public override void Update()
        {
            if (animationClipLength == 0.0f)
            {
                if (player.animator.GetCurrentAnimatorStateInfo(animatorLayer).IsName(motionName))
                {
                    animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
                    //Debug.Log($"Testing IsHit animationClipLength = {animationClipLength}");
                }
            }
            else if(Time.time - startTime >= animationClipLength + clipLengthAdjustment)
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