using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class BasicAttackState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private int animatorLayer = 2;
        private string motionName = "Player_Attack_1";

        public BasicAttackState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.BASIC_ATTACK;
        }

        public override void Enter()
        {
            animator.SetBool("IsAttacking", true);

            player.inputTriggers["BasicAttack"] = false;
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;

            //         lastKnowledge = player.Data.Knowledges.find_if()

            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsCastGrounded())
            {
                player.ChangeState(player._airborneState);
                return;
            }

            if (animationClipLength == 0.0f)
            {
                if (animator.GetCurrentAnimatorStateInfo(animatorLayer).IsName(motionName))
                {
                    animationClipLength = animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
                    Debug.Log($"Testing crouchedattack animationClipLength = {animationClipLength}");
                }
            }
            else if (Time.time - startTime >= animationClipLength)
            {
                player.ChangeState(player._idleState);
                return;
            }
        }

        public override void Exit()
        {
            animator.SetBool("IsAttacking", false);
            base.Exit();
        }
    }
}