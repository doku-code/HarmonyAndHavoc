using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Charles
public class NPCPathFinding : ActionNode
{
    public GameObject[] POSPatrolRoute;
    public string PatrolAnimString;
    public string gameobjNpcName;
    public float moveSpeed = 2000.0f;

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
        if (POSPatrolRoute.Length == 0)
        {
            Debug.LogWarning("You forgot to add the waypoint in the Behavior Tree");
            return State.FAILURE;
        }

        Vector3 targetPOS = POSPatrolRoute[currentPOS].transform.position;
        Vector3 moveDirection = (targetPOS - npc.transform.position).normalized;
        npcRigidBody.velocity = moveDirection * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(npc.transform.position, targetPOS) < 1f) // 1f est la tolerance de la distance entre le point et le transform du npc
        {
            currentPOS++;
            if (currentPOS >= POSPatrolRoute.Length)
            {
                npcRigidBody.velocity = Vector3.zero; // ajout pour briser le continue de mouvement en kinematique
                return State.SUCCESS;
            }
        }

        if (moveDirection.x < 0)
        {
            npcSpriteRenderer.flipX = true;
        }
        else if (moveDirection.x > 0)
        {
            npcSpriteRenderer.flipX = false;
        }

        return State.RUNNING;
    }
}
