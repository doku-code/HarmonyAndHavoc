using AF;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JFM
{ 
    [Serializable]
    public class ObjectList
    {
        public List<GameObject> objects;
    }

    public struct GridInfo : IComparable
    {
        public Grid grid;
        public string name;

        public GridInfo(Grid grid)
        {
            this.grid = grid;
            name = grid.gameObject.name;
        }

        public int CompareTo(object other)
        {
            return String.Compare(name, ((GridInfo)other).name, true) ;
        }
    }

    public class ChaosOrderSystem : MonoBehaviour
    {
        public static ChaosOrderSystem Instance { get; private set; }    

        [SerializeField] private PlayerData playerData;

        [SerializeField] private int chaosAmount;
        public static int maxChaosAmount = 4;

        //Faire une autre liste de gameobject a desactiver dependant du niveau de chaos
        [SerializeField] private ObjectList[] activeObjects = new ObjectList[maxChaosAmount];

        //Faire une liste de gameobject pour les gameobject dans le village qui pourrais dependant du niveau du chaos changer de couleur
        [SerializeField] private List<GameObject> colorChangingObjects = new List<GameObject>();
        [SerializeField] private Color[] colors = new Color[maxChaosAmount];

        //Faire en sorte que si on gameover tout ce reactive et revienne comme couleur normal
        //Faire en sorte de changer le fullscreenpass material dependant du shader \

        public int ChaosAmount
        {
            get { return chaosAmount; }
            set { chaosAmount = value; }
        }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {        
            playerData.OnDeadDelegate += OnPlayerDead;
            GameManager.Instance.OnLoadMapDelegate += OnLoadMap;
        }

        private void OnLoadMap()
        {
            //Debug.Log("OnLoadMap()");

            // Bail out if map is not the Village
            if (GameManager.Instance.actualMap != "Village")
            {
                return;
            }

            // Call VillageChaosConfig script
            VillageChaosConfig villageConfig = FindObjectOfType<VillageChaosConfig>();
            activeObjects = villageConfig.activeObjects;

            Grid[] grids = FindObjectsOfType<Grid>();
            //Debug.Log("grids.Length=" +grids.Length);

            if(grids is null)
            {
                return;
            }

            // Create a list of Grid objects that are on layer "ChaosGrids"
            List<GridInfo> sortedGrids = new List<GridInfo>();
            foreach (Grid grid in grids)
            {
                if (grid.gameObject.layer == LayerMask.NameToLayer("ChaosGrids"))
                {
                    Debug.Log($"id={grid.gameObject.name}");
                    sortedGrids.Add(new GridInfo(grid));
                }
            }

            sortedGrids.Sort();
            /*foreach (GridInfo grid in sortedGrids)
            {
                Debug.Log($"sortedGrids= {grid.name}");
            }*/

            Debug.Log($"chaosAmount={chaosAmount}");


            if (chaosAmount > 0)
            {
                sortedGrids[chaosAmount - 1].grid.enabled = false;
            }
            else // if chaosAmount == 0
            {
            
            }
            sortedGrids[chaosAmount].grid.enabled = true;

            ActivateObjects();
        }

        private void OnPlayerDead()
        {
            chaosAmount++;
        }

        private void ActivateObjects()
        {
            if (chaosAmount == 0)
            {
                ActivateCurrentChaosLevel();
            }
            else
            {
                foreach (GameObject go in activeObjects[chaosAmount - 1].objects)
                {
                    if (go is not null)
                    {
                        if (activeObjects[chaosAmount].objects.Find(
                            (x) => { return x == go; }

                        ) is null )
                        {
                            go.SetActive(false);
                        }
                    }
                }

                ActivateCurrentChaosLevel();
            }
        }

        private void ActivateCurrentChaosLevel()
        {
            foreach (GameObject go in activeObjects[chaosAmount].objects)
            {
                if (go is not null)
                {
                    go.SetActive(true);
                }
            }
        }
    }
}
