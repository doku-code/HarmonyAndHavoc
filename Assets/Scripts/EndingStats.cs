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
                    break;
                case 1:
                    mp4Video.texture = chaosOneRenderer;
                    video.clip = chaosOne;
                    video.targetTexture = chaosOneRenderer;
                    break;
                case 2:
                    mp4Video.texture = chaosTwoRenderer;
                    video.clip = chaosTwo;
                    video.targetTexture = chaosTwoRenderer;
                    break;
                default:
                    mp4Video.texture = chaosThreeRenderer;
                    video.clip = chaosThree;
                    video.targetTexture = chaosThreeRenderer;
                    break;
            }
        }
        public void returnToMainMenu()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}