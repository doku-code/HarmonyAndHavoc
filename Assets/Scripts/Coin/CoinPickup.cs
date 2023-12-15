using AF;
using JFM;
using UnityEngine;

namespace charles
{
    public class CoinPickup : MonoBehaviour, IInteractible
    {        
        void OnTriggerEnter2D(Collider2D other)
        {
            // If Player touches the coin, deactivate its gameObject.
            if (other.CompareTag("Player") && other is CapsuleCollider2D)
            {
                if (SoundManager.Instance is not null)
                {
                    SoundManager.Instance.PlayFxClip(2);
                }
                gameObject.SetActive(false);
            }
        }

        // Add to gold in PlayerData.
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
