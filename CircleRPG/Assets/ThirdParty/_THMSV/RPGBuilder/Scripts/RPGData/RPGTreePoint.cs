using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGTreePoint : ScriptableObject
{
    public int ID = -1;
    public Sprite icon;
    public string _name;
    public string _fileName;
    public string _displayName;
    public string description;

    public int startAmount;
    public int maxPoints;

    public enum TreePointGainRequirementTypes
    {
        classLevelUp,
        skillLevelUp,
        npcKilled,
        itemGained
    }

    [Serializable]
    public class GainRequirements
    {
        public TreePointGainRequirementTypes gainType;
        public int amountGained;

        public int classRequiredID = -1;
        public RPGClass classRequiredREF;
        public int skillRequiredID = -1;
        public RPGSkill skillRequiredREF;
        public int itemRequiredID = -1;
        public RPGItem itemRequiredREF;
        public int itemRequiredCount;
        public int npcRequiredID = -1;
        public RPGNpc npcRequiredREF;
    }

    public List<GainRequirements> gainPointRequirements = new List<GainRequirements>();

    public void updateThis(RPGTreePoint newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        description = newData.description;
        startAmount = newData.startAmount;
        maxPoints = newData.maxPoints;
        gainPointRequirements = newData.gainPointRequirements;
        icon = newData.icon;
    }
}