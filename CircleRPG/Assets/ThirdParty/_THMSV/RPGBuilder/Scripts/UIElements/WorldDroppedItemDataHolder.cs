using System;
using System.Collections;
using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using THMSV.RPGBuilder.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class WorldDroppedItemDataHolder : MonoBehaviour
    {
        public Image BackgroundImage, BackgroundBorder;
        public TextMeshProUGUI NameText;


        public float defaultWaitBeforeStart;

        public float InterpolateSpeed;

        private Coroutine hpfilldelayCoroutine;

        private GameObject thisItemGO;

        private RPGItem thisItem;
        private WorldDroppedItem thisWorldDroppedItemREF;
        public GameObject GetThisItemGO()
        {
            return thisItemGO;
        }

        public void InitializeThisNameplate(GameObject itemGO, RPGItem itemREF)
        {
            thisItemGO = itemGO;

            BackgroundBorder.color = RPGBuilderUtilities.getItemQualityColor(itemREF.quality);
            NameText.text = itemREF.displayName;
            NameText.color = RPGBuilderUtilities.getItemQualityColor(itemREF.quality);
            thisItem = itemREF;
            thisWorldDroppedItemREF = thisItemGO.GetComponent<WorldDroppedItem>();
        }

        public void LootItem()
        {
            HideTooltip();
            InventoryManager.Instance.LootWorldDroppedItem(thisWorldDroppedItemREF);
        }

        public void ShowTooltip()
        {
            ItemTooltip.Instance.Show(thisItem.ID, InventoryManager.Instance.getWorldDroppedItemRandomID(thisWorldDroppedItemREF), true);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }

        public void ResetThisNameplate()
        {
            ScreenSpaceWorldDroppedItems.Instance.ResetThisNP(thisItemGO);
        }


        
    }
}