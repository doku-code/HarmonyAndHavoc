using UnityEngine;
using UnityEngine.SceneManagement;
using JFM;
using AF;

public class DebugFunction : MonoBehaviour
{
    [SerializeField] GameObject playerPrefabs;

    public void ModifyTimeScale(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;        
    }

    public void ChangeScene(string index)
    {
        if(index == "Village")
            GameManager.Instance.LoadNextMap(index, SpawnerPosition.END);
        else
            GameManager.Instance.LoadNextMap(index, SpawnerPosition.BEGIN);
    }

    public void HitPlayer(int damage)
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();
        pc.Data.TakeDamage(damage);
    }

    public void ChangeChaos(int chaosNumber)
    {
        ChaosOrderSystem.Instance.ChaosAmount = chaosNumber;
    }

    public void SetKnowldege(KnowledgeID knowldegeID)
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();

        pc.Data.KnownKnowledgeDictionary[knowldegeID] = true;
        pc.Data.AvailableKnowledgeDictionary[knowldegeID] = AvailableKnowledgePosition.NOT_AVAILABLE;
    }

    public void UnsetKnowldege(KnowledgeID knowldegeID)
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();

        pc.Data.KnownKnowledgeDictionary[knowldegeID] = false;
    }
}
