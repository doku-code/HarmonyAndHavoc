using AF;
using JFM;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttackKnowledge", menuName = "Knowledges/Combo Attack")]
public class ComboAttackKnowledge : Knowledge
{
    private float startTime;
    private float animationClipLength;
    private bool isAirborned;
    [SerializeField] private int animatorLayer = 2;
    [SerializeField] private string groundMotionName = "Player_Attack_1";
    [SerializeField] private string airMotionName = "Player_Air_Attack_1";
    private string motionName;

    public override void Activate()
    {
    }

    public override void Deactivate()
    {
    }

    public override void Enter()
    {
        // If airborne, use air attack animation
        if (isAirborned = !player.IsGrounded())
        {
            player.animator.SetBool("IsAirborne", true);
            motionName = airMotionName;
        }
        else
        {
            motionName = groundMotionName;
        }

        player.animator.SetBool("IsAttacking", true);

        AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.COMBO_ATTACK];
        player.SetKnowledgeTrigger(knowledgePosition, false);
        //player.rb.velocity = Vector2.zero;
        startTime = Time.time;
        player.Attack();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
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
            && availableKnowledge
            && player.GetKnowledgeTrigger(knowledgePosition)
            && player.MoveInput.x != 0.0f;
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
