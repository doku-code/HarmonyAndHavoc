//#define _DEBUG

using AF;
using JFM;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

//Charles
public class AirAttack : ActionNode
{
    public float patrolRadius = 3.0f;
    public float attackCooldown = 1.5f;
    public float attackDistance = 1.5f;
    public int attackDamage = 1;
    public float detectionDistance = 3.0f;
    public bool detectForwardOnly = true;
    public float leaveDistance = 5.0f;
    public string attackAnimString;
    public string attackClipName;
    public int comboSoundIdx;
    public int animatorLayer = 0;

    public float moveSpeed = 1000.0f;
    public float maxSpeed = 2.0f;
    public Vector2 holeDistance = new Vector2(1.0f, 1.5f);
    public float obstacleDistance = 1.5f;
    [Tooltip("In degrees")]
    public float angleMaxDeviance = 5.0f;   

    private bool hasAttacked = false;
    private float lastAttackTime = 1.0f;
    private float attackAnimationLength;

    private Vector2 currentDirection;
    private Vector2 lastPosition;

    public int maxFramesFacingSide = 5;
    private int nFramesFacingSide;
    private float sideFacing;    

    public float minMoveThreshold = 0.01f;
    public int maxFramesWithoutMoving = 5;
    private int nFrames;
    private int noMoveFrameIndex;
    private float totalMoveDistance;
    protected override void OnStart() 
    {
        player = GameObject.FindGameObjectWithTag("Player");        
        hasAttacked = false;        
        npcAnimator.ResetTrigger(attackAnimString);
        lastAttackTime = Time.time;

        nFramesFacingSide = maxFramesFacingSide;
        sideFacing = npc.transform.localScale.x;
        lastPosition = Vector2.zero;
        nFrames = 0;
        noMoveFrameIndex = -1;
        totalMoveDistance = 0;
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

        State returnedState = State.RUNNING;
        bool playerIsSeen = Time.time - npcController.Blackboard.lastSeenPlayer <= 5.0f;
        Vector2 lastDirection = currentDirection;
        Vector2 playerDistance = player.GetComponent<Rigidbody2D>().position - npcRigidBody.position;
        currentDirection = playerDistance.normalized;

        if (!playerIsSeen)
        {
            //RaycastHit2D playerHit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * currentDirection, detectionDistance, playerLayer);
            //Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * currentDirection * detectionDistance, Color.yellow);
            RaycastHit2D playerHit = Physics2D.CircleCast(npcRigidBody.position, detectionDistance, Vector2.zero, 0.0f, playerLayer);
            Platformer2DUtilities.DebugDrawCircle(npcRigidBody.position, detectionDistance, Color.yellow);

            if (playerHit.collider is null)
            {
                npcController.Blackboard.lastSeenPlayer = -999.0f;
                returnedState = State.FAILURE;               
            }
            else
            {
                playerIsSeen = true;
            }
        }

        if(playerIsSeen)
        {
            CapsuleCollider2D capsule = npc.GetComponent<CapsuleCollider2D>();
            //Debug.Log($"obstacleDistance={obstacleDistance}");

            float distance = capsule.size.y - capsule.size.x;
            float radius = capsule.size.x / 2.0f;

            Vector2 position = npcRigidBody.position +
                                Vector2.up * capsule.offset.y -
                                Vector2.up * (capsule.size.y / 2.0f - radius) +
                                Vector2.right * capsule.offset.x +
                                currentDirection * obstacleDistance;

            Platformer2DUtilities.DebugDrawCircle(position, radius, Color.yellow);
            Platformer2DUtilities.DebugDrawCircle(position + Vector2.up * distance, radius, Color.yellow);

            //RaycastHit2D[] obstacleHits = Physics2D.CircleCastAll(npcRigidBody.position, obstacleDistance, Vector2.zero, 0.0f, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));        
            RaycastHit2D[] obstacleHits = Physics2D.CircleCastAll(position, radius, Vector2.up, distance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));
            
            bool obstacleFound = false;
            Vector2 obstacleNormal = Vector2.zero;
            //Debug.Log("Testing for collisions");
            if (obstacleHits is not null)
            {
                foreach (RaycastHit2D hit in obstacleHits)
                {
                    if (hit.collider.gameObject != npc)
                    {
#if _DEBUG
                        Debug.Log($"hit={hit.collider.gameObject.name} hit.normal={hit.normal}");
#endif
                        obstacleFound = true;
                        obstacleNormal = hit.normal;
                        /*Vector2 perpendicularDirection = Platformer2DUtilities.GetPerpendicularVector2(currentDirection);
                        RaycastHit2D obstacleHit2 = Physics2D.Raycast(npcRigidBody.position + perpendicularDirection * 0.1f, currentDirection, obstacleDistance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));

                        if (obstacleHit2.collider.gameObject == hit.collider.gameObject)
                        {
                            obstacleNormal = hit.normal;
                        }*/
                        //break;
                    }
                }
            }            

            if (obstacleFound)
            {
                bool obstacleBetweenFound = false;
                RaycastHit2D[] obstacleBetween = Physics2D.RaycastAll(npcRigidBody.position, currentDirection, playerDistance.magnitude, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Ground"));

                if (obstacleBetween is not null)
                {
                    foreach (RaycastHit2D hit in obstacleBetween)
                    {
                        if (hit.collider.gameObject != npc)
                        {
                            obstacleBetweenFound = true;
#if _DEBUG
                            Debug.Log("Found an obstacle between player and NPC.");
#endif
                        }
                    }
                }

                if (obstacleBetweenFound)
                {
                    npcController.Blackboard.lastSeenPlayer = -999.0f;
                    npcController.Blackboard.playerForgetTime = Time.time;

                    return State.FAILURE; // RUNNING;
                }                
            }
        }

#if _DEBUG
        Debug.Log($"Attack node. {attackClipName}");
#endif

        float distanceToPlayer = playerDistance.magnitude;

        // Manage distance to player
        if (distanceToPlayer <= attackDistance)
        {
#if _DEBUG
            Debug.Log("Attempting to attack...");
#endif

            // Turn NPC to face Player
            npcController.TurnSide(Mathf.Sign(player.transform.position.x - npc.transform.position.x));            

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

                if (!npcController.IsKnockedBack)
                {
                    npcAnimator.ResetTrigger(attackAnimString);
                    npcAnimator.SetBool("IsIdle", true);
                }

                if (returnedState == State.FAILURE)
                {
                    npcController.Blackboard.lastSeenPlayer = -999.0f;
                    npcController.Blackboard.playerForgetTime = Time.time;
                    return returnedState;
                }                
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

            if (!npcController.IsKnockedBack 
                &&
                (
                    !hasAttacked 
                    || 
                    (
                        elapsedTime >= attackCooldown 
                        && 
                        elapsedTime >= attackAnimationLength)
                    )
                )
            {
                if (hasAttacked)
                {
#if _DEBUG
                    Debug.Log($"Attack node. {attackClipName} Has attacked and returns SUCCESS.");
#endif
                    if(returnedState == State.RUNNING)
                    {
                        returnedState = State.SUCCESS;
                    }
                    return returnedState;
                }

                npcAnimator.SetBool("Run", false);
                npcAnimator.SetBool("IsIdle", false);
                npcAnimator.SetTrigger(attackAnimString);                

                npcController.AttackDamage = attackDamage;
                lastAttackTime = Time.time;
                hasAttacked = true;
#if _DEBUG
                Debug.Log($"Attack node. {attackClipName} Has attacked. ^^^^^^^^^^^^^^^^^^^^^^^^^");
#endif

                if (SoundManager.Instance is not null)
                {
                    SoundManager.Instance.PlayNpcSounds(comboSoundIdx);
                }
                return returnedState;
            }

            if (npcAnimator.GetBool("Run"))
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
            return returnedState;
        }
        else if(distanceToPlayer <= leaveDistance)
        {
            
#if _DEBUG
            Debug.Log($"Attack node. {attackClipName} Approaching to attack...");
#endif

            if (npcAnimator.GetBool("IsIdle"))
            {
                npcAnimator.SetBool("IsIdle", false);
                npcAnimator.SetBool("Run", true);
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
                if (nFramesFacingSide < 0)
                {
                    nFramesFacingSide = 0;
                }
            }

            float angleDifference = Mathf.Abs(Mathf.Atan2(lastDirection.y, lastDirection.x) - Mathf.Atan2(currentDirection.y, currentDirection.x));
            //Debug.Log($"angleDifference={angleDifference}");

            if (!npcController.IsKnockedBack && (npcRigidBody.velocity.magnitude < maxSpeed || angleDifference >= angleMaxDeviance * Mathf.Deg2Rad))
            {
                npcRigidBody.AddForce(currentDirection * moveSpeed * dt);
#if _DEBUG
                Debug.Log($"currentDirection * moveSpeed * dt = {currentDirection * moveSpeed * dt}");
#endif
            }

            //Debug.Log($"Player is not found! obstacleFound={obstacleFound}");

            // Check if moved distance is != 0
            float movedDistance = Vector2.Distance(npcRigidBody.position, lastPosition);
            if (movedDistance <= minMoveThreshold && noMoveFrameIndex == -1)
            {
                noMoveFrameIndex = nFrames;
                totalMoveDistance = 0;
            }

            if(noMoveFrameIndex >= 0)
            {
#if _DEBUG
                Debug.Log($"returnedState={returnedState} {noMoveFrameIndex} + 5 == {nFrames} totalMoveDistance={totalMoveDistance}");
#endif
                totalMoveDistance += movedDistance;

                if (nFrames >= noMoveFrameIndex + maxFramesWithoutMoving)
                {
                    noMoveFrameIndex = -1;
                    if (totalMoveDistance <= minMoveThreshold * maxFramesWithoutMoving)
                    {
#if _DEBUG
                        Debug.Log($"Not moving enough! Back to patrolling...");
#endif
                        npcController.Blackboard.lastSeenPlayer = -999.0f;
                        npcController.Blackboard.playerForgetTime = Time.time;
                        return State.FAILURE;
                    }
                }
            }                      

            lastPosition = npcRigidBody.position;
            nFrames++;
            return returnedState;
        }
        
#if _DEBUG
        Debug.Log($"Attack node. {attackClipName} returns FAILURE. distanceToPlayer <= attackDistance {distanceToPlayer} <= {attackDistance} || <= {leaveDistance}");
#endif
        npcController.Blackboard.playerForgetTime = Time.time;
        npcController.Blackboard.lastSeenPlayer = -999.0f;
        return State.FAILURE;
    }    
}
