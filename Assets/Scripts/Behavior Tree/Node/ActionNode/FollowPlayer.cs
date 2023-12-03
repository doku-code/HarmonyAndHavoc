using charles;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayer : ActionNode
{
    public string runAnimString;

    public float followDistanceX = 0.2f;
    public float followDistanceY = 0.2f;
    public float maxDistance = 10f;
    public float followSpeedX = 250f;
    public float followSpeedY = 250f;
    public bool followOnY = false;
    public LayerMask obstacleLayer;
    public float distanceOffset = 1f;
    public float stoppingDistance = 0.1f;
    
    void OnEnable()
    {
        runAnimString = "Run";
    }

    
    protected override void OnStart() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnStop() {}

    protected override State OnUpdate(float dt)
    {

        if (npcController.IsDead)
        {
            return State.FAILURE;
        }
        EnemyController enemyController = npc.GetComponent<EnemyController>();

        float distanceX = player.transform.position.x - npc.transform.position.x;
        float distanceY = player.transform.position.y - npc.transform.position.y;

        Vector2 vecDirection = Vector2.right;

        if (player.transform.position.x > npc.transform.position.x)
        {
            npc.transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            npc.transform.localScale = new Vector3(-1, 1, 1);
            vecDirection = -vecDirection;

        }

        if (distanceX > maxDistance || distanceX < -maxDistance)
        {
            return State.FAILURE;
        }

        float followSpeed = followOnY ? followSpeedY : followSpeedX;
        float followDistance = followOnY ? followDistanceY : followDistanceX;
        float targetX = player.transform.position.x - (vecDirection.x * (followDistance + distanceOffset));
        float targetY = followOnY ? player.transform.position.y + (followDistance + distanceOffset) : npc.transform.position.y;
        float distanceToTarget = Vector2.Distance(npc.transform.position, new Vector2(targetX, targetY));

        Vector2 followDirection = new Vector2(targetX - npc.transform.position.x, targetY - npc.transform.position.y).normalized;
        RaycastHit2D hit = Physics2D.Raycast(npcRigidBody.position + vecDirection * 1f, Vector2.down, 1f, obstacleLayer);

        if (hit.collider == null && !followOnY)
        {
            ChangeNpcVelocity(Vector2.zero);

            return State.RUNNING;
        }
        if (npcAnimator.GetCurrentAnimatorStateInfo(0).IsName("Combo1")||
            npcAnimator.GetCurrentAnimatorStateInfo(0).IsName("Combo2")||
            npcAnimator.GetCurrentAnimatorStateInfo(0).IsName("Combo3"))             
        {
            ChangeNpcVelocity(Vector2.zero);
            
            npcAnimator.SetBool(runAnimString, false);

            return State.RUNNING;
        }
        if (distanceToTarget > stoppingDistance)
        {
            ChangeNpcVelocity( followDirection * followSpeed * Time.fixedDeltaTime);
            npcAnimator.SetBool(runAnimString, true);
            
            return State.RUNNING;
        }
        else
        {
            ChangeNpcVelocity(Vector2.zero);
            
            npcAnimator.SetBool(runAnimString, false);

            return State.SUCCESS;
        }
    }
}
