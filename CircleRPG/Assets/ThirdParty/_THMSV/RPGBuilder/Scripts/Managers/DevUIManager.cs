using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.Managers
{
    public class DevUIManager : MonoBehaviour, IDisplayPanel
    {
        private bool showing;
        public CanvasGroup thisCG, getItemCG;

        public Image GeneralCategory, CombatCategory, WorldCategory;
        public GameObject GeneralCategoryPanel, CombatCategoryPanel, WorldCategoryPanel;

        public TMP_Dropdown currencyDropdown;
        public TMP_InputField addCurrencyField;
        public TMP_InputField getItemName;
        public TMP_InputField getItemCount;

        public TMP_InputField alterHealthField;
        public TMP_InputField classXPField;

        public Transform itemsParent;
        public GameObject getItemSlotPrefab;

        public List<GameObject> curGetItemListSlots = new List<GameObject>();

        public Color selectedColor, NotSelectedColor;

        public void selectCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "general":
                    GeneralCategory.color = selectedColor;
                    CombatCategory.color = NotSelectedColor;
                    WorldCategory.color = NotSelectedColor;
                    break;

                case "combat":
                    GeneralCategory.color = NotSelectedColor;
                    CombatCategory.color = selectedColor;
                    WorldCategory.color = NotSelectedColor;
                    break;

                case "world":
                    GeneralCategory.color = NotSelectedColor;
                    CombatCategory.color = NotSelectedColor;
                    WorldCategory.color = selectedColor;
                    break;
            }

            ShowCategory(categoryName);
        }

        private void ShowCategory(string categoryName)
        {
            switch (categoryName)
            {
                case "general":
                    GeneralCategoryPanel.SetActive(true);
                    CombatCategoryPanel.SetActive(false);
                    WorldCategoryPanel.SetActive(false);
                    PopulateCurrencyDropdown();
                    break;

                case "combat":
                    GeneralCategoryPanel.SetActive(false);
                    CombatCategoryPanel.SetActive(true);
                    WorldCategoryPanel.SetActive(false);
                    break;

                case "world":
                    GeneralCategoryPanel.SetActive(false);
                    CombatCategoryPanel.SetActive(false);
                    WorldCategoryPanel.SetActive(true);
                    break;
            }
        }

        private void PopulateCurrencyDropdown()
        {
            var currencyOptions = new List<TMP_Dropdown.OptionData>();
            foreach (var currency in RPGBuilderEssentials.Instance.allCurrencies)
            {
                var newOption = new TMP_Dropdown.OptionData();
                newOption.text = currency.displayName;
                newOption.image = currency.icon;
                currencyOptions.Add(newOption);
            }

            currencyDropdown.ClearOptions();
            currencyDropdown.options = currencyOptions;
        }

        public void DEVAlterCurrency()
        {
            InventoryManager.Instance.AddCurrency(
                RPGBuilderUtilities.getCurrencyByName(currencyDropdown.options[currencyDropdown.value].text).ID,
                int.Parse(addCurrencyField.text));
        }

        public void DEVAlterHealth()
        {
            var value = int.Parse(alterHealthField.text);
            if (value < 0)
                value += Mathf.Abs(value) * 2;
            else
                value -= value * 2;
            CombatManager.playerCombatInfo.TakeDamage(CombatManager.playerCombatInfo, value, null);
        }

        public void AddClassXP()
        {
            LevelingManager.Instance.AddClassXP(int.Parse(classXPField.text));
        }

        public void GetItem(RPGItem item)
        {
            if (item == null) return;
            int amt = 0;
            if (getItemCount.text == "" || int.Parse(getItemCount.text) == 0 || int.Parse(getItemCount.text) == 1)
                amt = 1;
            else
                amt = int.Parse(getItemCount.text);

            for (int i = 0; i < amt; i++)
            {
                InventoryManager.Instance.AddItem(item.ID, 1,false, item.randomStats.Count> 0 ?  RPGBuilderUtilities.GenerateRandomItemStats(item.ID, false, true) : -1);
            }
            
        }

        public void HideGetItemPanel()
        {
            RPGBuilderUtilities.DisableCG(getItemCG);
        }

        public void ShowGetItemPanel()
        {
            RPGBuilderUtilities.EnableCG(getItemCG);

            UpdateGetItemList();
        }

        private void ClearGetItemList()
        {
            foreach (var t in curGetItemListSlots)
                Destroy(t);

            curGetItemListSlots.Clear();
        }

        public void UpdateGetItemList()
        {
            ClearGetItemList();
            var curSearch = getItemName.text;

            var allItems = RPGBuilderEssentials.Instance.allItems;
            var validItems = new List<RPGItem>();


            if (curSearch.Length > 0 && !string.IsNullOrEmpty(curSearch) && !string.IsNullOrWhiteSpace(curSearch))
                foreach (var t in allItems)
                {
                    var itemNameToCheck = t._name;
                    itemNameToCheck = itemNameToCheck.ToLower();
                    curSearch = curSearch.ToLower();

                    if (itemNameToCheck.Contains(curSearch)) validItems.Add(t);
                }
            else
                validItems = allItems;


            foreach (var t in validItems)
            {
                var newGetItemSlot = Instantiate(getItemSlotPrefab, itemsParent);
                var newGetItemSlotRef = newGetItemSlot.GetComponent<GetItemSlot>();
                newGetItemSlotRef.thisitem = t;
                newGetItemSlotRef.icon.sprite = t.icon;
                curGetItemListSlots.Add(newGetItemSlot);
            }
        }

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

            selectCategory("general");
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


        public static DevUIManager Instance { get; private set; }
    }
}