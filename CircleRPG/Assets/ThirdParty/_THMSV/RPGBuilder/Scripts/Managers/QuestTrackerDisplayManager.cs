using System;
using System.Collections.Generic;
using THMSV.RPGBuilder.UIElements;
using TMPro;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class QuestTrackerDisplayManager : MonoBehaviour
    {
        public CanvasGroup thisCG;
        private bool showing;

        public Color CompletionColor1, CompletionColor2, CompletionColor3, CompletionColor4, CompletionColor5;


        public GameObject trackedQuestSlotPrefab, trackedQuestObjectiveTextSlot;
        public Transform trackedQuestSlotParent;

        [Serializable]
        public class TrackedQuestDATA
        {
            public QuestTrackerSlotHolder slotREF;
            public RPGQuest quest;

            [Serializable]
            public class QuestObjectives
            {
                public RPGTask task;
                public TextMeshProUGUI text;
            }

            public List<QuestObjectives> objectives = new List<QuestObjectives>();
        }

        public List<TrackedQuestDATA> trackedQuest = new List<TrackedQuestDATA>();

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static QuestTrackerDisplayManager Instance { get; private set; }

        public bool isQuestAlreadyTracked(RPGQuest quest)
        {
            foreach (var t in trackedQuest)
                if (t.quest == quest)
                    return true;

            return false;
        }

        private int getTrackedQuestIndexFromQuest(RPGQuest quest)
        {
            for (var i = 0; i < trackedQuest.Count; i++)
                if (trackedQuest[i].quest == quest)
                    return i;
            return -1;
        }

        public void UpdateTrackerUI(RPGQuest quest, int objectiveID)
        {
            if (!isQuestAlreadyTracked(quest)) return;
            var objText = getObjectiveColor(quest, objectiveID);
            var trackedQuestIndex = getTrackedQuestIndexFromQuest(quest);
            objText += trackedQuest[trackedQuestIndex].objectives[objectiveID].task.description + ": " +
                       CharacterData.Instance.getQuestDATA(quest).objectives[objectiveID].currentProgressValue + " / " +
                       CharacterData.Instance.getQuestDATA(quest).objectives[objectiveID].maxProgressValue;

            trackedQuest[trackedQuestIndex].objectives[objectiveID].text.text = objText;
            trackedQuest[trackedQuestIndex].slotREF.globalCompletionText.text =
                CalculateGlobalCompletion(CharacterData.Instance.getQuestDATA(quest),
                    trackedQuest[trackedQuestIndex].slotREF);
        }

        public void TrackQuest(RPGQuest quest)
        {
            if (isQuestAlreadyTracked(quest)) return;
            var newTrackedQuest = new TrackedQuestDATA();
            newTrackedQuest.quest = quest;

            var newTrackedQuestSlot = Instantiate(trackedQuestSlotPrefab, trackedQuestSlotParent);
            var slotref = newTrackedQuestSlot.GetComponent<QuestTrackerSlotHolder>();
            slotref.InitSlot(quest);
            newTrackedQuest.slotREF = slotref;
            slotref.questNameText.text = quest.displayName;

            for (var i = 0; i < quest.objectives.Count; i++)
            {
                var newObjective = new TrackedQuestDATA.QuestObjectives();
                newObjective.task = RPGBuilderUtilities.GetTaskFromID(quest.objectives[i].taskID);
                var newObjectiveText = Instantiate(trackedQuestObjectiveTextSlot, slotref.objectiveTextParent);
                newObjective.text = newObjectiveText.GetComponent<TextMeshProUGUI>();
                var objRef = newObjectiveText.GetComponent<QuestObjectiveTextSlot>();
                var objText = getObjectiveColor(quest, i);
                objText += RPGBuilderUtilities.GetTaskFromID(quest.objectives[i].taskID).description + ": " +
                           CharacterData.Instance.getQuestDATA(quest).objectives[i].currentProgressValue + " / " +
                           CharacterData.Instance.getQuestDATA(quest).objectives[i].maxProgressValue;
                objRef.objectiveText.text = objText;
                newTrackedQuest.objectives.Add(newObjective);
            }

            slotref.globalCompletionText.text =
                CalculateGlobalCompletion(CharacterData.Instance.getQuestDATA(quest), slotref);
            trackedQuest.Add(newTrackedQuest);

            RPGBuilderUtilities.EnableCG(thisCG);
        }

        private string getObjectiveColor(RPGQuest quest, int i)
        {
            var text = "";
            var thisQuestDATA = CharacterData.Instance.getQuestDATA(quest);
            text = thisQuestDATA.objectives[i].state == QuestManager.questObjectiveState.completed ? "<color=green>" : "<color=red>";
            return text;
        }

        private string CalculateGlobalCompletion(CharacterData.QuestDATA trackedQuestDATA, QuestTrackerSlotHolder slotREF)
        {
            float completion = 0;
            float maxCompletion = trackedQuestDATA.objectives.Count * 100;
            foreach (var t in trackedQuestDATA.objectives)
                switch (t.state)
                {
                    case QuestManager.questObjectiveState.completed:
                        completion += 100;
                        break;
                    case QuestManager.questObjectiveState.failed:
                        completion += 0;
                        break;
                    case QuestManager.questObjectiveState.onGoing:
                    {
                        var taskREF = RPGBuilderUtilities.GetTaskFromID(t.taskID);
                        switch (taskREF.taskType)
                        {
                            case RPGTask.TASK_TYPE.enterRegion:
                                completion += 0;
                                break;
                            case RPGTask.TASK_TYPE.enterScene:
                                completion += 0;
                                break;
                            case RPGTask.TASK_TYPE.getItem:
                                completion += t.currentProgressValue /
                                    (float) t.maxProgressValue * 100;
                                break;
                            case RPGTask.TASK_TYPE.killNPC:
                                completion += t.currentProgressValue /
                                    (float) t.maxProgressValue * 100;
                                break;
                            case RPGTask.TASK_TYPE.learnAbility:
                                completion += 0;
                                break;
                            case RPGTask.TASK_TYPE.learnRecipe:
                                completion += 0;
                                break;
                            case RPGTask.TASK_TYPE.reachClassLevel:
                                completion += CharacterData.Instance.classDATA.currentClassLevel / (float) taskREF.taskValue *
                                              100;
                                break;
                            case RPGTask.TASK_TYPE.reachSkillLevel:
                                //completion += (CharacterData.Instance.currentClassLevel / trackedQuestDATA.objectives[i].task.taskValue) * 100;
                                break;
                            case RPGTask.TASK_TYPE.talkToNPC:
                                completion += 0;
                                break;
                            case RPGTask.TASK_TYPE.useItem:
                                completion += 0;
                                break;
                        }

                        break;
                    }
                }

            var globalCompletion = completion / maxCompletion * 100;
            SetTrackedQuestBGColor((int) globalCompletion, slotREF);
            return (int) globalCompletion + " %";
        }

        private void SetTrackedQuestBGColor(int percent, QuestTrackerSlotHolder slotREF)
        {
            if (percent >= 0 && percent <= 20)
                slotREF.backgroundImage.color = CompletionColor1;
            else if (percent > 20 && percent <= 40)
                slotREF.backgroundImage.color = CompletionColor2;
            else if (percent > 40 && percent <= 60)
                slotREF.backgroundImage.color = CompletionColor3;
            else if (percent > 60 && percent <= 80)
                slotREF.backgroundImage.color = CompletionColor4;
            else if (percent > 80 && percent <= 100) slotREF.backgroundImage.color = CompletionColor5;
        }


        public void UnTrackQuest(RPGQuest quest)
        {
            var index = getTrackedQuestIndexFromQuest(quest);
            Destroy(trackedQuest[index].slotREF.gameObject);
            trackedQuest.RemoveAt(index);

            if (trackedQuest.Count == 0) RPGBuilderUtilities.DisableCG(thisCG);
        }


        private void Show()
        {
            showing = true;
            RPGBuilderUtilities.EnableCG(thisCG);
        }


        public void Hide()
        {
            showing = false;
            RPGBuilderUtilities.DisableCG(thisCG);
        }

        private void Awake()
        {
            Hide();
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }
    }
}