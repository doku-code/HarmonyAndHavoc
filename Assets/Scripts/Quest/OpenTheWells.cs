using AF;
using UnityEngine;
namespace charles
{
    public class OpenTheWells : MonoBehaviour
    {
        [SerializeField] private GameObject wellsCollider;
        private bool wellsIsOpen = false;
        public void onAcceptQuest()
        {
            if (!wellsIsOpen)
            {
            SoundManager.Instance.PlayAClip(4);
            wellsCollider.SetActive(false);
                wellsIsOpen = true;
            }
            else
            {
                wellsCollider.SetActive(false);
            }
        }
    }
}