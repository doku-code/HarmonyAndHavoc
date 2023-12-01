using AF;
using UnityEngine;
//Charles
public class CheckForTarget : ActionNode
{
    public float attackDistance = 1.5f;
    
    protected override void OnStart() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    protected override void OnStop() {}
    protected override State OnUpdate(float dt)
    {        
        float facing = npcRigidBody.velocity.x > 0 ? 1.0f : -1.0f;

        RaycastHit2D hit = Physics2D.Raycast(npcRigidBody.position, Vector2.right * facing, attackDistance, playerLayer);
        Debug.DrawLine(npc.transform.position, npc.transform.position + Vector3.right * facing * attackDistance, Color.yellow);
        
        if (hit.collider is not null)
        {            
            Debug.Log("Player in sight...");
                
            return State.SUCCESS;            
        }
        else 
        {
            return State.FAILURE;
        }        
    }
}
