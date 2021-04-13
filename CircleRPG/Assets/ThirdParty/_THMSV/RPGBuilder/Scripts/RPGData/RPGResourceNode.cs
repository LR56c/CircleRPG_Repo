using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGResourceNode : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public Sprite icon;
    public bool learnedByDefault;
    public int skillRequiredID = -1;
    public RPGSkill skillRequiredREF;

    [Serializable]
    public class rankDATA
    {
        public int rankID = -1;
        public RPGResourceNodeRankData rankREF;
        public bool ShowedInEditor;
    }

    public List<rankDATA> ranks = new List<rankDATA>();


    public void updateThis(RPGResourceNode newDATA)
    {
        ID = newDATA.ID;
        _name = newDATA._name;
        _fileName = newDATA._fileName;
        icon = newDATA.icon;
        learnedByDefault = newDATA.learnedByDefault;
        ranks = newDATA.ranks;
        skillRequiredID = newDATA.skillRequiredID;
        displayName = newDATA.displayName;
    }
}