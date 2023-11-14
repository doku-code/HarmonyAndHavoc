using AF;
using UnityEngine;

namespace charles
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] int amountofDamage;
        [SerializeField] Animator[] trapAnim;
        [SerializeField] string trapAnimStringName;
        [SerializeField] private PlayerData playerData;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                SetBoolForAllAnimators(true);
                playerData.TakeDamage(amountofDamage);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            SetBoolForAllAnimators(false);
        }

        private void SetBoolForAllAnimators(bool value)
        {
            foreach (Animator animator in trapAnim)
            {
                animator.SetBool(trapAnimStringName, value);
            }
        }
    }
}
