using System;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New RPG NPC", menuName = "RPG BUILDER/NPC")]
public class RPGNpc : ScriptableObject
{
    public enum ABILITY_CONDITION_TYPE
    {
        NONE,
        STAT_ABOVE,
        STAT_BELOW
    }

    public enum NPC_TYPE
    {
        MOB,
        RARE,
        BOSS,
        MERCHANT,
        BANK,
        QUEST_GIVER
    }

    public NPC_TYPE npcType;

    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public Sprite icon;

    public GameObject NPCPrefab;
    public bool isDummyTarget;

    public int merchantTableID = -1;
    public RPGMerchantTable merchantTableREF;

    [Serializable]
    public class NPC_QUEST_DATA
    {
        public int questID = -1;
        public RPGQuest questREF;
    }

    public List<NPC_QUEST_DATA> questGiven = new List<NPC_QUEST_DATA>();
    public List<NPC_QUEST_DATA> questCompleted = new List<NPC_QUEST_DATA>();

    public float AggroRange;
    public float distanceFromTarget;
    public float distanceFromOwner;
    public float DistanceToTargetReset;

    public float RoamRange;
    public float RoamDelay;

    public float MinRespawn;
    public float MaxRespawn;

    public int MinEXP;
    public int MaxEXP;
    public int EXPBonusPerLevel;

    public int MinLevel;
    public int MaxLevel;

    public enum ALIGNMENT_TYPE
    {
        ALLY,
        NEUTRAL,
        ENEMY
    }

    public ALIGNMENT_TYPE alignmentType;

    [Serializable]
    public class NPC_ABILITY_DATA
    {
        public int abilityID = -1;
        public RPGAbility abilityREF;
        public ABILITY_CONDITION_TYPE condition;
    }

    public List<NPC_ABILITY_DATA> abilities = new List<NPC_ABILITY_DATA>();


    [Serializable]
    public class NPC_STATS_DATA
    {
        public int statID = -1;
        public RPGStat statREF;
        public float minValue;
        public float maxValue;
        public float baseValue;
        public float bonusPerLevel;
    }

    public List<NPC_STATS_DATA> stats = new List<NPC_STATS_DATA>();

    [Serializable]
    public class LOOT_TABLES
    {
        public int lootTableID = -1;
        public RPGLootTable lootTableREF;
        public float dropRate = 100f;
    }

    public List<LOOT_TABLES> lootTables = new List<LOOT_TABLES>();

    public bool isCombatEnabled = true;
    public bool isMovementEnabled = true;
    
    public void updateThis(RPGNpc newNPCData)
    {
        ID = newNPCData.ID;
        _name = newNPCData._name;
        _fileName = newNPCData._fileName;
        AggroRange = newNPCData.AggroRange;
        distanceFromTarget = newNPCData.distanceFromTarget;
        RoamRange = newNPCData.RoamRange;
        RoamDelay = newNPCData.RoamDelay;
        DistanceToTargetReset = newNPCData.DistanceToTargetReset;
        abilities = newNPCData.abilities;
        alignmentType = newNPCData.alignmentType;
        stats = newNPCData.stats;
        isDummyTarget = newNPCData.isDummyTarget;
        lootTables = newNPCData.lootTables;
        MinRespawn = newNPCData.MinRespawn;
        MaxRespawn = newNPCData.MaxRespawn;
        npcType = newNPCData.npcType;
        MinEXP = newNPCData.MinEXP;
        MaxEXP = newNPCData.MaxEXP;
        EXPBonusPerLevel = newNPCData.EXPBonusPerLevel;
        MinLevel = newNPCData.MinLevel;
        MaxLevel = newNPCData.MaxLevel;
        distanceFromOwner = newNPCData.distanceFromOwner;
        merchantTableID = newNPCData.merchantTableID;
        questGiven = newNPCData.questGiven;
        questCompleted = newNPCData.questCompleted;
        NPCPrefab = newNPCData.NPCPrefab;
        displayName = newNPCData.displayName;
        isCombatEnabled = newNPCData.isCombatEnabled;
        isMovementEnabled = newNPCData.isMovementEnabled;
    }
}