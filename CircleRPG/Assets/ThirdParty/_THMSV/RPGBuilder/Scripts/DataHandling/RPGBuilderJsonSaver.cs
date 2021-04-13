using System.IO;
using THMSV.RPGBuilder;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;


public static class RPGBuilderJsonSaver
{
    private static void SaveInventory()
    {
        for (var i = 0; i < InventoryManager.Instance.bags[0].slots.Count; i++)
        {
            var itemID = -1;
            var itemStack = 0;
            var itemRandomID = -1;
            if (InventoryManager.Instance.bags[0].slots[i].inUse)
            {
                itemID = InventoryManager.Instance.bags[0].slots[i].itemStored.ID;
                itemStack = InventoryManager.Instance.bags[0].slots[i].curStack;
                itemRandomID = InventoryManager.Instance.bags[0].slots[i].itemRandomID;
            }

            CharacterData.Instance.inventoryItemsDATA[i].itemID = itemID;
            CharacterData.Instance.inventoryItemsDATA[i].itemStack = itemStack;
            CharacterData.Instance.inventoryItemsDATA[i].itemRandomID = itemRandomID;
        }
    }

    public static void GenerateCharacterEquippedtemsData()
    {
        if (CharacterData.Instance.armorsEquipped.Count == 0)
            for (var i = 0; i < RPGBuilderEssentials.Instance.itemSettings.armorSlots.Length; i++)
                CharacterData.Instance.armorsEquipped.Add(new CharacterData.ArmorsEquippedDATA());
        
        if (CharacterData.Instance.weaponsEquipped.Count == 0)
            for (var i = 0; i < RPGBuilderEssentials.Instance.itemSettings.weaponSlots.Length; i++)
                CharacterData.Instance.weaponsEquipped.Add(new CharacterData.WeaponsEquippedDATA());
    }
    
    private static void SaveEquippedItems()
    {

        if (CharacterData.Instance.armorsEquipped.Count <
            RPGBuilderEssentials.Instance.itemSettings.armorSlots.Length)
        {
            int diff = RPGBuilderEssentials.Instance.itemSettings.armorSlots.Length -
                       CharacterData.Instance.armorsEquipped.Count;

            for (int i = 0; i < diff; i++)
            {
                CharacterData.Instance.armorsEquipped.Add(new CharacterData.ArmorsEquippedDATA());
            }
        }
        if (CharacterData.Instance.armorsEquipped.Count >
            RPGBuilderEssentials.Instance.itemSettings.armorSlots.Length)
        {
            int diff = CharacterData.Instance.armorsEquipped.Count - RPGBuilderEssentials.Instance.itemSettings.armorSlots.Length;

            for (int i = 0; i < diff; i++)
            {
                CharacterData.Instance.armorsEquipped.RemoveAt(CharacterData.Instance.armorsEquipped.Count-1);
            }
        }

        for (var i = 0; i < CharacterData.Instance.armorsEquipped.Count; i++)
        {
            var itemID = -1;
            var randomItemID = -1;
            if (InventoryManager.Instance.equippedArmors[i].itemEquipped != null)
            {
                itemID = InventoryManager.Instance.equippedArmors[i].itemEquipped.ID;
                randomItemID = InventoryManager.Instance.equippedArmors[i].temporaryRandomItemID;
            }

            CharacterData.Instance.armorsEquipped[i].itemID = itemID;
            CharacterData.Instance.armorsEquipped[i].randomItemID = randomItemID;
        }

        if (CharacterData.Instance.weaponsEquipped.Count <
            RPGBuilderEssentials.Instance.itemSettings.weaponSlots.Length)
        {
            int diff = RPGBuilderEssentials.Instance.itemSettings.weaponSlots.Length -
                       CharacterData.Instance.weaponsEquipped.Count;

            for (int i = 0; i < diff; i++)
            {
                CharacterData.Instance.weaponsEquipped.Add(new CharacterData.WeaponsEquippedDATA());
            }
        }
        if (CharacterData.Instance.weaponsEquipped.Count >
            RPGBuilderEssentials.Instance.itemSettings.weaponSlots.Length)
        {
            int diff = CharacterData.Instance.weaponsEquipped.Count - RPGBuilderEssentials.Instance.itemSettings.weaponSlots.Length;

            for (int i = 0; i < diff; i++)
            {
                CharacterData.Instance.weaponsEquipped.RemoveAt(CharacterData.Instance.weaponsEquipped.Count-1);
            }
        }

        for (var i = 0; i < CharacterData.Instance.weaponsEquipped.Count; i++)
        {
            var itemID = -1;
            var randomItemID = -1;
            if (InventoryManager.Instance.equippedWeapons[i].itemEquipped != null)
            {
                itemID = InventoryManager.Instance.equippedWeapons[i].itemEquipped.ID;
                randomItemID = InventoryManager.Instance.equippedWeapons[i].temporaryRandomItemID;
            }

            CharacterData.Instance.weaponsEquipped[i].itemID = itemID;
            CharacterData.Instance.weaponsEquipped[i].randomItemID = randomItemID;
        }
    }

    public static void SaveCharacterData(string charName, CharacterData charCombatData)
    {
        if (CombatManager.playerCombatInfo)
        {
            charCombatData.position = CombatManager.playerCombatInfo.transform.position;
            SaveEquippedItems();
            SaveInventory();
        }

        var json = JsonUtility.ToJson(charCombatData);
        WriteToFile(charName + "_CharacterData.txt", json);
    }

    public static CharacterData LoadCharacterData(string charName)
    {
        var curCharCombatData = new CharacterData();
        var json = ReadFromFile(charName + "_CharacterData.txt");
        JsonUtility.FromJsonOverwrite(json, curCharCombatData);
        return curCharCombatData;
    }
    
    public static void SaveRandomItemsData(RandomizedItemsData randomItemsData)
    {
        var json = JsonUtility.ToJson(randomItemsData);
        WriteToFile("RandomizedItemsData.txt", json);
    }

    public static RandomizedItemsData LoadRandomItemsData()
    {
        var randomItemsData = new RandomizedItemsData();
        var json = ReadFromFile("RandomizedItemsData.txt");
        JsonUtility.FromJsonOverwrite(json, randomItemsData);
        return randomItemsData;
    }

    private static void WriteToFile(string fileName, string json)
    {
        var path = GetFilePath(fileName);
        var fileStream = new FileStream(path, FileMode.Create);

        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    public static void DeleteCharacter(string characterName)
    {

        var filePath = Application.persistentDataPath + "/" + characterName + "_CharacterData.txt";

        // check if file exists
        if (!File.Exists(filePath))
            Debug.LogError("This character save file does not exist");
        else
            File.Delete(filePath);
    }

    private static string ReadFromFile(string fileName)
    {
        var path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            using (var reader = new StreamReader(path))
            {
                var json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            Debug.LogWarning("File not found " + fileName);
            return "";
        }
    }

    private static string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
}

