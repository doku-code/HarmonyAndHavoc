using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("PlayerData"), Tooltip("The PlayerData ScriptableObject")]
    [SerializeField] private PlayerData playerData;

    [Header("Sliders"), Tooltip("")]
    [SerializeField] private Slider order;
    [SerializeField] private Slider chaos;

    void Awake()
    {
        playerData.OnDeadDelegate += OnDeadSlider;
        playerData.OnHitDelegate += UpdateOrderSlider;
        playerData.OnChaosDelegate += UpdateChaosSlider;

        UpdateOrderSlider();
        UpdateChaosSlider();
    }

    private void OnDeadSlider()
    {
        order.value = 0;
    }

    private void UpdateOrderSlider()
    {
        order.value = playerData.ActualOrder / (float)playerData.MaxOrder;
    }

    private void UpdateChaosSlider()
    {
        chaos.value = playerData.ActualChaos / (float)playerData.MaxChaos;
    }
}
