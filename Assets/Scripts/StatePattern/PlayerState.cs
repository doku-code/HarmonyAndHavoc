using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace JFM
{
    public class PlayerState
    {
        public enum STATE
        {
            IDLE,
            WALK,
            RUN,
            JUMP,
            WALLGRIP,
            WALLJUMP,
            AIRBORNE,
            DASH,
            LADDER,
            LEDGE,
            NORMAL_ATTACK
        };

        public enum EVENT
        {
            ENTER,
            UPDATE,
            EXIT
        };

        public STATE name;
        protected EVENT stage;
        protected Animator animator;
        protected PlayerController player;
        protected PlayerState nextState;

        public PlayerState(Animator animator, PlayerController player)
        {
            this.animator = animator;
            this.player = player;
            stage = EVENT.ENTER;
        }

        public virtual void Enter()
        {
            Debug.Log(name);
            stage = EVENT.UPDATE;
        }

        public virtual void Update() { stage = EVENT.UPDATE; }
        public virtual void Exit() { stage = EVENT.EXIT; }

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
    }
}
