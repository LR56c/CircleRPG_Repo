using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class CraftingRecipeSlotHolder : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI nameText, statusText, countText;
        public RPGCraftingRecipe thisRecipe;

        public void InitSlot(RPGCraftingRecipe recipe)
        {
            icon.sprite = recipe.icon;
            nameText.text = recipe.displayName;
            thisRecipe = recipe;
        }

        public void UpdateState(string status, int count)
        {
            statusText.text = status;
            countText.text = count.ToString();
        }

        public void SelectRecipe()
        {
            CraftingPanelDisplayManager.Instance.DisplayRecipe(thisRecipe);
        }

        public void ShowTooltip()
        {
            var curRank = 0;
            curRank = RPGBuilderUtilities.getNodeCurrentRank(thisRecipe);
            var rankREF = RPGBuilderUtilities.GetCraftingRecipeRankFromID(thisRecipe.ranks[curRank].rankID);
            ItemTooltip.Instance.Show(rankREF.allCraftedItems[0].craftedItemID, -1, true);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }
    }
}