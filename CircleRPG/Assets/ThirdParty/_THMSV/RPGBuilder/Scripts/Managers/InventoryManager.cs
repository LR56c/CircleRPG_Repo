using System;
using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.Character;
using THMSV.RPGBuilder.Logic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using THMSV.RPGBuilder.World;
using UnityEngine;
using Random = UnityEngine.Random;

namespace THMSV.RPGBuilder.Managers
{
    public class InventoryManager : MonoBehaviour
    {
        public Transform draggedItemParent;
        public GameObject draggedItemImage;

        public RectTransform[] inventorySlotsRect;


        [Serializable]
        public class INVENTORY_CONTAINERS
        {
            public string bagName;

            [Serializable]
            public class BAG_SLOTS
            {
                public string item_NAME;
                public bool inUse;
                public RPGItem itemStored;
                public int curStack;
                public RectTransform rect;
                public int itemRandomID = -1;
            }

            public List<BAG_SLOTS> slots = new List<BAG_SLOTS>();
        }

        public INVENTORY_CONTAINERS[] bags;

        [Serializable]
        public class INVENTORY_EQUIPPED_ITEMS
        {
            public string slotType;
            public RPGItem itemEquipped;
            public int temporaryRandomItemID = -1;
        }

        public List<INVENTORY_EQUIPPED_ITEMS> equippedArmors = new List<INVENTORY_EQUIPPED_ITEMS>();
        public List<INVENTORY_EQUIPPED_ITEMS> equippedWeapons = new List<INVENTORY_EQUIPPED_ITEMS>();

        public int goldAmount;

        public GameObject lootBagPrefab;

        [System.Serializable]
        public class WorldLootItems_DATA
        {
            public RPGItem item;
            public int count;
            public WorldDroppedItem worldDroppedItemREF;
            public int randomItemID;
        }
        public List<WorldLootItems_DATA> allWorldDroppedItems = new List<WorldLootItems_DATA>();

        public void LootWorldDroppedItem(WorldDroppedItem worldDroppedItemREF)
        {
            for (int i = 0; i < allWorldDroppedItems.Count; i++)
            {
                if (allWorldDroppedItems[i].worldDroppedItemREF != worldDroppedItemREF) continue;
                AddItem(allWorldDroppedItems[i].item.ID, allWorldDroppedItems[i].count, false, allWorldDroppedItems[i].randomItemID);
                Destroy(allWorldDroppedItems[i].worldDroppedItemREF.gameObject);
                allWorldDroppedItems.RemoveAt(i);
                return;
            }
        }

        public int getWorldDroppedItemRandomID(WorldDroppedItem worldDroppedItemREF)
        {
            foreach (var t in allWorldDroppedItems)
            {
                if (t.worldDroppedItemREF != worldDroppedItemREF) continue;
                return t.randomItemID;
            }

            return -1;
        }
        
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public void DestroyWorldDroppedItem(WorldDroppedItem worldItemREF)
        {
            for (var index = 0; index < allWorldDroppedItems.Count; index++)
            {
                var v = allWorldDroppedItems[index];
                if (v.worldDroppedItemREF != worldItemREF) continue;
                Destroy(v.worldDroppedItemREF.gameObject);
                allWorldDroppedItems.RemoveAt(index);
            }
        }

        public void InitInventory()
        {
            bags[0].slots.Clear();
            for (var i = 0; i < RPGBuilderEssentials.Instance.itemSettings.InventorySlots; i++)
            {
                var newSlot = new INVENTORY_CONTAINERS.BAG_SLOTS();
                if (CharacterData.Instance.inventoryItemsDATA[i].itemID != -1)
                {
                    newSlot.inUse = true;
                    newSlot.itemStored =
                        RPGBuilderUtilities.GetItemFromID(CharacterData.Instance.inventoryItemsDATA[i].itemID);
                    newSlot.curStack = CharacterData.Instance.inventoryItemsDATA[i].itemStack;
                    newSlot.item_NAME = newSlot.itemStored._name;
                    newSlot.itemRandomID = CharacterData.Instance.inventoryItemsDATA[i].itemRandomID;
                }

                newSlot.rect = inventorySlotsRect[i];
                bags[0].slots.Add(newSlot);
            }
        }

        public void InitEquippedItems()
        {
            foreach (var t in CharacterData.Instance.armorsEquipped)
                if (t.itemID != -1)
                    InitEquipArmor(RPGBuilderUtilities.GetItemFromID(t.itemID), t.randomItemID);

            for (int i = 0; i < CharacterData.Instance.weaponsEquipped.Count; i++)
            {
                if (CharacterData.Instance.weaponsEquipped[i].itemID == -1) continue;
                InitEquipWeapon(RPGBuilderUtilities.GetItemFromID(CharacterData.Instance.weaponsEquipped[i].itemID),
                    i, CharacterData.Instance.weaponsEquipped[i].randomItemID);
            }
        }


        
        public void SellItemToMerchant(int itemID, int count, int bagIndex, int bagSlotIndex)
        {
            RemoveItem(itemID, count, bagIndex, bagSlotIndex, true);
            var itemREF = RPGBuilderUtilities.GetItemFromID(itemID);
            for (var i = 0; i < count; i++) AddCurrency(itemREF.sellCurrencyID, itemREF.sellPrice);
        }

        

        public void HideAllItemsMainMenu(PlayerAppearanceHandler appearanceREF)
        {
            foreach (var t in appearanceREF.armorPieces)
            {
                t.SetActive(false);
            }
            
            if(appearanceREF.weapon1GO != null) Destroy(appearanceREF.weapon1GO);
            if(appearanceREF.weapon2GO != null) Destroy(appearanceREF.weapon2GO);

            if(appearanceREF.chestBody != null) appearanceREF.chestBody.SetActive(true);
            if(appearanceREF.HandsBody != null) appearanceREF.HandsBody.SetActive(true);
            if(appearanceREF.LegsBody != null) appearanceREF.LegsBody.SetActive(true);
            if(appearanceREF.FeetBody != null) appearanceREF.FeetBody.SetActive(true);
        }
        
        public void InitEquipItemMainMenu(RPGItem itemToEquip, PlayerAppearanceHandler appearanceRef, int i)
        {
            if (itemToEquip.itemType != "ARMOR" && itemToEquip.itemType != "WEAPON") return;
            if (itemToEquip.itemType == "ARMOR")
            {
                if (itemToEquip.itemModelName == "") return;
                appearanceRef.ShowArmor(itemToEquip.itemModelName);
                HideBodyPart(itemToEquip.equipmentSlot, appearanceRef);
            }
            else
            {
                var weaponID = 0;
                switch (i)
                {
                    case 6:
                        weaponID = 1;
                        break;
                    case 7:
                        weaponID = 2;
                        break;
                }

                if (itemToEquip.weaponModel != null) appearanceRef.ShowWeapon(itemToEquip, weaponID);
            }
        }
        
