using UnityEngine;

public class RPGTask : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string displayName;
    public string _fileName;

    public string description;

    public enum TASK_TYPE
    {
        enterScene,
        enterRegion,
        learnAbility,
        learnRecipe,
        killNPC,
        getItem,
        reachClassLevel,
        reachSkillLevel,
        useItem,
        talkToNPC
    }

    public TASK_TYPE taskType;

    public string sceneName;
    public int abilityToLearnID = -1;
    public RPGAbility abilityToLearnREF;
    public int npcToKillID = -1;
    public RPGNpc npcToKillREF;
    public int itemToGetID = -1;
    public RPGItem itemToGetREF;
    public bool keepItems;
    public int classRequiredID = -1;
    public RPGClass classRequiredREF;
    public int skillRequiredID = -1;
    public RPGSkill skillRequiredREF;
    public int itemToUseID = -1;
    public RPGItem itemToUseREF;
    public int npcToTalkToID = -1;
    public RPGNpc npcToTalkToREF;
    public int taskValue;

    public void updateThis(RPGTask newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        displayName = newData.displayName;
        description = newData.description;
        taskType = newData.taskType;
        sceneName = newData.sceneName;
        abilityToLearnID = newData.abilityToLearnID;
        npcToKillID = newData.npcToKillID;
        taskValue = newData.taskValue;
        itemToGetID = newData.itemToGetID;
        classRequiredID = newData.classRequiredID;
        skillRequiredID = newData.skillRequiredID;
        itemToUseID = newData.itemToUseID;
        npcToTalkToID = newData.npcToTalkToID;
        keepItems = newData.keepItems;
    }
}