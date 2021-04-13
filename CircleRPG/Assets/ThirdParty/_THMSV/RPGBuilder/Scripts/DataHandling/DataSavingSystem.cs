using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataSavingSystem
{

    public static List<CharacterData> LoadAllCharacters()
    {
        var path = Application.persistentDataPath;
        var di = new DirectoryInfo(path);
        var files = di.GetFiles().Where(o => o.Name.Contains("_CharacterData.txt")).ToArray();
        var allCharacters = new List<CharacterData>();
        foreach (var t in files)
        {
            var charname = t.Name.Replace("_CharacterData.txt", "");
            allCharacters.Add(RPGBuilderJsonSaver.LoadCharacterData(charname));
        }

        return allCharacters;
    }

 

    public static void SaveAssetID(AssetIDHandler assetIDHandler)
    {
        var formatter = new BinaryFormatter();
        var path = getPath(assetIDHandler.assetType);
        var stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, assetIDHandler);
        stream.Close();
    }

    public static AssetIDHandler LoadAssetID(AssetIDHandler.ASSET_TYPE_ID assetType)
    {
        var path = getPath(assetType);
        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);
            var data = formatter.Deserialize(stream) as AssetIDHandler;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }

    private static string getPath(AssetIDHandler.ASSET_TYPE_ID assetType)
    {
        switch (assetType)
        {
            case AssetIDHandler.ASSET_TYPE_ID.ability:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/AbilitiesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.effect:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/EffectsIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.npc:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/NPCsIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.stat:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/StatIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.treePoint:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/TreePointsIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.item:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/ItemIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.skill:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/SkillsIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.levelTemplate:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/LevelTemplatesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.race:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/RacesIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID._class:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/ClassesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.lootTable:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/LootTablesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.merchantTable:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/MerchantTableIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.currency:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/CurrenciesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.craftingRecipe:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/CraftingRecipesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.craftingStation:
                return Application.dataPath + "/" +
                       "Resources/THMSV/RPGBuilderData/PersistentData/CraftingStationsIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.talentTree:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/TalentTreesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.bonus:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/BonusesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.task:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/TasksIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.quest:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/QuestsIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.worldPosition:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/WorldPositionsIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.resourceNode:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/ResourceNodesIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.abilityRank:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/AbilityRankDataIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.recipeRank:
                return Application.dataPath + "/" +
                       "Resources/THMSV/RPGBuilderData/PersistentData/CraftingRecipeRankDataIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.resourceNodeRank:
                return Application.dataPath + "/" +
                       "Resources/THMSV/RPGBuilderData/PersistentData/ResourceNodeRankDataIDs" + ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.bonusRank:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/BonusRankDataIDs" +
                       ".THMSV";
            case AssetIDHandler.ASSET_TYPE_ID.gameScene:
                return Application.dataPath + "/" + "Resources/THMSV/RPGBuilderData/PersistentData/GameSceneIDs" +
                       ".THMSV";
            default:
                return "";
        }
    }
}