        public void InitEquipClassItemMainMenu(RPGItem itemToEquip, PlayerAppearanceHandler appearanceRef, int i)
        {
            if (itemToEquip.itemType != "ARMOR" && itemToEquip.itemType != "WEAPON") return;
            if (itemToEquip.itemType == "ARMOR")
            {
                if (itemToEquip.itemModelName == "") return;
                appearanceRef.ShowArmor(itemToEquip.itemModelName);
                HideBodyPart(itemToEquip.equipmentSlot, appearanceRef);
            }
            else
            {
                var weaponID = 0;
                switch (itemToEquip.slotType)
                {
                    case "TWO HAND":
                        weaponID = 1;
                        break;
                    case "MAIN HAND":
                        weaponID = 1;
                        break;
                    case "OFF HAND":
                        weaponID = 2;
                        break;
                    case "ANY HAND":
                        weaponID = appearanceRef.weapon1GO == null ? 1 : 2;
                        break;
                }

                if (itemToEquip.weaponModel != null) appearanceRef.ShowWeapon(itemToEquip, weaponID);
            }
        }

        private void InitEquipArmor(RPGItem itemToEquip, int randomItemID)
        {
            if (itemToEquip.itemType != "ARMOR") return;
            int armorSlotIndex = RPGBuilderUtilities.getArmorSlotIndex(itemToEquip.equipmentSlot);
            equippedArmors[armorSlotIndex].itemEquipped = itemToEquip;
            equippedArmors[armorSlotIndex].temporaryRandomItemID = randomItemID;

            CharacterPanelDisplayManager.Instance.InitCharEquippedItems();
            if (itemToEquip.itemModelName != "")
            {
                CombatManager.playerCombatInfo.appearanceREF.ShowArmor(itemToEquip.itemModelName);
                HideBodyPart(itemToEquip.equipmentSlot);
            }

            StatCalculator.UpdateItemStats(itemToEquip, randomItemID);
        }

        private void InitEquipWeapon(RPGItem itemToEquip, int weaponIndex, int randomItemID)
        {
            if (itemToEquip.itemType != "WEAPON") return;
            // EQUIP WEAPON

            equippedWeapons[weaponIndex].itemEquipped = itemToEquip;
            equippedWeapons[weaponIndex].temporaryRandomItemID = randomItemID;
            CharacterPanelDisplayManager.Instance.InitCharEquippedItems();

            var weaponID = 0;
            switch (weaponIndex)
            {
                case 0:
                {
                    weaponID = 1;
                    if (itemToEquip.autoAttackAbilityID != -1)
                        CombatManager.playerCombatInfo.currentAutoAttackAbilityID = itemToEquip.autoAttackAbilityID;
                    break;
                }
                case 1:
                {
                    weaponID = 2;
                    if (CombatManager.playerCombatInfo.currentAutoAttackAbilityID == -1 &&
                        itemToEquip.autoAttackAbilityID != -1)
                        CombatManager.playerCombatInfo.currentAutoAttackAbilityID = itemToEquip.autoAttackAbilityID;
                    break;
                }
            }

            if (itemToEquip.weaponModel != null)
                CombatManager.playerCombatInfo.appearanceREF.ShowWeapon(itemToEquip, weaponID);

            StatCalculator.UpdateItemStats(itemToEquip, randomItemID);
        }

        private bool CheckItemRequirements(RPGItem item)
        {
            List<bool> reqResults = new List<bool>();
            foreach (var t in item.useRequirements)
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
                    case RequirementsManager.RequirementType._class:
                        intValue1 = t.classRequiredID;
                        break;
                }
                reqResults.Add(RequirementsManager.Instance.HandleRequirementType(t, intValue1, true));
            }

