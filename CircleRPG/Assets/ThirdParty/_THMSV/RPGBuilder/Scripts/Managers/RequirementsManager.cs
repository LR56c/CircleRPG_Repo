using System;
using THMSV.RPGBuilder.LogicMono;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class RequirementsManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static RequirementsManager Instance { get; private set; }

        public enum RequirementType
        {
            pointSpent,
            classLevel,
            skillLevel,
            itemOwned,
            abilityKnown,
            recipeKnown,
            resourceNodeKnown,
            race,
            questState,
            npcKilled,
            bonusKnown,
            _class,
            abilityNotKnown,
            recipeNotKnown,
            resourceNodeNotKnown,
            bonusNotKnown,
        }

        [Serializable]
        public class RequirementDATA
        {
            public RequirementType requirementType;

            public int pointSpentValue;

            public int classRequiredID = -1;
            public RPGClass classRequiredREF;
            public int classLevelValue;

            public int itemRequiredID = -1;
            public RPGItem itemRequiredREF;
            public int itemRequiredCount;
            public bool consumeItem;

            public int skillRequiredID = -1;
            public RPGSkill skillRequiredREF;
            public int skillLevelValue;

            public int bonusRequiredID = -1;
            public RPGBonus bonusRequiredREF;

            public int abilityRequiredID = -1;
            public RPGAbility abilityRequiredREF;

            public int craftingRecipeRequiredID = -1;
            public RPGCraftingRecipe recipeRequiredREF;

            public int resourceNodeRequiredID = -1;
            public RPGResourceNode resourceNodeRequiredREF;

            public int raceRequiredID = -1;
            public RPGRace raceRequiredREF;

            public int questRequiredID = -1;
            public RPGQuest questRequiredREF;
            public QuestManager.questState questStateRequired;

            public int npcRequiredID = -1;
            public RPGNpc npcRequiredREF;
            public int npcKillsRequired;

            public string weaponRequired;
        }

        public enum AbilityUseRequirementType
        {
            item,
            weaponTypeEquipped,
            statCost
        }

        [Serializable]
        public class AbilityUseRequirementDATA
        {
            public AbilityUseRequirementType requirementType;

            public int itemRequiredID = -1;
            public RPGItem itemRequiredREF;
            public int itemRequiredCount;
            public bool consumeItem;

            public string weaponRequired;

            public int useCost;
            public int statCostID = -1;
            public RPGStat statCostREF;
        }

        public enum BonusRequirementType
        {
            weaponTypeEquipped,
            statState,
            pointSpent,
            _class,
            classLevel,
            skillLevel,
            itemOwned,
            abilityKnown,
            recipeKnown,
            resourceNodeKnown,
            race,
            questState,
            npcKilled
        }

        public enum StatStateType
        {
            above,
            below,
            equal
        }

        [Serializable]
        public class BonusRequirementDATA
        {
            public BonusRequirementType requirementType;

            public int itemRequiredID = -1;
            public RPGItem itemRequiredREF;
            public bool itemEquipped;

            public string weaponRequired;

            public int statValue;
            public int statID = -1;
            public RPGStat statREF;
            public StatStateType statStateRequired;
            public bool isStatValuePercent;

            public int pointSpentValue;

            public int classRequiredID = -1;
            public RPGClass classRequiredREF;
            public int classLevelValue;

            public int skillRequiredID = -1;
            public RPGSkill skillRequiredREF;
            public int skillLevelValue;

            public int abilityRequiredID = -1;
            public RPGAbility abilityRequiredREF;

            public int craftingRecipeRequiredID = -1;
            public RPGCraftingRecipe recipeRequiredREF;

            public int resourceNodeRequiredID = -1;
            public RPGResourceNode resourceNodeRequiredREF;

            public int raceRequiredID = -1;
            public RPGRace raceRequiredREF;

            public int questRequiredID = -1;
            public RPGQuest questRequiredREF;
            public QuestManager.questState questStateRequired;

            public int npcRequiredID = -1;
            public RPGNpc npcRequiredREF;
            public int npcKillsRequired;
        }

        public bool HandleRequirementType(RequirementDATA requirement, int intValue1, bool showErrorEvent)
        {
            switch (requirement.requirementType)
            {
                case RequirementType.classLevel:
                    return requirementCheckerGeneric(requirement.requirementType, intValue1, requirement.classLevelValue,
                        null, null, null, null, null, null, QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType._class:
                    return requirementCheckerGeneric(requirement.requirementType, intValue1, CharacterData.Instance.classDATA.classID,
                        null, null, null, null, null, null, QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.skillLevel:
                    return requirementCheckerGeneric(requirement.requirementType, intValue1, requirement.skillLevelValue,
                        null, null, null, null, null, null, QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.itemOwned:
                    return requirementCheckerGeneric(requirement.requirementType, requirement.itemRequiredCount, 0, null,
                        RPGBuilderUtilities.GetItemFromID(requirement.itemRequiredID), null, null, null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.pointSpent:
                    return requirementCheckerGeneric(requirement.requirementType, intValue1, requirement.pointSpentValue,
                        null, null, null, null, null, null, QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.abilityKnown:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0,
                        RPGBuilderUtilities.GetAbilityFromID(requirement.abilityRequiredID), null, null, null, null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.recipeKnown:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0, null, null,
                        RPGBuilderUtilities.GetCraftingRecipeFromID(requirement.craftingRecipeRequiredID), null, null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.resourceNodeKnown:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0, null, null, null,
                        RPGBuilderUtilities.GetResourceNodeFromID(requirement.resourceNodeRequiredID), null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.race:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0, null, null, null, null,
                        RPGBuilderUtilities.GetRaceFromID(requirement.raceRequiredID), null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.questState:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0, null, null, null, null, null,
                        RPGBuilderUtilities.GetQuestFromID(requirement.questRequiredID), requirement.questStateRequired,
                        null, showErrorEvent);
                case RequirementType.npcKilled:
                    return requirementCheckerGeneric(requirement.requirementType, requirement.npcKillsRequired, 0, null,
                        null, null, null, null, null, QuestManager.questState.onGoing,
                        RPGBuilderUtilities.GetNPCFromID(requirement.npcRequiredID), showErrorEvent);
                case RequirementType.abilityNotKnown:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0,
                        RPGBuilderUtilities.GetAbilityFromID(requirement.abilityRequiredID), null, null, null, null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.recipeNotKnown:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0, null, null,
                        RPGBuilderUtilities.GetCraftingRecipeFromID(requirement.craftingRecipeRequiredID), null, null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
                case RequirementType.resourceNodeNotKnown:
                    return requirementCheckerGeneric(requirement.requirementType, 0, 0, null, null, null,
                        RPGBuilderUtilities.GetResourceNodeFromID(requirement.resourceNodeRequiredID), null, null,
                        QuestManager.questState.onGoing, null, showErrorEvent);
            }

            return false;
        }

        private bool requirementCheckerGeneric(RequirementType reqType, int intValue1, int intValue2, RPGAbility ability,
            RPGItem item, RPGCraftingRecipe recipe, RPGResourceNode resourceNode, RPGRace race, RPGQuest quest,
            QuestManager.questState questState, RPGNpc npc, bool showErrorEvent)
        {
            switch (reqType)
            {
                case RequirementType.pointSpent:
                    if (intValue1 < intValue2)
                    {
                        //NOT ENOUGH POINTS SPENT
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "Not Enough Points have been spent in this tree", 3);
                        return false;
                    }

                    break;
                case RequirementType.classLevel:
                    if (intValue1 < intValue2)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The class level is too low", 3);
                        return false;
                    }
                    break;
                    
                case RequirementType._class:
                    if (intValue1 != intValue2)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (!showErrorEvent) return false;
                        RPGClass classREF = RPGBuilderUtilities.GetClassFromID(intValue1);
                        ErrorEventsDisplayManager.Instance.ShowErrorEvent("The " + classREF.displayName + " is required", 3);

                        return false;
                    }

                    break;
                case RequirementType.skillLevel:
                    if (intValue1 < intValue2)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The skill level is too low", 3);
                        return false;
                    }

                    break;
                case RequirementType.abilityKnown:
                    if (!RPGBuilderUtilities.isAbilityKnown(ability.ID))
                    {
                        //ABILITY IS NOT KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The ability " + ability.displayName + " needs to be known", 3);
                        return false;
                    }

                    break;
                case RequirementType.recipeKnown:
                    if (!RPGBuilderUtilities.isRecipeKnown(recipe.ID))
                    {
                        //ABILITY IS NOT KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The recipe " + recipe.displayName + " needs to be known", 3);
                        return false;
                    }

                    break;
                case RequirementType.resourceNodeKnown:
                    if (!RPGBuilderUtilities.isResourceNodeKnown(resourceNode.ID))
                    {
                        //ABILITY IS NOT KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The resource node " + resourceNode.displayName + " needs to be known", 3);
                        return false;
                    }

                    break;
                case RequirementType.itemOwned:
                    if (!InventoryManager.Instance.isItemOwned(item.ID, intValue1))
                    {
                        //ITEM NOT OWNED
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The items required are not in your bags", 3);
                        return false;
                    }

                    break;
                case RequirementType.race:
                    if (CharacterData.Instance.raceID != race.ID)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The " + race.displayName + " is required for this",
                                3);
                        return false;
                    }

                    break;
                case RequirementType.questState:
                    if (!QuestManager.Instance.isQuestMatchingState(quest, questState))
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The " + quest.displayName + " needs to be " + questState, 3);
                        return false;
                    }

                    break;
                case RequirementType.npcKilled:
                    if (!isNpcKilled(npc, intValue1))
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "You need to kill " + intValue1 + " " + npc.displayName, 3);
                        return false;
                    }

                    break;
                case RequirementType.abilityNotKnown:
                    if (RPGBuilderUtilities.isAbilityKnown(ability.ID))
                    {
                        //ABILITY IS KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The ability " + ability.displayName + " cannot be known", 3);
                        return false;
                    }

                    break;
                case RequirementType.recipeNotKnown:
                    if (RPGBuilderUtilities.isRecipeKnown(recipe.ID))
                    {
                        //ABILITY IS KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The recipe " + recipe.displayName + " cannot be known", 3);
                        return false;
                    }

                    break;
                case RequirementType.resourceNodeNotKnown:
                    if (RPGBuilderUtilities.isResourceNodeKnown(resourceNode.ID))
                    {
                        //ABILITY IS KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The resource node " + resourceNode.displayName + " cannot be known", 3);
                        return false;
                    }

                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool HandleAbilityRequirementUseType(CombatNode nodeREF, AbilityUseRequirementDATA requirement,
            bool showErrorEvent)
        {
            switch (requirement.requirementType)
            {
                case AbilityUseRequirementType.item:
                    return abilityUseRequirementCheckerGeneric(nodeREF, requirement.requirementType,
                        requirement.itemRequiredCount, 0, RPGBuilderUtilities.GetItemFromID(requirement.itemRequiredID),
                        null, "", showErrorEvent);
                case AbilityUseRequirementType.weaponTypeEquipped:
                    return abilityUseRequirementCheckerGeneric(nodeREF, requirement.requirementType, 0, 0, null, null,
                        requirement.weaponRequired, showErrorEvent);
                case AbilityUseRequirementType.statCost:
                    return abilityUseRequirementCheckerGeneric(nodeREF, requirement.requirementType, requirement.useCost, 0,
                        null, RPGBuilderUtilities.GetStatFromID(requirement.statCostID), null, showErrorEvent);
            }

            return false;
        }

        private bool abilityUseRequirementCheckerGeneric(CombatNode nodeREF, AbilityUseRequirementType reqType,
            int intValue1, int intValue2, RPGItem item, RPGStat stat, string weaponType, bool showErrorEvent)
        {
            switch (reqType)
            {
                case AbilityUseRequirementType.item:
                    if (!InventoryManager.Instance.isItemOwned(item.ID, intValue1))
                    {
                        //ITEM NOT OWNED
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The items required are not in your bags", 3);
                        return false;
                    }

                    break;
                case AbilityUseRequirementType.weaponTypeEquipped:
                    if (!isWeaponTypeEquipped(weaponType))
                    {
                        //ITEM NOT OWNED
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "Item of type " + weaponType + " is required to be equipped for this action", 3);
                        return false;
                    }

                    break;
                case AbilityUseRequirementType.statCost:
                    if (!checkCost(nodeREF, intValue1, stat))
                    {
                        //ITEM NOT OWNED
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("Not enough " + stat.displayName, 3);
                        return false;
                    }

                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool HandleBonusRequirementUseType(BonusRequirementDATA requirement, int intValue1, bool showErrorEvent)
        {
            switch (requirement.requirementType)
            {
                case BonusRequirementType.classLevel:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, intValue1,
                        requirement.classLevelValue, null, null, null, null, null, null, QuestManager.questState.onGoing,
                        null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.skillLevel:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, intValue1,
                        requirement.skillLevelValue, null, null, null, null, null, null, QuestManager.questState.onGoing,
                        null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.itemOwned:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, 0, 0, null,
                        RPGBuilderUtilities.GetItemFromID(requirement.itemRequiredID), null, null, null, null,
                        QuestManager.questState.onGoing, null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.pointSpent:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, intValue1,
                        requirement.pointSpentValue, null, null, null, null, null, null, QuestManager.questState.onGoing,
                        null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.abilityKnown:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, 0, 0,
                        RPGBuilderUtilities.GetAbilityFromID(requirement.abilityRequiredID), null, null, null, null, null,
                        QuestManager.questState.onGoing, null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.recipeKnown:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, 0, 0, null, null,
                        RPGBuilderUtilities.GetCraftingRecipeFromID(requirement.craftingRecipeRequiredID), null, null, null,
                        QuestManager.questState.onGoing, null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.resourceNodeKnown:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, 0, 0, null, null, null,
                        RPGBuilderUtilities.GetResourceNodeFromID(requirement.resourceNodeRequiredID), null, null,
                        QuestManager.questState.onGoing, null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.race:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, 0, 0, null, null, null, null,
                        RPGBuilderUtilities.GetRaceFromID(requirement.raceRequiredID), null,
                        QuestManager.questState.onGoing, null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.questState:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, 0, 0, null, null, null, null,
                        null, RPGBuilderUtilities.GetQuestFromID(requirement.questRequiredID),
                        requirement.questStateRequired, null, "", requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.npcKilled:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, false, requirement.npcKillsRequired,
                        0, null, null, null, null, null, null, QuestManager.questState.onGoing,
                        RPGBuilderUtilities.GetNPCFromID(requirement.npcRequiredID), "", requirement.statStateRequired,
                        showErrorEvent);
                case BonusRequirementType.weaponTypeEquipped:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, requirement.itemEquipped, 0, 0, null,
                        null, null, null, null, null, QuestManager.questState.onGoing, null, requirement.weaponRequired,
                        requirement.statStateRequired, showErrorEvent);
                case BonusRequirementType.statState:
                    return bonusRequirementCheckerGeneric(requirement.requirementType, requirement.isStatValuePercent,
                        requirement.statValue, 0, null, null, null, null, null, null, QuestManager.questState.onGoing, null,
                        requirement.weaponRequired, requirement.statStateRequired, showErrorEvent);
            }

            return false;
        }

        private bool bonusRequirementCheckerGeneric(BonusRequirementType reqType, bool boolValue1, int intValue1,
            int intValue2, RPGAbility ability, RPGItem item, RPGCraftingRecipe recipe, RPGResourceNode resourceNode,
            RPGRace race, RPGQuest quest, QuestManager.questState questState, RPGNpc npc, string weaponType,
            StatStateType statTate, bool showErrorEvent)
        {
            switch (reqType)
            {
                case BonusRequirementType.pointSpent:
                    if (intValue1 < intValue2)
                    {
                        //NOT ENOUGH POINTS SPENT
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "Not Enough Points have been spent in this tree.", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.classLevel:
                    if (intValue1 < intValue2)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The class level is too low", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.skillLevel:
                    if (intValue1 < intValue2)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The skill level is too low", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.abilityKnown:
                    if (!RPGBuilderUtilities.isAbilityKnown(ability.ID))
                    {
                        //ABILITY IS NOT KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The ability " + ability.displayName + " needs to be known", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.recipeKnown:
                    if (!RPGBuilderUtilities.isRecipeKnown(recipe.ID))
                    {
                        //ABILITY IS NOT KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The recipe " + recipe.displayName + " needs to be known", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.resourceNodeKnown:
                    if (!RPGBuilderUtilities.isResourceNodeKnown(resourceNode.ID))
                    {
                        //ABILITY IS NOT KNOWN
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The resource node " + resourceNode.displayName + " needs to be known", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.itemOwned:
                    if (!InventoryManager.Instance.isItemOwned(item.ID, intValue1))
                    {
                        //ITEM NOT OWNED
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The items required are not in your bags", 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.race:
                    if (CharacterData.Instance.raceID != race.ID)
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("The " + race.displayName + " is required for this",
                                3);
                        return false;
                    }

                    break;
                case BonusRequirementType.questState:
                    if (!QuestManager.Instance.isQuestMatchingState(quest, questState))
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "The " + quest.displayName + " needs to be " + questState, 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.npcKilled:
                    if (!isNpcKilled(npc, intValue1))
                    {
                        //CLASS NOT HIGH LVL ENOUGH
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "You need to kill " + intValue1 + " " + npc.displayName, 3);
                        return false;
                    }

                    break;
                case BonusRequirementType.weaponTypeEquipped:
                    if (!isWeaponTypeEquipped(weaponType))
                    {
                        //ITEM NOT OWNED
                        if (showErrorEvent)
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                "Item of type " + weaponType + " is required to be equipped for this action", 3);
                        return false;
                    }

                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool checkCost(CombatNode nodeRef, int costAmount, RPGStat stat)
        {
            if (stat.statType != RPGStat.STAT_TYPE.VITALITY) return false;
            var curCostStatAmount = (int) nodeRef.getCurrentValue(stat._name);
            return curCostStatAmount >= costAmount;
        }

        public bool isClassRequirementMet(RPGClass _class, int level)
        {
            return RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID) == _class &&
                   CharacterData.Instance.classDATA.currentClassLevel >= level;
        }

        public bool isWeaponTypeEquipped(string weaponType)
        {
            foreach (var t in InventoryManager.Instance.equippedWeapons)
                if (t.itemEquipped != null)
                    if (t.slotType == "WEAPON1" ||
                        t.slotType == "WEAPON2")
                        if (t.itemEquipped.weaponType == weaponType)
                            return true;

            return false;
        }

        private bool isNpcKilled(RPGNpc npc, int count)
        {
            foreach (var t in CharacterData.Instance.npcKilled)
                if (t.npcID == npc.ID &&
                    t.killedAmount >= count)
                    return true;

            return false;
        }

        public bool isItemOwned(RPGItem item, int count)
        {
            foreach (var t in InventoryManager.Instance.bags)
            foreach (var t1 in t.slots)
                if (t1.inUse &&
                    t1.itemStored == item &&
                    t1.curStack >= count)
                    return true;

            return false;
        }
    }
}