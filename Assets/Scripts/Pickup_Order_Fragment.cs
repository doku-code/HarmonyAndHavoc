using AF;
using UnityEngine;

public class Pickup_Order_Fragment : MonoBehaviour
{
    public PlayerData playerData;

    private void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.CompareTag("Player"))
        {             
                playerData.OrderFragments += 1;
                gameObject.SetActive(false);
        }
    }
}
