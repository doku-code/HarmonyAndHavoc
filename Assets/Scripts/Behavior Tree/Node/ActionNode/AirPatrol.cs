using JFM;
using UnityEngine;

//Charles
public class AirPatrol : ActionNode
{
    public float patrolRadius = 3.0f;
    public string PatrolAnimString;
    public float moveSpeed = 1000.0f;
    public float maxSpeed = 2.0f;
    public float distBeforeChanging = 1.0f;
    public float obstacleDistance = 1.5f;
    public float detectionDistance = 3.0f;
    public bool detectForwardOnly = true; 
    
    private Vector2 nextPosition;
    private Vector2 currentDirection;
    private Vector2 initialPosition;
    private float lastGoalDistance;
    public int maxFramesFacingSide = 5;
    private int nFramesFacingSide;
    private float sideFacing;

    void OnEnable()
    {
    }

    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //Debug.Log($"OnStart()");        

        if (!npcController.IsKnockedBack)
        {
            npcAnimator.SetBool(PatrolAnimString, true);
            npcAnimator.SetBool("IsIdle", false);
        }

        if(initialPosition == Vector2.zero)
        {
            initialPosition = npcRigidBody.position;
        }

        nextPosition = GetNextPosition();

        nFramesFacingSide = maxFramesFacingSide;
        sideFacing = npc.transform.localScale.x;
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

        currentDirection = (nextPosition - npcRigidBody.position).normalized;

        if (npcAnimator.GetBool("IsIdle") && !npcController.IsKnockedBack)
        {
            npcAnimator.SetBool(PatrolAnimString, true);
            npcAnimator.SetBool("IsIdle", false);
        }        

        if (!npcController.IsKnockedBack && npcRigidBody.velocity.magnitude < maxSpeed)
        {
            //Debug.Log($"currentDirection={currentDirection} Moving forward={currentDirection * moveSpeed * dt}");
            npcRigidBody.AddForce(currentDirection * moveSpeed * dt);
        }

        if (Mathf.Sign(currentDirection.x) != sideFacing && nFramesFacingSide == 0)
        {
            float newSide = Mathf.Sign(currentDirection.x);
            npcController.TurnSide(newSide);
            nFramesFacingSide = maxFramesFacingSide;
            sideFacing = newSide;
        }
        else
        {
            nFramesFacingSide--;
            if(nFramesFacingSide < 0)
            {
                nFramesFacingSide = 0;
            }
        }
        CapsuleCollider2D capsule = npc.GetComponent<CapsuleCollider2D>();
        //Debug.Log($"obstacleDistance={obstacleDistance}");

        float distance = capsule.size.y - capsule.size.x;
        float radius = capsule.size.x / 2.0f;

        Vector2 position = npcRigidBody.position +
                            Vector2.up * capsule.offset.y -
                            Vector2.up * (capsule.size.y / 2.0f - radius ) +
                            Vector2.right * capsule.offset.x + 
                            currentDirection * obstacleDistance;        

        Platformer2DUtilities.DebugDrawCircle(position, radius, Color.yellow);
        Platformer2DUtilities.DebugDrawCircle(position + Vector2.up * distance, radius, Color.yellow);
        
