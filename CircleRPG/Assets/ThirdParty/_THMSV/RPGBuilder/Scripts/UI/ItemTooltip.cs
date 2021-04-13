using System;
using System.Collections.Generic;
using THMSV.RPGBuilder.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UI
{
    public class ItemTooltip : MonoBehaviour
    {
        public CanvasGroup thisCG;

        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemSlotTypeText;
        public TextMeshProUGUI itemTypeText, itemQualityText;
        public Image icon;
        
        public TextMeshProUGUI statsText, descriptionText, requirementsText, sellPriceText, statsChangeText;

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        void ResetContent()
        {
            statsText.text = "";
            descriptionText.text = "";
            requirementsText.text = "";
            sellPriceText.text = "";
            statsChangeText.text = "";
        }

        public void ShowCurrencyTooltip(int currencyID)
        {
            ResetContent();
            RPGBuilderUtilities.EnableCG(thisCG);

            var currency = RPGBuilderUtilities.GetCurrencyFromID(currencyID);

            itemNameText.text = currency.displayName;
            icon.sprite = currency.icon;
            itemSlotTypeText.text = "";

        }

        public void ShowTreePointTooltip(int ID)
        {
            ResetContent();
            RPGBuilderUtilities.EnableCG(thisCG);

            var treePoint = RPGBuilderUtilities.GetTreePointFromID(ID);

            itemNameText.text = treePoint._displayName;
            icon.sprite = treePoint.icon;
            itemSlotTypeText.text = "";
        }

        public void Show(int itemID, int rdmItemID, bool showCompare)
        {
            ResetContent();
            thisCG.alpha = 1;

            var item = RPGBuilderUtilities.GetItemFromID(itemID);

            itemNameText.text = item.displayName;
            itemSlotTypeText.text = "";
            switch (item.itemType)
            {
                case "WEAPON":
                    itemTypeText.text = item.weaponType;
                    itemSlotTypeText.text = item.slotType;
                    break;
                case "ARMOR":
                    itemTypeText.text = item.equipmentSlot;
                    itemSlotTypeText.text = item.armorType;
                    break;
            }

            itemQualityText.text = item.quality;
            itemQualityText.color = RPGBuilderUtilities.getItemQualityColor(item.quality);

            icon.sprite = item.icon;


            foreach (var t in item.stats)
            {
                float amt = t.amount;
                if (amt == 0) continue;

                string modifierText = amt > 0 ? "+" : "-";
                statsText.text += RPGBuilderUtilities.addLineBreak(modifierText + " " + amt + " " +
                                                                   RPGBuilderUtilities.GetStatFromID(t.statID)
                                                                       .displayName);
            }

            if (rdmItemID != -1)
            {
                List<RPGItemDATA.RandomizedStat> rdmStatList = new List<RPGItemDATA.RandomizedStat>();
                int rdmItemIndex = RPGBuilderUtilities.getRandomPlayerOwnerItemIndexFromID(rdmItemID);
                if (rdmItemIndex == -1)
                {
                    rdmItemIndex = RPGBuilderUtilities.getRandomItemIndexFromID(rdmItemID);
                    rdmStatList = RandomizedItemsData.Instance.allRandomizedItems[rdmItemIndex].randomStats;
                }
                else
                {
                    rdmStatList = RandomizedItemsData.Instance.allPlayerOwnedRandomItems[rdmItemIndex].randomStats;
                }

                if (rdmItemIndex != -1)
                {
                    foreach (var t in rdmStatList)
                    {
                        float amt = t.statValue;
                        if (amt == 0) continue;

                        string modifierText = amt > 0 ? "+" : "-";
                        statsText.text += RPGBuilderUtilities.addLineBreak(modifierText + " " + amt + " " +
                                                                           RPGBuilderUtilities.GetStatFromID(t.statID)
                                                                               .displayName);
                    }
                }
            }

            descriptionText.text = item.description;

            if (item.sellCurrencyID != -1)
            {
                RPGCurrency currencyREF = RPGBuilderUtilities.GetCurrencyFromID(item.sellCurrencyID);
                sellPriceText.text = item.sellPrice + " " + currencyREF.displayName;
            }
            

            foreach (var t in item.useRequirements)
            {
                bool reqMet = false;
                string reqText = "";
                var intValue1 = -1;
                switch (t.requirementType)
                {
                    case RequirementsManager.RequirementType._class:
                        intValue1 = t.classRequiredID;
                        reqText = "Class: " + RPGBuilderUtilities.GetClassFromID(t.classRequiredID).displayName;
                        break;
                    case RequirementsManager.RequirementType.race:
                        reqText = "Race: " + RPGBuilderUtilities.GetRaceFromID(t.raceRequiredID).displayName;
                        break;
                    case RequirementsManager.RequirementType.abilityKnown:
                        reqText = RPGBuilderUtilities.GetAbilityFromID(t.abilityRequiredID).displayName + " Known";
                        break;
                    case RequirementsManager.RequirementType.bonusKnown:
                        reqText = RPGBuilderUtilities.GetBonusFromID(t.bonusRequiredID).displayName + " Known";
                        break;
                    case RequirementsManager.RequirementType.classLevel:
                        intValue1 = CharacterData.Instance.classDATA.currentClassLevel;
                        reqText = "Level: " + t.classLevelValue;
                        break;
                    case RequirementsManager.RequirementType.itemOwned:
                        reqText = "Item Owned: " + RPGBuilderUtilities.GetItemFromID(t.itemRequiredID).displayName;
                        break;
                    case RequirementsManager.RequirementType.npcKilled:
                        reqText = "Killed: " + RPGBuilderUtilities.GetNPCFromID(t.npcRequiredID).displayName;
                        break;
                    case RequirementsManager.RequirementType.questState:
                        reqText = "Quest: " + RPGBuilderUtilities.GetQuestFromID(t.questRequiredID).displayName + " " + t.questStateRequired;
                        break;
                    case RequirementsManager.RequirementType.recipeKnown:
                        reqText = "Recipe: " + RPGBuilderUtilities.GetCraftingRecipeFromID(t.craftingRecipeRequiredID).displayName + " Known";
                        break;
                    case RequirementsManager.RequirementType.skillLevel:
                        intValue1 = RPGBuilderUtilities.getSkillLevel(t.skillRequiredID);
                        reqText = RPGBuilderUtilities.GetSkillFromID(t.skillRequiredID).displayName + " Level " + t.skillLevelValue;
                        break;
                    case RequirementsManager.RequirementType.abilityNotKnown:
                        reqText = RPGBuilderUtilities.GetAbilityFromID(t.abilityRequiredID).displayName + " Unkown";
                        break;
                    case RequirementsManager.RequirementType.bonusNotKnown:
                        reqText = RPGBuilderUtilities.GetBonusFromID(t.bonusRequiredID).displayName + " Unkown";
                        break;
                    case RequirementsManager.RequirementType.recipeNotKnown:
                        reqText = RPGBuilderUtilities.GetCraftingRecipeFromID(t.craftingRecipeRequiredID).displayName + " Unkown";
                        break;
                    case RequirementsManager.RequirementType.resourceNodeKnown:
                        reqText = RPGBuilderUtilities.GetResourceNodeFromID(t.resourceNodeRequiredID).displayName + " Known";
                        break;
                    case RequirementsManager.RequirementType.resourceNodeNotKnown:
                        reqText = RPGBuilderUtilities.GetResourceNodeFromID(t.resourceNodeRequiredID).displayName + " Unkown";
                        break;
                }
                
                reqMet = RequirementsManager.Instance.HandleRequirementType(t, intValue1, false);
                reqText = AssignRequirementColor(reqMet, reqText);
                requirementsText.text += RPGBuilderUtilities.addLineBreak(reqText);
            }

            if (showCompare)
            {
                // SHOW STAT DIFFERENCES

                RPGItem itemREF = null;
                List<string> statGains = new List<string>();
                List<string> statLosses = new List<string>();
                switch (item.itemType)
                {
                    case "ARMOR":
                        itemREF = RPGBuilderUtilities.getEquippedArmor(item.equipmentSlot);
                        int armorIndex = RPGBuilderUtilities.getArmorSlotIndex(item.equipmentSlot);
                        if (itemREF != null)
                        {
                            foreach (var t in CombatManager.playerCombatInfo.nodeStats)
                            {
                                float armorPieceVal = 0;
                                float inspectedWeaponStatVal = getItemStatValue(t.stat.ID, item, rdmItemID);

                                armorPieceVal = getItemStatValue(t.stat.ID, itemREF,
                                    InventoryManager.Instance.equippedArmors[armorIndex].temporaryRandomItemID);

                                if (inspectedWeaponStatVal == 0 && armorPieceVal == 0) continue;
                                if (inspectedWeaponStatVal == armorPieceVal) continue;
                                if (inspectedWeaponStatVal != 0)
                                {
                                    bool isGain = !(armorPieceVal > inspectedWeaponStatVal);
                                    float diffAmt = RPGBuilderUtilities.getAmountDifference(inspectedWeaponStatVal,
                                        armorPieceVal);
                                    diffAmt = (float) Math.Round(diffAmt, 2);
                                    string statChangeText = "";
                                    string modifierText = isGain ? "+" : "-";
                                    statChangeText += AssignRequirementColor(isGain,
                                        modifierText + " " + diffAmt + " " + t.stat.displayName);
                                    statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                    if (isGain)
                                        statGains.Add(statChangeText);
                                    else
                                        statLosses.Add(statChangeText);
                                }
                                else
                                {
                                    if (armorPieceVal == 0) continue;
                                    armorPieceVal = (float) Math.Round(armorPieceVal, 2);
                                    string statChangeText = "";
                                    string modifierText = "-";
                                    statChangeText += AssignRequirementColor(false,
                                        modifierText + " " + armorPieceVal + " " + t.stat.displayName);
                                    statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                    statLosses.Add(statChangeText);
                                }
                            }
                        }

                        break;
                    case "WEAPON":
                        switch (item.slotType)
                        {
                            case "TWO HAND":
                                itemREF = InventoryManager.Instance.equippedWeapons[0].itemEquipped;
                                RPGItem itemREF2 = InventoryManager.Instance.equippedWeapons[1].itemEquipped;

                                if (itemREF == null && itemREF2 == null) break;
                                foreach (var t in CombatManager.playerCombatInfo.nodeStats)
                                {
                                    float weapon1StatVal = 0;
                                    float weapon2StatVal = 0;
                                    float inspectedWeaponStatVal = getItemStatValue(t.stat.ID, item, rdmItemID);
                                    if (itemREF != null)
                                    {
                                        weapon1StatVal = getItemStatValue(t.stat.ID, itemREF,
                                            InventoryManager.Instance.equippedWeapons[0].temporaryRandomItemID);
                                    }

                                    if (itemREF2 != null)
                                    {
                                        weapon2StatVal = getItemStatValue(t.stat.ID, itemREF2,
                                            InventoryManager.Instance.equippedWeapons[1].temporaryRandomItemID);
                                    }
                                    
                                    float otherWeaponsStatVal = weapon1StatVal + weapon2StatVal;
                                    if(inspectedWeaponStatVal == 0 && otherWeaponsStatVal == 0) continue;
                                    if(inspectedWeaponStatVal == otherWeaponsStatVal) continue;
                                    if (inspectedWeaponStatVal != 0)
                                    {
                                        bool isGain = !(otherWeaponsStatVal > inspectedWeaponStatVal);
                                        float diffAmt = RPGBuilderUtilities.getAmountDifference(inspectedWeaponStatVal,
                                            otherWeaponsStatVal);
                                        diffAmt = (float)Math.Round(diffAmt, 2);
                                        string statChangeText = "";
                                        string modifierText = isGain ? "+" : "-";
                                        statChangeText += AssignRequirementColor(isGain,
                                            modifierText + " " + diffAmt + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        if (isGain)
                                            statGains.Add(statChangeText);
                                        else
                                            statLosses.Add(statChangeText);
                                    }
                                    else
                                    {
                                        if (otherWeaponsStatVal == 0) continue;
                                        otherWeaponsStatVal = (float)Math.Round(otherWeaponsStatVal, 2);
                                        string statChangeText = "";
                                        string modifierText = "-";
                                        statChangeText += AssignRequirementColor(false,
                                            modifierText + " " + otherWeaponsStatVal + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        statLosses.Add(statChangeText);
                                    }
                                }
                                break;
                            case "MAIN HAND":
                                itemREF = InventoryManager.Instance.equippedWeapons[0].itemEquipped;

                                if (itemREF == null) break;
                                foreach (var t in CombatManager.playerCombatInfo.nodeStats)
                                {
                                    float weapon1StatVal = 0;
                                    float inspectedWeaponStatVal = getItemStatValue(t.stat.ID, item, rdmItemID);
                                    if (itemREF != null)
                                    {
                                        weapon1StatVal = getItemStatValue(t.stat.ID, itemREF,
                                            InventoryManager.Instance.equippedWeapons[0].temporaryRandomItemID);
                                    }
                                    
                                    if(inspectedWeaponStatVal == 0 && weapon1StatVal == 0) continue;
                                    if(inspectedWeaponStatVal == weapon1StatVal) continue;
                                    if (inspectedWeaponStatVal != 0)
                                    {
                                        bool isGain = !(weapon1StatVal > inspectedWeaponStatVal);
                                        float diffAmt = RPGBuilderUtilities.getAmountDifference(inspectedWeaponStatVal,
                                            weapon1StatVal);
                                        diffAmt = (float)Math.Round(diffAmt, 2);
                                        string statChangeText = "";
                                        string modifierText = isGain ? "+" : "-";
                                        statChangeText += AssignRequirementColor(isGain,
                                            modifierText + " " + diffAmt + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        if (isGain)
                                            statGains.Add(statChangeText);
                                        else
                                            statLosses.Add(statChangeText);
                                    }
                                    else
                                    {
                                        if (weapon1StatVal == 0) continue;
                                        weapon1StatVal = (float)Math.Round(weapon1StatVal, 2);
                                        string statChangeText = "";
                                        string modifierText = "-";
                                        statChangeText += AssignRequirementColor(false,
                                            modifierText + " " + weapon1StatVal + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        statLosses.Add(statChangeText);
                                    }
                                }
                                break;
                            case "OFF HAND":
                                itemREF = InventoryManager.Instance.equippedWeapons[1].itemEquipped;

                                if (itemREF == null) break;
                                foreach (var t in CombatManager.playerCombatInfo.nodeStats)
                                {
                                    float weapon1StatVal = 0;
                                    float inspectedWeaponStatVal = getItemStatValue(t.stat.ID, item, rdmItemID);
                                    if (itemREF != null)
                                    {
                                        weapon1StatVal = getItemStatValue(t.stat.ID, itemREF,
                                            InventoryManager.Instance.equippedWeapons[1].temporaryRandomItemID);
                                    }
                                    
                                    if(inspectedWeaponStatVal == 0 && weapon1StatVal == 0) continue;
                                    if(inspectedWeaponStatVal == weapon1StatVal) continue;
                                    if (inspectedWeaponStatVal != 0)
                                    {
                                        bool isGain = !(weapon1StatVal > inspectedWeaponStatVal);
                                        float diffAmt = RPGBuilderUtilities.getAmountDifference(inspectedWeaponStatVal,
                                            weapon1StatVal);
                                        diffAmt = (float)Math.Round(diffAmt, 2);
                                        string statChangeText = "";
                                        string modifierText = isGain ? "+" : "-";
                                        statChangeText += AssignRequirementColor(isGain,
                                            modifierText + " " + diffAmt + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        if (isGain)
                                            statGains.Add(statChangeText);
                                        else
                                            statLosses.Add(statChangeText);
                                    }
                                    else
                                    {
                                        if (weapon1StatVal == 0) continue;
                                        weapon1StatVal = (float)Math.Round(weapon1StatVal, 2);
                                        string statChangeText = "";
                                        string modifierText = "-";
                                        statChangeText += AssignRequirementColor(false,
                                            modifierText + " " + weapon1StatVal + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        statLosses.Add(statChangeText);
                                    }
                                }
                                break;
                            case "ANY HAND":
                                int weaponComparedIndex = 0;
                                itemREF = InventoryManager.Instance.equippedWeapons[0].itemEquipped;

                                if (itemREF == null)
                                {
                                    itemREF = InventoryManager.Instance.equippedWeapons[1].itemEquipped;
                                    weaponComparedIndex = 1;
                                }
                                if (itemREF == null) break;
                                foreach (var t in CombatManager.playerCombatInfo.nodeStats)
                                {
                                    float weapon1StatVal = 0;
                                    float inspectedWeaponStatVal = getItemStatValue(t.stat.ID, item, rdmItemID);
                                    if (itemREF != null)
                                    {
                                        weapon1StatVal = getItemStatValue(t.stat.ID, itemREF,
                                            InventoryManager.Instance.equippedWeapons[weaponComparedIndex].temporaryRandomItemID);
                                    }
                                    
                                    if(inspectedWeaponStatVal == 0 && weapon1StatVal == 0) continue;
                                    if(inspectedWeaponStatVal == weapon1StatVal) continue;
                                    if (inspectedWeaponStatVal != 0)
                                    {
                                        bool isGain = !(weapon1StatVal > inspectedWeaponStatVal);
                                        float diffAmt = RPGBuilderUtilities.getAmountDifference(inspectedWeaponStatVal,
                                            weapon1StatVal);
                                        diffAmt = (float)Math.Round(diffAmt, 2);
                                        string statChangeText = "";
                                        string modifierText = isGain ? "+" : "-";
                                        statChangeText += AssignRequirementColor(isGain,
                                            modifierText + " " + diffAmt + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        if (isGain)
                                            statGains.Add(statChangeText);
                                        else
                                            statLosses.Add(statChangeText);
                                    }
                                    else
                                    {
                                        if (weapon1StatVal == 0) continue;
                                        weapon1StatVal = (float)Math.Round(weapon1StatVal, 2);
                                        string statChangeText = "";
                                        string modifierText = "-";
                                        statChangeText += AssignRequirementColor(false,
                                            modifierText + " " + weapon1StatVal + " " + t.stat.displayName);
                                        statChangeText = RPGBuilderUtilities.addLineBreak(statChangeText);
                                        statLosses.Add(statChangeText);
                                    }
                                }
                                break;
                        }

                        break;
                }

                if (statLosses.Count + statGains.Count > 0)
                {
                    statsChangeText.text = RPGBuilderUtilities.addLineBreak("STAT CHANGES IF EQUIPPED:");

                    foreach (var t in statGains)
                    {
                        statsChangeText.text += t;
                    }

                    foreach (var t in statLosses)
                    {
                        statsChangeText.text += t;
                    }
                }
            }
            
        }

        
        float getItemStatValue(int statID, RPGItem item, int rdmItemID)
        {
            float totalAmt = 0;
            foreach (var t in item.stats)
            {
                if(t.statID == statID)
                {
                    totalAmt += t.amount;
                }
            }

            if (rdmItemID == -1) return totalAmt;
            {
                List<RPGItemDATA.RandomizedStat> rdmStatList = new List<RPGItemDATA.RandomizedStat>();
                int rdmItemIndex = RPGBuilderUtilities.getRandomPlayerOwnerItemIndexFromID(rdmItemID);
                if (rdmItemIndex == -1)
                {
                    rdmItemIndex = RPGBuilderUtilities.getRandomItemIndexFromID(rdmItemID);
                    rdmStatList = RandomizedItemsData.Instance.allRandomizedItems[rdmItemIndex].randomStats;
                }
                else
                {
                    rdmStatList = RandomizedItemsData.Instance.allPlayerOwnedRandomItems[rdmItemIndex].randomStats;
                }

                if (rdmItemIndex == -1) return totalAmt;
                foreach (var t in item.randomStats)
                {
                    foreach (var t1 in rdmStatList)
                    {
                        if (t.statID != t1.statID) continue;
                        if (t.statID != statID) continue;
                        totalAmt += t1.statValue;
                    }
                }
            }

            return totalAmt;
        }

        string AssignRequirementColor(bool reqMet, string reqText)
        {
            return reqMet ? "<color=green>" + reqText + "</color>" : "<color=red>" + reqText + "</color>";
        }

        public void Hide()
        {
            thisCG.alpha = 0f;
            thisCG.blocksRaycasts = false;
            thisCG.interactable = false;
        }

        private void Awake()
        {
            Hide();
        }
        
        

        public static ItemTooltip Instance { get; private set; }
    }
}