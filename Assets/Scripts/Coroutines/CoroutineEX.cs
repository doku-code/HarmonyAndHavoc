using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday
{
    public class CoroutineEX : MonoBehaviour
    {
        IEnumerator Start()
        {
            for (int i = 0; i < 1000; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(Mathf.Sin(i)*i/2, 0, Mathf.Cos(i)*i/2);
                yield return null;
            }
        }

    }
}
