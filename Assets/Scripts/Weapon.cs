using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday
{
    public enum WeaponType
    {
        Axe,
        BattleAxe,
        Sword,
        Hammer,
        Bow,
        Spear,
        Masse,
        BareHand,
    }


    [CreateAssetMenu(fileName = "Weapon", menuName = "Inventory/Weapon")]
    [System.Serializable]
    public class Weapon : ItemData
    {
        public WeaponType type;
        public float damage;
    }
}