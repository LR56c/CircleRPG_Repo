using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGClass : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public string description;
    public Sprite icon;

    public int autoAttackAbilityID = -1;
    public RPGAbility autoAttackAbilityREF;

    [Serializable]
    public class CLASS_STATS_DATA
    {
        public string _name;
        public int statID = -1;
        public RPGStat statREF;
        public float amount;
        public bool isPercent;
        public float bonusPerLevel;
    }

    public List<CLASS_STATS_DATA> stats = new List<CLASS_STATS_DATA>();

    public int levelTemplateID = -1;
    public RPGLevelsTemplate levelTemplateREF;

    [Serializable]
    public class TalentTreesDATA
    {
        public int talentTreeID = -1;
        public RPGTalentTree talentTreeREF;
    }

    public List<TalentTreesDATA> talentTrees = new List<TalentTreesDATA>();

    public List<RPGItemDATA.StartingItemsDATA> startItems = new List<RPGItemDATA.StartingItemsDATA>();
    public void updateThis(RPGClass newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        icon = newData.icon;
        description = newData.description;
        stats = newData.stats;
        levelTemplateID = newData.levelTemplateID;
        talentTrees = newData.talentTrees;
        autoAttackAbilityID = newData.autoAttackAbilityID;
        startItems = newData.startItems;
        displayName = newData.displayName;
    }
}