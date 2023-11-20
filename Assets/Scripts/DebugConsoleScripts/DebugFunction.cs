using UnityEngine;
using UnityEngine.SceneManagement;
using JFM;


public class DebugFunction : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

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

    public void HitPlayer(int damage)
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();
        pc.Data.TakeDamage(damage);
    }
}
