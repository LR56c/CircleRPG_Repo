using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGCraftingRecipe : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public Sprite icon;
    public bool learnedByDefault;
    public int craftingSkillID = -1;
    public RPGSkill craftingSkillREF;
    public int craftingStationID = -1;
    public RPGCraftingStation craftingStationREF;

    [Serializable]
    public class rankDATA
    {
        public int rankID = -1;
        public RPGCraftingRecipeRankData rankREF;
        public bool ShowedInEditor;
    }

    public List<rankDATA> ranks = new List<rankDATA>();


    public void updateThis(RPGCraftingRecipe newDATA)
    {
        _name = newDATA._name;
        _fileName = newDATA._fileName;
        icon = newDATA.icon;
        ID = newDATA.ID;
        ranks = newDATA.ranks;
        learnedByDefault = newDATA.learnedByDefault;
        craftingSkillID = newDATA.craftingSkillID;
        craftingStationID = newDATA.craftingStationID;
        displayName = newDATA.displayName;
    }
}