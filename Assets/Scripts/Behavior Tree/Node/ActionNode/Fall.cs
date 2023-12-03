using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : ActionNode
{
    public float fallForceY = 10f;
    public float horizontalSpeed = 5f;
    protected override void OnStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnStop()
    {
        npcAnimator.ResetTrigger("Fall");
    }


    protected override State OnUpdate(float dt)
    {
        npcAnimator.SetTrigger("Fall");
        return State.SUCCESS;
    }
}
