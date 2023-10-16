using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{ 
    public class LedgeClimbingState : PlayerState
    {        
        private float startTime;

        public LedgeClimbingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.LEDGE;
        }

        public override void Enter()
        {
            //animator.SetTrigger("isIdle");
            player.rb.gravityScale = 0.0f;
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;

            base.Enter();
        }

        public override void Update()
        {
            if (Time.time - startTime > player.LedgeAnimationDuration)
            {
                player.ChangeState(player._idleState);
                return;
            }
        }

        public override void Exit()
        {
            //animator.ResetTrigger("isIdle");
            player.rb.gravityScale = 1.0f;
            base.Exit();
        }
    }
}