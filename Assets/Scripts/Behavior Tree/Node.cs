using UnityEngine;
//Charles
public abstract class Node : ScriptableObject
{
    public enum State
    {
        RUNNING,
        FAILURE,
        SUCCESS
    }
    [HideInInspector] public State state = State.RUNNING;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [TextArea] public string Description;
    public State DoUpdate(float dt)
    {
        if (!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate(dt);

        if (state == State.FAILURE || state == State.SUCCESS)
        {
            OnStop();
            started = false;
        }
        return state;
    }

    public virtual Node Clone()
    {
        started = false;
        return Instantiate(this);
    }
    public void Abort()
    {
        BehaviourTree.Traverse(this, (node) => 
        {
            node.started = false;
            node.state = State.RUNNING;
            node.OnStop();
        });
    }

    public abstract void OnInitialize(GameObject go);
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate(float dt);
}