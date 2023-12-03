using AF;
using charles;
using JFM;
using UnityEngine;
using UnityEngine.EventSystems;
//Charles
public class Attack : ActionNode
{
    public float attackCooldown = 3.0f;
    public float attackDistance = 3.0f;
    public float leaveDistance = 5.0f;
    public string attackAnimString;
    public int comboSoundIdx;
    public int animatorLayer = 0;
    public float moveSpeed = 1000.0f;
    public float maxSpeed = 2.0f;

    private bool isCooldown = true;
    private bool hasAttacked = false;
    private float lastAttackTime = 1.0f;
    private float attackAnimationLength;

    protected override void OnStart() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attackCooldown = 3.0f;
    }
    protected override void OnStop() {}

    protected override State OnUpdate(float dt)
    {
        if(npcController.IsDead)
        {
            return State.FAILURE;
        }

        if(attackAnimationLength == 0.0f)
        {
            attackAnimationLength = Platformer2DUtilities.GetClipLength(npcAnimator, "Attack1");
            Debug.Log($"attackAnimationLength = {attackAnimationLength}");
        }

        //Debug.Log("Attack node.");
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.transform.position);

        // Manage distance to player
        if (distanceToPlayer <= attackDistance)
        {
            //Debug.Log("Attacking...");
            if (!npcController.IsKnockedBack)
            {
                npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            }

            if (npcAnimator.GetBool("Run") && npcController.CanTakeDamage)
            {
                npcAnimator.SetBool("IsIdle", true);
                npcAnimator.SetBool("Run", false);
            }

            if (npcController.Blackboard.lastHitTime < lastAttackTime + npcAnimator.GetCurrentAnimatorStateInfo(animatorLayer).length
               && npcController.Blackboard.lastHitTime >= lastAttackTime)
            {
                npcController.Blackboard.lastHitTime = lastAttackTime + npcAnimator.GetCurrentAnimatorStateInfo(animatorLayer).length;
                Debug.Log("&&&&&&&&&&&&&&&&&&&&&&&&&&");
                
                return State.SUCCESS;
            }

            float elapsedTime = Time.time - lastAttackTime;
            
            //if (!isCooldown && !hasAttacked)
            if (elapsedTime >= attackCooldown 
                &&
                elapsedTime >= npcAnimator.GetCurrentAnimatorStateInfo(animatorLayer).length)
            {
                npcAnimator.SetBool("IsIdle", false);
                npcAnimator.ResetTrigger(attackAnimString);
                npcAnimator.SetTrigger(attackAnimString);                
                //isCooldown = true;
                lastAttackTime = Time.time;
                //hasAttacked = true;
                if (SoundManager.Instance is not null)
                {
                    SoundManager.Instance.PlayNpcSounds(comboSoundIdx);
                }
                return State.RUNNING;
            }

            return State.RUNNING;
        }
        else if(distanceToPlayer <= leaveDistance)
        {
            //Debug.Log("Approaching to attack...");

            Vector3 moveDirection = (player.transform.position - npc.transform.position).normalized;
            if (npcController.CanTakeDamage)
            {
                npcAnimator.SetBool("IsIdle", false);
                npcAnimator.SetBool("Run", true);
            }
            if (moveDirection.x > 0)
            {
                npc.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveDirection.x < 0)
            {
                npc.transform.localScale = new Vector3(-1, 1, 1);
            }

            if (npcRigidBody.velocity.magnitude < maxSpeed)
            {
                npcRigidBody.AddForce(moveDirection * moveSpeed * dt);
            }            

            return State.RUNNING;
        }

        npcAnimator.SetBool("Run", false);
        npcAnimator.SetBool("IsIdle", true);       

        Debug.Log($"distanceToPlayer <= attackDistance {distanceToPlayer} <= {attackDistance} {leaveDistance}");
        return State.FAILURE;
    }    
}
