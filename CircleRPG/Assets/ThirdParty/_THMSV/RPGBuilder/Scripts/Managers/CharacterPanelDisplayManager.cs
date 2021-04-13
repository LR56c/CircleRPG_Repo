using System.Collections.Generic;
using THMSV.RPGBuilder.DisplayHandler;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using TMPro;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class CharacterPanelDisplayManager : MonoBehaviour, IDisplayPanel
    {
        public CanvasGroup thisCG, CharGearCG, CharInfoCG, CharStatsCG, CharTalentsCG, StatTooltipCG;
        private bool showing = false;

        public TextMeshProUGUI CharacterNameText, RaceNameText, ClassNameText, LevelText, ExperienceText, StatTooltipText;

        [System.Serializable]
        public class EquipSlotDATA
        {
            public string slotType;
            public EquipmentItemSlotDisplayHandler itemUIRef;
        }
        public EquipSlotDATA[] armorSlotData;
        public EquipSlotDATA[] weaponSlotData;

        public GameObject StatTitlePrefab, StatTextPrefab, CombatTreeSlotPrefab;
        public Transform StatTextsParent, CombatTreeSlotsParent;
        private List<GameObject> statTextGO = new List<GameObject>();
        private List<GameObject> cbtTreeSlots = new List<GameObject>();

        public enum characterInfoTypes
        {
            gear,
            info,
            stats,
            talents
        }
        public characterInfoTypes curCharInfoType;

        [SerializeField] private Animator talenttCategoryAnimator;
        
        
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
        
            InitCharacterCategory(curCharInfoType.ToString());
            SkillBookDisplayManager.Instance.Hide();
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

        private void disableAllCharCategoriesCG ()
        {
            RPGBuilderUtilities.DisableCG(CharInfoCG);
            RPGBuilderUtilities.DisableCG(CharGearCG);
            RPGBuilderUtilities.DisableCG(CharStatsCG);
            RPGBuilderUtilities.DisableCG(CharTalentsCG);
        }

        public void InitCharacterCategory (string newCategory)
        {
            var parsedEnum = (characterInfoTypes)System.Enum.Parse(typeof(characterInfoTypes), newCategory);
            disableAllCharCategoriesCG();
            switch(parsedEnum)
            {
                case characterInfoTypes.gear:
                    curCharInfoType = characterInfoTypes.gear;
                    RPGBuilderUtilities.EnableCG(CharGearCG);
                    InitCharEquippedItems();
                    break;
                case characterInfoTypes.info:
                    curCharInfoType = characterInfoTypes.info;
                    RPGBuilderUtilities.EnableCG(CharInfoCG);
                    InitCharacterInfo();
                    break;
                case characterInfoTypes.stats:
                    curCharInfoType = characterInfoTypes.stats;
                    RPGBuilderUtilities.EnableCG(CharStatsCG);
                    InitCharStats();
                    break;
                case characterInfoTypes.talents:
                    curCharInfoType = characterInfoTypes.talents;
                    RPGBuilderUtilities.EnableCG(CharTalentsCG);
                    InitCharCombatTrees();
                    break;
            }

            talenttCategoryAnimator.SetBool("glowing", RPGBuilderUtilities.hasPointsToSpendInClassTrees());
        }

        private void InitCharacterInfo ()
        {
            CharacterNameText.text = CharacterData.Instance.CharacterName;
            RaceNameText.text = "Race: " + RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID).displayName; ;
            ClassNameText.text = "Class: " + RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID).displayName;
            LevelText.text = "Level: " + CharacterData.Instance.classDATA.currentClassLevel;
            ExperienceText.text = "Experience: " + CharacterData.Instance.classDATA.currentClassXP + " / " + CharacterData.Instance.classDATA.maxClassXP;
        }

        private void ClearCombatTreeSlots()
        {
            foreach (var t in cbtTreeSlots) Destroy(t);

            cbtTreeSlots.Clear();
        }

        private void InitCharCombatTrees()
        {
            ClearCombatTreeSlots();
            foreach (var t in RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID).talentTrees)
            {
                var cbtTree = Instantiate(CombatTreeSlotPrefab, CombatTreeSlotsParent);
                cbtTreeSlots.Add(cbtTree);
                var slotREF = cbtTree.GetComponent<CombatTreeSlot>();
                slotREF.InitSlot(RPGBuilderUtilities.GetTalentTreeFromID(t.talentTreeID));
            }
        }
        public void InitCharEquippedItems ()
        {
            for (var i = 0; i < armorSlotData.Length; i++)
                if (InventoryManager.Instance.equippedArmors[i].itemEquipped != null)
                    armorSlotData[i].itemUIRef.InitItem(InventoryManager.Instance.equippedArmors[i].itemEquipped, InventoryManager.Instance.equippedArmors[i].temporaryRandomItemID);
                else
                    armorSlotData[i].itemUIRef.ResetItem();
            
            for (var i = 0; i < weaponSlotData.Length; i++)
                if (InventoryManager.Instance.equippedWeapons[i].itemEquipped != null)
                    weaponSlotData[i].itemUIRef.InitItem(InventoryManager.Instance.equippedWeapons[i].itemEquipped, InventoryManager.Instance.equippedWeapons[i].temporaryRandomItemID);
                else
                    weaponSlotData[i].itemUIRef.ResetItem();
        }

        public void InitCharStats ()
        {
            ClearStatText();
            foreach (var t in RPGBuilderEssentials.Instance.combatSettings.UIStatsCategories)
            {
                if (t == "None") continue;
                var statTitle = Instantiate(StatTitlePrefab, StatTextsParent);
                statTextGO.Add(statTitle);
                statTitle.GetComponent<TextMeshProUGUI>().text = t;

                foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
                {
                    if (t1.stat.StatUICategory != t) continue;
                    var statText = Instantiate(StatTextPrefab, StatTextsParent);
                    statTextGO.Add(statText);
                    StatDataHolder statREF = statText.GetComponent<StatDataHolder>();
                    if (statREF != null)
                    {
                        statREF.InitStatText(t1);
                    }
                }
            }
        }

        public void ShowStatTooltipPanel(RPGStat stat)
        {
            StatTooltipCG.alpha = 1;
            StatTooltipText.text = stat.description;
        }
        public void HideStatTooltipPanel()
        {
            StatTooltipCG.alpha = 0;
            StatTooltipText.text = "";
        }

        private void ClearStatText ()
        {
            foreach (var t in statTextGO) Destroy(t);

            statTextGO.Clear();
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        public static CharacterPanelDisplayManager Instance { get; private set; }
    }
}
