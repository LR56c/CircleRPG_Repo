using System.Collections.Generic;
using UnityEngine;

public class RandomizedItemsData : MonoBehaviour
{
    private static RandomizedItemsData instance;
    public static RandomizedItemsData Instance => instance;

    public int nextAvailableID = 0;

    [System.Serializable]
    public class RandomizedItems
    {
        public string itemName = "";
        public int itemID = -1;
        public int id = -1;
        public List<RPGItemDATA.RandomizedStat> randomStats = new List<RPGItemDATA.RandomizedStat>();
        public RPGItemDATA.randomItemState state;
    }

    public List<RandomizedItems> allPlayerOwnedRandomItems = new List<RandomizedItems>();
    public List<RandomizedItems> allRandomizedItems = new List<RandomizedItems>();
    private void Start()
    {
        if (instance != null) return;
        instance = this;
    }

}
