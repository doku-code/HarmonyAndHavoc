using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Charles
public class Breakpoint : ActionNode
{
    protected override void OnStart()
    {
        Debug.Log("Trigging Breakpoint");
        Debug.Break();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate(float dt)
    {
        return State.SUCCESS;
    }
}
