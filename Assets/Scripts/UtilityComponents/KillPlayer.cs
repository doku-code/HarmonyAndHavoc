using JFM;
using UnityEngine;

namespace AF
{
    public class KillPlayer : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerData data = other.GetComponent<PlayerController>().Data;
                data.TakeDamage(data.MaxOrder);
            }
        }
    }
}