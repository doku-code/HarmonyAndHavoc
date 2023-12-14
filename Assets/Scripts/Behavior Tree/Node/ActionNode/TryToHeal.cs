using charles;
using JFM;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

//Charles
public class TryToHeal : ActionNode
{
    
    public string healingAnimString;
    public float healingChance = .1f;
    public float healingWaitDuration = 2.0f;

    
    private float currentDirection;
    private float healingWaitStartTime = -1.0f;

    void OnEnable()
    {
        //healingAnimString = "Run";
    }

    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //Debug.Log($"OnStart()");

        if (currentDirection == 0.0f)
        {
            currentDirection = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;            
        }
        else
        {
            currentDirection = Mathf.Sign(player.transform.position.x - npcRigidBody.position.x);
        }

        healingWaitStartTime = -1.0f;
    }

    protected override void OnStop()
    {        
    }

    protected override State OnUpdate(float dt)
    {
        if (npcController.IsDead)
        {
            return State.FAILURE;
        }

        if (npcController.IsKnockedBack)
        {
            return State.FAILURE;
        }

        if (Random.Range(0.0f, 1.0f) <= healingChance)
        {
            healingWaitStartTime = Time.time;
            if (!npcController.IsKnockedBack)
            {
                npcAnimator.SetBool(healingAnimString, true);
                npcAnimator.SetBool("IsIdle", false);
            }
        }
        else
        {
            return State.SUCCESS;
        }

        if (healingWaitStartTime > 0.0f)
        {
            if (Time.time - healingWaitStartTime >= healingWaitDuration)
            {
                // Continue as normal
                healingWaitStartTime = -1.0f;
            }
            else
            {
                //npcAnimator.SetBool(healingAnimString, false);
                //npcAnimator.SetBool("IsIdle", true);
                //return State.RUNNING;
            }
        }

        if (npcAnimator.GetBool("IsIdle") && !npcController.IsKnockedBack)
        {
            npcAnimator.SetBool(healingAnimString, true);
            npcAnimator.SetBool("IsIdle", false);
        }        

        npcController.TurnSide(currentDirection);

        /*if (Mathf.Abs(nextPosition.x - npcRigidBody.position.x) < distBeforeChanging ||
            Mathf.Sign(nextPosition.x - npcRigidBody.position.x) != Mathf.Sign(currentDirection))
        {
            npcController.Blackboard.lastSeenPlayer = -999.0f;
            //Debug.Log("Turning around!");
            currentDirection = -currentDirection;
            nextPosition = npcRigidBody.position + currentDirection * Vector2.right * 2.0f * patrolRadius;
            if (!npcController.IsKnockedBack)
            {
                npcRigidBody.AddForce(-npcRigidBody.velocity, ForceMode2D.Impulse);
            }

            healingWaitStartTime = Time.time;
        }*/
        //Debug.Log($"Player is not found! obstacleFound={obstacleFound}");

        return State.RUNNING;
    }
}
