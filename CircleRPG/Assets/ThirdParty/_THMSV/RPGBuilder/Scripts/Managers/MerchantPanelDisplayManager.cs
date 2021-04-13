using System.Collections.Generic;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class MerchantPanelDisplayManager : MonoBehaviour, IDisplayPanel
    {
        public CanvasGroup thisCG;

        public GameObject merchantItemSlotPrefab;

        public Transform merchantItemsSlotsParent;
        public List<GameObject> currentMerchantItemSlots = new List<GameObject>();

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static MerchantPanelDisplayManager Instance { get; private set; }


        private void ClearAllMerchantItemsSlots()
        {
            foreach (var t in currentMerchantItemSlots)
                Destroy(t);

            currentMerchantItemSlots.Clear();
        }


        private void InitializeMerchantPanel(RPGNpc npc)
        {
            ClearAllMerchantItemsSlots();
            var merchantTableREF = RPGBuilderUtilities.GetMerchantTableFromID(npc.merchantTableID);
            foreach (var t in merchantTableREF.onSaleItems)
            {
                var newItemSlot = Instantiate(merchantItemSlotPrefab, merchantItemsSlotsParent);
                var holder = newItemSlot.GetComponent<MerchantItemSlotHolder>();
                holder.Init(RPGBuilderUtilities.GetItemFromID(t.itemID),
                    RPGBuilderUtilities.GetCurrencyFromID(t.currencyID),
                    t.cost);
                currentMerchantItemSlots.Add(newItemSlot);
            }
        }

        public void Show(RPGNpc npc)
        {
            Show();
            InitializeMerchantPanel(npc);
        }

        public void Show()
        {
            RPGBuilderUtilities.EnableCG(thisCG);
            transform.SetAsLastSibling();
            CustomInputManager.Instance.AddOpenedPanel(thisCG);
        }

        public void Hide()
        {
            gameObject.transform.SetAsFirstSibling();
            RPGBuilderUtilities.DisableCG(thisCG);
            if(CustomInputManager.Instance != null && CustomInputManager.Instance.allOpenedPanels.Contains(thisCG)) CustomInputManager.Instance.allOpenedPanels.Remove(thisCG);
        }

        private void Awake()
        {
            Hide();
        }
    }
}