//Charles

using System;
using charles;
using UnityEngine;

public abstract class ActionNode : Node
{
    public LayerMask playerLayer;
    
    protected GameObject player;
    protected GameObject npc;
    protected Animator npcAnimator;
    protected Rigidbody2D npcRigidBody;
    protected EnemyController npcController;

    public override void OnInitialize(GameObject go)
    {        
        //npc = GameObject.Find(gameobjNpcName);
        npc = go;
        if (npc is not null)
        {
            npcAnimator = npc.GetComponent<Animator>();
            npcRigidBody = npc.GetComponent<Rigidbody2D>();
            npcController = npc.GetComponent<EnemyController>();
        }
        else
        {
            Debug.Log($"NPC not found : desc={Description}");
        }
    }

    protected void ChangeNpcVelocity(Vector2 newVelocity)
    {
        if (npcController is null || !npcController.IsKnockedBack)
        {
            if (npcRigidBody is not null)
            {
                npcRigidBody.velocity = newVelocity;
            }
        }
    }
}
