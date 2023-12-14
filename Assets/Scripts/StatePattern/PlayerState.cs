using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using AF;

namespace JFM
{
    /* * * * * * * * * * * * * * * 
     * 
     * To avoid stucking on the tilemapped floor, do this:
     *   For the TilemapCollider2D check the box "Used by Composite".
     *   Then add the Component CompositeCollider2D (Found under Physics2D). This will automatically add a Rigidbody2D to your object if it doesn't have one already.
     *   Change the "Body Type" of the Rigidbody2D to Kinematic unless you want physical interaction with the tilemap.
     * 
     * * * * * * * * * * * * * * */
    public abstract class PlayerState : ScriptableObject
    {
        public enum STATE
        {
            IDLE,
            WALK,
            //RUN,
            JUMP,
            PAUSE,
            CROUCH,
            CROUCH_ATTACK,
            WALLJUMP,
            AIRBORNE,
            LAND,
            KNOWLEDGE,
            LADDER,
            STAIRS_UP,
            STAIRS_DOWN,            
            BASIC_ATTACK,
            HURT,
            DEAD,
            VANISH
        };

        public enum EVENT
        {
            ENTER,
            UPDATE,
            EXIT
        };

        public new STATE name;
        protected EVENT stage;        
        protected PlayerController player;
        protected PlayerState nextState;
        private string _animatorObserverName;

        public PlayerState GetNextState()
        {
            return nextState;
        }

        public void Initialize(PlayerController player)
        {
            this.player = player;
            stage = EVENT.ENTER;
        }

        public virtual void Enter()
        {            
            if (name == STATE.KNOWLEDGE)
            {
                KnowledgeState state = (KnowledgeState)this;
                Debug.Log($"{name} : {state.knowledge.ID}");
            }
            else
            {
                Debug.Log(name);
            }
            stage = EVENT.UPDATE;
        }

        public virtual void Update() { stage = EVENT.UPDATE; }
        public virtual void Exit() 
        {            
            stage = EVENT.EXIT; 
        }

        public PlayerState Process()
        {
            if (stage == EVENT.ENTER) Enter();
            if (stage == EVENT.UPDATE) Update();
            if (stage == EVENT.EXIT)
            {
                Exit();
                return nextState;
            }
            return this;
        }

        public void SetNextState(PlayerState nextState)
        {
            this.nextState = nextState;
            stage = EVENT.EXIT;
            nextState.stage = EVENT.ENTER;
        }

        public abstract void OnLeaveState();

        public void SubscribeToAnimatorObserver(string observerName)
        {
            _animatorObserverName = observerName;

            AnimatorObserver[] behaviors = player.animator.GetBehaviours<AnimatorObserver>();
            
            foreach (AnimatorObserver behavior in behaviors)
            {
                if (behavior is not null && behavior.name == observerName)
                {
                    //Debug.Log($"Behavior found.");
                    behavior.onLeaveState += OnLeaveState;
                }
                else
                {
                    //Debug.Log($"Behavior NOT found.");
                }
            }
        }

        public void UnsubscribeToAnimatorObserver()
        {
            AnimatorObserver[] behaviors = player.animator.GetBehaviours<AnimatorObserver>();
            foreach (AnimatorObserver behavior in behaviors)
            {
                if (behavior is not null && behavior.name == _animatorObserverName)
                {
                    behavior.onLeaveState -= OnLeaveState;
                }
            }
        }
    }
}
