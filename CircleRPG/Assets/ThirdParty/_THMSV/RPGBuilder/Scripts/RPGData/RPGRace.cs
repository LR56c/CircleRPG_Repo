using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGRace : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public string description;
    public Sprite icon;
    public int startingSceneID = -1;
    public RPGGameScene startingSceneREF;
    public int startingPositionID = -1;
    public RPGWorldPosition startingPositionREF;

    public enum RACE_GENDER
    {
        Male,
        Female
    }

    public GameObject malePrefab;
    public GameObject femalePrefab;

    [Serializable]
    public class RACE_CLASSES_DATA
    {
        public int classID = -1;
        public RPGClass classREF;
    }

    public List<RACE_CLASSES_DATA> availableClasses = new List<RACE_CLASSES_DATA>();

    [Serializable]
    public class RACE_STATS_DATA
    {
        public int statID = -1;
        public RPGStat statREF;
        public float amount;
        public bool isPercent;
    }

    public List<RACE_STATS_DATA> stats = new List<RACE_STATS_DATA>();

    public List<RPGItemDATA.StartingItemsDATA> startItems = new List<RPGItemDATA.StartingItemsDATA>();
    
    public void updateThis(RPGRace newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        icon = newData.icon;
        description = newData.description;
        malePrefab = newData.malePrefab;
        femalePrefab = newData.femalePrefab;
        availableClasses = newData.availableClasses;
        stats = newData.stats;
        startingSceneID = newData.startingSceneID;
        startingPositionID = newData.startingPositionID;
        startItems = newData.startItems;
        displayName = newData.displayName;
    }
}