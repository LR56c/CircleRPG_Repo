using System.Collections.Generic;
using THMSV.RPGBuilder.UI;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class AbilityManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static AbilityManager Instance { get; private set; }

        private bool abilityRequiresThisWeaponType(string weaponType, RPGAbility ab, int curRank)
        {
            var abilityRankID = RPGBuilderUtilities.GetAbilityRankFromID(ab.ranks[curRank].rankID);
            foreach (var t in abilityRankID.useRequirements)
                if (t.requirementType ==
                    RequirementsManager.AbilityUseRequirementType.weaponTypeEquipped
                    && t.weaponRequired == weaponType
                    && !RequirementsManager.Instance.isWeaponTypeEquipped(weaponType))
                    return true;

            return false;
        }

        public void RankDownAbility(RPGAbility ab, RPGTalentTree tree)
        {
            foreach (var t in CharacterData.Instance.talentTrees)
            {
                if (tree.ID != t.treeID) continue;
                foreach (var t1 in t.nodes)
                {
                    if (t1.nodeData.abilityID != ab.ID) continue;
                    if (t1.rank <= 0) continue;
                    if (!CheckAbilityRankingDown(ab, tree)) continue;
                    var abilityRankID = RPGBuilderUtilities.GetAbilityRankFromID(ab.ranks[t1.rank - 1].rankID);
                    TreePointsManager.Instance.AddTreePoint(tree.treePointAcceptedID, abilityRankID.unlockCost);
                    t.pointsSpent -= abilityRankID.unlockCost;
                    t1.rank--;

                    if (t1.rank == 0) t1.known = false;
                    TreesDisplayManager.Instance.InitTree(tree);

                    AbilityTooltip.Instance.Hide();
                    TreesDisplayManager.Instance.HideRequirements();
                }
            }
        }

        public void RankUpAbility(RPGAbility ab, RPGTalentTree tree)
        {
            for (var i = 0; i < CharacterData.Instance.talentTrees.Count; i++)
                if (tree.ID == CharacterData.Instance.talentTrees[i].treeID)
                    for (var x = 0; x < CharacterData.Instance.talentTrees[i].nodes.Count; x++)
                        if (CharacterData.Instance.talentTrees[i].nodes[x].nodeData.abilityID == ab.ID)
                            if (CharacterData.Instance.talentTrees[i].nodes[x].rank < RPGBuilderUtilities
                                .GetAbilityFromID(CharacterData.Instance.talentTrees[i].nodes[x].nodeData.abilityID)
                                .ranks
                                .Count)
                                if (CheckAbilityRankingRequirements(ab, tree, i, x))
                                {
                                    var abilityRankID =
                                        RPGBuilderUtilities.GetAbilityRankFromID(
                                            ab.ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank].rankID);
                                    TreePointsManager.Instance.RemoveTreePoint(tree.treePointAcceptedID,
                                        abilityRankID.unlockCost);
                                    CharacterData.Instance.talentTrees[i].pointsSpent += abilityRankID.unlockCost;
                                    CharacterData.Instance.talentTrees[i].nodes[x].rank++;
                                    CharacterData.Instance.talentTrees[i].nodes[x].known = true;

                                    TreesDisplayManager.Instance.InitTree(tree);

                                    if (CharacterData.Instance.talentTrees[i].nodes[x].rank == 1)
                                        CharacterEventsManager.Instance.AbilityLearned(ab);

                                    AbilityTooltip.Instance.Hide();
                                    TreesDisplayManager.Instance.HideRequirements();
                                }
        }

        private bool CheckAbilityRankingDown(RPGAbility ab, RPGTalentTree tree)
        {
            foreach (var t in tree.nodeList)
            foreach (var t1 in t.requirements)
                if (t1.requirementType == RequirementsManager.RequirementType.abilityKnown &&
                    t1.abilityRequiredID == ab.ID &&
                    RPGBuilderUtilities.isAbilityKnown(t.abilityID) &&
                    RPGBuilderUtilities.getNodeCurrentRank(ab) == 0)
                {
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                        "Cannot unlearn an ability that is required for others", 3);
                    return false;
                }

            return true;
        }

        private bool CheckAbilityRankingRequirements(RPGAbility ab, RPGTalentTree tree, int i, int x)
        {
            var abilityRankID =
                RPGBuilderUtilities.GetAbilityRankFromID(ab.ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank]
                    .rankID);
            if (CharacterData.Instance.getTreePointsAmountByPoint(tree.treePointAcceptedID) < abilityRankID.unlockCost)
            {
                //NOT ENOUGH POINTS
                ErrorEventsDisplayManager.Instance.ShowErrorEvent("Not Enough Points", 3);
                return false;
            }

            List<bool> reqResults = new List<bool>();
            foreach (var t in tree.nodeList[x].requirements)
            {
                var intValue1 = 0;
                switch (t.requirementType)
                {
                    case RequirementsManager.RequirementType.classLevel:
                        intValue1 = CharacterData.Instance.classDATA.currentClassLevel;
                        break;
                    case RequirementsManager.RequirementType.skillLevel:
                        intValue1 = RPGBuilderUtilities.getSkillLevel(t.skillRequiredID);
                        break;
                    case RequirementsManager.RequirementType.pointSpent:
                        intValue1 = RPGBuilderUtilities.getTreePointSpentAmount(tree);
                        break;
                    case RequirementsManager.RequirementType._class:
                        intValue1 = t.classRequiredID;
                        break;
                }
                reqResults.Add(RequirementsManager.Instance.HandleRequirementType(t, intValue1, true));
            }

            return !reqResults.Contains(false);
        }
    }
}