using Fineallday.BT;
using UnityEngine;

namespace Fineallday
{
    public abstract class LeafEvalGeneric : ScriptableObject
    {
        public abstract NodeExecutionState evalMethod(Object caller);
    }
}
