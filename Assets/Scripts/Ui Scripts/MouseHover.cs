using UnityEngine;
using UnityEngine.EventSystems;
using AF;
namespace charles
{

    public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] PlayerData pData;
        [SerializeField] GameObject hoverPanel;
        [SerializeField] KnowledgeID knowledgeID;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(pData.KnownKnowledgeDictionary[knowledgeID])
            {
                hoverPanel.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverPanel.SetActive(false);
        }
    }
}
