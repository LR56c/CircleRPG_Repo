using System.Collections.Generic;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class GatheringManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static GatheringManager Instance { get; private set; }

        private bool CheckResourceNodeRankingRequirements(RPGResourceNode resourceNode, RPGTalentTree tree, int i, int x)
        {
            var rankREF =
                RPGBuilderUtilities.GetResourceNodeRankFromID(resourceNode
                    .ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank].rankID);
            if (CharacterData.Instance.getTreePointsAmountByPoint(tree.treePointAcceptedID) < rankREF.unlockCost)
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

        private bool CheckResourceNodeRankingDown(RPGResourceNode resourceNode, RPGTalentTree tree)
        {
            foreach (var t in tree.nodeList)
            foreach (var t1 in t.requirements)
                if (t1.requirementType ==
                    RequirementsManager.RequirementType.resourceNodeKnown &&
                    t1.resourceNodeRequiredID == resourceNode.ID &&
                    RPGBuilderUtilities.isResourceNodeKnown(t.resourceNodeID) &&
                    RPGBuilderUtilities.getNodeCurrentRank(resourceNode) == 0)
                {
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                        "Cannot unlearn a resource node that is required for others", 3);
                    return false;
                }

            return true;
        }

        public void RankUpResourceNode(RPGResourceNode ab, RPGTalentTree tree)
        {
            for (var i = 0; i < CharacterData.Instance.talentTrees.Count; i++)
                if (tree.ID == CharacterData.Instance.talentTrees[i].treeID)
                    for (var u = 0; u < CharacterData.Instance.talentTrees[i].nodes.Count; u++)
                        if (CharacterData.Instance.talentTrees[i].nodes[u].nodeData.resourceNodeID == ab.ID)
                            if (CharacterData.Instance.talentTrees[i].nodes[u].rank < RPGBuilderUtilities
                                .GetResourceNodeFromID(CharacterData.Instance.talentTrees[i].nodes[u].nodeData
                                    .resourceNodeID).ranks.Count)
                                if (CheckResourceNodeRankingRequirements(ab, tree, i, u))
                                {
                                    var rankREF = RPGBuilderUtilities.GetResourceNodeRankFromID(
                                        ab.ranks[CharacterData.Instance.talentTrees[i].nodes[u].rank].rankID);
                                    TreePointsManager.Instance.RemoveTreePoint(tree.treePointAcceptedID,
                                        rankREF.unlockCost);
                                    CharacterData.Instance.talentTrees[i].pointsSpent += rankREF.unlockCost;
                                    CharacterData.Instance.talentTrees[i].nodes[u].rank++;
                                    CharacterData.Instance.talentTrees[i].nodes[u].known = true;

                                    TreesDisplayManager.Instance.InitTree(tree);

                                    if (CharacterData.Instance.talentTrees[i].nodes[u].rank == 1)
                                        CharacterEventsManager.Instance.ResourceNodeLearned(ab);
                                }
        }

        public void RankDownResourceNode(RPGResourceNode ab, RPGTalentTree tree)
        {
            foreach (var t in CharacterData.Instance.talentTrees)
            foreach (var t1 in t.nodes)
                if (t1.nodeData.resourceNodeID == ab.ID)
                    if (t1.rank > 0)
                        if (CheckResourceNodeRankingDown(ab, tree))
                        {
                            var rankREF = RPGBuilderUtilities.GetResourceNodeRankFromID(ab .ranks[t1.rank].rankID-1);
                            TreePointsManager.Instance.AddTreePoint(tree.treePointAcceptedID, rankREF.unlockCost);
                            t.pointsSpent -= rankREF.unlockCost;
                            t1.rank--;

                            if (t1.rank == 0)
                                t1.known = false;
                            TreesDisplayManager.Instance.InitTree(tree);
                        }
        }
    }
}