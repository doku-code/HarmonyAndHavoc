using System.Collections.Generic;
using UnityEngine;
//Charles
public class SequencerRetry : CompositeNode
{
    int current;
    [SerializeField] private float retryDelay = 1.0f;
    private Dictionary<Node, float> failedNodeTimers = new Dictionary<Node, float>();

    protected override void OnStart()
    {
        current = 0;
        failedNodeTimers.Clear();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        var child = children[current];

        if (failedNodeTimers.ContainsKey(child))
        {
            failedNodeTimers[child] += Time.deltaTime;

            if (failedNodeTimers[child] >= retryDelay)
            {
                failedNodeTimers.Remove(child);
            }
            else
            {
                return State.RUNNING;
            }
        }

        switch (child.Update())
        {
            case State.RUNNING:
                return State.RUNNING;
            case State.FAILURE:
                failedNodeTimers[child] = 0.0f;
                return State.RUNNING;
            case State.SUCCESS:
                current++;
                break;
        }
        return current == children.Count ? State.SUCCESS : State.RUNNING;
    }
}