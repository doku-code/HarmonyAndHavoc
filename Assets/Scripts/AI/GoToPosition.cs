using Fineallday.BT;
using UnityEngine;
using UnityEngine.AI;

namespace Fineallday
{
    [CreateAssetMenu(fileName = "NewTargetPosition", menuName = "ScriptableObjects/Leaves/TargetPosition")]
    public class GoToPosition : LeafEvalGeneric
    {
        public int positionTargetIndex;
        public override NodeExecutionState evalMethod(Object callerObject)
        {
            NPCController caller = callerObject as NPCController;

            if (CheckIfTargetVisited(caller))
            {
                return NodeExecutionState.SUCCEED;
            }

            SetDestination(caller);

            if (!CheckReachability(caller))
            {
                return NodeExecutionState.FAILED;
            }

            if(CheckIfPositionReached(caller))
            {
                return NodeExecutionState.SUCCEED; 
            }

            return NodeExecutionState.RUNNING;
        }

        bool CheckIfPositionReached(NPCController caller)
        {
            if (caller.nMAgent.remainingDistance < 0.2f && !caller.nMAgent.pathPending)
            {
                caller.patrolTargetReached[positionTargetIndex] = true;
                return true;
            }
            return false;
        }
        
        bool CheckReachability(NPCController caller)
        {
            return caller.nMAgent.CalculatePath(caller.nMAgent.destination, new NavMeshPath());
        }
        private void SetDestination(NPCController caller)
        {
            caller.nMAgent.destination = caller.patrolRoutes.positions[positionTargetIndex].transform.position;
        }

        private bool CheckIfTargetVisited(NPCController caller)
        {
            if (caller.patrolTargetReached[positionTargetIndex])
            {
                if (positionTargetIndex == caller.patrolTargetReached.Count - 1)
                {
                    for (int i = 0; i < caller.patrolTargetReached.Count; i++) //reset the list
                    {
                        caller.patrolTargetReached[i] = false;
                    }
                }
                return true; 
            }
            return false;
        }
    }
}
