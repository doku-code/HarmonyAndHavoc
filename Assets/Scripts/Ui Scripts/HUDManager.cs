using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    public class HUDManager : MonoBehaviour
    {
        [Header("PlayerData"), Tooltip("The PlayerData ScriptableObject")]
        [SerializeField] private PlayerData playerData;

        [Header("Sliders"), Tooltip("Order and Chaos sliders")]
        [SerializeField] private Slider order;
        [SerializeField] private Slider chaos;

        void Awake()
        {
            //playerData.OnDeadDelegate += OnDeadSlider;
            playerData.OnOrderDelegate += UpdateOrderSlider;
            playerData.OnChaosDelegate += UpdateChaosSlider;

            UpdateOrderSlider(0);
            UpdateChaosSlider(0);
        }
        /*
        private void OnDeadSlider()
        {
            order.value = 0;
        }*/

        private void UpdateOrderSlider(int value)
        {
            order.value = playerData.ActualOrder / (float)playerData.MaxOrder;
        }

        private void UpdateChaosSlider(int value)
        {
            chaos.value = playerData.ActualChaos / (float)playerData.MaxChaos;
        }
    }
}