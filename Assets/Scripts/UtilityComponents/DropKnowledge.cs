using System.Collections;
using AF;
using charles;
using JFM;
using UnityEngine;

public class DropKnowledge : MonoBehaviour
{
    [SerializeField] private KnowledgeID knowledgeToDrop;
    [SerializeField] private float waitDelay = 2.0f;
    private EnemyController controller;

    void Awake()
    {        
        if (GameManager.Instance.data.KnownKnowledgeDictionary[knowledgeToDrop])
        {
            return;
        }

        controller = GetComponent<EnemyController>();
        controller.OnDeadDelegate += OnKill;
    }

    private void OnKill()
    {
        StartCoroutine(OnKillCoroutine());
    }

    private IEnumerator OnKillCoroutine()
    {
        yield return new WaitForSeconds(waitDelay);

        GameManager.Instance.data.LearnKnowledge(knowledgeToDrop);
    }
}
