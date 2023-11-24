using UnityEngine;

namespace Fineallday.Coroutines
{
    public class Player : MonoBehaviour
    {
        private Inventory myInventory;
      
        void Awake()
        {
            myInventory = new Inventory();
            foreach (var ob in myInventory)
            {
                Debug.Log(ob);
               // Debug.Log(ob.GetType());
                
                if (ob is Item)
                {
                    Item item = ob as Item;
                    Debug.Log(item.Name);
                   
                    if (item is ICauseDamage)
                    {
                        ICauseDamage weapon = item as ICauseDamage;
                        Debug.Log("Damage is: " + weapon.Damage);
                    }
                }else if (ob is int)
                {
                    int? value = ob as int?;
                    Debug.Log("int value is: " +value);  
                }
            }
        }

       
    }
}
