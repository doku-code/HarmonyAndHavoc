using AF;
using UnityEngine;

namespace charles
{
    public class EnemyController : MonoBehaviour
    {
        public int maxHealth = 100;
        public int attackDamage = 10;
        public int defense = 5;

        [SerializeField] private PlayerData playerData;
        [SerializeField] private Animator npcAnimator;
        private int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                Attack(playerData);
                Debug.Log("this is a CRITICAL HIT");
            }
        }
        public void TakeDamage(int damage)
        {
           int actualOrder = playerData.ActualOrder;
           int armorUpgrade = playerData.ArmorUpgrade;
           int damageTaken = Mathf.Max(0, damage - (defense + armorUpgrade));

           currentHealth -= damageTaken;

            npcAnimator.SetTrigger("GetHit");

            if (currentHealth <= 0)
            {
                Die();
            }
            npcAnimator.ResetTrigger("GetHit");
        }

        public void Attack(PlayerData playerData)
        {
            int armorUpgrade = playerData.ArmorUpgrade;
            int damageToPlayer = Mathf.Max(0, attackDamage - armorUpgrade);
            playerData.TakeDamage(damageToPlayer);
        }

        private void Die()
        {
          Destroy(gameObject);
        }
    }
}
