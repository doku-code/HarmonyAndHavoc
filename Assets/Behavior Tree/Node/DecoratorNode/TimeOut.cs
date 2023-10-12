using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TimeOut : DecoratorNode
{
    public float duration = 1.0f;
    float startTime;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (child == null)
        {
            return State.FAILURE;
        }

        if (Time.time - startTime > duration)
        {
            return State.FAILURE;
        }

        return child.Update();
    }
}
