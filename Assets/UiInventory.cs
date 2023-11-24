using UnityEngine;
using UnityEngine.UI;

namespace Fineallday
{
    public class UiInventory : MonoBehaviour
    {
        [SerializeField] private InventoryData inventory;

        private void OnEnable()
        {
            for (int i = 0; i < inventory.inventorySize; i++)
            {
                var im = transform.GetChild(i).GetChild(0).GetComponent<Image>();
               
                if (inventory.items.Count > i)
                {
                  im.sprite = inventory.items[i].uiTexture;
                }
                else
                {
                    im.sprite = null;
                }
            }
        }
    }
}
