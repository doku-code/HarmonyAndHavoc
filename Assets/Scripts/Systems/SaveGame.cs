using AF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace JFM
{
    public class DictionaryEntry
    {
        public object Key;
        public object Value;

        public DictionaryEntry()
        {
        }

        public DictionaryEntry(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }

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

        public string currentMapName;
        
        public SaveGame()
        {
        }

        private void FromPlayerData(PlayerData playerData)
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

        private void ToPlayerData(PlayerData playerData)
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
            //playerData.MaxChaos = maxChaos;

            playerData.Gold = gold;

            playerData.OrderFragments = orderFragments;

            playerData.WeaponUpgrade = weaponUpgrade;
            playerData.ArmorUpgrade = armorUpgrade;
        }

        public static void Save(PlayerData pd, string mapName, string filename)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
            TextWriter writer = new StreamWriter(filename);

            SaveGame sg = new SaveGame();

            sg.FromPlayerData(pd);

            sg.currentMapName = mapName;

            serializer.Serialize(writer, sg);
            writer.Close();
        }

        public static void Load(PlayerData pd, out string mapName, string filename)
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
            // If the XML document has been altered with unknown
            // nodes or attributes, handles them with the
            // UnknownNode and UnknownAttribute events.
            serializer.UnknownNode += new XmlNodeEventHandler(SerializerUnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(SerializerUnknownAttribute);

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);
            // Declares an object variable of the type to be deserialized.
            SaveGame sg;
            // Uses the Deserialize method to restore the object's state
            // with data from the XML document. 
            sg = (SaveGame)serializer.Deserialize(fs);

            sg.ToPlayerData(pd);

            mapName = sg.currentMapName;

            // Reads the order date.
            //Debug.Log("ActualOrder: " + pd.ActualOrder);
            //Debug.Log("dash= " + playerData.AvailableKnowledgeDictionary[KnowledgeID.DASH]);        
        }

        private static void SerializerUnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private static void SerializerUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
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
}