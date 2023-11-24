using AF;
using System.Collections;
using UnityEngine;

namespace charles
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int numberOfCoins = 20;
        [SerializeField] private float minForce = 300f;
        [SerializeField] private float maxForce = 600f;
        [SerializeField] private float spawnDelay = 0.1f;

        public void SpawnCoins()
        {
            StartCoroutine(SpawnCoinsWithDelay());
        }
        //La couroutine a lair d'aider a en spawner plus mais y'a quand meme un bug ou parfois 
        private IEnumerator SpawnCoinsWithDelay()
        {
            for (int i = 0; i < numberOfCoins; i++)
            {
                GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
                Vector2 trajectory = Random.insideUnitCircle * 200f;

                float forceX = Random.Range(-minForce, maxForce) + trajectory.x;
                float forceY = maxForce + trajectory.y;

                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(forceX, forceY));
                coin.AddComponent<CoinPickup>();
                SoundManager.Instance.PlayAClip(2);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
