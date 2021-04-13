using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Item Settings", menuName = "RPG BUILDER/ITEM SETTINGS")]
public class RPGItemDATA : ScriptableObject
{
    public string[] itemQuality;
    public Sprite[] itemQualityImages;
    public Color[] itemQualityColors;
    public string[] itemType;
    public string[] weaponType;
    public string[] armorType;
    public string[] buildingType;
    public string[] armorSlots;
    public string[] weaponSlots;
    public string[] slotType;

    public int InventorySlots;

    [System.Serializable]
    public class StartingItemsDATA
    {
        public int itemID = -1;
        public RPGItem itemREF;
        public int count = 1;
        public bool equipped;
    }

    
    [System.Serializable]
    public class RandomItemData
    {
        public List<RandomizedStat> randomStats = new List<RandomizedStat>();
        public int randomItemID = -1;
    }
    
    [System.Serializable]
    public class RandomizedStat
    {
        public int statID = -1;
        public float statValue;
    }
    
    [System.Serializable]
    public class RandomizedStatData
    {
        public int statID = -1;
        public RPGStat statREF;
        public float minValue, maxValue = 1f;
        public bool isPercent;
        public bool isInt;
        public float chance = 100f;
    }
    
    public enum randomItemState
    {
        equipped,
        inBag
    }
    
    public void updateThis(RPGItemDATA newData)
    {
        itemType = newData.itemType;
        weaponType = newData.weaponType;
        armorType = newData.armorType;
        buildingType = newData.buildingType;
        itemQuality = newData.itemQuality;
        armorSlots = newData.armorSlots;
        weaponSlots = newData.weaponSlots;
        slotType = newData.slotType;
        InventorySlots = newData.InventorySlots;
        itemQualityImages = newData.itemQualityImages;
        itemQualityColors = newData.itemQualityColors;
    }
}