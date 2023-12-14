using charles;
using JFM;
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
    public float detectionDistance = 3.0f;
    public bool detectForwardOnly = true; 
    public Vector2 holeDistance = new Vector2(1.0f, 1.5f);
    public float turningAroundMinWaitDuration = 2.0f;
    public float turningAroundMaxWaitDuration = 2.0f;

    private Vector2 nextPosition;
    private float currentDirection;
    private float turningAroundWaitStartTime = -1.0f;
    private float turningAroundCurrentWaitDuration;
    void OnEnable()
    {
        //PatrolAnimString = "Run";
    }

    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //Debug.Log($"OnStart()");

        if (currentDirection == 0.0f)
        {
            currentDirection = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
            nextPosition = npcRigidBody.position + currentDirection * Vector2.right * patrolRadius;
        }
        else
        {
            currentDirection = Mathf.Sign(player.transform.position.x - npcRigidBody.position.x);
        }

        if (!npcController.IsKnockedBack)
        {
            npcAnimator.SetBool(PatrolAnimString, true);
            npcAnimator.SetBool("IsIdle", false);
        }

        turningAroundWaitStartTime = -1.0f;
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

        if(turningAroundWaitStartTime > 0.0f)
        {
            if (Time.time - turningAroundWaitStartTime >= turningAroundCurrentWaitDuration)
            {
                // Continue as normal
                turningAroundWaitStartTime = -1.0f;
            }
            else
            {
                npcAnimator.SetBool(PatrolAnimString, false);
                npcAnimator.SetBool("IsIdle", true);
                return State.RUNNING;
            }
        }

        if (npcAnimator.GetBool("IsIdle") && !npcController.IsKnockedBack)
        {
            npcAnimator.SetBool(PatrolAnimString, true);
            npcAnimator.SetBool("IsIdle", false);
        }        

        if (!npcController.IsKnockedBack && npcRigidBody.velocity.magnitude < maxSpeed)
        {
            //Debug.Log($"currentDirection={currentDirection} Moving forward={Vector2.right * currentDirection * moveSpeed * dt}");
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
        if (!obstacleFound)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x, Vector2.down, holeDistance.y, LayerMask.GetMask("Ground"));
            /*Debug.DrawLine(npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x, 
                npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x + Vector2.down * holeDistance.y);
            */
            if (groundHit.collider is null)
            {
                //Debug.Log("No ground found!");
                obstacleFound = true;
            }
        }

        bool playerIsSeen = Time.time - npcController.Blackboard.lastSeenPlayer <= 5.0f;
        //Debug.Log($"Time.time={Time.time} playerIsSeen ={playerIsSeen}");
        if (!obstacleFound)
        {
            //Debug.Log("Checking for player.");
            RaycastHit2D playerHit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * currentDirection, detectionDistance, playerLayer);
            Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * currentDirection * detectionDistance, Color.yellow);
            //RaycastHit2D playerHit = Physics2D.CircleCast(npcRigidBody.position, detectionDistance, Vector2.zero, 0.0f, playerLayer);
            //Platformer2DUtilities.DebugDrawCircle(npcRigidBody.position, detectionDistance, Color.yellow);

            if (playerHit.collider is not null)/* && (Mathf.Sign(player.transform.position.x - npcRigidBody.position.x) == currentDirection || !detectForwardOnly))*/
            {                
                playerIsSeen = true;
            }
            else if (!detectForwardOnly)
            {
                RaycastHit2D backPlayerHit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * -currentDirection, detectionDistance, playerLayer);
                Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * -currentDirection * detectionDistance, Color.yellow);

                if (backPlayerHit.collider is not null)
                {
                    playerIsSeen = true;
                }
            }
        }
        if (playerIsSeen && !obstacleFound)
        {
            npcController.Blackboard.lastSeenPlayer = Time.time;
            //Debug.Log($"Player is seen! npcController.Blackboard.lastSeenPlayer={npcController.Blackboard.lastSeenPlayer}");            
            return State.SUCCESS;
        }

        if (Mathf.Abs(nextPosition.x - npcRigidBody.position.x) < distBeforeChanging ||
            Mathf.Sign(nextPosition.x - npcRigidBody.position.x) != Mathf.Sign(currentDirection) ||
            obstacleFound)
        {
            npcController.Blackboard.lastSeenPlayer = -999.0f;
            //Debug.Log("Turning around!");
            currentDirection = -currentDirection;
            nextPosition = npcRigidBody.position + currentDirection * Vector2.right * 2.0f * patrolRadius;
            if (!npcController.IsKnockedBack)
            {
                npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            }

            turningAroundWaitStartTime = Time.time;
            turningAroundCurrentWaitDuration = Random.Range(turningAroundMinWaitDuration, turningAroundMaxWaitDuration);
        }
        //Debug.Log($"Player is not found! obstacleFound={obstacleFound}");

        return State.RUNNING;
    }
}