            return !reqResults.Contains(false);
        }

        public void UseItemFromBar(RPGItem itemToUse)
        {
            int slotIndex = -1;

            for (int i = 0; i < bags[0].slots.Count; i++)
            {
                if (!bags[0].slots[i].inUse || bags[0].slots[i].itemStored == null) continue;
                if (bags[0].slots[i].itemStored == itemToUse)
                {
                    slotIndex = i;
                }
            }
            
            UseItem(itemToUse, 0 , slotIndex);
        }
        
        public void UseItem(RPGItem itemUsed, int bagIndex, int slotIndex)
        {
            if (!CheckItemRequirements(itemUsed)) return;
            var isConsumed = false;
            foreach (var t in itemUsed.onUseActions)
            {
                if (t.isConsumed) isConsumed = true;
                switch (t.actionType)
                {
                    case RPGItem.OnUseActionType.acceptQuest:
                        QuestInteractionDisplayManager.Instance.InitializeQuestContent(RPGBuilderUtilities.GetQuestFromID(t.questID), false);
                        break;
                    case RPGItem.OnUseActionType.equip:
                        EquipItem(itemUsed, bagIndex, slotIndex, bags[bagIndex].slots[slotIndex].itemRandomID);
                        break;
                    case RPGItem.OnUseActionType.gainClassLevel:
                        LevelingManager.Instance.AddClassLevel(t.classLevelGained);
                        break;
                    case RPGItem.OnUseActionType.gainClassXP:
                        LevelingManager.Instance.AddClassXP(t.classXPGained);
                        break;
                    case RPGItem.OnUseActionType.gainSkillLevel:
                        LevelingManager.Instance.AddSkillLevel(t.skillID,
                            t.skillXPGained);
                        break;
                    case RPGItem.OnUseActionType.gainSkillXP:
                        LevelingManager.Instance.AddSkillXP(t.skillID,
                            t.skillXPGained);
                        break;
                    case RPGItem.OnUseActionType.gainTreePoint:
                        TreePointsManager.Instance.AddTreePoint(t.treePointID,
                            t.treePointGained);
                        break;
                    case RPGItem.OnUseActionType.learnAbility:
                        foreach (var t1 in CharacterData.Instance.talentTrees)
                        foreach (var t2 in t1.nodes)
                            if (t2.nodeData.nodeType ==
                                RPGTalentTree.TalentTreeNodeType.ability &&
                                t2.nodeData.abilityID ==
                                t.abilityID)
                                AbilityManager.Instance.RankUpAbility(
                                    RPGBuilderUtilities.GetAbilityFromID(t.abilityID),
                                    RPGBuilderUtilities.GetTalentTreeFromID(
                                        t1.treeID));

                        break;
                    case RPGItem.OnUseActionType.learnRecipe:
                        foreach (var t1 in CharacterData.Instance.talentTrees)
                        foreach (var t2 in t1.nodes)
                            if (t2.nodeData.nodeType ==
                                RPGTalentTree.TalentTreeNodeType.recipe &&
                                t2.nodeData.recipeID ==
                                t.recipeID)
                                CraftingManager.Instance.RankUpRecipe(
                                    RPGBuilderUtilities.GetCraftingRecipeFromID(t.recipeID),
                                    RPGBuilderUtilities.GetTalentTreeFromID(
                                        t1.treeID));

                        break;
                    case RPGItem.OnUseActionType.learnResourceNode:
                        foreach (var t1 in CharacterData.Instance.talentTrees)
                        foreach (var t2 in t1.nodes)
                            if (t2.nodeData.nodeType ==
                                RPGTalentTree.TalentTreeNodeType.resourceNode &&
                                t2.nodeData.resourceNodeID ==
                                t.resourceNodeID)
                                GatheringManager.Instance.RankUpResourceNode(
                                    RPGBuilderUtilities.GetResourceNodeFromID(t.resourceNodeID),
                                    RPGBuilderUtilities.GetTalentTreeFromID(
                                        t1.treeID));

                        break;
                    case RPGItem.OnUseActionType.learnBonus:
                        foreach (var t1 in CharacterData.Instance.talentTrees)
                        foreach (var t2 in t1.nodes)
                            if (t2.nodeData.nodeType ==
                                RPGTalentTree.TalentTreeNodeType.bonus &&
                                t2.nodeData.bonusID ==
                                t.bonusID)
                                BonusManager.Instance.RankUpBonus(
                                    RPGBuilderUtilities.GetBonusFromID(t.bonusID),
                                    RPGBuilderUtilities.GetTalentTreeFromID(
                                        t1.treeID));

                        break;
                    case RPGItem.OnUseActionType.useAbility:
                        CombatManager.Instance.InitAbility(CombatManager.playerCombatInfo,
                            RPGBuilderUtilities.GetAbilityFromID(t.abilityID), false);
                        break;
                    case RPGItem.OnUseActionType.useEffect:
                        CombatNode target = null;
                        target = CombatManager.Instance.currentTarget == null ? CombatManager.playerCombatInfo : CombatManager.Instance.currentTarget;
                        CombatManager.Instance.ExecuteEffect(CombatManager.playerCombatInfo, target,
                            RPGBuilderUtilities.GetEffectFromID(t.effectID),null);
                        break;
                }
            }

            if (isConsumed) RemoveItem(itemUsed.ID, 1, -1, -1, false);
            ActionBarDisplayManager.Instance.CheckItemBarState();
        }

        private int[] getWeaponSituation(RPGItem weaponToEquip, RPGItem weaponEquipped1, RPGItem weaponEquipped2, int randomItemID)
        {
            var newWeaponState = new int[2];
            if (weaponEquipped1 == null && weaponEquipped2 == null)
            {
                if (weaponToEquip.slotType == "OFF HAND")
                {
                    newWeaponState[0] = 1;
                    newWeaponState[1] = 2;
                }
                else
                {
                    newWeaponState[0] = 0;
                    newWeaponState[1] = 1;
                }
            }
            else if (weaponEquipped1 != null)
            {
                switch (weaponToEquip.slotType)
                {
                    case "TWO HAND":
                    {
                        UnequipItem(weaponEquipped1, 1);
                        newWeaponState[0] = 0;
                        newWeaponState[1] = 1;

                        if (weaponEquipped2 != null) UnequipItem(weaponEquipped2, 2);
                        break;
                    }
                    case "MAIN HAND" when weaponEquipped1.slotType == "OFF HAND" || weaponEquipped1.slotType == "ANY HAND":
                    {
                        equippedWeapons[0].itemEquipped = weaponEquipped1;
                        equippedWeapons[0].temporaryRandomItemID = randomItemID;
                        CombatManager.playerCombatInfo.appearanceREF.HideWeapon(1);
                        if (weaponEquipped1.weaponModel != null)
                            CombatManager.playerCombatInfo.appearanceREF.ShowWeapon(weaponEquipped1, 2);

                        newWeaponState[0] = 0;
                        newWeaponState[1] = 1;
                        break;
                    }
                    case "MAIN HAND":
                        // CONDITIONS NOT MET
                        UnequipItem(weaponEquipped1, 1);
                        newWeaponState[0] = 0;
                        newWeaponState[1] = 1;
                        break;
                    case "ANY HAND" when weaponEquipped1.slotType == "MAIN HAND" || weaponEquipped1.slotType == "ANY HAND":
                    {
                        if (weaponEquipped2 != null) UnequipItem(weaponEquipped2, 2);
                        newWeaponState[0] = 1;
                        newWeaponState[1] = 2;
                        break;
                    }
                    case "ANY HAND":
                        UnequipItem(weaponEquipped1, 1);
                        newWeaponState[0] = 0;
                        newWeaponState[1] = 1;
                        break;
                    case "OFF HAND":
                    {
                        if (weaponEquipped2 != null) UnequipItem(weaponEquipped2, 2);
                        if(weaponEquipped1.slotType == "TWO HAND") UnequipItem(weaponEquipped1, 1);
                        newWeaponState[0] = 1;
                        newWeaponState[1] = 2;
                        break;
                    }
                }
            }
            else if (weaponEquipped2 != null)
            {
                if (weaponToEquip.slotType == "MAIN HAND" || weaponToEquip.slotType == "ANY HAND")
                {
                    newWeaponState[0] = 0;
                    newWeaponState[1] = 1;
                }
                else
                {
                    if(weaponEquipped1 != null)UnequipItem(weaponEquipped1, 1);
                    if (weaponToEquip.slotType == "TWO HAND")
                    {
                        UnequipItem(weaponEquipped2, 2);
                        newWeaponState[0] = 0;
                        newWeaponState[1] = 1;
                    } else if (weaponToEquip.slotType == "OFF HAND")
                    {
                        UnequipItem(weaponEquipped2, 2);
                        newWeaponState[0] = 1;
                        newWeaponState[1] = 2;
                    }
                }
            }

            return newWeaponState;
        }
        
        

        public void EquipItem(RPGItem itemToEquip, int bagIndex, int slotIndex, int randomItemID)
        {
            if (itemToEquip.itemType != "ARMOR" && itemToEquip.itemType != "WEAPON") return;
            if (itemToEquip.itemType == "ARMOR")
            {
                int armorSlotIndex = RPGBuilderUtilities.getArmorSlotIndex(itemToEquip.equipmentSlot);
                var itemToUnequip = RPGBuilderUtilities.getEquippedArmor(itemToEquip.equipmentSlot);
                if (itemToUnequip != null)
                {
                    UnequipItem(itemToUnequip, 0);
                    CombatManager.playerCombatInfo.appearanceREF.HideArmor(itemToUnequip.itemModelName);
                }

                equippedArmors[armorSlotIndex].itemEquipped = itemToEquip;
                equippedArmors[armorSlotIndex].temporaryRandomItemID = randomItemID;
                
                CharacterData.Instance.armorsEquipped[armorSlotIndex].itemID = itemToEquip.ID;
                CharacterData.Instance.armorsEquipped[armorSlotIndex].randomItemID = randomItemID;
                
                RemoveEquippedItem(bagIndex, slotIndex);
                CharacterPanelDisplayManager.Instance.InitCharEquippedItems();

                if (itemToEquip.itemModelName != "")
                {
                    CombatManager.playerCombatInfo.appearanceREF.ShowArmor(itemToEquip.itemModelName);
                    HideBodyPart(itemToEquip.equipmentSlot);
                }
                
                RPGBuilderUtilities.modifyPlayerOwnerRandomItemState(randomItemID, RPGItemDATA.randomItemState.equipped);
            }
            else
            {
                // EQUIP WEAPON

                var weaponState = getWeaponSituation(itemToEquip, equippedWeapons[0].itemEquipped,
                    equippedWeapons[1].itemEquipped, randomItemID);

                equippedWeapons[weaponState[0]].itemEquipped = itemToEquip;
                equippedWeapons[weaponState[0]].temporaryRandomItemID = randomItemID;
                RemoveEquippedItem(bagIndex, slotIndex);
                CharacterData.Instance.weaponsEquipped[weaponState[0]].itemID = itemToEquip.ID;
                CharacterData.Instance.weaponsEquipped[weaponState[0]].randomItemID = randomItemID;

                RPGBuilderUtilities.modifyPlayerOwnerRandomItemState(randomItemID, RPGItemDATA.randomItemState.equipped);
                
                CharacterPanelDisplayManager.Instance.InitCharEquippedItems();

                if (itemToEquip.weaponModel != null)
                    CombatManager.playerCombatInfo.appearanceREF.ShowWeapon(itemToEquip, weaponState[1]);

                BonusManager.Instance.InitBonuses();

                if (weaponState[0] == 0)
                {
                    if (equippedWeapons[1].itemEquipped == null)
                    {
                        if (equippedWeapons[0].itemEquipped.autoAttackAbilityID != -1)
                            CombatManager.playerCombatInfo.currentAutoAttackAbilityID =
                                equippedWeapons[0].itemEquipped.autoAttackAbilityID;
                    }
                    else
                    {
                        if (equippedWeapons[0].itemEquipped.autoAttackAbilityID == -1)
                            if (equippedWeapons[1].itemEquipped.autoAttackAbilityID != -1)
                                CombatManager.playerCombatInfo.currentAutoAttackAbilityID =
                                    equippedWeapons[1].itemEquipped.autoAttackAbilityID;
                    }
                }
                else
                {
                    if (equippedWeapons[0].itemEquipped == null)
                    {
                        if (equippedWeapons[1].itemEquipped.autoAttackAbilityID != -1)
                            CombatManager.playerCombatInfo.currentAutoAttackAbilityID =
                                equippedWeapons[1].itemEquipped.autoAttackAbilityID;
                    }
                    else
                    {
                        if (equippedWeapons[0].itemEquipped.autoAttackAbilityID != -1)
                            CombatManager.playerCombatInfo.currentAutoAttackAbilityID =
                                equippedWeapons[0].itemEquipped.autoAttackAbilityID;
                    }
                }
            }

            StatCalculator.UpdateItemStats(itemToEquip, randomItemID);
            CharacterEventsManager.Instance.ItemEquipped(itemToEquip);
        }

        public void UnequipItem(RPGItem itemToUnequip, int weaponID)
        {
            int cachedRdmItemID = -1;
            switch (itemToUnequip.itemType)
            {
                case "ARMOR":
                {
                    int armorSlotIndex = RPGBuilderUtilities.getArmorSlotIndex(itemToUnequip.equipmentSlot);
                    cachedRdmItemID = equippedArmors[armorSlotIndex].temporaryRandomItemID;
                    equippedArmors[armorSlotIndex].itemEquipped = null;
                    equippedArmors[armorSlotIndex].temporaryRandomItemID = -1;
                
                    CharacterData.Instance.armorsEquipped[armorSlotIndex].itemID = -1;
                    CharacterData.Instance.armorsEquipped[armorSlotIndex].randomItemID = -1;

                    if (itemToUnequip.itemModelName != "")
                    {
                        CombatManager.playerCombatInfo.appearanceREF.HideArmor(itemToUnequip.itemModelName);
                        ShowBodyPart(itemToUnequip.equipmentSlot);
                    }
                    
                    RPGBuilderUtilities.modifyPlayerOwnerRandomItemState(
                        equippedArmors[armorSlotIndex].temporaryRandomItemID,
                        RPGItemDATA.randomItemState.inBag);

                    break;
                }
                case "WEAPON":
                {
                    if (weaponID == 1)
                    {
                        cachedRdmItemID = equippedWeapons[0].temporaryRandomItemID;
                        equippedWeapons[0].itemEquipped = null;
                        equippedWeapons[0].temporaryRandomItemID = -1;
                        CharacterData.Instance.weaponsEquipped[0].itemID = -1;
                        CharacterData.Instance.weaponsEquipped[0].randomItemID = -1;
                    }
                    else
                    {
                        cachedRdmItemID = equippedWeapons[1].temporaryRandomItemID;
                        equippedWeapons[1].itemEquipped = null;
                        equippedWeapons[1].temporaryRandomItemID = -1;
                        CharacterData.Instance.weaponsEquipped[1].itemID = -1;
                        CharacterData.Instance.weaponsEquipped[1].randomItemID = -1;
                    }

                    if (itemToUnequip.weaponModel != null)
                        CombatManager.playerCombatInfo.appearanceREF.HideWeapon(itemToUnequip, weaponID);

                    BonusManager.Instance.CancelBonusFromUnequippedWeapon(itemToUnequip.weaponType);

                    if (weaponID == 1)
                    {
                        if (equippedWeapons[1].itemEquipped == null)
                        {
                            CombatManager.playerCombatInfo.currentAutoAttackAbilityID = RPGBuilderUtilities
                                .GetClassFromID(CharacterData.Instance.classDATA.classID).autoAttackAbilityID;
                        }
                        else
                        {
                            if (equippedWeapons[1].itemEquipped.autoAttackAbilityID != -1)
                                CombatManager.playerCombatInfo.currentAutoAttackAbilityID =
                                    equippedWeapons[1].itemEquipped.autoAttackAbilityID;
                            else
                                CombatManager.playerCombatInfo.currentAutoAttackAbilityID = RPGBuilderUtilities
                                    .GetClassFromID(CharacterData.Instance.classDATA.classID).autoAttackAbilityID;
                        }
                    }
                    else
                    {
                        if (equippedWeapons[0].itemEquipped == null)
                        {
                            CombatManager.playerCombatInfo.currentAutoAttackAbilityID = RPGBuilderUtilities
                                .GetClassFromID(CharacterData.Instance.classDATA.classID).autoAttackAbilityID;
                        }
                        else
                        {
                            if (equippedWeapons[0].itemEquipped.autoAttackAbilityID != -1)
                                CombatManager.playerCombatInfo.currentAutoAttackAbilityID =
                                    equippedWeapons[0].itemEquipped.autoAttackAbilityID;
                            else
                                CombatManager.playerCombatInfo.currentAutoAttackAbilityID = RPGBuilderUtilities
                                    .GetClassFromID(CharacterData.Instance.classDATA.classID).autoAttackAbilityID;
                        }
                    }

                    break;
                }
            }

            CharacterPanelDisplayManager.Instance.InitCharEquippedItems();

            RPGBuilderUtilities.modifyPlayerOwnerRandomItemState(cachedRdmItemID, RPGItemDATA.randomItemState.inBag);
            AddItem(itemToUnequip.ID, 1, false, cachedRdmItemID);

            StatCalculator.RemoveItemStats(itemToUnequip, cachedRdmItemID);
        }

        private void UnequipItemFromDrag(RPGItem itemToUnequip, int weaponID)
        {
            int cachedRdmItemID = -1;
            switch (itemToUnequip.itemType)
            {
                case "ARMOR":
                {
                    cachedRdmItemID = equippedWeapons[RPGBuilderUtilities.getArmorSlotIndex(itemToUnequip.equipmentSlot)].temporaryRandomItemID;
                    equippedWeapons[RPGBuilderUtilities.getArmorSlotIndex(itemToUnequip.equipmentSlot)].itemEquipped = null;
                    equippedWeapons[RPGBuilderUtilities.getArmorSlotIndex(itemToUnequip.equipmentSlot)].temporaryRandomItemID = -1;

                    if (itemToUnequip.itemModelName != "")
                    {
                        CombatManager.playerCombatInfo.appearanceREF.HideArmor(itemToUnequip.itemModelName);
                        ShowBodyPart(itemToUnequip.equipmentSlot);
                    }

                    break;
                }
                case "WEAPON":
                {
                    if (weaponID == 1)
                    {
                        cachedRdmItemID = equippedWeapons[0].temporaryRandomItemID;
                        equippedWeapons[0].itemEquipped = null;
                        equippedWeapons[0].temporaryRandomItemID = -1;
                    }
                    else
                    {
                        cachedRdmItemID = equippedWeapons[1].temporaryRandomItemID;
                        equippedWeapons[1].itemEquipped = null;
                        equippedWeapons[1].temporaryRandomItemID = -1;
                    }

                    if (itemToUnequip.weaponModel != null)
                        CombatManager.playerCombatInfo.appearanceREF.HideWeapon(itemToUnequip, weaponID);
                    break;
                }
            }

            RPGBuilderUtilities.modifyPlayerOwnerRandomItemState(cachedRdmItemID, RPGItemDATA.randomItemState.inBag);
            CharacterPanelDisplayManager.Instance.InitCharEquippedItems();

            StatCalculator.RemoveItemStats(itemToUnequip, cachedRdmItemID);
        }


        private void HideBodyPart(string slotType)
        {
            switch (slotType)
            {
                case "CHEST":
                    CombatManager.playerCombatInfo.appearanceREF.chestBody.SetActive(false);
                    break;
                case "PANTS":
                    CombatManager.playerCombatInfo.appearanceREF.LegsBody.SetActive(false);
                    break;
                case "GLOVES":
                    CombatManager.playerCombatInfo.appearanceREF.HandsBody.SetActive(false);
                    break;
                case "BOOTS":
                    CombatManager.playerCombatInfo.appearanceREF.FeetBody.SetActive(false);
                    break;
            }
        }

        private void HideBodyPart(string slotType, PlayerAppearanceHandler appearanceRef)
        {
            switch (slotType)
            {
                case "CHEST":
                    appearanceRef.chestBody.SetActive(false);
                    break;
                case "PANTS":
                    appearanceRef.LegsBody.SetActive(false);
                    break;
                case "GLOVES":
                    appearanceRef.HandsBody.SetActive(false);
                    break;
                case "BOOTS":
                    appearanceRef.FeetBody.SetActive(false);
                    break;
            }
        }

        private void ShowBodyPart(string slotType)
        {
            switch (slotType)
            {
                case "CHEST":
                    CombatManager.playerCombatInfo.appearanceREF.chestBody.SetActive(true);
                    break;
                case "PANTS":
                    CombatManager.playerCombatInfo.appearanceREF.LegsBody.SetActive(true);
                    break;
                case "GLOVES":
                    CombatManager.playerCombatInfo.appearanceREF.HandsBody.SetActive(true);
                    break;
                case "BOOTS":
                    CombatManager.playerCombatInfo.appearanceREF.FeetBody.SetActive(true);
                    break;
            }
        }

        private void ShowBodyPart(string slotType, PlayerAppearanceHandler appearanceRef)
        {
            switch (slotType)
            {
                case "CHEST":
                    appearanceRef.chestBody.SetActive(true);
                    break;
                case "PANTS":
                    appearanceRef.LegsBody.SetActive(true);
                    break;
                case "GLOVES":
                    appearanceRef.HandsBody.SetActive(true);
                    break;
                case "BOOTS":
                    appearanceRef.FeetBody.SetActive(true);
                    break;
            }
        }

        private void RemoveEquippedItem(int bagIndex, int slotIndex)
        {
            bags[bagIndex].slots[slotIndex].itemStored = null;
            bags[bagIndex].slots[slotIndex].item_NAME = "";
            bags[bagIndex].slots[slotIndex].curStack = 0;
            bags[bagIndex].slots[slotIndex].inUse = false;
            bags[bagIndex].slots[slotIndex].itemRandomID = -1;

            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateSlots();
        }

        public void AddCurrency(int currencyID, int amount)
        {
            foreach (var t in CharacterData.Instance.currencies)
            {
                var currencyREF = RPGBuilderUtilities.GetCurrencyFromID(t.currencyID);
                if (currencyREF.ID != currencyID) continue;
                var curCurrencyAmount = t.amount;
                var convertCurrencyREF = RPGBuilderUtilities.GetCurrencyFromID(currencyREF.convertToCurrencyID);
                if (convertCurrencyREF != null && currencyREF.AmountToConvert > 0)
                {
                    // CONVERT CURRENCY
                    if (curCurrencyAmount + amount >= currencyREF.AmountToConvert)
                    {
                        var amountToAdd = amount;
                        while (amountToAdd + curCurrencyAmount >= currencyREF.AmountToConvert)
                        {
                            var amountAdded = currencyREF.AmountToConvert - curCurrencyAmount;
                            curCurrencyAmount = 0;
                            amountToAdd -= amountAdded;

                            // ADD THE CONVERTED CURRENCY
                            AddCurrency(convertCurrencyREF.ID, 1);
                        }

                        curCurrencyAmount = amountToAdd;
                    }
                    else
                    {
                        curCurrencyAmount += amount;
                    }
                }
                else
                {
                    if (currencyREF.maxValue > 0 && curCurrencyAmount + amount >= currencyREF.maxValue)
                        curCurrencyAmount = currencyREF.maxValue;
                    else
                        curCurrencyAmount += amount;
                }

                t.amount = curCurrencyAmount;
            }

            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateCurrency();
        }

        public void RemoveCurrency(int amount)
        {
            goldAmount -= amount;

            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateCurrency();
        }


        public void GenerateDroppedLoot(RPGNpc npc, CombatNode nodeRef)
        {
            var totalItemDropped = 0;
            var lootData = new List<LootBagHolder.Loot_Data>();
            float LOOTCHANCEMOD = CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                RPGStat.STAT_TYPE.LOOT_CHANCE_MODIFIER);
            foreach (var t in npc.lootTables)
            {
                var dropAmount = Random.Range(0f, 100f);
                if (!(dropAmount <= t.dropRate)) continue;
                var lootTableREF = RPGBuilderUtilities.GetLootTableFromID(t.lootTableID);
                foreach (var t1 in lootTableREF.lootItems)
                {
                    var itemDropAmount = Random.Range(0f, 100f);
                    if (LOOTCHANCEMOD > 0) itemDropAmount += itemDropAmount * (LOOTCHANCEMOD / 100);
                    if (!(itemDropAmount <= t1.dropRate)) continue;
                    var stack =  t1.min == t1.max ? t1.min : Random.Range(t1.min, t1.max + 1);

                    RPGItem itemREF = RPGBuilderUtilities.GetItemFromID(t1.itemID);
                    if (itemREF.dropInWorld)
                    {
                        var newLoot = new WorldLootItems_DATA();
                        newLoot.item = itemREF;
                        newLoot.count = stack;
                        GameObject newLootGO = Instantiate(itemREF.itemWorldModel, new Vector3(nodeRef.transform.position.x,
                            nodeRef.transform.position.y + 1, nodeRef.transform.position.z), Quaternion.identity);
                        newLoot.worldDroppedItemREF = newLootGO.AddComponent<WorldDroppedItem>();
                        newLoot.worldDroppedItemREF.curLifetime = 0;
                        newLoot.worldDroppedItemREF.maxDuration = itemREF.durationInWorld;
                        newLoot.worldDroppedItemREF.item = itemREF;
                        if (itemREF.randomStats.Count > 0)
                        {
                            newLoot.randomItemID = RPGBuilderUtilities.GenerateRandomItemStats(itemREF.ID, false, false);
                        }
                        newLoot.worldDroppedItemREF.InitPhysics(nodeRef.transform.position);
                        allWorldDroppedItems.Add(newLoot);
                    }
                    else
                    {
                        var newLoot = new LootBagHolder.Loot_Data();
                        newLoot.item = itemREF;
                        newLoot.count = stack;
                        
                        if (itemREF.randomStats.Count > 0)
                        {
                            newLoot.randomItemID = RPGBuilderUtilities.GenerateRandomItemStats(itemREF.ID, false, false);
                        }
                        lootData.Add(newLoot);
                    }
                    totalItemDropped++;
                }
            }

            if (totalItemDropped <= 0) return;
            if (lootData.Count <= 0) return;
            var lootbag = Instantiate(lootBagPrefab, nodeRef.gameObject.transform.position,
                lootBagPrefab.transform.rotation);

            var lootBagRef = lootbag.GetComponent<LootBagHolder>();
            lootBagRef.lootData = lootData;
        }

        


        public void MoveItem(int prevBagIndex, int prevSlotIndex, int newBagIndex, int newSlotIndex)
        {
            if (bags[newBagIndex].slots[newSlotIndex].inUse)
            {
                var previousSlot = new INVENTORY_CONTAINERS.BAG_SLOTS();
                previousSlot.itemStored = bags[prevBagIndex].slots[prevSlotIndex].itemStored;
                previousSlot.itemRandomID = bags[prevBagIndex].slots[prevSlotIndex].itemRandomID;
                previousSlot.item_NAME = bags[prevBagIndex].slots[prevSlotIndex].item_NAME;
                previousSlot.curStack = bags[prevBagIndex].slots[prevSlotIndex].curStack;

                var newSlot = new INVENTORY_CONTAINERS.BAG_SLOTS();
                newSlot.itemStored = bags[newBagIndex].slots[newSlotIndex].itemStored;
                newSlot.itemRandomID = bags[newBagIndex].slots[newSlotIndex].itemRandomID;
                newSlot.item_NAME = bags[newBagIndex].slots[newSlotIndex].item_NAME;
                newSlot.curStack = bags[newBagIndex].slots[newSlotIndex].curStack;

                bags[newBagIndex].slots[newSlotIndex].itemStored = previousSlot.itemStored;
                bags[newBagIndex].slots[newSlotIndex].itemRandomID = previousSlot.itemRandomID;
                bags[newBagIndex].slots[newSlotIndex].item_NAME = previousSlot.item_NAME;
                bags[newBagIndex].slots[newSlotIndex].curStack = previousSlot.curStack;

                bags[prevBagIndex].slots[prevSlotIndex].itemStored = newSlot.itemStored;
                bags[prevBagIndex].slots[prevSlotIndex].itemRandomID = newSlot.itemRandomID;
                bags[prevBagIndex].slots[prevSlotIndex].item_NAME = newSlot.item_NAME;
                bags[prevBagIndex].slots[prevSlotIndex].curStack = newSlot.curStack;
            }
            else
            {
                bags[newBagIndex].slots[newSlotIndex].itemStored = bags[prevBagIndex].slots[prevSlotIndex].itemStored;
                bags[newBagIndex].slots[newSlotIndex].itemRandomID = bags[prevBagIndex].slots[prevSlotIndex].itemRandomID;
                bags[newBagIndex].slots[newSlotIndex].item_NAME = bags[prevBagIndex].slots[prevSlotIndex].item_NAME;
                bags[newBagIndex].slots[newSlotIndex].curStack = bags[prevBagIndex].slots[prevSlotIndex].curStack;
                bags[newBagIndex].slots[newSlotIndex].inUse = bags[prevBagIndex].slots[prevSlotIndex].inUse;

                bags[prevBagIndex].slots[prevSlotIndex].itemStored = null;
                bags[prevBagIndex].slots[prevSlotIndex].itemRandomID = -1;
                bags[prevBagIndex].slots[prevSlotIndex].item_NAME = "";
                bags[prevBagIndex].slots[prevSlotIndex].curStack = 0;
                bags[prevBagIndex].slots[prevSlotIndex].inUse = false;
            }


            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateSlots();
        }

        public void MoveItemFromCharToBag(RPGItem item, int newBagIndex, int newSlotIndex, int weaponID, int randomItemID)
        {
            if (bags[newBagIndex].slots[newSlotIndex].inUse)
            {
            }
            else
            {
                bags[newBagIndex].slots[newSlotIndex].itemStored = item;
                bags[newBagIndex].slots[newSlotIndex].itemRandomID = randomItemID;
                bags[newBagIndex].slots[newSlotIndex].item_NAME = item._name;
                bags[newBagIndex].slots[newSlotIndex].curStack = 1;
                bags[newBagIndex].slots[newSlotIndex].inUse = true;
                
                RPGBuilderUtilities.modifyPlayerOwnerRandomItemState(randomItemID, RPGItemDATA.randomItemState.inBag);
            }


            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateSlots();

            UnequipItemFromDrag(item, weaponID);
        }


        public void AddItem(int itemID, int Amount, bool automaticallyEquip, int randomItemID)
        {
            var itemToAdd = RPGBuilderUtilities.GetItemFromID(itemID);
            if (itemToAdd.itemType == "CURRENCY")
            {
                AddCurrency(itemToAdd.onUseActions[0].currencyID, Amount);
            }
            else
            {
                int bagIndex = -1, slotIndex = -1;
                foreach (var t in bags)
                {
                    bagIndex++;
                    foreach (var t1 in t.slots)
                    {
                        slotIndex++;
                        if (t1.inUse)
                        {
                            if (t1.itemStored.ID != itemID) continue;
                            if (t1.curStack >= itemToAdd.stackLimit) continue;
                            if (t1.curStack + Amount <= itemToAdd.stackLimit)
                            {
                                t1.curStack += Amount;
                                CharacterEventsManager.Instance.ItemGain(itemToAdd);
                                TreePointsManager.Instance.CheckIfItemGainPoints(itemToAdd);
                                break;
                            }

                            var acceptedOnThisSlot = itemToAdd.stackLimit - t1.curStack;
                            t1.curStack += acceptedOnThisSlot;
                            AddItem(itemID, Amount - acceptedOnThisSlot, false, -1);
                            CharacterEventsManager.Instance.ItemGain(itemToAdd);
                            TreePointsManager.Instance.CheckIfItemGainPoints(itemToAdd);
                            if (automaticallyEquip)
                            {
                                EquipItem(itemToAdd, bagIndex, slotIndex, bags[bagIndex].slots[slotIndex].itemRandomID);
                            }
                            break;
                        }
                        else
                        {
                            t1.inUse = true;
                            if (Amount <= itemToAdd.stackLimit)
                            {
                                t1.itemStored = itemToAdd;
                                t1.item_NAME = itemToAdd._name;
                                t1.curStack += Amount;
                                t1.itemRandomID = -1;
                                
                                if (randomItemID != -1)
                                {
                                    if (!RPGBuilderUtilities.doPlayerOwnRandomItem(randomItemID))
                                    {
                                        RPGBuilderUtilities.addNonOwnedRandomItemToPlayer(randomItemID);
                                    }

                                    t1.itemRandomID = randomItemID;
                                }
                                
                                CharacterEventsManager.Instance.ItemGain(itemToAdd);
                                TreePointsManager.Instance.CheckIfItemGainPoints(itemToAdd);
                                if (automaticallyEquip)
                                {
                                    EquipItem(itemToAdd, bagIndex, slotIndex, t1.itemRandomID);
                                }

                                break;
                            }

                            var remainingStacks = Amount - itemToAdd.stackLimit;
                            t1.itemStored = itemToAdd;
                            t1.item_NAME = itemToAdd._name;
                            t1.curStack = itemToAdd.stackLimit;
                            AddItem(itemID, remainingStacks, false, -1);
                            CharacterEventsManager.Instance.ItemGain(itemToAdd);
                            TreePointsManager.Instance.CheckIfItemGainPoints(itemToAdd);
                            if (automaticallyEquip)
                            {
                                EquipItem(itemToAdd, bagIndex, slotIndex,bags[bagIndex].slots[slotIndex].itemRandomID);
                            }
                            break;
                        }
                    }
                }
            }

            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateSlots();
            if (CraftingPanelDisplayManager.Instance.thisCG.alpha == 1)
                CraftingPanelDisplayManager.Instance.UpdateCraftingView();
            
            ActionBarDisplayManager.Instance.CheckItemBarState();
        }

        public void RemoveItem(int itemID, int Amount, int bagIndex, int bagSlotIndex, bool removeAtSlot)
        {
            if (removeAtSlot)
            {
                if (bags[bagIndex].slots[bagSlotIndex].curStack == Amount)
                {
                        bags[bagIndex].slots[bagSlotIndex].itemStored = null;
                        bags[bagIndex].slots[bagSlotIndex].item_NAME = "";
                        bags[bagIndex].slots[bagSlotIndex].curStack = 0;
                        bags[bagIndex].slots[bagSlotIndex].inUse = false;
                } else if (bags[bagIndex].slots[bagSlotIndex].curStack > Amount)
                {
                    bags[bagIndex].slots[bagSlotIndex].curStack -= Amount;
                }
            }
            else
            {
                foreach (var t in bags)
                foreach (var t1 in t.slots.Where(t1 => t1.inUse).Where(t1 => t1.itemStored.ID == itemID))
                {
                    if (t1.curStack == Amount)
                    {
                        t1.itemStored = null;
                        t1.item_NAME = "";
                        t1.curStack = 0;
                        t1.inUse = false;
                        break;
                    }

                    if (t1.curStack > Amount)
                    {
                        t1.curStack -= Amount;
                        break;
                    }

                    if (t1.curStack >= Amount) continue;
                    var remainingStacks = Amount - t1.curStack;
                    t1.itemStored = null;
                    t1.item_NAME = "";
                    t1.curStack = 0;
                    t1.inUse = false;
                    RemoveItem(itemID, remainingStacks, -1, -1, false);
                    break;
                }
            }

            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateSlots();
            if (CraftingPanelDisplayManager.Instance.thisCG.alpha == 1)
                CraftingPanelDisplayManager.Instance.UpdateCraftingView();
            
            ActionBarDisplayManager.Instance.CheckItemBarState();
        }

        public bool isItemOwned(int ID, int count)
        {
            return (from t in bags from t1 in t.slots where t1.inUse where t1.itemStored.ID == ID select t1).Any(t1 => t1.curStack >= count);
        }

        private int getTotalCurrencyOfGroup(RPGCurrency initialCurrency)
        {
            var thisTotalLowestCurrency = 0;
            var lowestCurrency = RPGBuilderUtilities.GetCurrencyFromID(initialCurrency.lowestCurrencyID);

            for (var x = lowestCurrency.aboveCurrencies.Count; x > 0; x--)
            {
                var currenciesBeforeThisOne = x - 1;
                if (currenciesBeforeThisOne > 0)
                {
                    var thisCurrencyAmount = CharacterData.Instance.getCurrencyAmount(
                        RPGBuilderUtilities.GetCurrencyFromID(lowestCurrency.aboveCurrencies[x - 1].currencyID));
                    for (var i = 0; i < currenciesBeforeThisOne; i++)
                        thisCurrencyAmount *= RPGBuilderUtilities
                            .GetCurrencyFromID(lowestCurrency.aboveCurrencies[x - 2].currencyID).AmountToConvert;
                    thisCurrencyAmount *= lowestCurrency.AmountToConvert;
                    thisTotalLowestCurrency += thisCurrencyAmount;
                }
                else
                {
                    var thisCurrencyAmount =
                        CharacterData.Instance.getCurrencyAmount(
                            RPGBuilderUtilities.GetCurrencyFromID(lowestCurrency.aboveCurrencies[x - 1].currencyID)) *
                        RPGBuilderUtilities.GetCurrencyFromID(lowestCurrency.aboveCurrencies[x - 1].currencyID)
                            .AmountToConvert;
                    thisTotalLowestCurrency += thisCurrencyAmount;
                }
            }

            thisTotalLowestCurrency += CharacterData.Instance.getCurrencyAmount(lowestCurrency);
            return thisTotalLowestCurrency;
        }

        public int getTotalCountOfItem(RPGItem item)
        {
            var totalcount = 0;
            foreach (var t in bags)
            foreach (var t1 in t.slots)
                if (t1.inUse && t1.itemStored == item)
                    totalcount += t1.curStack;

            return totalcount;
        }

        private int getValueInLowestCurrency(RPGCurrency initialCurrency, int amount)
        {
            var lowestCurrency = RPGBuilderUtilities.GetCurrencyFromID(initialCurrency.lowestCurrencyID);
            var thisTotalLowestCurrency = CharacterData.Instance.getCurrencyAmount(lowestCurrency);
            if (lowestCurrency == initialCurrency || amount < initialCurrency.maxValue) return amount;

            var amountOfAboveCurrency = amount / initialCurrency.AmountToConvert;
            var restOfThisCurrency = amount % initialCurrency.AmountToConvert;

            amountOfAboveCurrency =
                amountOfAboveCurrency * initialCurrency.AmountToConvert * initialCurrency.AmountToConvert;
            restOfThisCurrency = restOfThisCurrency * initialCurrency.AmountToConvert;
            thisTotalLowestCurrency += amountOfAboveCurrency;
            thisTotalLowestCurrency += restOfThisCurrency;
            return thisTotalLowestCurrency;
        }

        private void ConvertCurrenciesToGroups(RPGCurrency lowestCurrency, int totalAmount)
        {
            setCurrencyAmount(lowestCurrency, 0);
            foreach (var t in lowestCurrency.aboveCurrencies)
            {
                var aboceCurrency = RPGBuilderUtilities.GetCurrencyFromID(t.currencyID);
                setCurrencyAmount(aboceCurrency, 0);
            }

            for (var i = lowestCurrency.aboveCurrencies.Count; i > 0; i--)
            {
                var inferiorCurrenciesCount = i - 1;
                var hasToBeDividedBy = 0;
                for (var u = inferiorCurrenciesCount; u > 0; u--)
                    if (hasToBeDividedBy == 0)
                        hasToBeDividedBy += RPGBuilderUtilities
                            .GetCurrencyFromID(lowestCurrency.aboveCurrencies[u - 1].currencyID).AmountToConvert;
                    else
                        hasToBeDividedBy *= RPGBuilderUtilities
                            .GetCurrencyFromID(lowestCurrency.aboveCurrencies[u - 1].currencyID).AmountToConvert;
                if (hasToBeDividedBy == 0)
                    hasToBeDividedBy += lowestCurrency.AmountToConvert;
                else
                    hasToBeDividedBy *= lowestCurrency.AmountToConvert;

                if (hasToBeDividedBy <= 0) continue;
                var amountOfThisCurrency = totalAmount / hasToBeDividedBy;
                totalAmount -= amountOfThisCurrency * hasToBeDividedBy;
                setCurrencyAmount(
                    RPGBuilderUtilities.GetCurrencyFromID(lowestCurrency.aboveCurrencies[i - 1].currencyID),
                    amountOfThisCurrency);
            }

            setCurrencyAmount(lowestCurrency, totalAmount);
        }

        private void setCurrencyAmount(RPGCurrency currency, int amount)
        {
            CharacterData.Instance.currencies[CharacterData.Instance.getCurrencyIndex(currency)].amount = amount;
        }

        private void TryBuyItemFromMerchant(RPGItem item, RPGCurrency currency, int cost)
        {
            var curTotalCurrencyAmount = getTotalCurrencyOfGroup(currency);
            var priceInLowestCurrency = getValueInLowestCurrency(currency, cost);
            if (curTotalCurrencyAmount >= priceInLowestCurrency)
            {
                // enough to buys
                curTotalCurrencyAmount -= priceInLowestCurrency;
                ConvertCurrenciesToGroups(RPGBuilderUtilities.GetCurrencyFromID(currency.lowestCurrencyID),
                    curTotalCurrencyAmount);
                AddItem(item.ID, 1,false, item.randomStats.Count> 0 ?  RPGBuilderUtilities.GenerateRandomItemStats(item.ID, false, true) : -1);
            }
            else
            {
                // not enough to buy
                return;
            }

            if (InventoryDisplayManager.Instance.thisCG.alpha == 1) InventoryDisplayManager.Instance.UpdateCurrency();
        }

        public void BuyItemFromMerchant(RPGItem item, RPGCurrency currency, int amount)
        {
            TryBuyItemFromMerchant(item, currency, amount);
        }


        public static InventoryManager Instance { get; private set; }
    }
}