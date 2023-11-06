using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    /* * * * * * * * * * * * * * * 
     * 
     * This state is meant to prevent the player to go in opposite direction 
     * as his input will be in direction of the wall that initiated the walljump.
     * 
     * * * * * * * * * * * * * * */
    public class WallJumpingState : PlayerState
    {
        private float startTime;
        
        public WallJumpingState(Animator animator, PlayerController player)
            : base(animator, player)
        {
            name = STATE.WALLJUMP;
        }

        public override void Enter()
        {
            animator.SetTrigger("Dash");
            player.Jump();
            player.inputTriggers["Jump"] = false;
            startTime = Time.time;

            base.Enter();
        }

        public override void Update()
        {

            if (player.IsCastGrounded(false))
            {
                if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f && player.MoveInput.y == 0.0f)
                {
                    if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                    player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
                        )
                    {
                        //resetGravityScale = false;
                        player.walkingState.newGravityScale = 0.0f;
                        player.walkingState.resetGravityScaleWithOther = true;
                        player.walkingState.otherGravityScale = player.DefaultGravityScale;
                        player.rb.gravityScale = 0.0f;
                        Debug.Log("walking on ladder");
                    }
                    else
                    {
                        Debug.Log("not walking on ladder");
                    }
                    player.ChangeState(player.walkingState);
                    return;
                }

                player.ChangeState(player.idleState);
                return;
            }

            

            if (player.WillGripToWall())// && player.inputTriggers["Move"])
            {
                player.ChangeState(player.wallGrippingState);
                return;
            }                  

            if (player.WillJump())
            {
                player.ChangeState(player.jumpingState);
                return;
            }

            if(Time.time - startTime > player.WallJumpDuration)
            {
                player.ChangeState(player.airborneState);
                return;
            }
        }

        public override void Exit()
        {
            animator.ResetTrigger("Dash");
            base.Exit();
        }
    }
}