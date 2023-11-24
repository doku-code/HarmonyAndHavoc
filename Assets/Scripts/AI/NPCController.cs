using System.Collections;
using UnityEngine.AI;
using Fineallday.BT;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Fineallday
{
    public class NPCController : MonoBehaviour
    {
        public NavMeshAgent nMAgent;
        [SerializeField] private LayerMask ennemisMask;
        [SerializeField] private Vector3 destinationTarget;
        [SerializeField] private Node behaviorTree;
        [SerializeField] private NodeExecutionState treeState;
        private IEnumerator tickCoroutine;
        [SerializeField] private float interval;
        [SerializeField] public List<bool> patrolTargetReached = new List<bool> { false, false };
        [SerializeField] private int life = 20;
        public PatrolRoutes patrolRoutes;
        void Awake()
        {
            nMAgent = GetComponent<NavMeshAgent>();
            patrolRoutes = GetComponent<PatrolRoutes>();
            //InvokeRepeating("Tick", 0f, 0.5f); // Vérification de l'arbre à toutes les 1/2 seconde
           
            // À retoucher
            behaviorTree.children[0].children[0].children[0].evalMethod = FleeEval;
            behaviorTree.children[0].children[0].children[1].evalMethod = AttackEval;
         
            
            //Garder
            tickCoroutine = Tick(interval);
            StartCoroutine(tickCoroutine);
        }

        //Garder
        private IEnumerator Tick(float waitTime)
        {
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                treeState = behaviorTree.evalMethod(this);
               // Debug.Log(Time.time + " Seconds");
                Debug.Log(treeState);
            }
        }
        
        //Méthodes pour mes leaves
        
        private NodeExecutionState FleeEval(Object caller)
        {
            Debug.Log("Flee Execution");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10, ennemisMask);
            if (life < 10 && hitColliders.Any() )
            {
                ChoseDestination();
                nMAgent.destination = destinationTarget;
                return NodeExecutionState.RUNNING;
            }
            else
            {
                return NodeExecutionState.FAILED;  
            }
        } 
        private NodeExecutionState AttackEval(Object caller)
        {
            Debug.Log("Attack Execution");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10, ennemisMask);
            if (hitColliders.Any())
            {
                destinationTarget = hitColliders[0].transform.position;
                nMAgent.destination = destinationTarget;
                return NodeExecutionState.RUNNING;
            }
            else
            {
                return NodeExecutionState.FAILED;  
            }
        }
        
       
        void ChoseDestination()
        {
        
            destinationTarget = Vector3.zero;
            //nMAgent.destination = destinationTarget;

            while ( (destinationTarget == Vector3.zero))
            {
                Vector3 randomPoint = Random.insideUnitSphere * 100 + transform.position;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPoint, out hit, 100, NavMesh.AllAreas))
                {
                    destinationTarget = hit.position;
                    //SetDestination();
                    //nMAgent.destination = destinationTarget;
                }
            }
       
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(destinationTarget, "Assets/Gizmos/cible.png"); 
        }
    }
}