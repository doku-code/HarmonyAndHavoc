using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public class PlayerState
    {
        public enum STATE
        {
            IDLE,
            WALK,
            RUN,
            JUMP,
            CROUCH,
            CROUCH_ATTACK,
            WALLGRIP,
            WALLJUMP,
            AIRBORNE,
            LAND,
            DASH,
            LADDER,
            STAIRS_UP,
            STAIRS_DOWN,
            LEDGE,
            BASIC_ATTACK
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
