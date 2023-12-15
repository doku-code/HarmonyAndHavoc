using UnityEngine;
using JFM;
using System.Collections;
using System;
using AF;
using System.Text.RegularExpressions;

namespace charles
{
    public class EnemyController : MonoBehaviour, BehaviorTreeRunner
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
        //[SerializeField] private float damageCooldown = 0f;
        [SerializeField] private float deathDuration = 3.0f;

        private float initialLocalScaleX;

        public bool IsKnockedBack { get => isKnockedBack; }
        public bool CanTakeDamage { get => canTakeDamage; }
        public bool IsDead { get => currentHealth <= 0; }
        public int MaxHealth { get => maxHealth; }

        public ParametersLessDelegate OnDeadDelegate;

        public int AttackDamage
        {
            set => attackDamage = value;
        }

        public event IntegerParameterDelegate OnHealthDecrease;

        [Space]
        [Header("Behavior Tree")]
        [SerializeField] float tickInterval = 0.01f;
        public BehaviourTree tree;
        private IEnumerator tickCoroutine;
        [SerializeField] private EnemyBlackboard blackboard;

        public EnemyBlackboard Blackboard
        {
            get => blackboard;
        }

        public BehaviourTree GetBehaviorTree()
        {
            return tree;
        }
        
        public void TurnSide(float side)
        {
            transform.localScale = new Vector3(side * initialLocalScaleX, 1.0f, 1.0f);
            blackboard.healthBar.localScale = new Vector3(-side, 1.0f, 1.0f);
            //Debug.Log($"blackboard.healthBar.localScale.x={blackboard.healthBar.localScale.x} side={side} blackboard.healthBar.gameObject={blackboard.healthBar.gameObject.name}");            
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

            blackboard = blackboard.Clone();

            blackboard.healthBar = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
            blackboard.healthBar.localScale = new Vector3(-transform.localScale.x, 1.0f, 1.0f);

            currentHealth = maxHealth;

            initialLocalScaleX = transform.localScale.x;

            StartBehaviorTree();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision is CapsuleCollider2D)
            {
                CapsuleCollider2D capsule = collision.gameObject.GetComponent<CapsuleCollider2D>();
                Vector2 colliderOffset = GetComponent<CapsuleCollider2D>().offset;
                Vector2 pushDirection = Platformer2DUtilities.CalculateGroundDifference(collision.transform.position,
                                                                                       capsule.offset,
                                                                                       transform.position,
                                                                                       colliderOffset);

                //Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

                Attack(collision.gameObject.GetComponent<PlayerController>(), pushDirection);
                //Debug.Log("Enemy has hit Player");
                blackboard.lastHitTime = Time.time;
            }
        }

        //IEnumerator DamageCooldown()
        //{
        //    canTakeDamage = false;
        //    yield return new WaitForSeconds(damageCooldown);
        //    npcAnimator.ResetTrigger("GetHit");
        //    canTakeDamage = true;
        //}

        public bool TakeDamage(int damage, Vector2 pushDirection)
        {
            //Debug.Log($"Enemy taking damage");
            if (currentHealth <= 0 || !canTakeDamage)
            {
                return false;
            }
            currentHealth -= Mathf.Max(0, damage);
            
            OnHealthDecrease(currentHealth);

            if (currentHealth <= 0)
            {
                Die();

                return true;
            }
            //else
            //{
            //    //StartCoroutine(DamageCooldown());

            //    npcAnimator.SetBool("IsIdle", false);
            //    npcAnimator.SetBool("Run", false);
            //    npcAnimator.SetTrigger("GetHit");

            //    KnockBack(pushDirection);
            //}

            return false;
        }

        public void Attack(PlayerController player, Vector2 direction)
        {
            player.TakeDamage(attackDamage, direction);
        }

        private void Die()
        {
            Debug.Log("Dying");
            
            // Hide the UI (health bar)
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);

            /*CoinSpawner coinSpawner = GetComponent<CoinSpawner>();
            coinSpawner.SpawnCoins();*/

            if (OnDeadDelegate is not null)
            {
                OnDeadDelegate();
            }

            npcAnimator.SetTrigger("Death");
            StartCoroutine(DestroyAfterAnim());
        }

        private IEnumerator DestroyAfterAnim()
        {
            yield return new WaitForSeconds(deathDuration);            
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
