using AF;
using JFM;
using UnityEngine;

namespace charles
{
    public class OnTriggerMerchant : MonoBehaviour
    {
        [SerializeField] GameObject conversationPanel;
        [SerializeField] MerchantConversationManager merchant;
        private PlayerData data;
        private Coroutine displayCoroutine;
        [SerializeField] private bool keepTriggerable = true;

        void Awake()
        {
            data = GameManager.Instance.player.GetComponent<PlayerController>().Data;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && (!data.CurrentPlayerMapProgression[GameManager.Instance.currentMap] || keepTriggerable))
            {
                conversationPanel.SetActive(true);

                displayCoroutine = merchant.DisplayMerchantMessage(merchant.SellPitchMessage);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            conversationPanel.SetActive(false);

            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }
        }
    }
}
