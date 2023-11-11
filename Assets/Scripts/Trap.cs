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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            playerData.TakeDamage(amountofDamage);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                SetBoolForAllAnimators(true);
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
