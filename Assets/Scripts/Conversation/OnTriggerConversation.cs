using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace charles
{
    public class OnTriggerConversation : MonoBehaviour
    {
        [SerializeField] GameObject conversationPanel;
        [SerializeField] ConversationManager questionIdx;
        [SerializeField] bool resetConversation;

        private Coroutine textCoroutine;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {

                if (textCoroutine != null)
                {
                    StopCoroutine(textCoroutine);
                }

                textCoroutine = StartCoroutine(questionIdx.ShowText());
                conversationPanel.SetActive(true);

                if (resetConversation)
                {
                    questionIdx.LoadConversation(0);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (conversationPanel != null)
            {
                conversationPanel.SetActive(false);

                if (textCoroutine != null)
                {
                    StopCoroutine(textCoroutine);
                }

                if (resetConversation)
                {
                    questionIdx.LoadConversation(0);
                }
            }
        }
    }
}
