using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace charles
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        private EnemyController enemyController;
        private void Start()
        {
            enemyController = GetComponent<EnemyController>();
            enemyController.OnHealthDecrease += UpdateHealthBar;
        }

        private void UpdateHealthBar(int currentHealth)
        {
            //Debug.Log("is This Called " + "UpdateHealthBar");
            healthSlider.value = (float)currentHealth / enemyController.MaxHealth;
        }
        private void OnDestroy()
        {
            if (enemyController is not null)
            {
                enemyController.OnHealthDecrease -= UpdateHealthBar;
            }
        }
    }
}