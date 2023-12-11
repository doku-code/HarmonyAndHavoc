using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JFM
{
    public class DeathScreenController : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

        [SerializeField] private Canvas canvas;
        [SerializeField] private Animator fxAnimator;
        [SerializeField] private Animator textAnimator;
        [SerializeField] private Animator btnRetryAnimator;
        [SerializeField] private Animator btnExitAnimator;

        [SerializeField] private float waitForDeadAnimation = 2.0f;
        
        private void Start()
        {
            playerData.OnDeadDelegate += OnDead;

            //PlayAnimation(false);
            SetGOActive(false);
        }

        private void OnDestroy()
        {
            playerData.OnDeadDelegate -= OnDead;
        }

        private void OnDead()
        {
            StartCoroutine(WaitAndExecute(waitForDeadAnimation, () => { PlayAnimation(true); }));
        }

        private IEnumerator WaitAndExecute(float waitDelay, ParametersLessDelegate callback)
        {
            yield return new WaitForSeconds(waitDelay);

            if(callback is not null)
            {
                callback.Invoke();
            }
        }

        public void OnRetryClick()
        {
            PlayAnimation(false);
            playerData.ActualChaos = playerData.MaxChaos;
            playerData.ActualOrder = playerData.MaxOrder;
            GameManager.Instance.LoadNextMap("Village", SpawnerPosition.END);
        }

        public void OnExitClick()
        {
            PlayAnimation(false);           
            GameManager.Instance.LoadNextMap("MainMenu", SpawnerPosition.END);       
        }

        private void PlayAnimation(bool isOn)
        {
            if (isOn)
            {
                SetGOActive(isOn);
            }
            
            fxAnimator.SetBool("On", isOn);
            textAnimator.SetBool("On", isOn);
            btnRetryAnimator.SetBool("On", isOn);
            btnExitAnimator.SetBool("On", isOn);
        }

        public void DeactivateGOs()
        {
            SetGOActive(false);
        }

        private void SetGOActive(bool active)
        { 
            canvas.gameObject.SetActive(active);                
        }
    }
}