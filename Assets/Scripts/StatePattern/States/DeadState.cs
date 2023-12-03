using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "DeadState", menuName = "States/Dead")]
    public class DeadState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private int animatorLayer = 0;

        public override void Enter()
        {
            player.animator.SetTrigger("IsDead");
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
                //player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                return;
            }            
        }

        public override void Exit()
        {
            Debug.Log("Exiting Dead state.");
            player.animator.ResetTrigger("IsDead");
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}