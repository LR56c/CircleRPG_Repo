using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UI;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class TreePointsManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static TreePointsManager Instance { get; private set; }

        public void CheckIfItemGainPoints(RPGItem item)
        {
            foreach (var t in RPGBuilderEssentials.Instance.allTreePoints)
            foreach (var t1 in t.gainPointRequirements)
                if (t1.gainType ==
                    RPGTreePoint.TreePointGainRequirementTypes.itemGained
                    && t1.itemRequiredID == item.ID)
                    AddTreePoint(t.ID,
                        t1.amountGained);
        }

        public void CheckIfNPCkKilledGainPoints(RPGNpc npc)
        {
            foreach (var t in RPGBuilderEssentials.Instance.allTreePoints)
            foreach (var t1 in t.gainPointRequirements)
                if (t1.gainType ==
                    RPGTreePoint.TreePointGainRequirementTypes.npcKilled
                    && t1.npcRequiredID == npc.ID)
                    AddTreePoint(t.ID,
                        t1.amountGained);
        }

        public void CheckIfClassLevelUpGainPoints(RPGClass _class)
        {
            foreach (var t in RPGBuilderEssentials.Instance.allTreePoints)
            foreach (var t1 in t.gainPointRequirements)
                if (t1.gainType ==
                    RPGTreePoint.TreePointGainRequirementTypes.classLevelUp
                    && t1.classRequiredID == _class.ID)
                    AddTreePoint(t.ID,
                        t1.amountGained);
        }

        public void CheckIfSkillLevelUpGainPoints(RPGSkill _skill)
        {
            foreach (var t in RPGBuilderEssentials.Instance.allTreePoints)
            foreach (var t1 in t.gainPointRequirements)
                if (t1.gainType ==
                    RPGTreePoint.TreePointGainRequirementTypes.skillLevelUp
                    && t1.skillRequiredID == _skill.ID)
                    AddTreePoint(t.ID,
                        t1.amountGained);
        }

        public void AddTreePoint(int treeTypeID, int amount)
        {
            foreach (var t in CharacterData.Instance.treePoints)
                if (t.treePointID == treeTypeID)
                    t.amount += amount;

            Toolbar.Instance.InitToolbar();
        }

        public void RemoveTreePoint(int ID, int amount)
        {
            foreach (var t in CharacterData.Instance.treePoints)
                if (t.treePointID == ID)
                {
                    t.amount -= amount;
                    if (t.amount == 0)
                    {
                        Toolbar.Instance.InitToolbar();
                    }
                }

        }
    }
}