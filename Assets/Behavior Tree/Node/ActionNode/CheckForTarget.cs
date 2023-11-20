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
    protected override State OnUpdate()
    {        
        float distanceToPlayer = Vector3.Distance(npc.transform.position, 
            player.transform.position);

        if (distanceToPlayer <= attackDistance)
        {            
            //Debug.Log("Player in sight...");
                
            return State.SUCCESS;
            
        }
        else 
        {
            return State.FAILURE;
        }        
    }
}
