using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "CrouchedAttackState", menuName = "States/CrouchedAttack")]
    public class CrouchedAttackState : PlayerState
    {
        private float startTime;
        /*private float animationClipLength;
        [SerializeField] private int animatorLayer = 2;
        [SerializeField] private string motionName = "Player_Crouch_Attack";
        */
        private bool resetAnimatorParams;
        [SerializeField] private float clipLengthAdjustment = -0.02f;
        [SerializeField] private string animatorObserverName = "Attacks";
        public override void Enter()
        {
            player.animator.SetBool("IsCrouched", true);
            player.animator.SetTrigger("IsAttacking");

            player.inputTriggers["BasicAttack"] = false;
            player.rb.velocity = Vector2.zero;
            startTime = Time.time;

            resetAnimatorParams = true;

            player.Attack();

            SubscribeToAnimatorObserver(animatorObserverName);

            base.Enter();
        }

        public override void Update()
        {
            if (!player.IsGrounded())
            {
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
                return;
            }

            if (!player.inputTriggers["Move"] || player.MoveInput.y >= 0.0f)
            {
                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                return;
            }                                           
        }

        public override void Exit()
        {
            UnsubscribeToAnimatorObserver();
            player.animator.ResetTrigger("IsAttacking");
            if (resetAnimatorParams)
            {                
                player.animator.SetBool("IsCrouched", false);
            }
            base.Exit();
        }

        public override void OnLeaveState() 
        {
            resetAnimatorParams = false;
            player.StateMachine.ChangeState(player.States[STATE.CROUCH]);            
        }
    }
}