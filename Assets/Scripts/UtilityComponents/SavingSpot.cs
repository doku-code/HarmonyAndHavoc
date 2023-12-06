using System;
using System.Collections;
using System.Collections.Generic;
using AF;
using UnityEngine;

public class SavingSpot : MonoBehaviour
{

    private MapManager mapManager;
    
    void Awake()
    {
        mapManager = transform.parent.gameObject.GetComponent<MapManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
