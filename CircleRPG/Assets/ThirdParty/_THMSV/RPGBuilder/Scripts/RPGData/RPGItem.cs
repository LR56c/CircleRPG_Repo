using System;
using System.Collections.Generic;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

//[CreateAssetMenu(fileName = "New RPG Item", menuName = "RPG BUILDER/Item")]
public class RPGItem : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public Sprite icon;

    public string equipmentSlot;
    public string itemType;
    public string weaponType;
    public string armorType;
    public string slotType;
    public string quality;

    public string itemModelName;
    public GameObject weaponModel;

    [Serializable]
    public class WeaponPositionData
    {
        public int raceID = -1;
        public RPGRace raceREF;

        [Serializable]
        public class GenderPositionData
        {
            public RPGRace.RACE_GENDER gender;

            public GameObject combatGORef, restGORef;
            
            public Vector3 CombatPositionInSlot = Vector3.zero;
            public Vector3 CombatRotationInSlot = Vector3.zero;
            public Vector3 CombatScaleInSlot = Vector3.one;
            public Vector3 RestPositionInSlot = Vector3.zero;
            public Vector3 RestRotationInSlot = Vector3.zero;
            public Vector3 RestScaleInSlot = Vector3.one;
        }
        public List<GenderPositionData> genderPositionDatas = new List<GenderPositionData>();
    }
    public List<WeaponPositionData> weaponPositionDatas = new List<WeaponPositionData>();
    public bool showWeaponPositionData;
    
    public float AttackSpeed;
    public int minDamage;
    public int maxDamage;

    public int autoAttackAbilityID = -1;
    public RPGAbility autoAttackAbilityREF;

    [Serializable]
    public class ITEM_STATS
    {
        public int statID = -1;
        public RPGStat statREF;
        public float amount;
        public bool isPercent;
    }

    public List<ITEM_STATS> stats = new List<ITEM_STATS>();

    public bool useRandomStats;
    public List<RPGItemDATA.RandomizedStatData> randomStats = new List<RPGItemDATA.RandomizedStatData>();

    public int sellPrice;
    public int sellCurrencyID = -1;
    public RPGCurrency sellCurrencyREF;
    public int buyPrice;
    public int buyCurrencyID = -1;
    public RPGCurrency buyCurrencyREF;
    public int stackLimit = 1;
    public string description;

    public bool dropInWorld;
    public GameObject itemWorldModel;
    public float durationInWorld = 60f;
    
    public List<RequirementsManager.RequirementDATA> useRequirements = new List<RequirementsManager.RequirementDATA>();

    public enum OnUseActionType
    {
        equip,
        useAbility,
        useEffect,
        learnAbility,
        learnRecipe,
        learnResourceNode,
        learnBonus,
        gainTreePoint,
        gainClassXP,
        gainSkillXP,
        gainClassLevel,
        gainSkillLevel,
        acceptQuest,
        currency
    }

    [Serializable]
    public class OnUseActionDATA
    {
        public OnUseActionType actionType;

        public int treePointGained;
        public int treePointID = -1;
        public RPGTreePoint treePointREF;

        public int classLevelGained;
        public int classXPGained;

        public int skillLevelGained;
        public int skillXPGained;
        public int skillID = -1;
        public RPGSkill skillREF;

        public int abilityID = -1;
        public RPGAbility abilityREF;

        public int recipeID = -1;
        public RPGCraftingRecipe recipeREF;

        public int resourceNodeID = -1;
        public RPGResourceNode resourceNodeREF;

        public int effectID = -1;
        public RPGEffect effectREF;

        public int bonusID = -1;
        public RPGBonus bonusREF;

        public int questID = -1;
        public RPGQuest questREF;

        public int currencyID = -1;
        public RPGCurrency currencyREF;
        
        public bool isConsumed;
    }

    public List<OnUseActionDATA> onUseActions = new List<OnUseActionDATA>();

    public void updateThis(RPGItem newItemDATA)
    {
        ID = newItemDATA.ID;
        _name = newItemDATA._name;
        _fileName = newItemDATA._fileName;
        icon = newItemDATA.icon;
        itemType = newItemDATA.itemType;
        weaponType = newItemDATA.weaponType;
        armorType = newItemDATA.armorType;
        slotType = newItemDATA.slotType;
        itemModelName = newItemDATA.itemModelName;
        equipmentSlot = newItemDATA.equipmentSlot;
        weaponModel = newItemDATA.weaponModel;
        weaponPositionDatas = newItemDATA.weaponPositionDatas;
        AttackSpeed = newItemDATA.AttackSpeed;
        minDamage = newItemDATA.minDamage;
        maxDamage = newItemDATA.maxDamage;
        stats = newItemDATA.stats;
        sellPrice = newItemDATA.sellPrice;
        buyPrice = newItemDATA.buyPrice;
        stackLimit = newItemDATA.stackLimit;
        description = newItemDATA.description;
        sellCurrencyID = newItemDATA.sellCurrencyID;
        buyCurrencyID = newItemDATA.buyCurrencyID;
        useRequirements = newItemDATA.useRequirements;
        onUseActions = newItemDATA.onUseActions;
        autoAttackAbilityID = newItemDATA.autoAttackAbilityID;
        displayName = newItemDATA.displayName;
        quality = newItemDATA.quality;
        dropInWorld = newItemDATA.dropInWorld;
        itemWorldModel = newItemDATA.itemWorldModel;
        durationInWorld = newItemDATA.durationInWorld;
        randomStats = newItemDATA.randomStats;
    }
}