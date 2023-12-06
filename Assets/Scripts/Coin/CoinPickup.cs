using AF;
using JFM;
using UnityEngine;

namespace charles
{
    public class CoinPickup : MonoBehaviour, IInteractible
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other is CapsuleCollider2D)
            {
                SoundManager.Instance.PlayFxClip(3);
                gameObject.SetActive(false);
            }
        }

        public void Interact(GameObject gameObject) 
        {
            PlayerController pc = null;
            
            if(gameObject.TryGetComponent<PlayerController>(out pc))
            {
                pc.Data.Gold++;
            }            
        }
    }
}
