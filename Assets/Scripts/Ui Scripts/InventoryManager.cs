using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using AF;

namespace charles
{
    public class InventoryManager : MonoBehaviour
    {
        private bool isInventoryMenuOpen = false;

        [Header("Panel")]
        [SerializeField] private GameObject InventoryPanel;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        public void InventoryMenu(CallbackContext value)
        {
            if (value.performed)
            {
                if (!isInventoryMenuOpen)
                {
                    SoundManager.Instance.PlayAClip(1);
                    InventoryPanel.SetActive(true);
                    isInventoryMenuOpen = true;
                }
                else
                {
                    SoundManager.Instance.PlayAClip(1);
                    InventoryPanel.SetActive(false);
                    isInventoryMenuOpen = false;
                }
            }
        }
    }
}