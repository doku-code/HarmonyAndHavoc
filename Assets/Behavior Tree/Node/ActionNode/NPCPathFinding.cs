using charles;
using UnityEngine;

//Charles
public class NPCPathFinding : ActionNode
{
    public GameObject[] POSPatrolRoute;
    public string PatrolAnimString;
    public float moveSpeed = 2000.0f;
    public float sphereCastRadius = 2.0f;

    private int currentPOS;

    void OnEnable()
    {
        PatrolAnimString = "Run";
    }
    
    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
            ChangeNpcVelocity(Vector3.zero);
            
            return State.SUCCESS;
        }
        else
        {            
            ChangeNpcVelocity(moveDirection * moveSpeed * Time.fixedDeltaTime);
            

            if (moveDirection.x > 0)
            { 
                npc.transform.localScale = new Vector3(1,1,1);
            }
            else if (moveDirection.x < 0)
            {
                npc.transform.localScale = new Vector3(-1, 1, 1);
            }
            if (Vector2.Distance(npc.transform.position, targetPOS) < 1f)
            {
                currentPOS++;
                if (currentPOS >= POSPatrolRoute.Length)
                {                    
                    ChangeNpcVelocity(Vector3.zero);
                    
                    return State.FAILURE;
                }
            }
            return State.RUNNING;
        }
    }
}
