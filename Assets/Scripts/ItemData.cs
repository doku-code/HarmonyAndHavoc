using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday
{
    [System.Serializable]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public float weight;
        public Sprite uiTexture;
        // Object - Item - MEsh - Prefab
    }
}
