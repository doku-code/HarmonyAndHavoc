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

        //         lastKnowledge = player.Data.Knowledges.find_if()

        if (animationClipLength == 0.0f)
        {
            animationClipLength = player.animator.GetCurrentAnimatorStateInfo(animatorLayer).length;
        }
    }

    public override void Update()
    {
        bool foundSlopeBeneath = player.FindSlopeBeneath(out float slope);
        bool grounded = player.IsGrounded(player.GroundLayer | player.LadderLayer, Vector2.zero, player.GroundDistance * 2.0f, false) || (foundSlopeBeneath && Mathf.Abs(slope) > player.StairsUpMinSlope && Mathf.Abs(slope) < player.StairsUpMaxSlope);

        float elapsedTime = Time.time - activationTime;

        if (player.CheckForCollisions2())
        {
            Debug.Log("Found a collision!");
            nFrames = -5;
        }

        if (nFrames > 1)
        {
            

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

        nFrames++;
    }

    public override void Exit() {
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
        v = new Vector3((player.IsFacingRight ? 1.0f : -1.0f) * Mathf.Cos(angle), Mathf.Sin(angle)) * force;

        player.rb.AddForce(v, ForceMode2D.Force);
    }

    public override bool WillUse()
    {
        Vector2 v = player.IsFacingRight ? Vector2.right : -Vector2.right;
        /*
                if ((Raycast2DHelper.FindSlopeAtPoint(player.rb.position, out float slope, v * 0.02f + Vector2.up * 0.02f, v, player.StairsDownHeight, player.GroundLayer) && Mathf.Abs(slope) > player.StairsUpMinSlope && Mathf.Abs(slope) < player.StairsUpMaxSlope) ||
                    player.Raycast(false, player.GroundLayer, Vector2.up * 0.0f + (player.IsFacingRight ? Vector2.right : -Vector2.right) * 2.5f * player.ColliderSize.x, 0.01f, (player.IsFacingRight ? Vector2.right : -Vector2.right)))
                {            
                    return false;
                }*/

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

    private void OnGrounded()
    {
        
    }
}
