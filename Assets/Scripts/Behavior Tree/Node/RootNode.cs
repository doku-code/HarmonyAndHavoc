using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Charles
public class RootNode : Node
{
    public Node child;

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
        return child.DoUpdate(dt);
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
