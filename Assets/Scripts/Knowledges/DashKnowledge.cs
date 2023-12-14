using AF;
using UnityEngine;

namespace JFM
{
    /* * * * * * * * * * * * * * * 
    * 
    * This state is meant to prevent the player to go in opposite direction 
    * as his input will be in direction of the wall that initiated the walljump.
    * 
    * IMPORTANT NOTICE: Use 'Continuous' Collision Detection setting in Rigidbody2D, else
    * it will have a chance of passing through some walls.
    * 
    * * * * * * * * * * * * * * */
    [CreateAssetMenu(fileName = "DashKnowledge", menuName = "Knowledges/Dash")]
    public class DashKnowledge : Knowledge
    {
        private int nFrames;

        private bool hasDashed;
        private Vector2 dashDirection;
        [SerializeField] private float groundDashForce = 60.0f;
        [SerializeField] private float groundDashDeceleration = 1000.0f;
        [SerializeField] private float groundDashAcceleration = 500.0f;
        [SerializeField] private float minVelocityThreshold = 0.05f;

        // In degrees
        private float groundDashAngle = 0.0f;

        [SerializeField] private string animatorObserverName = "Movement";

        public override void Activate()
        {
            player.GroundedEvent += OnGrounded;
        }

        public override void Deactivate()
        {
            player.GroundedEvent -= OnGrounded;
        }

        public override void Enter()
        {
            player.animator.SetTrigger("Dash");
            Dash();
            AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.DASH];
            player.SetKnowledgeTrigger(knowledgePosition, false);

            nFrames = 0;

            //         lastKnowledge = player.Data.Knowledges.find_if()

           // player.CurrentState.SubscribeToAnimatorObserver(animatorObserverName);
        }

        public override void Update()
        {
            bool grounded = player.IsGroundedSlope();

            float elapsedTime = Time.time - activationTime;

            if (player.Data.GetKnowledgeByID(AF.KnowledgeID.WALL_SLIDE).WillUse())
            {
                player.UseKnowledge(AF.KnowledgeID.WALL_SLIDE);
                return;
            }

            if (player.rb.velocity.y < -0.001f && !grounded)
            {
                player.StateMachine.ChangeState(player.States[PlayerState.STATE.AIRBORNE]);
                return;
            }

            if (player.WillClimbLadder())
            {
                LadderClimbingState state = (LadderClimbingState)player.States[PlayerState.STATE.LADDER];
                state.targetX = player.GetBeneathObjectPosition().x + 0.5f - player.ColliderOffset.x;
                player.StateMachine.ChangeState(state);
                return;
            }

            if (player.WillClimbUpStairs())
            {
                player.StateMachine.ChangeState(player.States[PlayerState.STATE.STAIRS_UP]);
                return;
            }

            if (nFrames > 1)
            {
                if (Mathf.Abs(player.rb.velocity.x) < minVelocityThreshold && grounded)
                {
                    Debug.Log("Leaving state");
                    player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
                    OnLeave();
                    return;
                }
                // Let rigidbody have a little deceleration when dashing on the ground
                else 
                {
                    Debug.Log("Decelerating...");
                    player.rb.AddForce(Vector2.right * -player.rb.velocity.x * groundDashDeceleration * Time.fixedDeltaTime, ForceMode2D.Force);
                }
            }
            else
            {
                Debug.Log("ContinueDash()");
                ContinueDash();
            }

            nFrames++;
        }

        public override void Exit()
        {
            //player.CurrentState.UnsubscribeToAnimatorObserver();
            player.animator.ResetTrigger("Dash");
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
        }

        private void Dash()
        {
            hasDashed = true;

            if (player.CanTurn())
            {
                player.Turn();
            }

            Vector3 v;

            float angle = groundDashAngle * Mathf.Deg2Rad;
            v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * groundDashForce * groundDashAcceleration * Time.fixedDeltaTime;
            player.rb.AddForce(v, ForceMode2D.Force);

            dashDirection = player.MoveInput.x * Vector2.right;
        }

        private void ContinueDash()
        {
            Vector3 v;

            float angle = groundDashAngle * Mathf.Deg2Rad;
            v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * groundDashForce;

            player.rb.AddForce(v, ForceMode2D.Force);
        }

        public override bool WillUse()
        {
            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;

            if ((Raycast2DHelper.FindSlopeAtPoint(player.rb.position, out float slope, v * 0.02f + Vector2.up * 0.02f, v, player.StairsDownHeight, player.GroundLayer) && Mathf.Abs(slope) > player.StairsUpMinSlope && Mathf.Abs(slope) < player.StairsUpMaxSlope) ||
                player.Raycast(false, player.GroundLayer, Vector2.up * 0.0f + (player.IsFacingRight ? Vector2.right : -Vector2.right) * 2.5f * player.ColliderSize.x, 0.01f, (player.IsFacingRight ? Vector2.right : -Vector2.right)))
            {
                return false;
            }

            bool availableKnowledge = player.Data.KnownKnowledgeDictionary[KnowledgeID.DASH];
            AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.DASH];

            //Debug.Log($"cooldown = {Time.time - activationTime} >= {cooldown}");

            bool willUse = CanUse()
                && availableKnowledge && !hasDashed
                && player.GetKnowledgeTrigger(knowledgePosition)
                && player.MoveInput.x != 0.0f;
            if (willUse)
            {
                Use();
            }
            //Debug.Log($"willUse={willUse}");
            return willUse;
        }

        private void OnGrounded()
        {
            hasDashed = false;
        }

        public override void OnLeave()
        {
            IdleState state = (IdleState)player.States[PlayerState.STATE.IDLE];
            state.waitNFrames = 3;
            player.StateMachine.ChangeState(state);
        }
    }
}