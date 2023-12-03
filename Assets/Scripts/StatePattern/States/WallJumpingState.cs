using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace JFM
{
    /* * * * * * * * * * * * * * * 
     * 
     * This state is meant to prevent the player to go in opposite direction 
     * as his input will be in direction of the wall that initiated the walljump.
     * 
     * * * * * * * * * * * * * * */
    [CreateAssetMenu(fileName = "WallJumpingState", menuName = "States/WallJumping")]
    public class WallJumpingState : PlayerState
    {
        private float startTime;
        // In degrees
        [SerializeField] private float wallJumpAngle = 45.0f;
        [SerializeField] private float wallJumpDuration = 1.0f;
        [SerializeField] private float wallJumpForce = 1.0f;
        
        public override void Enter()
        {
            player.animator.SetTrigger("Dash");
            Jump();
            player.inputTriggers["Jump"] = false;
            startTime = Time.time;

            base.Enter();
        }

        public override void Update()
        {

            if (player.IsGrounded())
            {
                if (player.inputTriggers["Move"] && player.MoveInput.x != 0.0f && player.MoveInput.y == 0.0f)
                {
                    WalkingState state = (WalkingState)player.States[STATE.WALK];
                    if (!player.Raycast(false, player.LadderLayer, Vector2.up * 0.4f, 0.01f, Vector2.up) && //, false, true) &&
                    player.Raycast(false, player.LadderLayer, Vector2.zero, 0.3f, Vector2.down)
                        )
                    {
                        //resetGravityScale = false;                        
                        state.newGravityScale = 0.0f;
                        state.resetGravityScaleWithOther = true;
                        state.otherGravityScale = player.DefaultGravityScale;
                        player.rb.gravityScale = 0.0f;
                        //Debug.Log("walking on ladder");
                    }
                    else
                    {
                        //Debug.Log("not walking on ladder");
                    }
                    player.StateMachine.ChangeState(state);
                    return;
                }

                player.StateMachine.ChangeState(player.States[STATE.IDLE]);
                return;
            }

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.WALL_SLIDE).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.WALL_SLIDE);
                return;
            }

            if (player.WillJump())
            {
                player.StateMachine.ChangeState(player.States[STATE.JUMP]);
                return;
            }

            if(Time.time - startTime > wallJumpDuration)
            {
                AirborneState state = (AirborneState)player.States[STATE.AIRBORNE];
                state.wasGrounded = false;
                player.StateMachine.ChangeState(state);
                return;
            }
        }

        public override void Exit()
        {
            player.animator.ResetTrigger("Dash");
            base.Exit();
        }

        private void Jump()
        {            
            float angle = wallJumpAngle * Mathf.Deg2Rad;            
            Vector3 v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * wallJumpForce * 1.0f;
            //Debug.Log($"JUMP from wall {player.IsFacingRight}  v={v}  cos(angle)={Mathf.Cos(angle)}");
            player.rb.AddForce(v, ForceMode2D.Impulse);
        }

        public override void OnLeaveState() { }
    }
}