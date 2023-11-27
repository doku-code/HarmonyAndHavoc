using UnityEngine;
using UnityEngine.UIElements;

//CHARLES
public class Jump : ActionNode
{
    public float jumpForceY = 10f;
    public float horizontalSpeed = 5f;
        public string obstacleLayer;
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
       
        if (player.transform.position.y > npc.transform.position.y + 2f)
        {
            npcAnimator.SetTrigger("Jump");


            Vector2 jumpForce = new Vector2(0f, jumpForceY);
            npcRigidBody.AddForce(jumpForce, ForceMode2D.Impulse);

            //Vector2 direction = player.transform.position - npc.transform.position;
            //direction.Normalize();

            //Vector2 horizontalMovement = direction * horizontalSpeed;
            //npcRigidBody.velocity = new Vector2(horizontalMovement.x, npcRigidBody.velocity.y);
            //Debug.Log("Je suis dans le state Jump et J'arrive  pas sortir ");
            return State.SUCCESS;
        }
        npcAnimator.ResetTrigger("Fall");
        return State.FAILURE;
    }
}
