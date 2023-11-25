using AF;
using charles;
using UnityEngine;
//Charles
public class Attack : ActionNode
{
    public float attackCooldown = 3.0f;
    public float attackDistance = 3.0f;
    public string attackAnimString;
    public int comboSoundIdx;

    private bool isCooldown = true;
    private bool hasAttacked = false;
    private float lastAttackTime = 0f;

    protected override void OnStart() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attackCooldown = 3.0f;
    }
    protected override void OnStop() {}

    protected override State OnUpdate()
    {
        if(npcController.IsDead)
        {
            return State.FAILURE;
        }
          
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.transform.position);

        if (distanceToPlayer <= attackDistance && !hasAttacked)
        {
            if (!isCooldown)
            {
                npcAnimator.ResetTrigger(attackAnimString);
                npcAnimator.SetTrigger(attackAnimString);
                isCooldown = true;
                lastAttackTime = Time.time;
                hasAttacked = true;
                if (SoundManager.Instance is not null)
                {
                    SoundManager.Instance.PlayNpcSounds(comboSoundIdx);
                }
                return State.RUNNING;
            }
        }
        else if(distanceToPlayer > attackDistance)
        {
            npcAnimator.SetBool("IsIdle", true);
            return State.FAILURE;
        }
        else
        {
            if (hasAttacked && npcAnimator.GetCurrentAnimatorStateInfo(0).length <= Time.time - lastAttackTime)
            {
                npcAnimator.SetBool("IsIdle", true);
                hasAttacked = false;

                return State.SUCCESS;
            }
            return State.RUNNING;
        }
        if (isCooldown && Time.time - lastAttackTime >= attackCooldown)
        {
            isCooldown = false;
            npcAnimator.SetBool("IsIdle", false);
        }
        return State.RUNNING;
    }
}
