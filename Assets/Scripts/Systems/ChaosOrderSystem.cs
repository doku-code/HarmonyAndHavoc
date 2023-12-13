using AF;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

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
        private int previousChaosAmount;

        //Faire une autre liste de gameobject a desactiver dependant du niveau de chaos
        private List<GameObject> activatingObjects = null;
        private ObjectList[] activationObjectStates = new ObjectList[maxChaosAmount];

        //Faire une liste de gameobject pour les gameobject dans le village qui pourrais dependant du niveau du chaos changer de couleur
        private List<GameObject> colorChangingObjects = new List<GameObject>();
        private string[] colors = new string[maxChaosAmount];

        private GameObject[] gridGameObjects;

        //Faire en sorte que si on gameover tout ce reactive et revienne comme couleur normal
        //Faire en sorte de changer le fullscreenpass material dependant du shader \

        public int ChaosAmount
        {
            get 
            { 
                return chaosAmount;
            }

            set 
            {
                previousChaosAmount = chaosAmount;
                chaosAmount = value % maxChaosAmount;                
            }
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
            previousChaosAmount = -1;
            playerData.OnDeadDelegate += OnPlayerDead;
            GameManager.Instance.OnLoadMapDelegate += OnLoadMap;
        }

        private void OnLoadMap()
        {
            //Debug.Log("OnLoadMap()");

            // Bail out if map is not the Village
            if (GameManager.Instance.currentMap != "Village")
            {
                return;
            }

            // Call VillageChaosConfig script
            InitializeVariables();

            ActivateGrids();

            ActivateObjects();

            ChangeColoredObjects();
                                    
            Camera.main.GetComponent<UniversalAdditionalCameraData>().SetRenderer(chaosAmount);
        }

        private void OnPlayerDead()
        {
            if (chaosAmount >= maxChaosAmount - 1)
            {
                GameManager.Instance.LoadNextMap("Ending", SpawnerPosition.BEGIN);
            }
            else
            {
                IncrementChaosLevel();
            }
        }

        private void InitializeVariables()
        {
            VillageChaosConfig villageConfig = FindObjectOfType<VillageChaosConfig>();
            if (villageConfig is null)
            {
                Debug.LogError("You forgot to attach a VillageChaosConfig component on a GameObject in your Village scene!");
            }
            else 
            {
                //activatingObjects = villageConfig.activatingObjects;
                activationObjectStates = villageConfig.activationObjectStates;

                activatingObjects = new List<GameObject>();
                foreach(ObjectList list in activationObjectStates)
                {
                    foreach(GameObject obj in list.objects) 
                    {
                        if (activatingObjects.Find(
                        (x) => { return x == obj; }

                        ) is null)
                        {
                            activatingObjects.Add(obj);                            
                        }
                    }                    
                }
                 
                colorChangingObjects = villageConfig.colorChangingObjects;
                colors = villageConfig.colors;

                gridGameObjects = villageConfig.gridGameObjects;
            }
        }

        public void ResetChaosLevel()
        {
            chaosAmount = 0;
        }

        private void IncrementChaosLevel()
        {
            // Using the property here
            ChaosAmount++;
        }              

        private void ActivateGrids()
        {            
            //Debug.Log($"chaosAmount={chaosAmount}");

            for (int i = 0; i < chaosAmount; i++)
            {
                gridGameObjects[i].SetActive(false);
            }

            gridGameObjects[chaosAmount].SetActive(true);

            for (int i = chaosAmount + 1; i < maxChaosAmount; i++)
            {
                gridGameObjects[i].SetActive(false);
            }
        }

        private void ActivateObjects()
        {            
            DeactivatePreviousChaosLevel();
            ActivateCurrentChaosLevel();
        }

        private void ActivateCurrentChaosLevel()
        {
            if(activationObjectStates[chaosAmount] is null)
            {
                return;
            }

            foreach (GameObject go in activationObjectStates[chaosAmount].objects)
            {
                if (go is not null)
                {
                    go.SetActive(true);
                }
            }
        }

        private void DeactivatePreviousChaosLevel()
        {
            foreach (GameObject go in activatingObjects)
            {
                if (go is not null)
                {
                    //Debug.Log($"go={go.name}");

                    GameObject foundGO = null;
                    if ((foundGO = activationObjectStates[chaosAmount].objects.Find(
                        (x) => { return x == go; }

                    )) is null)
                    {
                        go.SetActive(false);
                    }
                    else
                    {
                        //Debug.Log($"foundGO={foundGO.name}");
                    }
                }
            }
        }

        private void ChangeColoredObjects()
        {
            foreach(GameObject go in colorChangingObjects)
            {
                Color color = Platformer2DUtilities.HexStringToColor(colors[chaosAmount]);
                SpriteRenderer[] sr = go.GetComponentsInChildren<SpriteRenderer>();
                //Debug.Log($"hexString={colors[chaosAmount]} color={color} for go={go.name} sr.Length={sr.Length}");

                foreach (SpriteRenderer spriteRenderer in sr)
                { 
                    spriteRenderer.color = color;
                }
            }
        }
    }
}
