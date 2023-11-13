using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFunction : MonoBehaviour
{
    public void ModifyTimeScale(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
        Debug.Log("This is called");
    }
}
