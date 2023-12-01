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

        public override void Enter()
        {
            player.animator.SetTrigger("IsHit");
            //startTime = Time.time;            

            //player.rb.velocity = new Vector2(0.0f, player.rb.velocity.y);
            Vector2 normal = Platformer2DUtilities.GetPerpendicularVector2(pushDirection).normalized;
            normal = new Vector2 (MathF.Abs(normal.x), MathF.Abs(normal.y));
            player.rb.velocity = new Vector2(player.rb.velocity.x * normal.x, player.rb.velocity.y * normal.y);
            //Debug.Log($"player.rb.velocity={player.rb.velocity}");
            player.rb.AddForce(pushDirection * throwBackImpulse, ForceMode2D.Impulse);

            SubscribeToAnimatorObserver("Movement");

            base.Enter();
        }

        public override void Update()
        {                 
        }

        public override void Exit()
        {
            UnsubscribeToAnimatorObserver();
            player.animator.ResetTrigger("IsHit");
            base.Exit();
        }

        public override void OnLeaveState() 
        {
            player.ChangeState(player.states[STATE.IDLE]);
        }
    }
}