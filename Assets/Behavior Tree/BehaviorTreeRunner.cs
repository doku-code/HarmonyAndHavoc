using System.Collections;
using UnityEngine;

//Charles
public class BehaviorTreeRunner : MonoBehaviour
{
    [SerializeField] float tickInterval;
    public BehaviourTree tree;
    private IEnumerator tickCoroutine;
    void Start()
    {
        tree = tree.Clone();
        tickCoroutine = Tick();
        StartCoroutine(tickCoroutine);
    }
    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            tree.Update();
        }
    }
}

