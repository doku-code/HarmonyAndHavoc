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
        //private float startTime;
        [SerializeField] private float throwBackImpulse = 0.5f;
        [NonSerialized] public Vector2 pushDirection;
        public int animatorLayerIndex = 0;
        public string motionName = "Player_Get_Hit";
        public string animatorObserverName = "Movement";
        private bool hasSubscribedToAnimatorObserver;

        public override void Enter()
        {
            player.animator.SetTrigger("IsHit");
            //startTime = Time.time;            

            //player.rb.velocity = new Vector2(0.0f, player.rb.velocity.y);
            Vector2 normal = Platformer2DUtilities.GetPerpendicularVector2(pushDirection).normalized;
            normal = new Vector2 (MathF.Abs(normal.x), MathF.Abs(normal.y));
            player.rb.velocity = new Vector2(player.rb.velocity.x * normal.x, player.rb.velocity.y * normal.y);
            //Debug.Log($"player.rb.velocity={player.rb.velocity} normal={normal}");
            player.rb.AddForce(pushDirection * throwBackImpulse, ForceMode2D.Impulse);

            hasSubscribedToAnimatorObserver = false;
            
            base.Enter();
        }

        public override void Update()
        {
            // It seems while in hurting Enter() method execution the animator might still be in 
            // the previous motion, so subscribing at that time is wrong and we have to wait
            // to do so.
            if (!hasSubscribedToAnimatorObserver
                && player.animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName(motionName))
            {
                SubscribeToAnimatorObserver(animatorObserverName);
                hasSubscribedToAnimatorObserver = true;
            }
        }

        public override void Exit()
        {
            UnsubscribeToAnimatorObserver();
            player.animator.ResetTrigger("IsHit");
            base.Exit();
        }

        public override void OnLeaveState() 
        {            
            bool grounded = player.IsGroundedSlope();

            if (grounded)
            {
                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
            }
            else
            {
                player.StateMachine.ChangeState(player.States[STATE.AIRBORNE]);
            }
        }
    }
}