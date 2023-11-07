using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemieController : MonoBehaviour
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