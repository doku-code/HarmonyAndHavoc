using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Charles

[System.Serializable]
public class Selector : CompositeNode
{
    protected int current;

    public override void OnInitialize(GameObject go)
    {

    }

    protected override void OnStart()
    {
        current = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate(float dt)
    {
        for (int i = current; i < children.Count; ++i)
        {
            current = i;
            var child = children[current];

            switch (child.DoUpdate(dt))
            {
                case State.RUNNING:
                    return State.RUNNING;
                case State.SUCCESS:
                    return State.SUCCESS;
                case State.FAILURE:
                    continue;
            }
        }
        return State.FAILURE;
    }
}
