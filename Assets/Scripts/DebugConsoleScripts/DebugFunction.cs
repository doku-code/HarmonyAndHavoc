using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugFunction : MonoBehaviour
{
    public void ModifyTimeScale(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
        Debug.Log("This is called");
    }
    public void ChangeScene(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            Debug.LogError("Invalid scene index" + index);
        }
    }
}
