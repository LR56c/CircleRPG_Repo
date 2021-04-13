using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGSkill : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public string description;
    public Sprite icon;
    public bool automaticlyAdded;
    public int MaxLevel;
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
    public void updateThis(RPGSkill newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        icon = newData.icon;
        MaxLevel = newData.MaxLevel;
        description = newData.description;
        levelTemplateID = newData.levelTemplateID;
        automaticlyAdded = newData.automaticlyAdded;
        talentTrees = newData.talentTrees;
        startItems = newData.startItems;
        displayName = newData.displayName;
    }
}