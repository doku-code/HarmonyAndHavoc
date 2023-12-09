using System.Collections;
using System.Collections.Generic;
using AF;
using JFM;
using UnityEngine;

namespace charles
{
    public class OnTriggerConversation : MonoBehaviour
    {
        [SerializeField] GameObject conversationPanel;
        [SerializeField] ConversationManager questionIdx;
        [SerializeField] bool resetConversation;
        private PlayerData data;

        private Coroutine textCoroutine;

        void Awake()
        {
            data = GameManager.Instance.player.GetComponent<PlayerController>().Data;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !data.CurrentPlayerMapProgression[GameManager.Instance.currentMap])
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
