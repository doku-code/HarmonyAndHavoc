using AF;
using System.Collections;
using UnityEngine;

namespace charles
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int numberOfCoins = 20;
        [Tooltip("Impulse force to apply when coin dropping")]
        [SerializeField] private float minForce = 2.0f;
        [SerializeField] private float maxForce = 5.0f;
        [Tooltip("Angle variation according to Up vector (in degrees).")]
        [SerializeField] private float angleVariance = 30.0f;
        [SerializeField] private float spawnDelay = 0.1f;
        
        private void OnEnable()
        {
            EnemyController enemy = GetComponent<EnemyController>();
            enemy.OnDeadDelegate += SpawnCoins;
        }

        private void OnDisable()
        {
            EnemyController enemy = GetComponent<EnemyController>();
            enemy.OnDeadDelegate -= SpawnCoins;
        }

        public void SpawnCoins()
        {            
            StartCoroutine(SpawnCoinsWithDelay());
        }

        private IEnumerator SpawnCoinsWithDelay()
        {
            for (int i = 0; i < numberOfCoins; i++)
            {
                GameObject coin = CoinPool.SharedInstance.GetPooledObject();

                if (coin != null)
                {
                    coin.SetActive(true);
                }

                coin.transform.position = transform.position + new Vector3(0,1f,0);

                // Old coin dropping code
                /*float force = Random.Range(minForce, maxForce);
                Vector2 trajectory = Random.insideUnitCircle;
                float forceX = force * trajectory.x;
                float forceY = force * Mathf.Abs(trajectory.y);
                Vector2 vForce = new Vector2(forceX, forceY);
                */

                // Allow an angle variation (+/-) according to Vector2.up
                // to obtain an impulse force
                float angle = angleVariance * Mathf.Deg2Rad;
                angle = Random.Range(-angle, angle) + Mathf.PI / 2.0f;
                float force = Random.Range(minForce, maxForce);
                Vector2 vForce = new Vector2(force * Mathf.Cos(angle), force * Mathf.Sin(angle));

                Rigidbody2D coinRigidbody = coin.GetComponent<Rigidbody2D>();
                coinRigidbody.velocity = Vector2.zero;
                coinRigidbody.AddForce(vForce, ForceMode2D.Impulse);

                //Debug.Log($"Testing coin spawning. i={i} vForce={vForce}");                

                if(SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayFxClip(2);
                }

                yield return new WaitForSeconds(spawnDelay);
            }

            /*if (SoundManager.Instance is not null)
            {
                SoundManager.Instance.PlayFxClip(2);
            }  */          
        }
    }
}
