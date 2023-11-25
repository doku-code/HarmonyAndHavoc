using System.Collections.Generic;
using UnityEngine;

namespace charles
{
    public class CoinPool : MonoBehaviour
    {
        public static CoinPool SharedInstance;
        [SerializeField] private List<GameObject> pooledObjects;
        [SerializeField] private GameObject objectToPool;
        [SerializeField] private int amountToPool;
        void Awake()
        {
            SharedInstance = this;
        }

        void Start()
        {
            pooledObjects = new List<GameObject>();
            GameObject tmp;

            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectToPool, transform);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }


        public GameObject GetPooledObject()
        {
            for (int i = 0; i < amountToPool; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
            return null;
        }
    }
}