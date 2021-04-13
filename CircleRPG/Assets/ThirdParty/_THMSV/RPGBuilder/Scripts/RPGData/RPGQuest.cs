using System;
using System.Collections.Generic;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

public class RPGQuest : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string displayName;
    public string _fileName;


    public string description;
    public string ObjectiveText;
    public string ProgressText;

    public bool repeatable;
    public bool canBeTurnedInWithoutNPC;

    public List<RequirementsManager.RequirementDATA>
        questRequirements = new List<RequirementsManager.RequirementDATA>();

    [Serializable]
    public class QuestItemsGivenDATA
    {
        public int itemID = -1;
        public RPGItem itemREF;
        public int count;
    }

    public List<QuestItemsGivenDATA> itemsGiven = new List<QuestItemsGivenDATA>();

    public enum QuestObjectiveType
    {
        task
    }

    [Serializable]
    public class QuestObjectiveDATA
    {
        public QuestObjectiveType objectiveType;
        public int taskID = -1;
        public RPGTask taskREF;
        public float timeLimit;
    }

    public List<QuestObjectiveDATA> objectives = new List<QuestObjectiveDATA>();

    public enum QuestRewardType
    {
        item,
        currency,
        treePoint,
        Experience
    }

    [Serializable]
    public class QuestRewardDATA
    {
        public QuestRewardType rewardType;
        public int itemID = -1;
        public RPGItem itemREF;
        public int currencyID = -1;
        public RPGCurrency currencyREF;
        public int treePointID = -1;
        public RPGTreePoint treePointREF;
        public int count;
        public int Experience;
    }

    public List<QuestRewardDATA> rewardsGiven = new List<QuestRewardDATA>();
    public List<QuestRewardDATA> rewardsToPick = new List<QuestRewardDATA>();

    public void updateThis(RPGQuest newItemDATA)
    {
        ID = newItemDATA.ID;
        _name = newItemDATA._name;
        _fileName = newItemDATA._fileName;
        displayName = newItemDATA.displayName;
        description = newItemDATA.description;
        ObjectiveText = newItemDATA.ObjectiveText;
        ProgressText = newItemDATA.ProgressText;
        repeatable = newItemDATA.repeatable;
        questRequirements = newItemDATA.questRequirements;
        itemsGiven = newItemDATA.itemsGiven;
        objectives = newItemDATA.objectives;
        rewardsGiven = newItemDATA.rewardsGiven;
        rewardsToPick = newItemDATA.rewardsToPick;
        canBeTurnedInWithoutNPC = newItemDATA.canBeTurnedInWithoutNPC;
    }
}