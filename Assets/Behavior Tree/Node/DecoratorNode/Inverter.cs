using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Inverter : DecoratorNode
{
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

        switch (child.Update())
        {
            case State.RUNNING:
                return State.RUNNING;
            case State.FAILURE:
                return State.SUCCESS;
            case State.SUCCESS:
                return State.FAILURE    ;
        }
        return State.FAILURE;
    }
}
