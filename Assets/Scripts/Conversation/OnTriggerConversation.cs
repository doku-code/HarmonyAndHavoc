using AF;
using JFM;
using UnityEngine;

namespace charles
{
    public class OnTriggerConversation : MonoBehaviour
    {
        [SerializeField] GameObject conversationPanel;
        [SerializeField] ConversationManager questionIdx;
        private PlayerData data;
        private Coroutine displayCoroutine;

        void Awake()
        {
            data = GameManager.Instance.player.GetComponent<PlayerController>().Data;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !data.CurrentPlayerMapProgression[GameManager.Instance.currentMap])
            {
                conversationPanel.SetActive(true);

                if (questionIdx.Conversations.Length > 0)
                {
                    if (displayCoroutine != null)
                    {
                        StopCoroutine(displayCoroutine);
                    }
                    displayCoroutine = StartCoroutine(questionIdx.DisplayMessage(questionIdx.Conversations[0].questionText));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            conversationPanel.SetActive(false);

            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }
            if (questionIdx.Conversations.Length > 0)
            {  
                questionIdx.questionIndex = 0;
            }
        }
    }
}
