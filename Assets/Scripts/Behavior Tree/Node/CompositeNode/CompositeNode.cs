using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//Charles
public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> children = new List<Node>();

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}
