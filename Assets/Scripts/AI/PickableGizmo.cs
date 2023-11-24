
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;


namespace Fineallday
{
    public class PickableGizmo : MonoBehaviour
    {
        private void Reset()
        {
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/cibleRouge.png");
            EditorGUIUtility.SetIconForObject(gameObject, icon);
        }

        void OnDrawGizmos()
        {
            var offset = Vector3.up * 10;
            if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 0.1f,NavMesh.AllAreas))
            {
                Gizmos.color = Color.green;  
            }else
            {
                Gizmos.color = Color.red;
            }
          
            Gizmos.DrawLine(transform.position, transform.position + offset);
            Gizmos.DrawSphere(transform.position + offset, 1);
            //Gizmos.DrawIcon(transform.position, "cibleRouge.png", true);
        }
    }
}