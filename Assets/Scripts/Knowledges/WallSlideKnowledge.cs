using AF;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "WallSlideKnowledge", menuName = "Knowledges/WallSlide")]
    public class WallSlideKnowledge : Knowledge
    {
        [SerializeField] private float wallSlideVelocityTolerance = 0.02f;
        [SerializeField] private float wallSlideForce = 1000.0f;
        [SerializeField] private float wallSlideMinHeight = 0.5f;

        public override void Activate() { }
        public override void Deactivate() { }

        public override void Enter()
        {
            player.animator.SetBool("IsWallSliding", true);
            if ((player.IsFacingRight && player.MoveInput.x > 0) || (!player.IsFacingRight && player.MoveInput.x < 0))
            {
                player.Turn();
            }
            player.inputTriggers["Jump"] = false;

            //Debug.Log($"player.IsFacingRight={player.IsFacingRight}");
        }

        public override void Update()
        {
            player.SetHighestAirborneY(true);

            //Debug.Log($"{player.IsGrounded()} && {player.rb.velocity.y >= 0.0f} player.rb.velocity.y={player.rb.velocity.y}");
            if (player.IsGrounded() && player.rb.velocity.y >= 0.0f)
            {
                IdleState state = (IdleState)player.States[PlayerState.STATE.IDLE];
                state.waitNFrames = 3;
                player.StateMachine.ChangeState(state);
                return;
            }

            if (!IsSlidingOnBackWall())
            {
                player.StateMachine.ChangeState(player.States[PlayerState.STATE.AIRBORNE]);
                return;
            }

            if (player.inputTriggers["Jump"])
            {
                player.StateMachine.ChangeState(player.States[PlayerState.STATE.WALLJUMP]);
                return;
            }

            if (!player.inputTriggers["Move"] || !((player.MoveInput.x > 0 && !player.IsFacingRight) || (player.MoveInput.x < 0 && player.IsFacingRight)))
            {
                player.StateMachine.ChangeState(player.States[PlayerState.STATE.AIRBORNE]);
                return;
            }

            player.rb.AddForce(Vector2.up * -player.rb.velocity.y * wallSlideForce * Time.fixedDeltaTime, ForceMode2D.Force);
            //player.ResetJump();
        }

        public override void Exit()
        {
            player.animator.SetBool("IsWallSliding", false);
        }

        public override bool WillUse()
        {
            //GameObject frontWall = IsInFrontOfWall();
            //bool front = frontWall is not null && (1 << frontWall.layer) == (int)player.GroundLayer;// && ((player.IsFacingRight && player.MoveInput.x > 0) || (!player.IsFacingRight && player.MoveInput.x < 0));
            bool front = /*front || */(player.IsWallColliding && player.WallIsToRight == player.IsFacingRight) && ((player.IsFacingRight && player.MoveInput.x > 0) || (!player.IsFacingRight && player.MoveInput.x < 0));

            bool text = false;
            /*if (frontWall is not null)
            {
                text = (1 << frontWall.layer) == (int)player.GroundLayer;
            }*/
            //Debug.Log($"front = {frontWall is not null} && ({text} && (({player.IsFacingRight && player.MoveInput.x > 0}) || ({!player.IsFacingRight && player.MoveInput.x < 0}))");
            //Debug.Log($"front = {player.IsWallColliding} && (({player.IsFacingRight && player.MoveInput.x > 0}) || ({!player.IsFacingRight && player.MoveInput.x < 0}))");

            // To avoid wallSliding when too close to the ground.
            bool groundHit = player.Raycast(false, player.GroundLayer | player.LadderLayer, Vector2.zero, wallSlideMinHeight, Vector2.down);

            bool back = false;

            if (!front)
            {
                // Check also back wall 
                //bool backWallHit = player.Raycast(false, player.GroundLayer, Vector2.zero, player.WallDistance, player.IsFacingRight ? -Vector2.right : Vector2.right);
                back = /*backWallHit || */(player.IsWallColliding && player.WallIsToRight != player.IsFacingRight) && ((player.IsFacingRight && player.MoveInput.x < 0) || (!player.IsFacingRight && player.MoveInput.x > 0));
            }

            bool availableKnowledge = player.Data.KnownKnowledgeDictionary[KnowledgeID.WALL_SLIDE] && player.Data.AvailableKnowledgeDictionary[KnowledgeID.WALL_SLIDE] != AvailableKnowledgePosition.NOT_AVAILABLE;

            //Debug.Log($"player.IsFacingRight={player.IsFacingRight} availableKnowledge ={availableKnowledge} ({front} || {back}) && {Mathf.Abs(player.rb.velocity.x) <= wallGripVelocityTolerance} rb.velocity.x={player.rb.velocity.x} moveInput.x={player.MoveInput.x}");
            return !groundHit && availableKnowledge && (front || back) && Mathf.Abs(player.rb.velocity.x) <= wallSlideVelocityTolerance;
        }

        private GameObject IsInFrontOfWall()
        {
            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;
            RaycastHit2D hit1 = Physics2D.Raycast(player.rb.position + new Vector2(0, player.ColliderSize.y / 2.0f), v, player.WallDistance, player.GroundLayer);

            if (hit1.collider is null)
            {
                RaycastHit2D hit2 = Physics2D.BoxCast(player.rb.position + Vector2.up * player.ColliderSize.y / 2.0f + v * player.WallDistance, player.ColliderSize, 0.0f, v, 0.0f, player.GroundLayer);
                if (hit2.collider is null)
                {
                    return null;
                }

                return hit2.transform.gameObject;
            }

            return hit1.transform.gameObject;
        }


        // Checks back wall
        private bool IsSlidingOnBackWall()
        {
            Vector2 v = player.IsFacingRight ? -Vector2.right : Vector2.right;

            //RaycastHit2D hit = Physics2D.CircleCast(new Vector2(player.transform.position.x, player.transform.position.y) + new Vector2(0, player.ColliderSize.y / 2.0f) + v * 0.5f, 0.4f, v, player.WallDistance, player.GroundLayer);
            RaycastHit2D hit = Physics2D.BoxCast(player.rb.position + Vector2.up * player.ColliderSize.y / 2.0f + v * player.WallDistance, player.ColliderSize, 0.0f, v, 0.0f, player.GroundLayer);
            if (hit.collider is null)
            {
                RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y) + new Vector2(0, player.ColliderSize.y / 2.0f), player.IsFacingRight ? -Vector2.right : Vector2.right, player.WallDistance, player.GroundLayer);
                hit = hit2;
            }
            //return hit.collider is not null && ((!player.IsFacingRight && player.MoveInput.x > 0) || (player.IsFacingRight && player.MoveInput.x < 0)) && Mathf.Abs(player.rb.velocity.x) <= 0.5f;
            return hit.collider is not null || (player.IsWallColliding && player.WallIsToRight != player.IsFacingRight) && ((!player.IsFacingRight && player.MoveInput.x > 0) || (player.IsFacingRight && player.MoveInput.x < 0)) && Mathf.Abs(player.rb.velocity.x) <= 0.5f;
        }

        public override void OnLeave()
        {
        }
    }
}