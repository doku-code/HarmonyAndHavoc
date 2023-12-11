using AF;
using JFM;
using TMPro;
using UnityEngine;

namespace charles
{


    public class EndingStats : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

        [Header("Text")]
        [SerializeField] private TMP_Text chaosLevel;
        [SerializeField] private TMP_Text enemyDeathCount;
        [SerializeField] private TMP_Text goldCollected;
        [SerializeField] private TMP_Text weaponUpgrade;
        [SerializeField] private TMP_Text armorUpgrade;
        [SerializeField] private TMP_Text healAmount;

        private void Start()
        {
            chaosLevel.text = "CHAOS LEVEL: " + "Not Yet implemented";
            enemyDeathCount.text = "Enemy Death Count: " + "Not Yet implemented";
            goldCollected.text = "Gold Collected: " + playerData.Gold;
            weaponUpgrade.text = "Weapon Upgrade: " + playerData.WeaponUpgrade;
            armorUpgrade.text = "Armor Upgrade: " + playerData.ArmorUpgrade;
            healAmount.text = "Total Heal: " + "Not yet Implemented";
        }

    }
}