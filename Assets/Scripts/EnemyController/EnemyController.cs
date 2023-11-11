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
        private int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
            playerData = FindObjectOfType<PlayerData>();
        }

        public void TakeDamage(int damage)
        {
            int actualOrder = playerData.ActualOrder;
            int armorUpgrade = playerData.ArmorUpgrade;

            int damageTaken = Mathf.Max(0, damage - (defense + armorUpgrade));
            currentHealth -= damageTaken;

            if (currentHealth <= 0)
            {
                Die();
            }
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
