using UnityEngine;

//Charles
public class NPCPathFinding : ActionNode
{
    public GameObject[] POSPatrolRoute;
    public string PatrolAnimString;
    public string gameobjNpcName;
    public float moveSpeed = 2000.0f;
    public float sphereCastRadius = 2.0f;
    public LayerMask playerLayer;

    private int currentPOS;
    private SpriteRenderer npcSpriteRenderer;
    private GameObject npc;
    private Animator npcAnimator;
    private Rigidbody2D npcRigidBody;

    protected override void OnStart()
    {
        npc = GameObject.Find(gameobjNpcName);
        npcSpriteRenderer = npc.GetComponent<SpriteRenderer>();
        npcAnimator = npc.GetComponent<Animator>();
        npcRigidBody = npc.GetComponent<Rigidbody2D>();

        npcAnimator.SetBool(PatrolAnimString, true);
    }

    protected override void OnStop()
    {
        currentPOS = 0;
        npcAnimator.SetBool(PatrolAnimString, false);
    }

    protected override State OnUpdate()
    {

        Vector3 targetPOS = POSPatrolRoute[currentPOS].transform.position;
        Vector3 moveDirection = (targetPOS - npc.transform.position).normalized;

        if (POSPatrolRoute.Length == 0)
        {
            Debug.LogWarning("You forgot to add the waypoint in the Behavior Tree");
            return State.FAILURE;
        }

        RaycastHit2D hit = Physics2D.CircleCast(npc.transform.position, sphereCastRadius, moveDirection, 1f, playerLayer);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            npcRigidBody.velocity = Vector3.zero;
            return State.SUCCESS;
        }
        else
        {
            npcRigidBody.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;


            if (moveDirection.x > 0)
            {
                npcSpriteRenderer.flipX = false;
            }
            else if (moveDirection.x < 0)
            {
                npcSpriteRenderer.flipX = true;
            }

            if (Vector2.Distance(npc.transform.position, targetPOS) < 1f)
            {
                currentPOS++;
                if (currentPOS >= POSPatrolRoute.Length)
                {
                    npcRigidBody.velocity = Vector3.zero;
                    return State.FAILURE;
                }
            }
            return State.RUNNING;
        }
    }
}
