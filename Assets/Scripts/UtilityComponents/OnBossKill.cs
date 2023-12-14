using System.Collections;
using AF;
using charles;
using JFM;
using UnityEngine;

public class OnBossKill : MonoBehaviour
{
    [SerializeField] private KnowledgeID knowledgeToDrop;
    [SerializeField] private float waitDelay = 2.0f;
    private EnemyController controller;

    void Awake()
    {        
        if (GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap])
        {
            Destroy(gameObject);
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

        MapManager.Instance.UnlockDoor();

        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>(); 
        MapManager.Instance.MakePortal(transform.position + new Vector3(capsuleCollider.offset.x, capsuleCollider.offset.y, 0.0f) - Vector3.up * capsuleCollider.size.y / 2.0f);
    }
}
