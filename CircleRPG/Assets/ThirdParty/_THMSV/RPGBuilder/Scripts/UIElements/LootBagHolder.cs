using System;
using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using UnityEngine;

namespace THMSV.RPGBuilder.UIElements
{
    public class LootBagHolder : MonoBehaviour
    {
        [Serializable]
        public class Loot_Data
        {
            public RPGItem item;
            public int count;
            public bool looted;
            public int randomItemID;
        }

        public List<Loot_Data> lootData = new List<Loot_Data>();

        public void CheckLootState()
        {
            var nonLootedItem = lootData.Count(t => !t.looted);

            if (nonLootedItem != 0) return;
            LootPanelDisplayManager.Instance.Hide();
            LootPanelDisplayManager.Instance.ClearAllLootItemSlots();
            Destroy(gameObject);
        }

        private void OnMouseOver()
        {
            if (!Input.GetMouseButton(1)) return;
            if (Vector3.Distance(transform.position, CombatManager.playerCombatInfo.transform.position) < 4)
            {
                LootPanelDisplayManager.Instance.DisplayLoot(this);
            }
            else
            {
                ErrorEventsDisplayManager.Instance.ShowErrorEvent("This is too far", 3);
            }
        }

    }
}