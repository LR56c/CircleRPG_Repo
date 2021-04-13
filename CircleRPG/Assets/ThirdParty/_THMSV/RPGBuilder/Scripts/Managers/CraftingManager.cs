using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class CraftingManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static CraftingManager Instance { get; private set; }

        public int getRecipeCraftCount(RPGCraftingRecipe recipe)
        {
            var craftCount = 0;
            var curRank = 0;
            curRank = RPGBuilderUtilities.getNodeCurrentRank(recipe);
            var recipeRankREF = RPGBuilderUtilities.GetCraftingRecipeRankFromID(recipe.ranks[curRank].rankID);
            foreach (var t in recipeRankREF.allComponents)
            {
                var totalOfThisComponent = 0;
                foreach (var t1 in InventoryManager.Instance.bags)
                foreach (var t2 in t1.slots)
                    if (t2.inUse && t2.itemStored.ID == t.componentItemID)
                        totalOfThisComponent += t2.curStack;

                if (totalOfThisComponent < t.count) return 0;

                totalOfThisComponent = totalOfThisComponent / t.count;
                if (totalOfThisComponent > craftCount) craftCount = totalOfThisComponent;
            }

            return craftCount;
        }

        public void GenerateCraftedItem(RPGCraftingRecipe recipeToCraft)
        {
            var curRank = 0;
            curRank = RPGBuilderUtilities.getNodeCurrentRank(recipeToCraft);
            var recipeRankREF = RPGBuilderUtilities.GetCraftingRecipeRankFromID(recipeToCraft.ranks[curRank].rankID);

            foreach (var t in recipeRankREF.allComponents) InventoryManager.Instance.RemoveItem(t.componentItemID, t.count, -1 ,-1, false);

            foreach (var t in recipeRankREF.allCraftedItems)
            {
                var chance = Random.Range(0f, 100f);
                RPGItem itemREF = RPGBuilderUtilities.GetItemFromID(t.craftedItemID);
                if (chance <= t.chance) InventoryManager.Instance.AddItem(t.craftedItemID, t.count,false, RPGBuilderUtilities.GenerateRandomItemStats(t.craftedItemID, false, true));
            }

            LevelingManager.Instance.GenerateSkillEXP(recipeToCraft.craftingSkillID, recipeRankREF.Experience);

            CraftingPanelDisplayManager.Instance.UpdateCraftingView();
        }

        private bool CheckRecipeRankingRequirements(RPGCraftingRecipe recipe, RPGTalentTree tree, int i, int x)
        {
            var recipeRankREF =
                RPGBuilderUtilities.GetCraftingRecipeRankFromID(recipe
                    .ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank].rankID);
            if (CharacterData.Instance.getTreePointsAmountByPoint(tree.treePointAcceptedID) < recipeRankREF.unlockCost)
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

        private bool CheckRecipeRankingDown(RPGCraftingRecipe recipe, RPGTalentTree tree)
        {
            foreach (var t in tree.nodeList)
            foreach (var t1 in t.requirements)
            {
                if (t1.requirementType != RequirementsManager.RequirementType.recipeKnown ||
                    t1.craftingRecipeRequiredID != recipe.ID || !RPGBuilderUtilities.isRecipeKnown(t.recipeID) ||
                    RPGBuilderUtilities.getNodeCurrentRank(recipe) != 0) continue;
                ErrorEventsDisplayManager.Instance.ShowErrorEvent("Cannot unlearn a recipe that is required for others", 3);
                return false;
            }

            return true;
        }

        public void RankUpRecipe(RPGCraftingRecipe ab, RPGTalentTree tree)
        {
            for (var i = 0; i < CharacterData.Instance.talentTrees.Count; i++)
            {
                if (tree.ID != CharacterData.Instance.talentTrees[i].treeID) continue;
                for (var x = 0; x < CharacterData.Instance.talentTrees[i].nodes.Count; x++)
                {
                    if (CharacterData.Instance.talentTrees[i].nodes[x].nodeData.recipeID != ab.ID) continue;
                    if (CharacterData.Instance.talentTrees[i].nodes[x].rank >= RPGBuilderUtilities
                        .GetCraftingRecipeFromID(CharacterData.Instance.talentTrees[i].nodes[x].nodeData.recipeID).ranks
                        .Count) continue;
                    if (!CheckRecipeRankingRequirements(ab, tree, i, x)) continue;
                    var recipeRankREF =
                        RPGBuilderUtilities.GetCraftingRecipeRankFromID(ab
                            .ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank].rankID);
                    TreePointsManager.Instance.RemoveTreePoint(tree.treePointAcceptedID, recipeRankREF.unlockCost);
                    CharacterData.Instance.talentTrees[i].pointsSpent += recipeRankREF.unlockCost;
                    CharacterData.Instance.talentTrees[i].nodes[x].rank++;
                    CharacterData.Instance.talentTrees[i].nodes[x].known = true;

                    TreesDisplayManager.Instance.InitTree(tree);

                    if (CharacterData.Instance.talentTrees[i].nodes[x].rank == 1)
                        CharacterEventsManager.Instance.RecipeLearned(ab);
                }
            }
        }

        public void RankDownRecipe(RPGCraftingRecipe ab, RPGTalentTree tree)
        {
            foreach (var t in CharacterData.Instance.talentTrees)
            foreach (var t1 in t.nodes)
            {
                if (t1.nodeData.recipeID != ab.ID) continue;
                if (t1.rank <= 0) continue;
                if (!CheckRecipeRankingDown(ab, tree)) continue;
                var recipeRankREF = RPGBuilderUtilities.GetCraftingRecipeRankFromID(ab.ranks[t1.rank-1].rankID);
                TreePointsManager.Instance.AddTreePoint(tree.treePointAcceptedID, recipeRankREF.unlockCost);
                t.pointsSpent -= recipeRankREF.unlockCost;
                t1.rank--;

                if (t1.rank == 0) t1.known = false;
                TreesDisplayManager.Instance.InitTree(tree);
            }
        }
    }
}