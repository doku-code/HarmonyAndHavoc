using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;


public class NPCDemo : MonoBehaviour
{
    private NavMeshAgent nMAgent;
    [SerializeField] private Vector3 destinationTarget;
    [SerializeField] private bool random;

    void Awake()
    {
        nMAgent = GetComponent<NavMeshAgent>();
        ChooseDestination();
    }

void ChooseDestination()
{
    if (random)
    {
        FindRandomPosition();
    }
    nMAgent.destination = destinationTarget;
}

void FindRandomPosition()
{
    Vector3 randomPoint = Random.insideUnitSphere * 100;
    NavMeshHit hit;
    NavMesh.SamplePosition(randomPoint, out hit, 100, NavMesh.AllAreas);
    destinationTarget = hit.position;
}

private void Update()
{
    if (nMAgent.remainingDistance < 0.2f)
    {
        ChooseDestination();
    }
}


private void OnDrawGizmos()
{
    Gizmos.DrawIcon(destinationTarget, "Assets/Gizmos/cible.png");
}
}