using UnityEngine;
using UnityEngine.EventSystems;

namespace charles
{

    public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject hoverPanel;
        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverPanel.SetActive(false);
        }
    }
}
