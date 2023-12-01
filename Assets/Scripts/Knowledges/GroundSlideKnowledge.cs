using AF;
using JFM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundSlideKnowledge", menuName = "Knowledges/Ground Slide")]
public class GroundSlideKnowledge : Knowledge
{
    private float animationClipLength;
    [SerializeField] private int animatorLayer = 0;
    private int nFrames;

    private Vector2 slideDirection;
    [SerializeField] private float force = 60.0f;
    [SerializeField] private float deceleration = 1000.0f;
    [SerializeField] private float acceleration = 500.0f;
    [SerializeField] private float minVelocity = 0.0005f;    

    private int idleFrames;
    private bool moving;
    private bool collisionOverHead;

    // In degrees
    private float angle = 0.0f;

    public override void Activate()
    {
        //player.GroundedEvent += OnGrounded;
    }

    public override void Deactivate()
    {
        //player.GroundedEvent -= OnGrounded;
    }

    public override void Enter() {
        player.animator.SetTrigger("IsGroundSliding");
        Slide();
        AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.GROUND_SLIDE];
        player.SetKnowledgeTrigger(knowledgePosition, false);

        nFrames = 0;
        moving = true;
        idleFrames = 0;

        //         lastKnowledge = player.Data.Knowledges.find_if()

        if (animationClipLength == 0.0f)
        {
            animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
        }

        collisionOverHead = player.CheckForCollisions2();
    }

    public override void Update()
    {
        bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
        bool grounded = player.IsGrounded(player.GroundLayer | player.LadderLayer, Vector2.zero, player.GroundDistance * 2.0f, false) || (foundSlopeBeneath && Mathf.Abs(slope) > player.StairsUpMinSlope && Mathf.Abs(slope) < player.StairsUpMaxSlope);

        float elapsedTime = Time.time - activationTime;
        bool currentCollisionOverHead = player.CheckForCollisions2();
        if (collisionOverHead |= currentCollisionOverHead)
        {
            //Debug.Log("Found a collision!");
            //nFrames = 2;
        }        

        //Debug.Log($"nFrames={nFrames} collisionOverHead={collisionOverHead} currentCollisionOverHead={currentCollisionOverHead}");
        if (nFrames > 1 && (collisionOverHead || !moving) && !currentCollisionOverHead)
        {

            //Debug.Log($"GroundSlide end....idleFrames={idleFrames}");
            if (slideDirection.y != 0.0f && player.rb.velocity.y < -0.01f && !grounded)
            {
                player.ChangeState(player.states[PlayerState.STATE.AIRBORNE]);
                return;
            }

            if (player.WillClimbLadder())
            {
                LadderClimbingState state = (LadderClimbingState)player.states[PlayerState.STATE.LADDER];
                state.targetX = player.GetBeneathObjectPosition().x + 0.5f - player.ColliderOffset.x;
                player.ChangeState(state);
                return;
            }

            if (player.WillClimbUpStairs())
            {
                player.ChangeState(player.states[PlayerState.STATE.STAIRS_UP]);
                return;
            }

            //Debug.Log($"Mathf.Abs(player.rb.velocity.x)={Mathf.Abs(player.rb.velocity.x)}");        
        
            if( elapsedTime > animationClipLength || (Mathf.Abs(player.rb.velocity.x) < minVelocity && grounded))
            {

                if (player.Data.GetKnowledgeByID(AF.KnowledgeID.WALL_SLIDE).WillUse())
                {
                    player.UseKnowledge(AF.KnowledgeID.WALL_SLIDE);
                    return;
                }

                player.ChangeState(player.states[PlayerState.STATE.IDLE]);
                return;
            }
            // Let rigidbody have a little deceleration when sliding on the ground
            else
            {
                player.rb.AddForce(Vector2.right * -player.rb.velocity.x * deceleration * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
        else
        {
            ContinueSlide();
        }

        if(!moving)
        {
            idleFrames++;
        }

        float s = Mathf.Abs(player.rb.velocity.x);
        if (s < minVelocity)
        {
            moving = false;
        }

        nFrames++;
    }

    public override void Exit() {

        //if (player.CheckForCollisions2())
        
            //Debug.Break();
        
        Debug.Log($"GroundSlideKnowledge.Exit() player.rb.velocity={player.rb.velocity}");
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
        player.rb.AddForce(v, ForceMode2D.Force);

        slideDirection = player.MoveInput.x * Vector2.right;
    }

    private void ContinueSlide()
    {
        Vector3 v;

        float angle = this.angle * Mathf.Deg2Rad;
        v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * force * Time.fixedDeltaTime;

        player.rb.AddForce(v, ForceMode2D.Force);
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
            if (willUse = player.Data.UseChaos(chaosCost))
            {
                Use();
            }
        }
        //Debug.Log($"willUse={willUse}");
        return willUse;
    }

    public override void OnLeave()
    {
    }
}
