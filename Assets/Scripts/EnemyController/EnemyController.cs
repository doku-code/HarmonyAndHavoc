using UnityEngine;
using JFM;
using System.Collections;
using System;
using AF;

namespace charles
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int attackDamage = 10;

        private Animator npcAnimator;
        private int currentHealth;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float knockBackImpulse;
        [SerializeField] private float knockBackDuration = 0.5f;
        private bool isKnockedBack;
        private bool canTakeDamage = true;
        [SerializeField] private float damageCooldown = 1.3f;

        public bool IsKnockedBack { get => isKnockedBack; }
        public bool CanTakeDamage { get => canTakeDamage; }
        public bool IsDead { get => currentHealth <= 0; }
        public int MaxHealth { get => maxHealth; }

        public event SingleParameterDelegate OnHealthDecrease;

        [Space]
        [Header("Behavior Tree")]
        [SerializeField] float tickInterval;
        public BehaviourTree tree;
        private IEnumerator tickCoroutine;
        [SerializeField] private EnemyBlackboard blackboard;

        public EnemyBlackboard Blackboard
        {
            get => blackboard;
        }

        void StartBehaviorTree()
        {
            tree = tree.Clone();

            foreach (Node node in tree.nodes)
            {
                node.OnInitialize(gameObject);
            }

            tickCoroutine = BehaviorTreeTick();
            StartCoroutine(tickCoroutine);
        }

        private void Start()
        {
            npcAnimator = GetComponent<Animator>();
            currentHealth = maxHealth;
            StartBehaviorTree();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision is CapsuleCollider2D)
            {
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

                Attack(collision.gameObject.GetComponent<PlayerController>(), pushDirection);
                //Debug.Log("Enemy hit");
                blackboard.lastHitTime = Time.time;
            }
        }

        IEnumerator DamageCooldown()
        {
            canTakeDamage = false;
            yield return new WaitForSeconds(damageCooldown);
            npcAnimator.ResetTrigger("GetHit");
            canTakeDamage = true;
        }

        public void TakeDamage(int damage, Vector2 pushDirection)
        {
            if (currentHealth <= 0 || !canTakeDamage)
            {
                return;
            }
            currentHealth -= Mathf.Max(0, damage);
            
            OnHealthDecrease(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(DamageCooldown());

                npcAnimator.SetBool("IsIdle", false);
                npcAnimator.SetBool("Run", false);
                npcAnimator.SetTrigger("GetHit");

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
                normal = new Vector2(Mathf.Abs(normal.x), Mathf.Abs(normal.y));
                rb.velocity = new Vector2(rb.velocity.x * normal.x, rb.velocity.y * normal.y);
                Debug.Log($"pushDirection={pushDirection}");
                rb.AddForce(pushDirection * knockBackImpulse, ForceMode2D.Impulse);
                StartCoroutine(KnockBackDelay(knockBackDuration));
            }
        }

        private IEnumerator KnockBackDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            isKnockedBack = false;
        }

        private IEnumerator BehaviorTreeTick()
        {
            while (true)
            {
                yield return new WaitForSeconds(tickInterval);
                tree.DoUpdate(tickInterval);
            }
        }
    }
}
