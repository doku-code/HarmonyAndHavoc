using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Charles

[System.Serializable]
public class Inverter : DecoratorNode
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
        if (child == null)
        {
            return State.FAILURE;
        }

        switch (child.DoUpdate(dt))
        {
            case State.RUNNING:
                return State.RUNNING;
            case State.FAILURE:
                return State.SUCCESS;
            case State.SUCCESS:
                return State.FAILURE;
        }
        return State.FAILURE;
    }
}
