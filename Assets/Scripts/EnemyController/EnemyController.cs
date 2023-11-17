using AF;
using UnityEngine;
using JFM;
using System.Collections;
using System;

namespace charles
{
    public class EnemyController : MonoBehaviour
    {
        public int maxHealth = 100;
        public int attackDamage = 10;

        private Animator npcAnimator;
        private int currentHealth;

        [Range(0.0f, 10.0f)]
        [SerializeField] private float pushBackImpulse;

        private void Start()
        {
            npcAnimator = GetComponent<Animator>();
            currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player") && collision is CapsuleCollider2D)
            {
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                Attack(collision.gameObject.GetComponent<PlayerController>(), pushDirection);
                Debug.Log("this is a CRITICAL HIT");
            }
        }

        public void TakeDamage(int damage, Vector2 pushDirection)
        {
            currentHealth -= Mathf.Max(0, damage);
            npcAnimator.SetTrigger("GetHit");

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                PushBack(pushDirection);
            }
            Debug.Log($"currentHealth={currentHealth}");
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

        private void PushBack(Vector2 pushDirection)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 normal = Platformer2DUtilities.GetPerpendicularVector2(pushDirection).normalized;
            normal = new Vector2(MathF.Abs(normal.x), MathF.Abs(normal.y));
            rb.velocity = new Vector2(rb.velocity.x * normal.x, rb.velocity.y * normal.y);
            //Debug.Log($"rb.velocity={rb.velocity}");
            rb.AddForce(pushDirection * pushBackImpulse, ForceMode2D.Impulse);
        }
    }
}
