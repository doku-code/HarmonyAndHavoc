using charles;
using UnityEngine;

//Charles
public class NPCPathFinding : ActionNode
{
    public GameObject[] POSPatrolRoute;
    public string PatrolAnimString;
    public float moveSpeed = 2000.0f;
    public float maxSpeed = 50.0f;
    public float sphereCastRadius = 2.0f;
    public float distBeforeChanging = 1.0f;
    
    private int currentPOS;
    private Vector3 lastPosition;
    private float currentDirection;

    void OnEnable()
    {
        PatrolAnimString = "Run";
    }
    
    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        npcAnimator.SetBool(PatrolAnimString, true);
        lastPosition = npc.transform.position;
    }

    protected override void OnStop()
    {
        //currentPOS = 0;
        npcAnimator.SetBool(PatrolAnimString, false);
    }

    protected override State OnUpdate(float dt)
    {
        if (npcController.IsDead)
        {
            return State.FAILURE;
        }
        //Debug.Log($"npc.name={npc.name} npcRigidbody.position={npcRigidBody.position}");
        Vector3 targetPOS = POSPatrolRoute[currentPOS].transform.position;
        Debug.Log($"targetPOS={targetPOS} npc.transform.position={npc.transform.position} {targetPOS - npc.transform.position}");
        Vector3 moveDirection = (targetPOS - npc.transform.position).normalized;
        moveDirection = Vector3.right * Mathf.Sign(moveDirection.x);
        if (currentDirection == 0.0f)
        {
            currentDirection = moveDirection.x;
        }

        if (POSPatrolRoute.Length == 0)
        {
            Debug.LogWarning("You forgot to add the waypoint in the Behavior Tree");
            return State.FAILURE;
        }

        /*RaycastHit2D hit = Physics2D.CircleCast(npc.transform.position, sphereCastRadius, moveDirection, 1f, playerLayer);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {            
            ChangeNpcVelocity(Vector3.zero);
            
            return State.SUCCESS;
        }
        else
        {     */
        //ChangeNpcVelocity(moveDirection * moveSpeed * Time.fixedDeltaTime);
        if (npcRigidBody.velocity.magnitude < maxSpeed)
        {
            npcRigidBody.AddForce(moveDirection * moveSpeed * dt);
        }
        Debug.Log($"moveDirection * moveSpeed * dt={moveDirection} * {moveSpeed} * {dt} currentPOS={currentPOS}");

        if (moveDirection.x > 0)
        { 
            npc.transform.localScale = new Vector3(1,1,1);
        }
        else if (moveDirection.x < 0)
        {
            npc.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Mathf.Abs(targetPOS.x - lastPosition.x) < distBeforeChanging || 
            Mathf.Sign(targetPOS.x - lastPosition.x) != Mathf.Sign(currentDirection))
        {            
            currentPOS++;
            currentPOS %= POSPatrolRoute.Length;
            Debug.Log($"currentPOS={currentPOS}");
            currentDirection = Mathf.Sign(POSPatrolRoute[currentPOS].transform.position.x - lastPosition.x);

            npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            
            /*if (currentPOS >= POSPatrolRoute.Length)
            {                    
                ChangeNpcVelocity(Vector3.zero);
                    
                return State.FAILURE;
            }*/
        }

        lastPosition = npc.transform.position;
        return State.SUCCESS; // RUNNING;
        //}
    }
}
