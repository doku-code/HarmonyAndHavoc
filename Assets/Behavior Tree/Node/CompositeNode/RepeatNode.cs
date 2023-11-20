//Charles
public class RepeatNode : CompositeNode
{
    public override void OnInitialize()
    {
        
    }

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        foreach (var child in children)
        {
            child.Update();
        }
        return State.RUNNING;
    }
}
