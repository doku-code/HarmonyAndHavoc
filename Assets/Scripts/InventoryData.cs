using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday
{
    [CreateAssetMenu(fileName = "InventoryAsset", menuName = "Inventory/InventoryAsset")]
    [Serializable]
    public class InventoryData : ScriptableObject, IEnumerable<ItemData>
    {
        public List<ItemData> items = new List<ItemData>();
        public int inventorySize;
        public IEnumerator<ItemData> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
