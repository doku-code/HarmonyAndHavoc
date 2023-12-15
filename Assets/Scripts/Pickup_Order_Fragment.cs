using AF;
using UnityEngine;

namespace charles
{
    public class Pickup_Order_Fragment : MonoBehaviour
    {
        public PlayerData playerData;
        public AudioClip clip;

        private void Awake()
        {
            if(GameManager.Instance.data.CurrentPlayerMapProgression[GameManager.Instance.currentMap])
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerData.OrderFragments += 1;
                gameObject.SetActive(false);
                SoundManager.Instance.PlayFxClip(clip);
                playerData.ActualOrder = playerData.MaxOrder;
            }
        }
    }
}

