using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Charles
public class Sequencer : CompositeNode
{
    int current;

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
        var child = children[current];
        switch (child.DoUpdate(dt))
        {
            case State.RUNNING:
                return State.RUNNING;
            case State.FAILURE:
                return State.FAILURE;
            case State.SUCCESS:
                current++;
                break;
        }
        return current == children.Count ? State.SUCCESS : State.RUNNING;
    }
}
