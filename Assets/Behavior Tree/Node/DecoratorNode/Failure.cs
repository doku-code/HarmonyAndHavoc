using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Charles

[System.Serializable]
public class Failure : DecoratorNode
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

    protected override State OnUpdate()
    {
        if (child == null)
        {
            return State.FAILURE;
        }

        var state = child.Update();
        if (state == State.SUCCESS)
        {
            return State.FAILURE;
        }
        return state;
    }
}
