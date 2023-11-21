using UnityEngine;
using AF;
using JFM;

public class SaveLoadMethods : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {        
        SaveGame.Save(playerData, "Assets\\SaveGames\\game1.xml");
        SaveGame.Load(playerData, "Assets\\SaveGames\\game1.xml");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
