using UnityEngine;
namespace charles
{
    public class OpenTheWells : MonoBehaviour
    {
        [SerializeField] private GameObject wellsCollider;

        public void onAcceptQuest()
        {
            wellsCollider.SetActive(false);
        }
    }
}