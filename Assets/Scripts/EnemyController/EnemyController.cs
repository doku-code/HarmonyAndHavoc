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

        [Range(0.0f, 50.0f)]
        [SerializeField] private float knockBackImpulse;
        [SerializeField] private float knockBackInterval = 0.001f;
        [SerializeField] private float knockBackFriction = 5.0f;
        private bool isKnockedBack;
        public bool IsKnockedBack {get => isKnockedBack;}
        public bool IsDead{ get => currentHealth <= 0; }

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
            if(currentHealth == 0)
            {
                return;
            }
            currentHealth -= Mathf.Max(0, damage);
            npcAnimator.SetTrigger("GetHit");

            if (currentHealth <= 0)
            {
                Die();

            }
            else
            {
                KnockBack(pushDirection);
            }            
        }

        public void Attack(PlayerController player, Vector2 direction)
        {
            player.TakeDamage(attackDamage, direction);
        }

        private void Die()
        {
            CoinSpawner coinSpawner = GetComponent<CoinSpawner>();
            coinSpawner.SpawnCoins(DestroyAfterAnim);
            npcAnimator.SetTrigger("Death");
        }

        private void DestroyAfterAnim()
        {            
            gameObject.SetActive(false);
        }
        
        private void KnockBack(Vector2 pushDirection)
        {
            if (!isKnockedBack)
            {
                isKnockedBack = true;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                Vector2 normal = Platformer2DUtilities.GetPerpendicularVector2(pushDirection).normalized;
                normal = new Vector2(MathF.Abs(normal.x), MathF.Abs(normal.y));
                rb.velocity = new Vector2(rb.velocity.x * normal.x, rb.velocity.y * normal.y);
                Debug.Log($"pushDirection={pushDirection}");
                //rb.AddForce(pushDirection * knockBackImpulse, ForceMode2D.Impulse);
                //StartCoroutine(KnockBackOverTime(rb, pushDirection, knockBackImpulse, knockBackInterval));
            }
        }

        private IEnumerator KnockBackOverTime(Rigidbody2D rb, Vector2 pushDirection, float knockBackImpulse, float delayTime)
        {            
            rb.AddForce(pushDirection * knockBackImpulse, ForceMode2D.Impulse);
            float friction = knockBackFriction;
            while(rb.velocity != Vector2.zero)
            {
                
                //Debug.Log($"rb.velocity={rb.velocity}");
                yield return new WaitForSeconds(delayTime);                
            }
            isKnockedBack = false;         
        }
    }
}
