//Charles
using UnityEngine;

public class RepeatNode : CompositeNode
{
    public override void OnInitialize(GameObject go)
    {
        
    }

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate(float dt)
    {
        foreach (var child in children)
        {
            child.DoUpdate(dt);
        }
        return State.RUNNING;
    }
}
