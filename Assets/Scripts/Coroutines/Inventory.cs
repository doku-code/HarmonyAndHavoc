using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fineallday.Coroutines;
using UnityEngine;

namespace Fineallday
{
    public class Inventory : IEnumerable
    {
        private Item[] myItems =
        {
            new Weapons("Excalibur", 10, 10),
            new Weapons("Hammer", 5, 20),
            new Shields("Shield of the Whale", 10, 100)
        };
        
        private int[] randomNumbers = { 4, 5, 2, 6, 7 };
        private string inventoryName = "my Inventory";
        public IEnumerator GetEnumerator()
        {
            object[] all = new object[myItems.Length + randomNumbers.Length +1];
           
            myItems.CopyTo(all, 0);
            randomNumbers.CopyTo(all, myItems.Length);
            all[all.Length - 1] = inventoryName;
                
            return new ItemCollection(all);
           
            // return all.GetEnumerator();
            
            //return new ItemCollection(myItems);

            // return randomNumbers.GetEnumerator();
        }

        public Inventory()
        {
            foreach (var VARIABLE in myItems)
            {
                
            }
        }

       
    }
}
