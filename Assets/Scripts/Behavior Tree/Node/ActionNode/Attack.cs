//#define _DEBUG

using AF;
using JFM;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

//Charles
public class Attack : ActionNode
{
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

        State returnedState = State.RUNNING;
        bool playerIsSeen = Time.time - npcController.Blackboard.lastSeenPlayer <= 5.0f;
        float currentDirection = Mathf.Sign(player.transform.position.x - npc.transform.position.x);
        if (!playerIsSeen)
        {
            RaycastHit2D playerHit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * currentDirection, detectionDistance, playerLayer);
            Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * currentDirection * detectionDistance, Color.yellow);
            //RaycastHit2D playerHit = Physics2D.CircleCast(npcRigidBody.position, detectionDistance, Vector2.zero, 0.0f, playerLayer);
            //Platformer2DUtilities.DebugDrawCircle(npcRigidBody.position, detectionDistance, Color.yellow);

            if (playerHit.collider is null)
            {
                RaycastHit2D backPlayerHit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * currentDirection, detectionDistance, playerLayer);
                Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * currentDirection * detectionDistance, Color.yellow);

                if (backPlayerHit.collider is null)
                {
                    returnedState = State.FAILURE;
                }
                else
                {
                    playerIsSeen = true;
                }
            }
            else
            {
                playerIsSeen = true;
            }
        }

        if(playerIsSeen)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x, Vector2.down, holeDistance.y, LayerMask.GetMask("Ground"));
            /*Debug.DrawLine(npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x, 
                npcRigidBody.position + Vector2.right * currentDirection * holeDistance.x + Vector2.down * holeDistance.y);
            */
            if (groundHit.collider is null)
            {
                Debug.Log("No ground found!");
                npcController.Blackboard.lastSeenPlayer = -999.0f;
                return State.FAILURE;
            }
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

            Vector3 moveDirection = new Vector3(Mathf.Sign(player.transform.position.x - npc.transform.position.x), 0.0f, 0.0f);

            // "IsIdle" doesn't get set since attacks have "Exit Times", but "Idle" motion
            // is played so we can check when it's actually the case
            //if (npcAnimator.GetBool("IsIdle"))
            if(npcAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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

            if (!npcController.IsKnockedBack && npcRigidBody.velocity.magnitude < maxSpeed)
            {
                npcRigidBody.AddForce(moveDirection * moveSpeed * dt);
            }            

            return returnedState;
        }
        
#if _DEBUG
        Debug.Log($"Attack node. {attackClipName} returns FAILURE. distanceToPlayer <= attackDistance {distanceToPlayer} <= {attackDistance} {leaveDistance}");
#endif

        return State.FAILURE;
    }    
}
