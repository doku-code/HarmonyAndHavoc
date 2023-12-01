using UnityEngine;

//Charles
public class WaitNode : ActionNode
{
    public float duration = 1f;
    private float startTime;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate(float dt)
    {
        if (Time.time - startTime > duration)
        {
            return State.SUCCESS;
        }
        return State.RUNNING;
    }
}
