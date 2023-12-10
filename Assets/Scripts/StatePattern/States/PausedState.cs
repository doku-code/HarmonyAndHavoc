using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JFM
{
    /* * * * * * * * * * * * * * * * * * * * 
     * 
     * You may have to set the default sprite displayed by the SpriteRenderer to something like an idle frame, 
     * because when having a multiple-layered animator, it can sometimes display this default sprite between two animations.
     *       
     * * * * * * * * * * * * * * * * * * * */
    [CreateAssetMenu(fileName = "PausedState", menuName = "States/Pause")]
    public class PausedState : PlayerState
    {
        [NonSerialized] public int waitNFrames;
        //private bool startingOnLadder;               

        public override void Enter()
        {           
            player.animator.SetBool("IsIdle", true);

            player.inputTriggers["Inventory"] = false;
            player.MoveInput = Vector2.zero;

            player.rb.velocity = Vector2.zero;
            
            if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
            )
            {
                //Debug.Log("Ladders on idle.Enter().");
                player.rb.velocity = Vector2.zero;
                player.rb.totalForce = Vector2.zero;
                player.rb.gravityScale = 0.0f;
                //startingOnLadder = true;
            }
            else
            {
                //startingOnLadder = false;
            }
            
            //player.rb.isKinematic = false;

            base.Enter();
        }

        public override void Update()
        {
            if (waitNFrames > 0)
            {
                waitNFrames--;
                return;
            }
            //Debug.Log($"player.inputTriggers[\"Inventory\"]={player.inputTriggers["Inventory"]}");
            if (player.inputTriggers["Inventory"])
            {
                player.inputTriggers["Inventory"] = false;
                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                return;
            }

            player.rb.velocity = Vector2.zero;
            player.rb.totalForce = Vector2.zero;
        }

        public override void Exit()
        {
            player.rb.gravityScale = player.DefaultGravityScale;
            
            player.animator.SetBool("IsIdle", false);
            base.Exit();
        }

        public override void OnLeaveState() { }
    }
}