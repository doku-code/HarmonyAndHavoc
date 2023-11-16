using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "CrouchedAttackState", menuName = "States/CrouchedAttack")]
    public class CrouchedAttackState : PlayerState
    {
        private float startTime;
        private float animationClipLength;
        [SerializeField] private int animatorLayer = 2;
        [SerializeField] private string motionName = "Player_Crouch_Attack";

        private bool resetAnimatorParams;
        [SerializeField] private float clipLengthAdjustment = -0.02f;

        public override void Enter()
        {
            player.animator.SetBool("IsCrouched", true);
            player.animator.SetBool("IsAttacking", true);

            player.inputTriggers["BasicAttack"] = false;
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;

            resetAnimatorParams = true;

            player.Attack();

            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsGrounded())
            {
                player.ChangeState(player.states[STATE.AIRBORNE]);
                return;
            }

            if (!player.inputTriggers["Move"] || player.MoveInput.y >= 0.0f)
            {
                player.ChangeState(player.states[STATE.IDLE]);
                return;
            }

            if (animationClipLength == 0.0f)
            {
                if (player.animator.GetCurrentAnimatorStateInfo(animatorLayer).IsName(motionName))
                {
                    animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
                    Debug.Log($"Testing crouchedattack animationClipLength = {animationClipLength}");
                }
                Debug.Log($"animationClipLength={animationClipLength}");
            }
            else if (Time.time - startTime >= animationClipLength + clipLengthAdjustment)
            {
                Debug.Log($"{Time.time - startTime} >= {animationClipLength + clipLengthAdjustment}");
                resetAnimatorParams = false;
                player.ChangeState(player.states[STATE.CROUCH]);
                return;
            }                        
        }

        public override void Exit()
        {
            player.animator.SetBool("IsAttacking", false);
            if (resetAnimatorParams)
            {                
                player.animator.SetBool("IsCrouched", false);
            }
            base.Exit();
        }
    }
}