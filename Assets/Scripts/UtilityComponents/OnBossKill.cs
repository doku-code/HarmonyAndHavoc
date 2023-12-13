using System.Collections;
using AF;
using charles;
using JFM;
using UnityEngine;

public class OnBossKill : MonoBehaviour
{
    [SerializeField]private KnowledgeID knowledgeToDrop;
    private EnemyController controller;


    void Awake()
    {
        controller = GetComponent<EnemyController>();
        controller.OnDeadDelegate += OnKill;
    }

    private void OnKill()
    {
        StartCoroutine(OnKillCoroutine());
    }

    private IEnumerator OnKillCoroutine()
    {
        GameManager.Instance.player.GetComponent<PlayerController>()
            .Data.LearnKnowledge(knowledgeToDrop);
        
        yield return new WaitForSeconds(3.0f);
        
        MapManager.Instance.UnlockDoor();
    }
}
