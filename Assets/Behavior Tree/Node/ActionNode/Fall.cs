using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : ActionNode
{
    public float fallForceY = 10f;
    public float horizontalSpeed = 5f;
    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnStop()
    {
        npcAnimator.ResetTrigger("Fall");
    }
      

    protected override State OnUpdate()
    {

        if (player.transform.position.y -2f < npc.transform.position.y)
        {
            npcAnimator.SetTrigger("Fall");

            
            //npcRigidBody.AddForce(Vector2.down * fallForceY, ForceMode2D.Impulse);

            //Vector2 direction = player.transform.position - npc.transform.position;
            //direction.Normalize();

            //Vector2 horizontalMovement = direction * horizontalSpeed;
            //npcRigidBody.velocity = new Vector2(horizontalMovement.x, npcRigidBody.velocity.y);

            return State.SUCCESS;
        }
        npcAnimator.ResetTrigger("Fall");
        return State.FAILURE;
    }
}
