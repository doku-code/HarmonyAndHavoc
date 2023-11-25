using AF;
using UnityEngine;
namespace charles
{
    public class OpenTheWells : MonoBehaviour
    {
        [SerializeField] private GameObject wellsCollider;

        public void onAcceptQuest()
        {
            SoundManager.Instance.PlayAClip(4);
            wellsCollider.SetActive(false);
        }
    }
}