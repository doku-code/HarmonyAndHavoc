using UnityEngine;
using AF;
using JFM;

public class SaveLoadMethods : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private string mapName;

    // Start is called before the first frame update
    void Start()
    {        
        SaveGame.Save(playerData, mapName, "Assets\\SaveGames\\game1.xml");
        
        string map = "";
        SaveGame.Load(playerData, out map, "Assets\\SaveGames\\game1.xml");

        Debug.Log($"map name = {map}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
