using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.UI;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class LootPanelDisplayManager : MonoBehaviour, IDisplayPanel
    {
        public CanvasGroup thisCG;
        private bool showing;

        public List<GameObject> curLootItemSlots = new List<GameObject>();
        public Transform lootItemsSlotsParent;

        public GameObject lootItemSlotPrefab;
        private LootBagHolder currentLootBag;
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static LootPanelDisplayManager Instance { get; private set; }

        public void ClearAllLootItemSlots()
        {
            foreach (var t in curLootItemSlots)
                Destroy(t);

            curLootItemSlots.Clear();
        }

        public void RemoveItemSlot(GameObject go)
        {
            for (var i = 0; i < curLootItemSlots.Count; i++)
                if (curLootItemSlots[i] == go)
                {
                    curLootItemSlots.Remove(go);
                    Destroy(go);
                }
        }

        public void LootAll()
        {
            foreach (var t in currentLootBag.lootData.Where(t => !t.looted))
            {
                InventoryManager.Instance.AddItem(t.item.ID,
                    t.count,false, t.randomItemID);
                t.looted = true;
                RemoveItemSlot(gameObject);
            }

            currentLootBag.CheckLootState();
            ItemTooltip.Instance.Hide();
        }

        public void DisplayLoot(LootBagHolder bagHolder)
        {
            currentLootBag = bagHolder;
            Show();
            ClearAllLootItemSlots();
            for (var i = 0; i < bagHolder.lootData.Count; i++)
                if (!bagHolder.lootData[i].looted)
                {
                    var newLootItemSlot = Instantiate(lootItemSlotPrefab, lootItemsSlotsParent);
                    var holder = newLootItemSlot.GetComponent<LootItemSlotHolder>();
                    holder.Init(i, bagHolder);
                    curLootItemSlots.Add(newLootItemSlot);
                }
        }

        public void Show()
        {
            showing = true;
            RPGBuilderUtilities.EnableCG(thisCG);
            transform.SetAsLastSibling();
            CustomInputManager.Instance.AddOpenedPanel(thisCG);
        }

        public void Hide()
        {
            gameObject.transform.SetAsFirstSibling();

            showing = false;
            RPGBuilderUtilities.DisableCG(thisCG);
            if(CustomInputManager.Instance != null && CustomInputManager.Instance.allOpenedPanels.Contains(thisCG)) CustomInputManager.Instance.allOpenedPanels.Remove(thisCG);
        }

        private void Awake()
        {
            Hide();
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }
    }
}