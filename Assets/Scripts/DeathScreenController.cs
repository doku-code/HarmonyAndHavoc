using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class DeathScreenController : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

        [SerializeField] private Animator fxAnimator;
        [SerializeField] private Animator textAnimator;
        [SerializeField] private Animator btnRetryAnimator;
        [SerializeField] private Animator btnExitAnimator;

        [SerializeField] private float waitForDeadAnimation = 2.0f;

        private void Start()
        {
            playerData.OnDeadDelegate += OnDead;

            PlayAnimation(false);
        }

        private void OnDestroy()
        {
            playerData.OnDeadDelegate -= OnDead;
        }

        private void OnDead()
        {
            StartCoroutine(WaitForDeadAnimation());
        }

        private IEnumerator WaitForDeadAnimation()
        {
            yield return new WaitForSeconds(waitForDeadAnimation);

            PlayAnimation(true);
        }

        public void OnRetryClick()
        {
            //PlayAnimation(false);
            //playerData.HealPlayer(playerData.MaxOrder);
            GameManager.Instance.LoadNextMap("Village", SpawnerPosition.BEGIN);
        }

        public void OnExitClick()
        {
            GameManager.Instance.LoadNextMap("MainMenu", SpawnerPosition.END);
        }

        private void PlayAnimation(bool isOn)
        {
            fxAnimator.SetBool("On", isOn);
            textAnimator.SetBool("On", isOn);
            btnRetryAnimator.SetBool("On", isOn);
            btnExitAnimator.SetBool("On", isOn);
        }
    }
}