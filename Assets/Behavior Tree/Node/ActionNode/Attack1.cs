using UnityEngine;
//charles
public class Attack1 : ActionNode
{
    public float attackCooldown = 3.0f;
    public float attackDistance = 1.5f;
    public string attackAnimString;
    public LayerMask playerLayer;

    private Animator npcAnimator;
    public string gameobjNpcName;
    private bool isCooldown = false;
    private float lastAttackTime = 0f;
    private GameObject npc;
    private GameObject player;
    private Rigidbody2D npcRigidBody;

    protected override void OnStart()
    {
        player = GameObject.Find("Player");
        npc = GameObject.Find(gameobjNpcName);
        npcAnimator = npc.GetComponent<Animator>();
        npcRigidBody = npc.GetComponent<Rigidbody2D>();
    }

    protected override void OnStop()
    {
        npcAnimator.SetBool("Combo1", false);
    }

    protected override State OnUpdate()
    {
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.transform.position);

        if (distanceToPlayer <= attackDistance)
        {
            if (!isCooldown)
            {
                npcAnimator.SetBool("Combo1", true);
                isCooldown = true;
                lastAttackTime = Time.deltaTime;
                return State.SUCCESS;
            }
        }
        else
        {
            npcAnimator.SetBool("Combo1", false);
            return State.FAILURE;
        }

        if (isCooldown && Time.deltaTime - lastAttackTime >= attackCooldown)
        {
            isCooldown = false;
        }

        return State.RUNNING;
    }
}
