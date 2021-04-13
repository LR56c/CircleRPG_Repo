using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class TreeNodeHolder : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
    {
        public RectTransform rect;
        public Image border, rankBorder, costBorder;
        public Image icon;
        public TextMeshProUGUI curRankText, costText;
        public CanvasGroup thisCG, costCG, rankUpCG, rankDownCG;
        private RPGAbility thisAb;
        private RPGCraftingRecipe thisRecipe;
        private RPGResourceNode thisResourceNode;
        private RPGBonus thisBonus;
        private RPGTalentTree thisTree;
        private GameObject curDraggedAbility;
        private RPGTalentTree.TalentTreeNodeType curTalentTreeNodeType;

        public bool used;

        public void Init(RPGTalentTree tree, TreesDisplayManager.TreeNodeSlotDATA nodeDATA)
        {
            used = true;
            curTalentTreeNodeType = nodeDATA.type;

            thisTree = tree;

            enableAllElements();
            var curNodeDATA = new CharacterData.TalentTrees_DATA.TalentTreeNode_DATA();
            var unlockCost = 0;
            var rank = -1;
            var maxRank = -1;
            switch (nodeDATA.type)
            {
                case RPGTalentTree.TalentTreeNodeType.ability:
                    icon.sprite = nodeDATA.ability.icon;
                    thisAb = nodeDATA.ability;
                    curNodeDATA = CharacterData.Instance.getTalentTreeNodeData(nodeDATA.ability.ID);
                    rank = curNodeDATA.rank;
                    if (curNodeDATA.known) rank--;
                    var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(nodeDATA.ability.ranks[rank].rankID);
                    unlockCost = rankREF.unlockCost;
                    maxRank = nodeDATA.ability.ranks.Count;

                    if (nodeDATA.ability.learnedByDefault && RPGBuilderUtilities.getNodeCurrentRank(nodeDATA.ability) == 0)
                        RPGBuilderUtilities.DisableCG(rankDownCG);
                    break;
                case RPGTalentTree.TalentTreeNodeType.recipe:
                    icon.sprite = nodeDATA.recipe.icon;
                    thisRecipe = nodeDATA.recipe;
                    curNodeDATA = CharacterData.Instance.getTalentTreeNodeData(nodeDATA.recipe);
                    rank = curNodeDATA.rank;
                    if (curNodeDATA.known) rank--;
                    var rankREF2 = RPGBuilderUtilities.GetCraftingRecipeRankFromID(nodeDATA.recipe.ranks[rank].rankID);
                    unlockCost = rankREF2.unlockCost;
                    maxRank = nodeDATA.recipe.ranks.Count;

                    if (nodeDATA.recipe.learnedByDefault && RPGBuilderUtilities.getNodeCurrentRank(nodeDATA.recipe) == 0)
                        RPGBuilderUtilities.DisableCG(rankDownCG);
                    break;
                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                    icon.sprite = nodeDATA.resourceNode.icon;
                    thisResourceNode = nodeDATA.resourceNode;
                    curNodeDATA = CharacterData.Instance.getTalentTreeNodeData(nodeDATA.resourceNode);
                    rank = curNodeDATA.rank;
                    if (curNodeDATA.known) rank--;
                    var rankREF3 = RPGBuilderUtilities.GetResourceNodeRankFromID(nodeDATA.resourceNode.ranks[rank].rankID);
                    unlockCost = rankREF3.unlockCost;
                    maxRank = nodeDATA.resourceNode.ranks.Count;

                    if (nodeDATA.resourceNode.learnedByDefault &&
                        RPGBuilderUtilities.getNodeCurrentRank(nodeDATA.resourceNode) == 0)
                        RPGBuilderUtilities.DisableCG(rankDownCG);
                    break;
                case RPGTalentTree.TalentTreeNodeType.bonus:
                    icon.sprite = nodeDATA.bonus.icon;
                    thisBonus = nodeDATA.bonus;
                    curNodeDATA = CharacterData.Instance.getTalentTreeNodeData(nodeDATA.bonus);
                    rank = curNodeDATA.rank;
                    if (curNodeDATA.known) rank--;
                    var rankREF4 = RPGBuilderUtilities.GetBonusRankFromID(nodeDATA.bonus.ranks[rank].rankID);
                    unlockCost = rankREF4.unlockCost;
                    maxRank = nodeDATA.bonus.ranks.Count;

                    if (nodeDATA.bonus.learnedByDefault && RPGBuilderUtilities.getNodeCurrentRank(nodeDATA.bonus) == 0)
                        RPGBuilderUtilities.DisableCG(rankDownCG);
                    break;
            }

            handleBorders(curNodeDATA.known);
            handleRank(curNodeDATA.rank == maxRank,
                unlockCost,
                CharacterData.Instance.getTreePointsAmountByPoint(tree.treePointAcceptedID) < unlockCost,
                curNodeDATA.rank);

            setCurRankText(curNodeDATA.rank + " / " + maxRank);
        }

        private void handleRank(bool maxRank, int cost, bool enoughPoints, int rank)
        {
            if (maxRank)
            {
                RPGBuilderUtilities.DisableCG(costCG);
                RPGBuilderUtilities.DisableCG(rankUpCG);
                border.color = TreesDisplayManager.Instance.MaxRankColor;
                rankBorder.color = TreesDisplayManager.Instance.MaxRankColor;
            }
            else
            {
                costText.text = cost.ToString();
                if (enoughPoints)
                {
                    //NOT ENOUGH POINTS
                    costBorder.color = TreesDisplayManager.Instance.NotUnlockedColor;
                    costText.color = TreesDisplayManager.Instance.NotUnlockedColor;
                }
                else
                {
                    costBorder.color = TreesDisplayManager.Instance.UnlockedColor;
                    costText.color = TreesDisplayManager.Instance.UnlockedColor;
                }
            }

            if (rank == 0) RPGBuilderUtilities.DisableCG(rankDownCG);
        }

        private void handleBorders(bool known)
        {
            if (known)
            {
                border.color = TreesDisplayManager.Instance.UnlockedColor;
                rankBorder.color = TreesDisplayManager.Instance.UnlockedColor;
            }
            else
            {
                border.color = TreesDisplayManager.Instance.NotUnlockedColor;
                rankBorder.color = TreesDisplayManager.Instance.NotUnlockedColor;
            }
        }

        private void enableAllElements()
        {
            RPGBuilderUtilities.EnableCG(costCG);
            RPGBuilderUtilities.EnableCG(rankUpCG);
            RPGBuilderUtilities.EnableCG(rankDownCG);
        }

        private void setCurRankText(string text)
        {
            curRankText.text = text;
        }

        public void InitHide()
        {
            used = false;
            RPGBuilderUtilities.DisableCG(thisCG);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                RankDown();
            }
            else
            {
                RankUp();
            }
        }
        
        public void RankUp()
        {
            switch (curTalentTreeNodeType)
            {
                case RPGTalentTree.TalentTreeNodeType.ability:
                    AbilityManager.Instance.RankUpAbility(thisAb, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.recipe:
                    CraftingManager.Instance.RankUpRecipe(thisRecipe, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                    GatheringManager.Instance.RankUpResourceNode(thisResourceNode, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.bonus:
                    BonusManager.Instance.RankUpBonus(thisBonus, thisTree);
                    break;
            }
        }

        public void RankDown()
        {
            switch (curTalentTreeNodeType)
            {
                case RPGTalentTree.TalentTreeNodeType.ability:
                    AbilityManager.Instance.RankDownAbility(thisAb, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.recipe:
                    CraftingManager.Instance.RankDownRecipe(thisRecipe, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                    GatheringManager.Instance.RankDownResourceNode(thisResourceNode, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.bonus:
                    BonusManager.Instance.RankDownBonus(thisBonus, thisTree);
                    break;
            }
        }

        public void ShowTooltip()
        {
            var curRank = 0;
            switch (curTalentTreeNodeType)
            {
                case RPGTalentTree.TalentTreeNodeType.ability:
                    AbilityTooltip.Instance.Show(thisAb);
                    TreesDisplayManager.Instance.ShowAbilityRequirements(thisAb, thisTree);
                    break;
                case RPGTalentTree.TalentTreeNodeType.recipe:
                    curRank = RPGBuilderUtilities.getNodeCurrentRank(thisRecipe);
                    if (curRank == -1) curRank = 0;
                    var rankREF = RPGBuilderUtilities.GetCraftingRecipeRankFromID(thisRecipe.ranks[curRank].rankID);
                    ItemTooltip.Instance.Show(rankREF.allCraftedItems[0].craftedItemID, -1, true);
                    break;
                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                    curRank = RPGBuilderUtilities.getNodeCurrentRank(thisResourceNode);
                    if (curRank == -1) curRank = 0;
                    var rankREF2 = RPGBuilderUtilities.GetResourceNodeRankFromID(thisResourceNode.ranks[curRank].rankID);
                    ItemTooltip.Instance.Show(RPGBuilderUtilities
                        .GetItemFromID(RPGBuilderUtilities.GetLootTableFromID(rankREF2.lootTableID).lootItems[0].itemID)
                        .ID, -1, true);
                    break;
                case RPGTalentTree.TalentTreeNodeType.bonus:
                    AbilityTooltip.Instance.ShowBonus(thisBonus);
                    TreesDisplayManager.Instance.ShowBonusRequirements(thisBonus, thisTree);
                    break;
            }
        }

        public void HideTooltip()
        {
            switch (curTalentTreeNodeType)
            {
                case RPGTalentTree.TalentTreeNodeType.ability:
                    AbilityTooltip.Instance.Hide();
                    TreesDisplayManager.Instance.HideRequirements();
                    break;
                case RPGTalentTree.TalentTreeNodeType.recipe:
                    ItemTooltip.Instance.Hide();
                    break;
                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                    ItemTooltip.Instance.Hide();
                    break;
                case RPGTalentTree.TalentTreeNodeType.bonus:
                    AbilityTooltip.Instance.Hide();
                    TreesDisplayManager.Instance.HideRequirements();
                    break;
            }
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (curTalentTreeNodeType != RPGTalentTree.TalentTreeNodeType.ability) return;
            var nodeDATA = CharacterData.Instance.getTalentTreeNodeData(thisAb.ID);
            if (nodeDATA.known)
            {
                curDraggedAbility = Instantiate(TreesDisplayManager.Instance.draggedNodeImage, transform.position,
                    Quaternion.identity);
                curDraggedAbility.transform.SetParent(TreesDisplayManager.Instance.draggedNodeParent);
                curDraggedAbility.GetComponent<Image>().sprite = thisAb.icon;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (curTalentTreeNodeType != RPGTalentTree.TalentTreeNodeType.ability) return;
            if (curDraggedAbility != null)
                curDraggedAbility.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (curTalentTreeNodeType != RPGTalentTree.TalentTreeNodeType.ability) return;
            if (curDraggedAbility == null) return;
            for (var i = 0; i < ActionBarDisplayManager.Instance.abilitySlots.Count; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    ActionBarDisplayManager.Instance.abilitySlots[i].slotREF.GetComponent<RectTransform>(),
                    Input.mousePosition))
                {
                    ActionBarDisplayManager.Instance.SetAbilityToSlot(thisAb, i);
                    Destroy(curDraggedAbility);
                }

            Destroy(curDraggedAbility);
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    }
}