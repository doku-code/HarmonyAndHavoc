using AF;
using JFM;
using UnityEngine;

namespace charles
{
    public class OnTriggerConversation : MonoBehaviour
    {
        [SerializeField] GameObject conversationPanel;
        [SerializeField] GeneralConversationManager conversation;
        private PlayerData data;
        private Coroutine displayCoroutine;
        [SerializeField] private bool unlockable = true;

        void Awake()
        {
            data = GameManager.Instance.player.GetComponent<PlayerController>().Data;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                conversationPanel.SetActive(true);

                if (!data.CurrentPlayerMapProgression[GameManager.Instance.currentMap] || !unlockable)
                {
                    /*if (conversation.Conversations.Length > 0)
                    {
                        if (displayCoroutine != null)
                        {
                            StopCoroutine(displayCoroutine);
                        }
                        displayCoroutine = StartCoroutine(conversation.DisplayMessage(conversation.Conversations[0].questionText));
                    }*/

                    conversation.DisplayConversationMessage();
                }
                else
                {
                    conversation.DisplayConversationMessage(1);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            conversationPanel.SetActive(false);

            conversation.StopMessage();

            /*if (conversation.messages.Length > 0)
            {  
                conversation.questionIndex = 0;
            }*/
        }
    }
}
