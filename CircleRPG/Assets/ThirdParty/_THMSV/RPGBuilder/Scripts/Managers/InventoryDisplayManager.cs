using System.Collections.Generic;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class InventoryDisplayManager : MonoBehaviour, IDisplayPanel
    {
        private bool showing;
        public CanvasGroup thisCG;

        public GameObject itemSlotPrefab;
        public Transform itemSlotsParent;

        private readonly List<GameObject> currentSlots = new List<GameObject>();


        public GameObject[] invSlotsParent;

        public List<CurrencyDisplaySlotHolder> allCurrencySlots = new List<CurrencyDisplaySlotHolder>();

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public void Show()
        {
            showing = true;
            RPGBuilderUtilities.EnableCG(thisCG);
            transform.SetAsLastSibling();

            UpdateSlots();
            UpdateCurrency();
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

        private void ClearSlots()
        {
            foreach (var t in currentSlots)
                Destroy(t);

            currentSlots.Clear();
        }

        public void UpdateCurrency()
        {
            foreach (var t in allCurrencySlots)
                t.UpdateCurrencySlot();
        }

        public void UpdateSlots()
        {
            ClearSlots();
            for (var i = 0; i < InventoryManager.Instance.bags.Length; i++)
            for (var x = 0; x < InventoryManager.Instance.bags[i].slots.Count; x++)
                if (InventoryManager.Instance.bags[i].slots[x].inUse)
                {
                    var newSlot = Instantiate(itemSlotPrefab, invSlotsParent[x].transform);
                    var newSlotHolder = newSlot.GetComponent<ItemSlotHolder>();
                    newSlotHolder.InitSlot(InventoryManager.Instance.bags[i].slots[x].itemStored, i, x);
                    currentSlots.Add(newSlot);
                }
        }

        public static InventoryDisplayManager Instance { get; private set; }
    }
}