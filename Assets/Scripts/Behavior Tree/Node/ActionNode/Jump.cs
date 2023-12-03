using UnityEngine;
using JFM;

//CHARLES
public class Jump : ActionNode
{
    public float jumpForceY = 1f;
    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnStop() 
    {
        npcAnimator.ResetTrigger("Fall");
    }

    protected override State OnUpdate(float dt)
    {
       
        if (player.transform.position.y > npc.transform.position.y + 2f && Raycast2DHelper.CheckGrounded(npcRigidBody.position, 0.14f, 0.1f,
                LayerMask.GetMask("Ground"), LayerMask.GetMask("Ground"), out int Layer))
        {
            npcAnimator.SetTrigger("Jump");


            Vector2 jumpForce = new Vector2(0f, jumpForceY);
            npcRigidBody.AddForce(jumpForce, ForceMode2D.Impulse);
            return State.SUCCESS;
        }
        npcAnimator.ResetTrigger("Fall");
        return State.FAILURE;
    }
}
