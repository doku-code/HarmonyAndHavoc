using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class CrouchedAttackState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        private int animatorLayer = 2;
        private string motionName = "Player_Crouch_Attack";

        public CrouchedAttackState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.CROUCH_ATTACK;
        }

        public override void Enter()
        {
            animator.SetBool("IsCrouched", true);
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

            if (!player.inputTriggers["Move"] || player.MoveInput.y >= 0.0f)
            {
                player.ChangeState(player._idleState);
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
                player.ChangeState(player._crouchedState);
                return;
            }                        
        }

        public override void Exit()
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsCrouched", false);
            base.Exit();
        }
    }
}