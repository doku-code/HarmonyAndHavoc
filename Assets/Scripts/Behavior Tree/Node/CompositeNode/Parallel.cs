using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//charles 
[System.Serializable]
public class Parallel : CompositeNode
{
    List<State> childrenLeftToExecute = new List<State>();

    public override void OnInitialize(GameObject go)
    {

    }

    protected override void OnStart()
    {
        childrenLeftToExecute.Clear();
        children.ForEach(a =>
        {
            childrenLeftToExecute.Add(State.RUNNING);
        });
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate(float dt)
    {
        bool stillRunning = false;
        for (int i = 0; i < childrenLeftToExecute.Count(); ++i)
        {
            if (childrenLeftToExecute[i] == State.RUNNING)
            {
                var status = children[i].DoUpdate(dt);
                if (status == State.FAILURE)
                {
                    AbortRunningChildren();
                    return State.FAILURE;
                }

                if (status == State.RUNNING)
                {
                    stillRunning = true;
                }

                childrenLeftToExecute[i] = status;
            }
        }

        return stillRunning ? State.RUNNING : State.SUCCESS;
    }

    void AbortRunningChildren()
    {
        for (int i = 0; i < childrenLeftToExecute.Count(); ++i)
        {
            if (childrenLeftToExecute[i] == State.RUNNING)
            {
                children[i].Abort();
            }
        }
    }
}
