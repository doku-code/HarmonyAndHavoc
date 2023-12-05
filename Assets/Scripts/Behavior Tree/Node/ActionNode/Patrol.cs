using charles;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

//Charles
public class Patrol : ActionNode
{
    public float patrolRadius = 3.0f;
    public string PatrolAnimString;
    public float moveSpeed = 1000.0f;
    public float maxSpeed = 2.0f;
    public float distBeforeChanging = 1.0f;
    public float obstacleDistance = 1.5f;
    public float attackDistance = 1.5f;
    public Vector2 holeDistance = Vector2.one;
    public bool isFlying;

    private Vector2 nextPosition;
    private float currentDirection;
    private Vector2 initialPosition;

    void OnEnable()
    {
        PatrolAnimString = "Run";
    }

    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
                
        if (initialPosition == Vector2.zero)
        {
            initialPosition = npcRigidBody.position;
        }

        if (currentDirection == 0.0f)
        {
            currentDirection = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
            nextPosition = npcRigidBody.position + currentDirection * Vector2.right * patrolRadius;
        }
        else
        {
            currentDirection = Mathf.Sign(player.transform.position.x - npcRigidBody.position.x);
        }
    }

    protected override void OnStop()
    {        
    }

    protected override State OnUpdate(float dt)
    {
        if (npcController.IsDead)
        {
            return State.FAILURE;
        }

        if (npcAnimator.GetBool("IsIdle") && !npcController.IsKnockedBack)
        {
            npcAnimator.SetBool(PatrolAnimString, true);
            npcAnimator.SetBool("IsIdle", false);
        }        

        if (!npcController.IsKnockedBack && npcRigidBody.velocity.magnitude < maxSpeed)
        {
            npcRigidBody.AddForce(Vector2.right * currentDirection * moveSpeed * dt);
        }

        npcController.TurnSide(currentDirection);

        RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(npcRigidBody.position, Vector2.right * currentDirection, obstacleDistance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));        

        bool obstacleFound = false;
        if (obstacleHits is not null)
        {
            foreach (RaycastHit2D hit in obstacleHits)
            {
                if (hit.collider.gameObject != npc)
                {
                    //Debug.Log($"hit={hit.collider.gameObject.name}");
                    obstacleFound = true;
                }
            }
        }
        if (!obstacleFound && !isFlying)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x, Vector2.down, holeDistance.y, LayerMask.GetMask("Ground"));
            //Debug.DrawLine(npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x, 
            //    npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x + Vector2.down * holeDistance.y);
            if (groundHit.collider is null)
            {
                obstacleFound = true;
            }
        }

        float facing = npcRigidBody.velocity.x > 0 ? 1.0f : -1.0f;

        RaycastHit2D playerHit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * facing, attackDistance, playerLayer);
        Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * facing * attackDistance, Color.yellow);

        if (playerHit.collider is not null)
        {
            return State.SUCCESS;
        }

        if (Mathf.Abs(nextPosition.x - npcRigidBody.position.x) < distBeforeChanging ||
            Mathf.Sign(nextPosition.x - npcRigidBody.position.x) != Mathf.Sign(currentDirection) ||
            obstacleFound)
        {            
            currentDirection = -currentDirection;
            nextPosition = npcRigidBody.position + currentDirection * Vector2.right * 2.0f * patrolRadius;
            if (!npcController.IsKnockedBack)
            {
                npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            }
        }

        return State.RUNNING;
    }
}
