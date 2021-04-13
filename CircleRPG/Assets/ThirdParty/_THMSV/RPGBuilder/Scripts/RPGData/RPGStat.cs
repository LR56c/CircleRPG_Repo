using System;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New RPG Stat", menuName = "RPG BUILDER/Ability")]
public class RPGStat : ScriptableObject
{
    [Header("-----BASE DATA-----")] public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public string description;
    public bool minCheck;
    public float minValue;
    public bool maxCheck;
    public float maxValue;
    public float baseValue;
    public float bonusPerLevel;

    public bool isShifting;
    public float shiftAmount;
    public float shiftInterval;

    [Serializable]
    public class OnHitEffectsData
    {
        public RPGEffect effectREF;
        public int effectID = -1;
        public RPGCombatDATA.TARGET_TYPE targetType;
        public RPGAbility.ABILITY_TAGS tagType;
        public float chance = 100f;
    }

    public List<OnHitEffectsData> onHitEffectsData = new List<OnHitEffectsData>();
    
    public enum STAT_TYPE
    {
        NONE,
        DAMAGE,
        RESISTANCE,
        PENETRATION,
        VITALITY,
        HEALING,
        ABSORBTION,
        CC_POWER,
        CC_RESISTANCE,
        DMG_TAKEN,
        DMG_DEALT,
        HEAL_RECEIVED,
        HEAL_DONE,
        CAST_SPEED,
        CRIT_CHANCE,
        BASE_DAMAGE_TYPE,
        BASE_RESISTANCE_TYPE,
        SUMMON_COUNT,
        CD_RECOVERY_SPEED,
        GLOBAL_HEALING,
        LIFESTEAL,
        THORN,
        BLOCK_CHANCE,
        BLOCK_FLAT,
        BLOCK_MODIFIER,
        DODGE_CHANCE,
        GLANCING_BLOW_CHANCE,
        CRIT_POWER,
        DOT_BONUS,
        HOT_BONUS,
        HEALTH_ON_HIT,
        HEALTH_ON_KILL,
        HEALTH_ON_BLOCK,
        EFFECT_TRIGGER,
        LOOT_CHANCE_MODIFIER,
        EXPERIENCE_MODIFIER,
        VITALITY_REGEN,
        MINION_DAMAGE,
        MINION_PHYSICAL_DAMAGE,
        MINION_MAGICAL_DAMAGE,
        MINION_HEALTH,
        MINION_CRIT_CHANCE,
        MINION_CRIT_POWER,
        MINION_DURATION,
        MINION_LIFESTEAL,
        MINION_THORN,
        MINION_DODGE_CHANCE,
        MINION_GLANCING_BLOW_CHANCE,
        MINION_HEALTH_ON_HIT,
        MINION_HEALTH_ON_KILL,
        MINION_HEALTH_ON_BLOCK,
        PROJECTILE_SPEED,
        PROJECTILE_ANGLE_SPREAD,
        PROJECTILE_RANGE,
        PROJECTILE_COUNT,
        AOE_RADIUS,
        ABILITY_MAX_HIT,
        ABILITY_TARGET_MAX_RANGE,
        ABILITY_TARGET_MIN_RANGE

    }

    public STAT_TYPE statType;

    public string StatUICategory;

    public string StatFunction;
    public string OppositeStat;

    public RPGStat vitalityRegenBonusStatREF;
    public int vitalityRegenBonusStatID = -1;

    public void updateThis(RPGStat newStatDATA)
    {
        ID = newStatDATA.ID;
        _name = newStatDATA._name;
        _fileName = newStatDATA._fileName;
        isShifting = newStatDATA.isShifting;
        shiftAmount = newStatDATA.shiftAmount;
        shiftInterval = newStatDATA.shiftInterval;
        statType = newStatDATA.statType;
        StatFunction = newStatDATA.StatFunction;
        OppositeStat = newStatDATA.OppositeStat;
        minValue = newStatDATA.minValue;
        maxValue = newStatDATA.maxValue;
        baseValue = newStatDATA.baseValue;
        displayName = newStatDATA.displayName;
        description = newStatDATA.description;
        StatUICategory = newStatDATA.StatUICategory;
        bonusPerLevel = newStatDATA.bonusPerLevel;
        minCheck = newStatDATA.minCheck;
        maxCheck = newStatDATA.maxCheck;
        onHitEffectsData = newStatDATA.onHitEffectsData;
        vitalityRegenBonusStatID = newStatDATA.vitalityRegenBonusStatID;
    }
}