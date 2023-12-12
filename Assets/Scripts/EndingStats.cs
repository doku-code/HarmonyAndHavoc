using AF;
using JFM;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

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
        [SerializeField] private TMP_Text endingText;
        [SerializeField] private VideoPlayer video;
        [Header("video")]
        [SerializeField] private VideoClip noChaos;
        [SerializeField] private VideoClip chaosOne;
        [SerializeField] private VideoClip chaosTwo;
        [SerializeField] private VideoClip chaosThree;
        [Header("Render Texture")]
        [SerializeField] private RawImage mp4Video;
        [SerializeField] private RenderTexture noChaosRenderer;
        [SerializeField] private RenderTexture chaosOneRenderer;
        [SerializeField] private RenderTexture chaosTwoRenderer;
        [SerializeField] private RenderTexture chaosThreeRenderer;



        private void Start()
        {
            chaosLevel.text = "CHAOS LEVEL: " + ChaosOrderSystem.Instance.ChaosAmount;
            enemyDeathCount.text = "Enemy Death Count: " + playerData.TotalKills;
            goldCollected.text = "Gold Collected: " + playerData.TotalGold;
            weaponUpgrade.text = "Weapon Upgrade: " + playerData.WeaponUpgrade;
            armorUpgrade.text = "Armor Upgrade: " + playerData.ArmorUpgrade;
            healAmount.text = "Total Heal: " + playerData.TotalHeals;

            SwitchVideoOnChaos();
        }

        private void SwitchVideoOnChaos()
        {
            switch (ChaosOrderSystem.Instance.ChaosAmount)
            {
                case 0:
                    mp4Video.texture = noChaosRenderer;
                    video.clip = noChaos;
                    video.targetTexture = noChaosRenderer;
                    endingText.text = "The grave digger, paysan, blacksmith, and healer all live long enough to see the player succeed. The grave digger gives mysterious clues about the subterranean, the paysan gives useful knowledge, the healer cares to the player's wounds, and the blacksmith crafts a formidable weapon. Together, they enable the player to successfully overcome the obstacles. The four major characters survive to see the peace returned, and the community stays intact.";
                    break;
                case 1:
                    mp4Video.texture = chaosOneRenderer;
                    video.clip = chaosOne;
                    video.targetTexture = chaosOneRenderer;
                    endingText.text = "The blacksmith's hopes of creating a weapon diminished despite his valiant efforts. Even when the challenges become intolerable, the healer never stops offering assistance. Both the paysan and the grave digger perish in the rising chaos. The player doesn't give up in the face of these obstacles. Even though the town sustains some minor damage, the survivors honor the player's efforts in spite of their injuries.";
                    break;
                case 2:
                    mp4Video.texture = chaosTwoRenderer;
                    video.clip = chaosTwo;
                    video.targetTexture = chaosTwoRenderer;
                    endingText.text = "The blacksmith began to decline rapidly. Likewise for the healer. With the limited power at their disposal, the player must overcome formidable obstacles below ground. They try, but the village is only partially destroyed. The player's valor is acknowledged by the remaining villagers, but the damage caused by chaos is severe.";
                    break;
                case 3:
                    mp4Video.texture = chaosThreeRenderer;
                    video.clip = chaosThree;
                    video.targetTexture = chaosThreeRenderer;
                    endingText.text = "The grave digger, paysan, healer, and blacksmith all fall victim to the powerful forces in the endless struggle against chaos. The player perseveres with unyielding resolve in spite of their sacrifices. The player finds the perseverance and expertise to descend into the underground and ultimately retrieve the Sword of Harmony.\r\n\r\nIt comes at a huge cost, though. The settlement is in ruins when the player returns from the depths. The once-thriving the village is in ruins as buildings crumble. As the last person standing in a village that has been overtaken by chaos,";
                    break;
            }
        }
        public void returnToMainMenu()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}