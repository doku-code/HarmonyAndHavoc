using AF;
using UnityEngine;

namespace charles
{
    public class CoinPickup : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other is CapsuleCollider2D)
            {
                SoundManager.Instance.PlayAClip(3);
                Destroy(gameObject);
            }
        }
    }
}