        //RaycastHit2D[] obstacleHits = Physics2D.CircleCastAll(npcRigidBody.position, obstacleDistance, Vector2.zero, 0.0f, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));        
        RaycastHit2D[] obstacleHits = Physics2D.CircleCastAll(position, radius, Vector2.up, distance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));
        
        //RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(npcRigidBody.position, currentDirection, obstacleDistance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));
        Debug.DrawRay(npcRigidBody.position, currentDirection * obstacleDistance, Color.yellow);

        bool obstacleFound = false;
        bool collisionFound = false;
        Vector2 obstacleNormal = Vector2.zero;
        //Debug.Log("Testing for collisions");
        if (obstacleHits is not null)
        {
            foreach (RaycastHit2D hit in obstacleHits)
            {
                if (hit.collider.gameObject != npc)
                {
                    collisionFound = true;

                    Debug.Log($"hit={hit.collider.gameObject.name} hit.normal={hit.normal}");
                    obstacleFound = true;
                    obstacleNormal = hit.normal;
                    /*Vector2 perpendicularDirection = Platformer2DUtilities.GetPerpendicularVector2(currentDirection);
                    RaycastHit2D obstacleHit2 = Physics2D.Raycast(npcRigidBody.position + perpendicularDirection * 0.1f, currentDirection, obstacleDistance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));

                    if (obstacleHit2.collider.gameObject == hit.collider.gameObject)
                    {
                        obstacleNormal = hit.normal;
                    }*/
                    break;                                     
                }                   
            }
        }

        bool playerIsSeen = Time.time - npcController.Blackboard.lastSeenPlayer <= 5.0f;
        //Debug.Log($"Time.time={Time.time} playerIsSeen ={playerIsSeen}");
        if (!obstacleFound && !playerIsSeen)
        {
            //Debug.Log("Checking for player.");
            RaycastHit2D playerHit = Physics2D.CircleCast(npcRigidBody.position, detectionDistance, Vector2.zero, 0.0f, playerLayer);
            Platformer2DUtilities.DebugDrawCircle(npcRigidBody.position, detectionDistance, Color.yellow);

            if (playerHit.collider is not null && (Mathf.Sign(player.transform.position.x - npcRigidBody.position.x) == Mathf.Sign(currentDirection.x) || !detectForwardOnly))
            {                
                playerIsSeen = true;
            }            
        }
        if (playerIsSeen && !obstacleFound)
        {
            npcController.Blackboard.lastSeenPlayer = Time.time;
            //Debug.Log($"Player is seen! npcController.Blackboard.lastSeenPlayer={npcController.Blackboard.lastSeenPlayer}");            
            return State.SUCCESS;
        }

        if(obstacleFound)
        {
            Vector2 inversedDirection = -currentDirection;
            float dot = Vector2.Dot(inversedDirection, obstacleNormal);
            Vector2 v = (obstacleNormal * dot - inversedDirection) * 2.0f;
            Vector2 newDirection = inversedDirection + v;
            float distanceFromInitialPosition = Vector2.Distance(npcRigidBody.position, initialPosition);
            float newDistance = Mathf.Max(patrolRadius - distanceFromInitialPosition, 1.0f);
            Debug.Log($"newDistance={newDistance}");

            nextPosition = npcRigidBody.position + newDirection * newDistance;
            lastGoalDistance = Vector2.Distance(nextPosition, npcRigidBody.position);
            Debug.Log($"currentDir={currentDirection} newDir={newDirection} nextPosition={nextPosition}");
            npcController.Blackboard.lastSeenPlayer = -999.0f;
            return State.RUNNING;
        }

        float distanceBeforeNextPosition = Vector2.Distance(nextPosition, npcRigidBody.position);
        //Debug.Log($"distanceBeforeNextPosition={distanceBeforeNextPosition}");

        if (distanceBeforeNextPosition <= distBeforeChanging ||
            distanceBeforeNextPosition > lastGoalDistance
            )
        {
            npcController.Blackboard.lastSeenPlayer = -999.0f;
            //Debug.Log("Turning around!");
            nextPosition = GetNextPosition();
            Debug.Log($"nextPosition={nextPosition}");
            if (!npcController.IsKnockedBack)
            {
                npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            }
            lastGoalDistance = Vector2.Distance(nextPosition, npcRigidBody.position);
        }
        else
        {
            lastGoalDistance = distanceBeforeNextPosition;
        }
        //Debug.Log($"Player is not found! obstacleFound={obstacleFound}");

        return State.RUNNING;
    }

    private Vector2 GetNextPosition()
    {
        float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
        Vector2 newPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) 
                                    * patrolRadius + initialPosition;
        float newDistance = Vector2.Distance(newPosition, nextPosition);
        Debug.Log($"GetNextPosition() says: newDistance={newDistance}");
        return newPosition; 
        //return Random.insideUnitCircle * patrolRadius + initialPosition;
    }
}
