using AF;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        Debug.Log("ok");
    }

    private void OnLoadMap()
    {
        Debug.Log("OnLoadMap()");
        if (GameManager.Instance.actualMap != "Village")
        {
            return;
        }
        Grid[] grids = FindObjectsOfType<Grid>();
        //Debug.Log("grids.Length=" +grids.Length);
        if(grids is null)
        {
            return;
        }
        List<GridInfo> list = new List<GridInfo>();
        foreach (Grid grid in grids)
        {
            if (grid.gameObject.layer == LayerMask.NameToLayer("ChaosGrids"))
            {
                Debug.Log($"id={grid.gameObject.name}");
                list.Add(new GridInfo(grid));
            }
        }

        list.Sort();
        foreach (GridInfo grid in list)
        {
            Debug.Log($"list= {grid.name}");
        }
        Debug.Log($"chaosAmount={chaosAmount}");
        if (chaosAmount > 0)
        {
            list[chaosAmount - 1].grid.enabled = false;
        }
        list[chaosAmount].grid.enabled = true;
    }

    private void OnPlayerDead()
    {
        chaosAmount++;

    }
}
