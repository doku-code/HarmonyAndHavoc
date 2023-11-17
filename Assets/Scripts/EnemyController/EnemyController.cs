using AF;
using UnityEngine;
using JFM;
using System.Collections;

namespace charles
{
    public class EnemyController : MonoBehaviour
    {
        public int maxHealth = 100;
        public int attackDamage = 10;

        private Animator npcAnimator;
        private int currentHealth;

        private void Start()
        {
            npcAnimator = GetComponent<Animator>();
            currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                Attack(collision.gameObject.GetComponent<PlayerController>(), Vector2.right * Mathf.Sign(collision.transform.position.x - transform.position.x));
                Debug.Log("this is a CRITICAL HIT");
            }
        }
        public void TakeDamage(int damage)
        {
            currentHealth -= Mathf.Max(0, damage);
            npcAnimator.SetTrigger("GetHit");

            if (currentHealth <= 0)
            {
                Die();
            }
            npcAnimator.ResetTrigger("GetHit");
        }

        public void Attack(PlayerController player, Vector2 direction)
        {
            player.TakeDamage(attackDamage, direction);
        }

        private void Die()
        {
            npcAnimator.SetTrigger("Death");
            StartCoroutine(DestroyAfterAnim(npcAnimator.GetCurrentAnimatorStateInfo(0).length));
        }

        private IEnumerator DestroyAfterAnim(float waitTime)
        {
            
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);

        }
    }
}
