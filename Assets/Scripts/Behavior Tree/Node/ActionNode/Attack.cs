#define _DEBUG

using AF;
using JFM;
using UnityEngine;

//Charles
public class Attack : ActionNode
{
    public float attackCooldown = 1.5f;
    public float attackDistance = 1.5f;
    public float leaveDistance = 5.0f;
    public string attackAnimString;
    public string attackClipName;
    public int comboSoundIdx;
    public int animatorLayer = 0;
    public float moveSpeed = 1000.0f;
    public float maxSpeed = 2.0f;

    private bool hasAttacked = false;
    private float lastAttackTime = 1.0f;
    private float attackAnimationLength;

    protected override void OnStart() 
    {
        player = GameObject.FindGameObjectWithTag("Player");        
        hasAttacked = false;        
        npcAnimator.ResetTrigger(attackAnimString);
        lastAttackTime = Time.time;
    }
    protected override void OnStop() {        
        npcAnimator.ResetTrigger(attackAnimString);
        npcAnimator.SetBool("IsIdle", true);
    }

    protected override State OnUpdate(float dt)
    {
        if(npcController.IsDead)
        {
            return State.FAILURE;
        }

        if(attackAnimationLength == 0.0f)
        {
            attackAnimationLength = Platformer2DUtilities.GetClipLength(npcAnimator, attackClipName);
#if _DEBUG
            Debug.Log($"attackAnimationLength = {attackAnimationLength}");
#endif
        }

        //Debug.Log($"Attack node. {attackClipName}");
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.transform.position);

        // Manage distance to player
        if (distanceToPlayer <= attackDistance)
        {
#if _DEBUG
            Debug.Log("Attempting to attack...");
#endif

            // Turn NPC to face Player
            if (player.transform.position.x - npc.transform.position.x > 0)
            {
                npc.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                npc.transform.localScale = new Vector3(-1, 1, 1);
            }

            if (!npcController.IsKnockedBack)
            {
                npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            }

            float elapsedTime = Time.time - lastAttackTime;


            if (hasAttacked && elapsedTime >= attackAnimationLength)
            {
#if _DEBUG
                Debug.Log($"Attack node. {attackClipName} Has attacked and finished animation after {elapsedTime}");
#endif

                npcAnimator.ResetTrigger(attackAnimString);
                npcAnimator.SetBool("IsIdle", true);
            }

            if (npcController.Blackboard.lastHitTime < lastAttackTime + attackAnimationLength
               && npcController.Blackboard.lastHitTime >= lastAttackTime
               && hasAttacked               
               )
            {
                npcController.Blackboard.lastHitTime = lastAttackTime + attackAnimationLength;
#if _DEBUG
                Debug.Log($"Attack node. {attackClipName} &&&&&&&&&&&&& Has hit Player &&&&&&&&&&&&&");
#endif                             
            }

            //if (!isCooldown && !hasAttacked)
            if (!hasAttacked 
                || 
                (elapsedTime >= attackCooldown && elapsedTime >= attackAnimationLength))
            {
                if (hasAttacked)
                {
#if _DEBUG
                    Debug.Log($"Attack node. {attackClipName} Has attacked and returns SUCCESS.");
#endif

                    return State.SUCCESS;
                }

                npcAnimator.SetBool("Run", false);
                npcAnimator.SetBool("IsIdle", false);
                npcAnimator.SetTrigger(attackAnimString);                

                //isCooldown = true;
                lastAttackTime = Time.time;
                hasAttacked = true;
#if _DEBUG
                Debug.Log($"Attack node. {attackClipName} Has attacked. ^^^^^^^^^^^^^^^^^^^^^^^^^");
#endif

                if (SoundManager.Instance is not null)
                {
                    SoundManager.Instance.PlayNpcSounds(comboSoundIdx);
                }
                return State.RUNNING;
            }

            if (npcAnimator.GetBool("Run") && npcController.CanTakeDamage)
            {
                npcAnimator.SetBool("IsIdle", true);
                npcAnimator.SetBool("Run", false);

#if _DEBUG
                Debug.Log($"Attack node. {attackClipName} Changing to Idle.");
#endif
            }

#if _DEBUG
            Debug.Log($"Attack node. {attackClipName} Failed to attack.");
#endif
            return State.RUNNING;
        }
        else if(distanceToPlayer <= leaveDistance)
        {
#if _DEBUG
            Debug.Log($"Attack node. {attackClipName} Approaching to attack...");
#endif

            Vector3 moveDirection = (player.transform.position - npc.transform.position).normalized;
            if (npcAnimator.GetBool("IsIdle") && npcController.CanTakeDamage)
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

#if _DEBUG
        Debug.Log($"Attack node. {attackClipName} returns FAILURE. distanceToPlayer <= attackDistance {distanceToPlayer} <= {attackDistance} {leaveDistance}");
#endif
        //npcAnimator.ResetTrigger(attackAnimString);

        return State.FAILURE;
    }    
}
