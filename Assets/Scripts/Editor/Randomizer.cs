
using UnityEditor;
using UnityEngine;



namespace Fineallday
{
    
    public class Randomizer : EditorWindow
    {
        [MenuItem("Window/Randomizer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Randomizer));
            Debug.Log("ok");
        }
        
        
        private void OnGUI()
        {
            GUILayout.Label("The Randomizer", EditorStyles.boldLabel);
            if (GUILayout.Button("Random!"))
            {
                foreach (var ob in Selection.objects)
                {
                    Debug.Log(ob.name);
                    GameObject o = ob as GameObject;
                    o.transform.localScale = new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5));
                }

                
            }
        }
    }
}
