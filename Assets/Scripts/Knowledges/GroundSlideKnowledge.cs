//#define _DEBUG

using AF;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "GroundSlideKnowledge", menuName = "Knowledges/Ground Slide")]
    public class GroundSlideKnowledge : Knowledge
    {
        private int nFrames;

        private Vector2 slideDirection;
        [SerializeField] private float force = 60.0f;
        [SerializeField] private float deceleration = 1000.0f;
        [SerializeField] private float acceleration = 500.0f;
        [SerializeField] private float minVelocity = 0.0005f;
        [SerializeField] private float maxVelocity = 5.0f;
        [SerializeField] private int minFrames = 5;
        [SerializeField] private int maxIdleFrames = 4;

        private int idleFrames;
        private bool moving;
        private bool collisionOverHead;

        // In degrees
        private float angle = 0.0f;

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Enter()
        {
            player.animator.SetTrigger("IsGroundSliding");
            Slide();
            AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.GROUND_SLIDE];
            player.SetKnowledgeTrigger(knowledgePosition, false);

            nFrames = 0;
            moving = true;
            idleFrames = 0;

            //         lastKnowledge = player.Data.Knowledges.find_if()        

            collisionOverHead = player.CheckForCollisions();
        }

        public override void Update()
        {
            bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
            bool grounded = player.IsGrounded(player.GroundLayer | player.LadderLayer, Vector2.zero, player.GroundDistance * 2.0f, false) || (foundSlopeBeneath && Mathf.Abs(slope) > player.StairsUpMinSlope && Mathf.Abs(slope) < player.StairsUpMaxSlope);

            //float elapsedTime = Time.time - activationTime;
            bool currentCollisionOverHead = player.CheckForCollisions();
            if (collisionOverHead |= currentCollisionOverHead)
            {
                //Debug.Log("Found a collision!");
                //nFrames = 2;
            }
#if _DEBUG
            Debug.Log($"nFrames={nFrames} collisionOverHead={collisionOverHead} currentCollisionOverHead={currentCollisionOverHead}");
#endif
            if (nFrames >= minFrames && (collisionOverHead || !moving) && (!currentCollisionOverHead || idleFrames >= maxIdleFrames))
            {
#if _DEBUG
                Debug.Log($"GroundSlide end....idleFrames={idleFrames}");
#endif
                if (slideDirection.y != 0.0f && player.rb.velocity.y < -0.01f && !grounded)
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

                //Debug.Log($"Mathf.Abs(player.rb.velocity.x)={Mathf.Abs(player.rb.velocity.x)}");        

                if (Mathf.Abs(player.rb.velocity.x) < minVelocity && grounded)
                {

                    if (player.Data.GetKnowledgeByID(AF.KnowledgeID.WALL_SLIDE).WillUse())
                    {
                        player.UseKnowledge(AF.KnowledgeID.WALL_SLIDE);
                        return;
                    }

                    player.StateMachine.ChangeState(player.States[PlayerState.STATE.IDLE]);
                    return;
                }
                // Let rigidbody have a little deceleration when sliding on the ground
                else
                {
                    player.rb.AddForce(Vector2.right * -player.rb.velocity.x * deceleration * Time.fixedDeltaTime, ForceMode2D.Force);
#if _DEBUG
                    Debug.Log($"Decelerating: player.rb.velocity.magnitude={player.rb.velocity.magnitude}");
#endif
                }
            }
            else
            {
                ContinueSlide();
            }

            if (!moving)
            {
                idleFrames++;
            }

            float s = Mathf.Abs(player.rb.velocity.x);
            if (s < minVelocity)
            {
                moving = false;
            }

#if _DEBUG
            Debug.Log($"player.rb.velocity.magnitude={player.rb.velocity.magnitude}");
#endif
            // Limit speed (looks like we have to do this here, since there's sometimes a mysterious force
            // applied in addition to the slide.
            Platformer2DUtilities.LimitVelocity(player.rb, maxVelocity);

            nFrames++;
        }

        public override void Exit()
        {

            //if (player.CheckForCollisions())

            //Debug.Break();
#if _DEBUG
            Debug.Log($"GroundSlideKnowledge.Exit() player.rb.velocity={player.rb.velocity}");
#endif
            player.animator.ResetTrigger("IsGroundSliding");
            player.rb.AddForce(-player.rb.velocity, ForceMode2D.Impulse);
        }

        private void Slide()
        {
            if (player.CanTurn())
            {
                player.Turn();
            }

            Vector3 v;

            float angle = this.angle * Mathf.Deg2Rad;
            v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * force * acceleration * Time.fixedDeltaTime;

            // It seems that limiting the speed preemptively (before applpying the force), we may 
            // limit too much the speed that we gain from running beforehand.
            //if (player.rb.velocity.magnitude < maxVelocity)
            {
                player.rb.AddForce(v, ForceMode2D.Force);
#if _DEBUG
                Debug.Log($"Slide() says: player.rb.velocity.magnitude={player.rb.velocity.magnitude}");
#endif
            }

            slideDirection = player.MoveInput.x * Vector2.right;
        }

        private void ContinueSlide()
        {
            Vector3 v;

            float angle = this.angle * Mathf.Deg2Rad;
            v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * force * Time.fixedDeltaTime;

            if (player.rb.velocity.magnitude < maxVelocity)
            {
                player.rb.AddForce(v, ForceMode2D.Force);
#if _DEBUG
                Debug.Log($"ContinueSlide() says: player.rb.velocity.magnitude={player.rb.velocity.magnitude}");
#endif
            }
        }

        public override bool WillUse()
        {
            Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;

            bool grounded = player.IsGrounded();

            bool availableKnowledge = player.Data.KnownKnowledgeDictionary[KnowledgeID.GROUND_SLIDE];
            AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.GROUND_SLIDE];

            //Debug.Log($"cooldown = {Time.time - activationTime} >= {cooldown}");

            bool willUse = CanUse()
                && availableKnowledge && grounded
                && player.GetKnowledgeTrigger(knowledgePosition);

            if (willUse)
            {
                Use();
            }
            //Debug.Log($"willUse={willUse}");
            return willUse;
        }

        public override void OnLeave()
        {
        }
    }
}
