using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fineallday
{
    public class PatrolRoutes : MonoBehaviour
    {
        [SerializeField] public List<GameObject> positions = new List<GameObject>();
        [SerializeField] private List<string> positionName = new List<string>();
        private GameObject positionContainer;

        private void Reset()
        {
            DestroyImmediate(positionContainer);
            positionContainer = new GameObject("PositionContainer of: " + name); // Comme exemple de ??=
        }


        // void OnDrawGizmos()
        // {
        //     //Gizmos.color = Color.red;
        //     Gizmos.DrawSphere(transform.position, 4);
        //     
        // }


        private void AddPosition()
        {
            if (positionContainer is null) Reset();
            positionName.Add("Set The Name of the position: " + positions.Count);
            GameObject go = new GameObject("Position " + positions.Count);
            go.transform.SetParent(positionContainer.transform);
            go.AddComponent<PickableGizmo>();
            positions.Add(go);
        }

        private void RemovePosition()
        {
            if (positions.Any())
            {
                GameObject go = positions[positions.Count - 1];
                if (go != null) DestroyImmediate(go);
                positions.RemoveAt(positions.Count - 1);
                positionName.RemoveAt(positionName.Count - 1);
            }
        }

    //     #if UNITY_EDITOR
    //     [CustomEditor(typeof(PatrolRoutes))]
    //     public class PatrolRoutesEditor : Editor
    //     {
    //         //  private SerializedProperty targetPosition;
    //         // private   SerializedProperty targetPositionName;
    //         private void OnEnable()
    //         {
    //             //    targetPosition = serializedObject.FindProperty("positions");
    //             //   targetPositionName = serializedObject.FindProperty("positionName");
    //         }
    //
    //         private void OnSceneGUI()
    //         {
    //             PatrolRoutes ptrRoutes = (PatrolRoutes)target;
    //             foreach (var go in ptrRoutes.positions)
    //             {
    //                 go.transform.position = Handles.PositionHandle(go.transform.position, Quaternion.identity);
    //                 Handles.Label(go.transform.position, go.name);
    //             }
    //         }
    //
    //         public override void OnInspectorGUI()
    //         {
    //             PatrolRoutes ptrRoutes = (PatrolRoutes)target;
    //             GUILayout.Label("Add Positions for the NPC to Patrol", EditorStyles.whiteLargeLabel);
    //             // EditorGUILayout.PropertyField(targetPositionName, new GUIContent("Location Name"));
    //             //  EditorGUILayout.PropertyField(targetPosition, new GUIContent("Position"));
    //             //  serializedObject.ApplyModifiedProperties();
    //             // base.OnInspectorGUI();
    //             // DrawDefaultInspector();
    //             if (GUILayout.Button("Add New Position"))
    //             {
    //                 ptrRoutes.AddPosition();
    //             }
    //
    //
    //             foreach (var pos in ptrRoutes.positions)
    //             {
    //                 int index = ptrRoutes.positions.FindIndex(x => x == pos);
    //                 GUILayout.Space(3);
    //                 Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(1));
    //                 EditorGUI.DrawRect(r, Color.gray);
    //                 GUILayout.Space(3);
    //
    //                 EditorGUILayout.BeginHorizontal();
    //                 EditorGUI.BeginChangeCheck();
    //                 string newName = GUILayout.TextField(ptrRoutes.positionName[index], GUILayout.MinWidth(130));
    //                 if (EditorGUI.EndChangeCheck())
    //                 {
    //                     ptrRoutes.positionName[index] = newName;
    //                     pos.name = newName;
    //                 }
    //
    //                 GUILayout.Space(10);
    //
    //
    //                 EditorGUI.BeginChangeCheck();
    //                 Vector3 newPosition = EditorGUILayout.Vector3Field("", pos.transform.position,
    //                     GUILayout.Width(Screen.width * 0.5f));
    //                 if (EditorGUI.EndChangeCheck())
    //                 {
    //                     pos.transform.position = newPosition;
    //                 }
    //
    //                 EditorGUILayout.EndHorizontal();
    //             }
    //
    //             if (GUILayout.Button("Remove Last Position"))
    //             {
    //                 ptrRoutes.RemovePosition();
    //             }
    //             //DrawDefaultInspector();
    //         }
    //     }
    // #endif
    }
}