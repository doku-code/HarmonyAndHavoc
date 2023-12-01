using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Charles
[System.Serializable]
public class TimeOut : DecoratorNode
{
    public float duration = 1.0f;
    float startTime;

    public override void OnInitialize(GameObject go)
    {

    }

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate(float dt)
    {
        if (child == null)
        {
            return State.FAILURE;
        }

        if (Time.time - startTime > duration)
        {
            return State.FAILURE;
        }

        return child.DoUpdate(dt);
    }
}
