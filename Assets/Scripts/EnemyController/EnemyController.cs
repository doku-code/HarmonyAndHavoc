using AF;
using UnityEngine;
using JFM;

namespace charles
{
    public class EnemyController : MonoBehaviour
    {
        public int maxHealth = 100;
        public int attackDamage = 10;
        public int defense = 5;

        [SerializeField] private PlayerData playerData;
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
                //Attack(playerData);
                Attack(collision.gameObject.GetComponent<PlayerController>(), Vector2.right * Mathf.Sign(collision.transform.position.x - transform.position.x));
                Debug.Log("this is a CRITICAL HIT");
            }
        }
        public void TakeDamage(int damage)
        {
            
            /* ? */
            int actualOrder = playerData.ActualOrder;

            /* ? */
            int armorUpgrade = playerData.ArmorUpgrade;

            /* ? */
            int damageTaken = Mathf.Max(0, damage - (defense + armorUpgrade));

            currentHealth -= damageTaken;

            npcAnimator.SetTrigger("GetHit");

            if (currentHealth <= 0)
            {
                Die();
            }
            npcAnimator.ResetTrigger("GetHit");
        }

        public void Attack(PlayerController player, Vector2 direction)
        {
            /*int armorUpgrade = playerData.ArmorUpgrade;
            int damageToPlayer = Mathf.Max(0, attackDamage - armorUpgrade);
            playerData.TakeDamage(damageToPlayer);
            */
            player.TakeDamage(attackDamage, direction);
        }

        private void Die()
        {
          Destroy(gameObject);
        }
    }
}
