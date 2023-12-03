using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Charles
public class RepeatUntilSuccess : CompositeNode
{
    private int currentChildIndex = 0;

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
        if (children.Count == 0)
        {
            return State.SUCCESS;
        }

        Node currentChild = children[currentChildIndex];
        State childState = currentChild.DoUpdate(dt);

        if (childState == State.FAILURE)
        {
            currentChildIndex++;
            if (currentChildIndex >= children.Count)
            {
                currentChildIndex = 0;
                return State.FAILURE;
            }
        }
        else if (childState == State.SUCCESS)
        {
            currentChildIndex = 0;
        }

        return State.RUNNING;
    }
}