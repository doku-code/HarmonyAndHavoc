//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
//using Object = UnityEngine.Object;

namespace Fineallday.BT
{
    public enum NodeExecutionState
    {
        RUNNING,
        SUCCEED,
        FAILED
    }

    public enum NodeType
    {
        ROOT,
        SEQUENCER,
        SELECTOR,
        LEAF
    }

    [CreateAssetMenu(fileName = "Node", menuName = "ScriptableObjects/BehaviorTreeNode", order = 1)]
    public class Node : ScriptableObject
    {
        public NodeType nodeType;
        public NodeExecutionState state = NodeExecutionState.FAILED;
        public List<Node> children = new List<Node>();

        public delegate NodeExecutionState EvalMethod(Object caller);

        public EvalMethod evalMethod;
        public LeafEvalGeneric leafEvalScript;

        private void OnEnable()
        {
            if (evalMethod is null)
            {
                switch (nodeType)
                {
                    case NodeType.ROOT:
                        evalMethod = EvaluateSelector;
                        break;
                    case NodeType.SEQUENCER:
                        evalMethod = EvaluateSequencer;
                        break;
                    case NodeType.SELECTOR:
                        evalMethod = EvaluateSelector;
                        break;
                    case NodeType.LEAF:

                        if (leafEvalScript is null)
                        {
                            Debug.LogError("You must add a custom Eval method for this leaf: " + name);    
                        }
                        else
                        {
                           // evalMethod = EvaluateLeaf;
                            evalMethod = leafEvalScript.evalMethod; 
                        }
                        break;
                    
                }
            }
        }

        public NodeExecutionState EvaluateSequencer(Object caller)
        {
            Debug.Log("Sequence of : " + name + " started");

            if (children.Any())
            {
                foreach (var c in children)
                {
                    state = c.evalMethod(caller);

                    switch (state)
                    {
                        case NodeExecutionState.RUNNING:
                            Debug.Log(" Sequence is running " + c.name);
                            return state;
                        case NodeExecutionState.SUCCEED:
                            Debug.Log(c.name + " = Succeed!");
                            break;
                        case NodeExecutionState.FAILED:
                            Debug.Log(" Sequence Failed because of " + c.name);
                            return state;
                      
                    }
                }
            }
            else
            {
                Debug.Log("Failed: Sequencer has no children");
                state = NodeExecutionState.FAILED;
                //return state;
            }

            return state;
        }

        public NodeExecutionState EvaluateSelector(Object caller)
        {
            if (children.Any())
            {
                foreach (var c in children)
                {
                    state = c.evalMethod(caller);
                    switch (state)
                    {
                        case NodeExecutionState.RUNNING:
                            Debug.Log(c.name + " Running");
                            return state;
                        case NodeExecutionState.SUCCEED:
                            Debug.Log(c.name + " Succeed");
                            return state;
                        case NodeExecutionState.FAILED:
                            Debug.Log(c.name + " has FAILED");
                            break;
                        
                    }

                }
            }
            else
            {
                Debug.Log("Selector has no Children");
                //Si le selector n'a pas d'enfant c'est failed... Un selector doit avoir un ou des enfants.
                state = NodeExecutionState.FAILED;
            }
            
            return state;
        }

        // public virtual NodeExecutionState EvaluateLeaf(Object caller)
        // {
        //     return NodeExecutionState.SUCCEED;
        // }



        [CustomEditor(typeof(Node))]
        //[CanEditMultipleObjects]
        public class NodeEditor : Editor
        {
            private SerializedProperty leafEvalScript;
            private void OnEnable()
            {
                leafEvalScript = serializedObject.FindProperty("leafEvalScript");
            }

            public override void OnInspectorGUI()
            {
                DrawPropertiesExcluding(serializedObject, "leafEvalScript");
                //var nodeType = serializedObject.FindProperty("nodeType");
                serializedObject.ApplyModifiedProperties();
                
                Node node = target as Node;

                if (node.nodeType == NodeType.LEAF)
                {
                    EditorGUILayout.PropertyField(leafEvalScript, new GUIContent("Custom Eval Script for Leaves"));
                }

            }
        }

    }
}