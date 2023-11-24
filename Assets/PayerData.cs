using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday
{
    public class PayerData : MonoBehaviour
    {
        [SerializeField] private InventoryData _playerInventory;

        public InventoryData PlayerInventory
        {
            get { return _playerInventory; }
            set { _playerInventory = value; }
        }
    }
}