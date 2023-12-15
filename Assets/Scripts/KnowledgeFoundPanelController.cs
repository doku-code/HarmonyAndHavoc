using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeFoundPanelController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Image uiImage;

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
        uiImage.sprite = playerData.knowledgeImgBank[(int)knowledgeID];
        
        Debug.Log($"Learned knowledge with ID {knowledgeID}");
        Animator animator = GetComponentInChildren<Animator>();
        animator.ResetTrigger("ShowKnowledge");
        animator.SetTrigger("ShowKnowledge");        
    }
}
