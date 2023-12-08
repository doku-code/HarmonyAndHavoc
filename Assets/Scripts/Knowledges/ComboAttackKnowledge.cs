using AF;
using UnityEngine;

namespace JFM
{
    [CreateAssetMenu(fileName = "ComboAttackKnowledge", menuName = "Knowledges/Combo Attack")]
    public class ComboAttackKnowledge : Knowledge
    {
        /*private float startTime;
        private float animationClipLength;
        private bool isAirborned;
        [SerializeField] private int animatorLayer = 2;
        [SerializeField] private string groundMotionName = "Player_Attack_1";
        [SerializeField] private string airMotionName = "Player_Air_Attack_1";
        private string motionName;
        */
        [SerializeField] private string animatorObserverName = "Attacks";

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Enter()
        {
            // If airborne, use air attack animation
            if (/*isAirborned =*/ !player.IsGrounded())
            {
                player.animator.SetBool("IsAirborne", true);
                //motionName = airMotionName;
            }
            else
            {
                //motionName = groundMotionName;
            }

            player.CurrentState.SubscribeToAnimatorObserver(animatorObserverName);

            player.animator.SetBool("IsAttacking", true);
            player.animator.SetInteger("KnowledgeAttackIndex", 1);
            AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.COMBO_ATTACK];
            player.SetKnowledgeTrigger(knowledgePosition, false);
           //startTime = Time.time;
            player.Attack();
        }

        public override void Update()
        {

        }

        public override void Exit()
        {
            player.animator.SetInteger("KnowledgeAttackIndex", 0);
            player.CurrentState.UnsubscribeToAnimatorObserver();            
        }

        public override bool WillUse()
        {
            bool availableKnowledge = player.Data.KnownKnowledgeDictionary[KnowledgeID.COMBO_ATTACK];
            AvailableKnowledgePosition knowledgePosition = player.Data.AvailableKnowledgeDictionary[KnowledgeID.COMBO_ATTACK];

            //Debug.Log($"cooldown = {Time.time - activationTime} >= {cooldown}");

            bool willUse = CanUse()
                           && availableKnowledge
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
            player.StateMachine.ChangeState(player.States[PlayerState.STATE.IDLE]);
        }
    }
}