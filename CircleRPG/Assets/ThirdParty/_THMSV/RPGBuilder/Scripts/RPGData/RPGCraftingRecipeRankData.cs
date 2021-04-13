using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGCraftingRecipeRankData : ScriptableObject
{
    public int ID = -1;
    public int unlockCost;
    public int Experience;
    public float craftTime;

    [Serializable]
    public class CraftedItemsDATA
    {
        public float chance;
        public int count;
        public int craftedItemID = -1;
        public RPGItem craftedItemREF;
    }

    public List<CraftedItemsDATA> allCraftedItems = new List<CraftedItemsDATA>();

    [Serializable]
    public class ComponentsRequired
    {
        public int count;
        public int componentItemID = -1;
        public RPGItem componentItemREF;
    }

    public List<ComponentsRequired> allComponents = new List<ComponentsRequired>();

    public void copyData(RPGCraftingRecipeRankData original, RPGCraftingRecipeRankData copied)
    {
        original.Experience = copied.Experience;
        original.allCraftedItems = copied.allCraftedItems;
        original.allComponents = copied.allComponents;
        original.craftTime = copied.craftTime;
        original.unlockCost = copied.unlockCost;
    }

    public void updateThis(RPGCraftingRecipeRankData newDATA)
    {
        ID = newDATA.ID;
        Experience = newDATA.Experience;
        allCraftedItems = newDATA.allCraftedItems;
        allComponents = newDATA.allComponents;
        craftTime = newDATA.craftTime;
        unlockCost = newDATA.unlockCost;
    }
}