using AF;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveGame
{
    public int orderUpgradeValue;
    public int playerBaseDamage;

    public List<DictionaryEntry> knownKnowledgeDictionary;
    public List<DictionaryEntry> availableKnowledgeDictionary;

    public int knowledgeSlots;
    
    public int actualOrder;    
    public int maxOrder;    
    public int actualChaos;    
    public int maxChaos;    

    public int gold;

    public int orderFragments;

    public int weaponUpgrade;
    public int armorUpgrade;

    public SaveGame()
    {
    }

    public void FromPlayerData(PlayerData playerData)
    {
        orderUpgradeValue = playerData.OrderUpgradeValue;
        playerBaseDamage = playerData.PlayerBaseDamage;

        knownKnowledgeDictionary = DictionaryToList(playerData.KnownKnowledgeDictionary);
        availableKnowledgeDictionary = DictionaryToList(playerData.AvailableKnowledgeDictionary);
        
        knowledgeSlots = playerData.KnowledgeSlots;

        actualOrder = playerData.ActualOrder;
        maxChaos = playerData.MaxChaos;
        actualOrder = playerData.ActualOrder;  
        maxChaos = playerData.MaxChaos;

        gold = playerData.Gold;

        orderFragments = playerData.OrderFragments;

        weaponUpgrade = playerData.WeaponUpgrade;
        armorUpgrade = playerData.ArmorUpgrade; 
    }

    public void ToPlayerData(PlayerData playerData)
    {
        //PlayerData playerData = new PlayerData();
        playerData.OrderUpgradeValue = orderUpgradeValue;
        playerData.PlayerBaseDamage = playerBaseDamage;

        playerData.KnownKnowledgeDictionary = new Dictionary<KnowledgeID, bool>();
        playerData.AvailableKnowledgeDictionary = new Dictionary<KnowledgeID, AvailableKnowledgePosition>();

        ListToDictionary(knownKnowledgeDictionary, playerData.KnownKnowledgeDictionary);
        ListToDictionary(availableKnowledgeDictionary, playerData.AvailableKnowledgeDictionary);

        playerData.KnowledgeSlots = knowledgeSlots;

        playerData.ActualOrder = actualOrder;
        playerData.MaxChaos = maxChaos;
        playerData.ActualOrder = actualOrder;
        playerData.MaxChaos = maxChaos;

        playerData.Gold = gold;

        playerData.OrderFragments = orderFragments;

        playerData.WeaponUpgrade = weaponUpgrade;
        playerData.ArmorUpgrade = armorUpgrade;

        //return playerData;
    }

    private List<DictionaryEntry> DictionaryToList(IDictionary dictionary)
    {
        List<DictionaryEntry> entries = new List<DictionaryEntry>(dictionary.Count);
        foreach (object key in dictionary.Keys)
        {
            entries.Add(new DictionaryEntry(key, dictionary[key]));
        }
        return entries;
    }

    private void ListToDictionary(List<DictionaryEntry> list, IDictionary dictionary)
    {
        dictionary.Clear();        
        foreach (DictionaryEntry entry in list)
        {
            dictionary[entry.Key] = entry.Value;
        }
    }
}
