using AF;
using JFM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "WallSlideKnowledge", menuName = "Knowledges/WallSlide")]
public class WallSlideKnowledge : Knowledge
{
    [SerializeField] private float wallGripVelocityTolerance = 0.5f;
    [SerializeField] private float wallGripForce = 1000.0f;

    public override void Activate() { }
    public override void Deactivate() { }

    public override void Enter() {        
        player.animator.SetBool("IsWallSliding", true);
        if ((player.IsFacingRight && player.MoveInput.x > 0) || (!player.IsFacingRight && player.MoveInput.x < 0))
        {
            player.Turn();
        }
        player.inputTriggers["Jump"] = false;

        Debug.Log($"player.IsFacingRight={player.IsFacingRight}");
    }

    public override void Update()
    {
        player.SetHighestAirborneY(true);

        //Debug.Log($"{player.IsGrounded()} && {player.rb.velocity.y >= 0.0f} player.rb.velocity.y={player.rb.velocity.y}");
        if (player.IsGrounded() && player.rb.velocity.y >= 0.0f)
        {
            IdleState state = (IdleState)player.states[PlayerState.STATE.IDLE];
            state.waitNFrames = 3;
            player.ChangeState(state);
            return;
        }

        if (!IsSlidingOnBackWall())
        {
            player.ChangeState(player.states[PlayerState.STATE.AIRBORNE]);
            return;
        }

        if (player.WillJump())
        {
            player.ChangeState(player.states[PlayerState.STATE.WALLJUMP]);
            return;
        }

        if (!player.inputTriggers["Move"] || !((player.MoveInput.x > 0 && !player.IsFacingRight) || (player.MoveInput.x < 0 && player.IsFacingRight)))
        {
            player.ChangeState(player.states[PlayerState.STATE.AIRBORNE]);
            return;
        }

        player.rb.AddForce(Vector2.up * -player.rb.velocity.y * wallGripForce * Time.fixedDeltaTime, ForceMode2D.Force);
        //player.ResetJump();
    }

    public override void Exit()
    {
        player.animator.SetBool("IsWallSliding", false);        
    }

    public override bool WillUseKnowledge()
    {
        bool front = player.FrontWall is not null && (1 << player.FrontWall.layer) == (int)player.GroundLayer && ((player.IsFacingRight && player.MoveInput.x > 0) || (!player.IsFacingRight && player.MoveInput.x < 0));
        bool text = false;
        if (player.FrontWall is not null)
        {
            text = (1 << player.FrontWall.layer) == (int)player.GroundLayer;
        }
        //Debug.Log($"front = {player.FrontWall is not null} && ({text} && (({player.IsFacingRight && player.MoveInput.x > 0}) || ({!player.IsFacingRight && player.MoveInput.x < 0}))");
        bool back = false;

        if (!front)
        {
            // Check also back wall 
            bool backWallHit = player.Raycast(false, player.GroundLayer, Vector2.zero, player.WallDistance, player.IsFacingRight ? -Vector2.right : Vector2.right);
            back = backWallHit && ((player.IsFacingRight && player.MoveInput.x < 0) || (!player.IsFacingRight && player.MoveInput.x > 0));
        }

        bool availableKnowledge = player.Data.KnownKnowledgeDictionary[KnowledgeID.WALL_SLIDE] && player.Data.AvalaibleKnowledgeDictionary[KnowledgeID.WALL_SLIDE] != AvalaibleKnowledgePosition.NOT_AVALAIBLE;        

        //Debug.Log($"player.IsFacingRight={player.IsFacingRight} availableKnowledge ={availableKnowledge} ({front} || {back}) && {Mathf.Abs(player.rb.velocity.x) <= wallGripVelocityTolerance} rb.velocity.x={player.rb.velocity.x} moveInput.x={player.MoveInput.x}");
        return availableKnowledge && (front || back) && Mathf.Abs(player.rb.velocity.x) <= wallGripVelocityTolerance;
    }

    // Checks back wall
    private bool IsSlidingOnBackWall()
    {
        Vector2 v = player.IsFacingRight ? -Vector2.right : Vector2.right;
        
        //RaycastHit2D hit = Physics2D.CircleCast(new Vector2(player.transform.position.x, player.transform.position.y) + new Vector2(0, player.ColliderSize.y / 2.0f) + v * 0.5f, 0.4f, v, player.WallDistance, player.GroundLayer);
        RaycastHit2D hit = Physics2D.BoxCast(player.rb.position + player.ColliderSize / 2.0f + v * player.WallDistance, player.ColliderSize, 0.0f, v, 0.0f, player.GroundLayer);
        if(hit.collider is null)
        {
            RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y) + new Vector2(0, player.ColliderSize.y / 2.0f), player.IsFacingRight ? -Vector2.right : Vector2.right, player.WallDistance, player.GroundLayer);
            hit = hit2;
        }
        return hit.collider is not null && ((!player.IsFacingRight && player.MoveInput.x > 0) || (player.IsFacingRight && player.MoveInput.x < 0)) && Mathf.Abs(player.rb.velocity.x) <= 0.5f;
    }
}
