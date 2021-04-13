using System;
using System.Collections.Generic;
using THMSV.RPGBuilder;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class RPGBuilderUtilities
{
    public static void EnableCG(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public static void DisableCG(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public static bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var results = new List<RaycastResult>();
        if (EventSystem.current == null) return false;
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static float[] vector3ToFloatArray(Vector3 vector3)
    {
        return new float[] {vector3.x, vector3.y, vector3.z};
    }

    public static Vector3 floatArrayToVector3(float[] floatArray)
    {
        return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
    }

    public static RPGCurrency getCurrencyByName(string name)
    {
        for (var i = 0; i < RPGBuilderEssentials.Instance.allCurrencies.Count; i++)
            if (RPGBuilderEssentials.Instance.allCurrencies[i].displayName == name)
                return RPGBuilderEssentials.Instance.allCurrencies[i];
        return null;
    }

    public static Object GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID _assetType, int ID)
    {
        switch (_assetType)
        {
            case AssetIDHandler.ASSET_TYPE_ID.ability:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allAbilities.Count; i++)
                    if (RPGBuilderEssentials.Instance.allAbilities[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allAbilities[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.effect:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allEffects.Count; i++)
                    if (RPGBuilderEssentials.Instance.allEffects[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allEffects[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.npc:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allNPCs.Count; i++)
                    if (RPGBuilderEssentials.Instance.allNPCs[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allNPCs[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.stat:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allStats.Count; i++)
                    if (RPGBuilderEssentials.Instance.allStats[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allStats[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.treePoint:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allTreePoints.Count; i++)
                    if (RPGBuilderEssentials.Instance.allTreePoints[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allTreePoints[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.item:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allItems.Count; i++)
                    if (RPGBuilderEssentials.Instance.allItems[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allItems[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.skill:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allSkills.Count; i++)
                    if (RPGBuilderEssentials.Instance.allSkills[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allSkills[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.levelTemplate:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allLevelTemplates.Count; i++)
                    if (RPGBuilderEssentials.Instance.allLevelTemplates[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allLevelTemplates[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.race:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allRaces.Count; i++)
                    if (RPGBuilderEssentials.Instance.allRaces[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allRaces[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID._class:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allClasses.Count; i++)
                    if (RPGBuilderEssentials.Instance.allClasses[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allClasses[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.lootTable:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allLootTables.Count; i++)
                    if (RPGBuilderEssentials.Instance.allLootTables[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allLootTables[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.merchantTable:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allMerchantTables.Count; i++)
                    if (RPGBuilderEssentials.Instance.allMerchantTables[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allMerchantTables[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.currency:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allCurrencies.Count; i++)
                    if (RPGBuilderEssentials.Instance.allCurrencies[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allCurrencies[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.craftingRecipe:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allCraftingRecipes.Count; i++)
                    if (RPGBuilderEssentials.Instance.allCraftingRecipes[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allCraftingRecipes[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.craftingStation:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allCraftingStation.Count; i++)
                    if (RPGBuilderEssentials.Instance.allCraftingStation[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allCraftingStation[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.talentTree:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allTalentTrees.Count; i++)
                    if (RPGBuilderEssentials.Instance.allTalentTrees[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allTalentTrees[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.bonus:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allBonuses.Count; i++)
                    if (RPGBuilderEssentials.Instance.allBonuses[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allBonuses[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.task:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allTasks.Count; i++)
                    if (RPGBuilderEssentials.Instance.allTasks[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allTasks[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.quest:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allQuests.Count; i++)
                    if (RPGBuilderEssentials.Instance.allQuests[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allQuests[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.worldPosition:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allWorldPositions.Count; i++)
                    if (RPGBuilderEssentials.Instance.allWorldPositions[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allWorldPositions[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.resourceNode:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allResourceNodes.Count; i++)
                    if (RPGBuilderEssentials.Instance.allResourceNodes[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allResourceNodes[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.abilityRank:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allAbilityRanks.Count; i++)
                    if (RPGBuilderEssentials.Instance.allAbilityRanks[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allAbilityRanks[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.recipeRank:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allRecipeRanks.Count; i++)
                    if (RPGBuilderEssentials.Instance.allRecipeRanks[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allRecipeRanks[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.resourceNodeRank:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allResourceNodeRanks.Count; i++)
                    if (RPGBuilderEssentials.Instance.allResourceNodeRanks[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allResourceNodeRanks[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.bonusRank:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allBonusRankData.Count; i++)
                    if (RPGBuilderEssentials.Instance.allBonusRankData[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allBonusRankData[i];
                return null;
            case AssetIDHandler.ASSET_TYPE_ID.gameScene:
                for (var i = 0; i < RPGBuilderEssentials.Instance.allGameScenes.Count; i++)
                    if (RPGBuilderEssentials.Instance.allGameScenes[i].ID == ID)
                        return RPGBuilderEssentials.Instance.allGameScenes[i];
                return null;
            default: return null;
        }
    }

    public static RPGGameScene GetGameSceneFromName(string sceneName)
    {
        for (var i = 0; i < RPGBuilderEssentials.Instance.allGameScenes.Count; i++)
            if (RPGBuilderEssentials.Instance.allGameScenes[i]._name == sceneName)
                return RPGBuilderEssentials.Instance.allGameScenes[i];
        return null;
    }

    public static RPGGameScene GetGameSceneFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.gameScene, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGGameScene) gameElement;
        return thisElementREF;
    }

    public static RPGAbility GetAbilityFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.ability, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGAbility) gameElement;
        return thisElementREF;
    }

    public static RPGEffect GetEffectFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.effect, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGEffect) gameElement;
        return thisElementREF;
    }

    public static RPGNpc GetNPCFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.npc, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGNpc) gameElement;
        return thisElementREF;
    }

    public static RPGStat GetStatFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.stat, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGStat) gameElement;
        return thisElementREF;
    }

    public static RPGTreePoint GetTreePointFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.treePoint, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGTreePoint) gameElement;
        return thisElementREF;
    }

    public static RPGItem GetItemFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.item, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGItem) gameElement;
        return thisElementREF;
    }

    public static RPGSkill GetSkillFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.skill, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGSkill) gameElement;
        return thisElementREF;
    }

    public static RPGLevelsTemplate GetLevelTemplateFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.levelTemplate, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGLevelsTemplate) gameElement;
        return thisElementREF;
    }

    public static RPGClass GetClassFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID._class, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGClass) gameElement;
        return thisElementREF;
    }

    public static RPGRace GetRaceFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.race, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGRace) gameElement;
        return thisElementREF;
    }

    public static RPGLootTable GetLootTableFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.lootTable, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGLootTable) gameElement;
        return thisElementREF;
    }

    public static RPGMerchantTable GetMerchantTableFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.merchantTable, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGMerchantTable) gameElement;
        return thisElementREF;
    }

    public static RPGCurrency GetCurrencyFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.currency, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGCurrency) gameElement;
        return thisElementREF;
    }

    public static RPGCraftingRecipe GetCraftingRecipeFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.craftingRecipe, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGCraftingRecipe) gameElement;
        return thisElementREF;
    }

    public static RPGCraftingStation GetCraftingStationFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.craftingStation, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGCraftingStation) gameElement;
        return thisElementREF;
    }

    public static RPGTalentTree GetTalentTreeFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.talentTree, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGTalentTree) gameElement;
        return thisElementREF;
    }

    public static RPGBonus GetBonusFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.bonus, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGBonus) gameElement;
        return thisElementREF;
    }

    public static RPGTask GetTaskFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.task, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGTask) gameElement;
        return thisElementREF;
    }

    public static RPGQuest GetQuestFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.quest, ID);
        if (gameElement != null)
        {
            var thisElementREF = (RPGQuest) gameElement;
            return thisElementREF;
        }

        return null;
    }

    public static RPGWorldPosition GetWorldPositionFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.worldPosition, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGWorldPosition) gameElement;
        return thisElementREF;
    }

    public static RPGResourceNode GetResourceNodeFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.resourceNode, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGResourceNode) gameElement;
        return thisElementREF;
    }

    public static RPGAbilityRankData GetAbilityRankFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.abilityRank, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGAbilityRankData) gameElement;
        return thisElementREF;
    }

    public static RPGCraftingRecipeRankData GetCraftingRecipeRankFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.recipeRank, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGCraftingRecipeRankData) gameElement;
        return thisElementREF;
    }

    public static RPGResourceNodeRankData GetResourceNodeRankFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.resourceNodeRank, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGResourceNodeRankData) gameElement;
        return thisElementREF;
    }

    public static RPGBonusRankDATA GetBonusRankFromID(int ID)
    {
        var gameElement = GetGameElementDATA(AssetIDHandler.ASSET_TYPE_ID.bonusRank, ID);
        if (gameElement == null) return null;
        var thisElementREF = (RPGBonusRankDATA) gameElement;
        return thisElementREF;
    }

    public static RPGAbility GetAbilityFromIDEditor(int ID, List<RPGAbility> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGNpc GetNPCFromIDEditor(int ID, List<RPGNpc> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGTask GetTaskFromIDEditor(int ID, List<RPGTask> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGGameScene GetGameSceneFromIDEditor(int ID, List<RPGGameScene> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGWorldPosition GetWorldPositionFromIDEditor(int ID, List<RPGWorldPosition> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGTreePoint GetTreePointFromIDEditor(int ID, List<RPGTreePoint> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGQuest GetQuestFromIDEditor(int ID, List<RPGQuest> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGSkill GetSkillFromIDEditor(int ID, List<RPGSkill> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGRace GetRaceFromIDEditor(int ID, List<RPGRace> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGLevelsTemplate GetLevelTemplateFromIDEditor(int ID, List<RPGLevelsTemplate> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGItem GetItemFromIDEditor(int ID, List<RPGItem> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGEffect GetEffectFromIDEditor(int ID, List<RPGEffect> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGTalentTree GetTalentTreeFromIDEditor(int ID, List<RPGTalentTree> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGStat GetStatFromIDEditor(int ID, List<RPGStat> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGCraftingRecipe GetCraftingRecipeFromIDEditor(int ID, List<RPGCraftingRecipe> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGResourceNode GetResourceNodeFromIDEditor(int ID, List<RPGResourceNode> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGBonus GetBonusFromIDEditor(int ID, List<RPGBonus> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGAbilityRankData GetAbilityRankFromIDEditor(int ID, List<RPGAbilityRankData> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGCraftingRecipeRankData GetCraftingRecipeRankFromIDEditor(int ID,
        List<RPGCraftingRecipeRankData> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGResourceNodeRankData GetResourceNodeRankFromIDEditor(int ID,
        List<RPGResourceNodeRankData> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGBonusRankDATA GetBonusRankFromIDEditor(int ID, List<RPGBonusRankDATA> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGClass GetClassFromIDEditor(int ID, List<RPGClass> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGMerchantTable GetMerchantTableFromIDEditor(int ID, List<RPGMerchantTable> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGLootTable GetlootTableFromIDEditor(int ID, List<RPGLootTable> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGCurrency GetCurrencyFromIDEditor(int ID, List<RPGCurrency> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static RPGCraftingStation GetCraftingStationFromIDEditor(int ID, List<RPGCraftingStation> rankList)
    {
        foreach (var t in rankList)
            if (t.ID == ID)
                return t;

        return null;
    }

    public static int getTreePointSpentAmount(RPGTalentTree tree)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
            if (t.treeID == tree.ID)
                return t.pointsSpent;

        return -1;
    }

    public static int getNodeCurrentRank(RPGAbility ability)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (ability.ID == t1.nodeData.abilityID)
                return t1.rank - 1;

        return -1;
    }

    public static int getNodeCurrentRank(RPGBonus bonus)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (bonus.ID == t1.nodeData.bonusID)
                return t1.rank - 1;

        return -1;
    }

    public static int getNodeCurrentRank(RPGCraftingRecipe recipe)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (recipe.ID == t1.nodeData.recipeID)
                return t1.rank - 1;

        return -1;
    }

    public static int getNodeCurrentRank(RPGResourceNode resourceNode)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (resourceNode.ID == t1.nodeData.resourceNodeID)
                return t1.rank - 1;

        return -1;
    }

    public static bool isBonusKnown(int ID)
    {
        foreach (var t in CharacterData.Instance.bonusesDATA)
            if (ID == t.bonusID && t.known)
                return true;

        return false;
    }

    public static int getItemCount(RPGItem item, int curRank)
    {
        var totalOfThisComponent = 0;
        foreach (var t in InventoryManager.Instance.bags)
        foreach (var t1 in t.slots)
            if (t1.inUse &&
                t1.itemStored == item)
                totalOfThisComponent += t1.curStack;

        return totalOfThisComponent;
    }

    public static List<RPGCraftingRecipe> getRecipeListOfSkill(RPGSkill skill, RPGCraftingStation station)
    {
        var recipeList = new List<RPGCraftingRecipe>();
        foreach (var t in RPGBuilderEssentials.Instance.allCraftingRecipes)
            if (t.craftingSkillID == skill.ID &&
                t.craftingStationID == station.ID)
                if (isRecipeKnown(t.ID))
                    recipeList.Add(t);

        return recipeList;
    }

    public static bool isAbilityKnown(int ID)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.abilityID != -1 &&
                ID == t1.nodeData.abilityID &&
                t1.known)
                return true;

        return false;
    }

    public static bool isRecipeKnown(int ID)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.recipeID != -1 &&
                ID == t1.nodeData.recipeID &&
                t1.known)
                return true;

        return false;
    }

    public static bool isResourceNodeKnown(int ID)
    {
        foreach (var t in CharacterData.Instance.talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.resourceNodeID != -1 &&
                ID == t1.nodeData.resourceNodeID &&
                t1.known)
                return true;

        return false;
    }

    public static int getSkillLevel(int skillID)
    {
        foreach (var t in CharacterData.Instance.skillsDATA)
            if (skillID == t.skillID)
                return t.currentSkillLevel;

        return -1;
    }

    public static float getSkillEXPPercent(RPGSkill skill)
    {
        foreach (var t in CharacterData.Instance.skillsDATA)
            if (skill.ID == t.skillID)
                return (float) t.currentSkillXP /
                       (float) t.maxSkillXP;

        return -1;
    }

    public static int getSkillCurXP(RPGSkill skill)
    {
        foreach (var t in CharacterData.Instance.skillsDATA)
            if (skill.ID == t.skillID)
                return t.currentSkillXP;

        return -1;
    }

    public static int getSkillMaxXP(RPGSkill skill)
    {
        foreach (var t in CharacterData.Instance.skillsDATA)
            if (skill.ID == t.skillID)
                return t.maxSkillXP;

        return -1;
    }

    public static int getTreeIndex(RPGTalentTree tree)
    {
        for (var i = 0; i < CharacterData.Instance.talentTrees.Count; i++)
            if (CharacterData.Instance.talentTrees[i].treeID == tree.ID)
                return i;
        return -1;
    }

    public static int getAbilityIndexInTree(RPGAbility ab, RPGTalentTree tree)
    {
        for (var i = 0; i < tree.nodeList.Count; i++)
            if (tree.nodeList[i].abilityID == ab.ID)
                return i;
        return -1;
    }
    public static int getBonusIndexInTree(RPGBonus ab, RPGTalentTree tree)
    {
        for (var i = 0; i < tree.nodeList.Count; i++)
            if (tree.nodeList[i].bonusID == ab.ID)
                return i;
        return -1;
    }

    public static bool hasPointsToSpendInClassTrees()
    {
        int points = 0;
        RPGClass classREF = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
        foreach (var t in classREF.talentTrees)
        {
            points += CharacterData.Instance.getTreePointsAmountByPoint(GetTalentTreeFromID(t.talentTreeID)
                .treePointAcceptedID);
        }

        return points > 0;
    }

    public static bool hasPointsToSpendInSkillTrees()
    {
        int points = 0;
        foreach (var t1 in CharacterData.Instance.skillsDATA)
        {
            RPGSkill skillREF = RPGBuilderUtilities.GetSkillFromID(t1.skillID);
            foreach (var t in skillREF.talentTrees)
            {
                points += CharacterData.Instance.getTreePointsAmountByPoint(GetTalentTreeFromID(t.talentTreeID)
                    .treePointAcceptedID);
            }
        }

        return points > 0;
    }
    public static bool hasPointsToSpendInSkill(int skillID)
    {
        int points = 0;
        RPGSkill skillREF = RPGBuilderUtilities.GetSkillFromID(skillID);
        foreach (var t in skillREF.talentTrees)
        {
            points += CharacterData.Instance.getTreePointsAmountByPoint(GetTalentTreeFromID(t.talentTreeID)
                .treePointAcceptedID);
        }

        return points > 0;
    }

    
    public static Sprite getItemQualitySprite(string quality)
    {
        int qualityIndex = -1;

        for (int i = 0; i < RPGBuilderEssentials.Instance.itemSettings.itemQuality.Length; i++)
        {
            if (RPGBuilderEssentials.Instance.itemSettings.itemQuality[i] == quality)
            {
                qualityIndex = i;
            }
        }

        return qualityIndex == -1 ? null : RPGBuilderEssentials.Instance.itemSettings.itemQualityImages[qualityIndex];
    }
    public static Color getItemQualityColor(string quality)
    {
        int qualityIndex = -1;

        for (int i = 0; i < RPGBuilderEssentials.Instance.itemSettings.itemQuality.Length; i++)
        {
            if (RPGBuilderEssentials.Instance.itemSettings.itemQuality[i] == quality)
            {
                qualityIndex = i;
            }
        }

        return qualityIndex == -1 ? Color.clear : RPGBuilderEssentials.Instance.itemSettings.itemQualityColors[qualityIndex];
    }

    public static int getRandomItemIndexFromID(int ID)
    {
        for (int i = 0; i < RandomizedItemsData.Instance.allRandomizedItems.Count; i++)
        {
            if (RandomizedItemsData.Instance.allRandomizedItems[i].id == ID)
            {
                return i;
            }
        }
        return -1;
    }
    public static int getRandomPlayerOwnerItemIndexFromID(int ID)
    {
        for (int i = 0; i < RandomizedItemsData.Instance.allPlayerOwnedRandomItems.Count; i++)
        {
            if (RandomizedItemsData.Instance.allPlayerOwnedRandomItems[i].id == ID)
            {
                return i;
            }
        }
        return -1;
    }
    public static RPGItemDATA.RandomItemData getRandomItemData(int id)
    {
        RPGItemDATA.RandomItemData rdmItemData = new RPGItemDATA.RandomItemData();
        rdmItemData.randomItemID = id;
        rdmItemData.randomStats = RandomizedItemsData.Instance.allRandomizedItems[getRandomItemIndexFromID(id)].randomStats;
        return rdmItemData;
    }
    
    public static int GenerateRandomItemStats(int itemID, bool isEquipped, bool addToPlayer)
    {
        RPGItem itemREF = GetItemFromID(itemID);
        List<RPGItemDATA.RandomizedStat> randomStats = new List<RPGItemDATA.RandomizedStat>();
        foreach (var t in itemREF.randomStats)
        {
            if (!(Random.Range(0f, 100f) <= t.chance)) continue;
            RPGItemDATA.RandomizedStat rdmStat = new RPGItemDATA.RandomizedStat();
            rdmStat.statID = t.statID;
            rdmStat.statValue = (float)Math.Round(Random.Range(t.minValue, t.maxValue), 2);
            if (t.isInt)
            {
                rdmStat.statValue = (float)Math.Round(rdmStat.statValue, 0);
            }
            randomStats.Add(rdmStat);
        }
        
        RandomizedItemsData.RandomizedItems newRandomItem = new RandomizedItemsData.RandomizedItems();
        newRandomItem.itemID = itemID;
        newRandomItem.id = RandomizedItemsData.Instance.nextAvailableID;
        newRandomItem.randomStats = randomStats;
        newRandomItem.state = isEquipped
            ? RPGItemDATA.randomItemState.equipped
            : RPGItemDATA.randomItemState.inBag;

        RandomizedItemsData.Instance.nextAvailableID++;
        RandomizedItemsData.Instance.allRandomizedItems.Add(newRandomItem);

        if (addToPlayer) addNonOwnedRandomItemToPlayer(newRandomItem.id);

        return newRandomItem.id;
    }
    public static void addNonOwnedRandomItemToPlayer(int rdmItemID)
    {
        foreach (var t in RandomizedItemsData.Instance.allRandomizedItems)
        {
            if (t.id != rdmItemID) continue;
            RandomizedItemsData.Instance.allPlayerOwnedRandomItems.Add(t);
            RandomizedItemsData.Instance.allRandomizedItems.Remove(t);
            return;
        }
    }
    public static int getRandomItemIDFromItemID(int itemID)
    {
        RPGItem itemREF = GetItemFromID(itemID);
        foreach (var t in RandomizedItemsData.Instance.allRandomizedItems)
        {
            RPGItem itemREF2 = GetItemFromID(t.itemID);
            if (itemREF2.equipmentSlot == itemREF.equipmentSlot)
            {
                return t.id;
            }
        }

        return -1;
    }
    public static RandomizedItemsData.RandomizedItems getRandomItemDataFromAllByID(int rdmItemID)
    {
        foreach (var t in RandomizedItemsData.Instance.allRandomizedItems)
        {
            if (t.id == rdmItemID)
            {
                return t;
            }
        }

        return null;
    }
    public static bool doPlayerOwnRandomItem(int rdmItemID)
    {
        foreach (var t in RandomizedItemsData.Instance.allPlayerOwnedRandomItems)
        {
            if (t.id == rdmItemID)
            {
                return true;
            }
        }

        return false;
    }

    public static void modifyPlayerOwnerRandomItemState(int randomItemID, RPGItemDATA.randomItemState newState)
    {
        int curIndex = getRandomPlayerOwnerItemIndexFromID(randomItemID);
        if (curIndex == -1) return;
        RandomizedItemsData.Instance.allPlayerOwnedRandomItems[curIndex].state = newState;
    }

    public static string addLineBreak(string text)
    {
        text += "\n";
        return text;
    }
    
    public static int getArmorSlotIndex(string slotType)
    {
        for (var i = 0; i < InventoryManager.Instance.equippedArmors.Count; i++)
            if (InventoryManager.Instance.equippedArmors[i].slotType == slotType)
                return i;
        return -1;
    }
    
    
    public static RPGItem getEquippedArmor(string slotType)
    {
        for (var i = 0; i < InventoryManager.Instance.equippedArmors.Count - 2; i++)
            if (InventoryManager.Instance.equippedArmors[i].slotType == slotType)
                return InventoryManager.Instance.equippedArmors[i].itemEquipped != null ? InventoryManager.Instance.equippedArmors[i].itemEquipped : null;

        return null;
    }

    public static float getAmountDifference(float val1, float val2)
    {
        return val1 > val2 ? val1 - val2 : val2 - val1;
    }
}