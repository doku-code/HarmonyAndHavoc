//Charles
using charles;
using UnityEngine;

public abstract class ActionNode : Node
{
    public LayerMask playerLayer;
    public string gameobjNpcName;

    protected GameObject player;
    protected GameObject npc;
    protected Animator npcAnimator;
    protected Rigidbody2D npcRigidBody;
    protected EnemyController npcController;

    public override void OnInitialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        npc = GameObject.Find(gameobjNpcName);
        if (npc is not null)
        {
            npcAnimator = npc.GetComponent<Animator>();
            npcRigidBody = npc.GetComponent<Rigidbody2D>();
            npcController = npc.GetComponent<EnemyController>();
        }
    }

    protected void ChangeNpcVelocity(Vector2 newVelocity)
    {
        if (!npcController.IsPushedBack)
        {
            npcRigidBody.velocity = newVelocity;
        }
    }
}
