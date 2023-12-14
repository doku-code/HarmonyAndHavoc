using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeFoundPanelController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    void OnEnable()
    {
        playerData.OnLearningKnowledge += OnLearningKnowledge;
    }

    void OnDisable()
    {
        playerData.OnLearningKnowledge -= OnLearningKnowledge;
    }

    private void OnLearningKnowledge(KnowledgeID knowledgeID)
    {
        Debug.Log($"Learned knowledge with ID {knowledgeID}");
        Animator animator = GetComponentInChildren<Animator>();
        animator.ResetTrigger("ShowKnowledge");
        animator.SetTrigger("ShowKnowledge");        
    }
}
