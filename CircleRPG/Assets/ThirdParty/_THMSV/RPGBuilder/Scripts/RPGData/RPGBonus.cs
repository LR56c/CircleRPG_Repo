using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGBonus : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string displayName;
    public string _fileName;
    public Sprite icon;
    public bool learnedByDefault;

    [Serializable]
    public class rankDATA
    {
        public int rankID = -1;
        public RPGBonusRankDATA rankREF;
        public bool ShowedInEditor;
    }

    public List<rankDATA> ranks = new List<rankDATA>();

    [Serializable]
    public class BonusEffectsApplied
    {
        public int effectID = -1;
        public RPGEffect effectREF;
    }

    public void updateThis(RPGBonus newDATA)
    {
        ID = newDATA.ID;
        _name = newDATA._name;
        displayName = newDATA.displayName;
        _fileName = newDATA._fileName;
        icon = newDATA.icon;
        ranks = newDATA.ranks;
        learnedByDefault = newDATA.learnedByDefault;
    }
}