using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.Character;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace THMSV.RPGBuilder.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        public GameObject RPGBuilderEssentialsPrefab;

        public CanvasGroup HomeCG, CreateCharCG, ContinueCG, BackHomeButtonCG;
        public TMP_InputField characterNameIF;
        public TextMeshProUGUI curSelectedCharacterNameText;

        public GameObject raceSlotPrefab, classSlotPrefab;
        public Transform raceSlotsParent, classSlotsParent;

        private readonly List<RaceSlotHolder> curRaceSlots = new List<RaceSlotHolder>();
        private readonly List<ClassSlotHolder> curClassSlots = new List<ClassSlotHolder>();
        public List<ClassSlotHolder> curSexSlots = new List<ClassSlotHolder>();
        private readonly List<CharacterSlotHolder> curCharSlots = new List<CharacterSlotHolder>();

        public List<RPGRace> allRaces;
        public List<RPGClass> allClasses;

        private int currentlySelectedRace;
        private int currentlySelectedClass;
        private RPGRace.RACE_GENDER currentlySelectedSex;

        public Color slotSelectedColor, slotNotSelectedColor;

        public Transform characterModelSpot;

        public GameObject curPlayerModel;
        //public CharacterData curCharacterDataHolder;

        private List<CharacterData> allCharacters;
        public GameObject characterSlotPrefab;
        public Transform characterSlotsParent;

        public static MainMenuManager Instance { get; private set; }

        private IEnumerator Start()
        {
            if (Instance != null) yield break;
            Instance = this;

            if (FindObjectOfType<RPGBuilderEssentials>() == null)
                Instantiate(RPGBuilderEssentialsPrefab, Vector3.zero, Quaternion.identity);

            disableAllCG();
            RPGBuilderUtilities.EnableCG(HomeCG);

            LoadDATA();
            LoadAllCharacter();
            
            yield return new WaitForEndOfFrame();
            LoadRandomItemsData();
        }

        private void LoadRandomItemsData()
        {
            RandomizedItemsData rdmItemsData = RPGBuilderJsonSaver.LoadRandomItemsData();
            AssignRandomizedItemsData(rdmItemsData);
        }

        private void LoadAllCharacter()
        {
            allCharacters = DataSavingSystem.LoadAllCharacters();
        }

        private void LoadDATA()
        {
            allRaces = Resources.LoadAll<RPGRace>("THMSV/RPGBuilderData/Races").ToList();
            allClasses = Resources.LoadAll<RPGClass>("THMSV/RPGBuilderData/Classes").ToList();
        }

        public void ClickNewChar()
        {
            disableAllCG();
            RPGBuilderUtilities.EnableCG(CreateCharCG);
            RPGBuilderUtilities.EnableCG(BackHomeButtonCG);

            InitCreateNewChar();
        }

        public void ClickContinue()
        {
            disableAllCG();
            RPGBuilderUtilities.EnableCG(ContinueCG);
            RPGBuilderUtilities.EnableCG(BackHomeButtonCG);

            InitContinue();
        }

        public void DeleteCharacter()
        {
            if (allCharacters.Count == 0) return;
            RPGBuilderJsonSaver.DeleteCharacter(CharacterData.Instance.CharacterName);
            allCharacters.Clear();
            LoadAllCharacter();
            if (allCharacters.Count == 0)
                ClickHome();
            else
                InitContinue();
        }

        private void InitContinue()
        {
            clearCharSlots();
            foreach (var t in allCharacters)
            {
                var charSlot = Instantiate(characterSlotPrefab, characterSlotsParent);
                var holder = charSlot.GetComponent<CharacterSlotHolder>();
                holder.Init(t);
                curCharSlots.Add(holder);
            }

            if (allCharacters.Count > 0) SelectCharacter(allCharacters[0].CharacterName);
        }

        public void ClickHome()
        {
            CharacterData.Instance.RESET_CHARACTER_DATA(false);
            disableAllCG();
            RPGBuilderUtilities.EnableCG(HomeCG);

            if (curPlayerModel != null) Destroy(curPlayerModel);
        }

        private void disableAllCG()
        {
            RPGBuilderUtilities.DisableCG(HomeCG);
            RPGBuilderUtilities.DisableCG(ContinueCG);
            RPGBuilderUtilities.DisableCG(CreateCharCG);
            RPGBuilderUtilities.DisableCG(BackHomeButtonCG);
        }

        private void InitCreateNewChar()
        {
            clearAllSlots();

            currentlySelectedRace = 0;
            currentlySelectedClass = 0;

            for (var i = 0; i < allRaces.Count; i++)
            {
                var raceSlot = Instantiate(raceSlotPrefab, raceSlotsParent);
                var holder = raceSlot.GetComponent<RaceSlotHolder>();
                holder.Init(allRaces[i], i);
                curRaceSlots.Add(holder);
            }

            GenerateClassSlots();

            resetAllClassBorders();
            resetAllRaceBorders();

            SelectRace(currentlySelectedRace);
        }

        private void GenerateClassSlots()
        {
            clearClassesSlots();
            for (var i = 0; i < allRaces[currentlySelectedRace].availableClasses.Count; i++)
            {
                var classSlot = Instantiate(classSlotPrefab, classSlotsParent);
                var holder = classSlot.GetComponent<ClassSlotHolder>();
                holder.Init(RPGBuilderUtilities.GetClassFromID(allRaces[currentlySelectedRace].availableClasses[i].classID),
                    i);
                curClassSlots.Add(holder);
            }
        }

        private void clearAllSlots()
        {
            foreach (var t in curRaceSlots)
                Destroy(t.gameObject);

            curRaceSlots.Clear();
            foreach (var t in curClassSlots)
                Destroy(t.gameObject);

            curClassSlots.Clear();
        }

        private void clearCharSlots()
        {
            foreach (var t in curCharSlots)
                Destroy(t.gameObject);

            curCharSlots.Clear();
        }

        private void clearClassesSlots()
        {
            foreach (var t in curClassSlots)
                Destroy(t.gameObject);

            curClassSlots.Clear();
        }

        private void resetAllRaceBorders()
        {
            foreach (var t in curRaceSlots)
                t.selectedBorder.color = slotNotSelectedColor;
        }

        private void resetAllClassBorders()
        {
            foreach (var t in curClassSlots)
                t.selectedBorder.color = slotNotSelectedColor;
        }

        private void resetAllSexBorders()
        {
            foreach (var t in curSexSlots)
                t.selectedBorder.color = slotNotSelectedColor;
        }

        public void SelectRace(int raceIndex)
        {
            currentlySelectedRace = raceIndex;
            CharacterData.Instance.raceID = allRaces[raceIndex].ID;
            CharacterData.Instance.classDATA.classID = allRaces[raceIndex].availableClasses[0].classID;
            CharacterData.Instance.position = RPGBuilderUtilities
                .GetWorldPositionFromID(allRaces[currentlySelectedRace].startingPositionID).position;
            CharacterData.Instance.currentGameSceneID =
                RPGBuilderUtilities.GetGameSceneFromID(allRaces[currentlySelectedRace].startingSceneID).ID;

            SelectSex("Male");
            SelectClass(0);
            resetAllRaceBorders();
            curRaceSlots[raceIndex].selectedBorder.color = slotSelectedColor;

            currentlySelectedClass = 0;
            GenerateClassSlots();

            curClassSlots[currentlySelectedClass].selectedBorder.color = slotSelectedColor;

        }

        public void SelectSex(string sex)
        {
            if (curPlayerModel != null) Destroy(curPlayerModel);

            resetAllSexBorders();
            currentlySelectedSex = (RPGRace.RACE_GENDER) Enum.Parse(typeof(RPGRace.RACE_GENDER), sex);
            if (currentlySelectedSex == RPGRace.RACE_GENDER.Male)
            {
                curSexSlots[0].selectedBorder.color = slotSelectedColor;
                curPlayerModel = Instantiate(allRaces[currentlySelectedRace].malePrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                curSexSlots[1].selectedBorder.color = slotSelectedColor;
                curPlayerModel = Instantiate(allRaces[currentlySelectedRace].femalePrefab, Vector3.zero,
                    Quaternion.identity);
            }


            if (curPlayerModel != null)
            {
                RPGClass classREF = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
                PlayerAppearanceHandler appearanceREF = curPlayerModel.GetComponent<PlayerAppearanceHandler>();
                InventoryManager.Instance.HideAllItemsMainMenu(appearanceREF);
                for (var i = 0; i < classREF.startItems.Count; i++)
                    if (classREF.startItems[i].itemID != -1)
                        InventoryManager.Instance.InitEquipClassItemMainMenu(
                            RPGBuilderUtilities.GetItemFromID(classREF.startItems[i].itemID),
                            appearanceREF, i);
            }

            CharacterData.Instance.gender = currentlySelectedSex;

            curPlayerModel.transform.SetParent(characterModelSpot);
            curPlayerModel.transform.localPosition = Vector3.zero;
            curPlayerModel.transform.localRotation = Quaternion.identity;
            Destroy(curPlayerModel.GetComponent<CharacterController>());
            Destroy(curPlayerModel.GetComponent<CombatNode>());
            Destroy(curPlayerModel.GetComponent<RPGBCharacterController>());
        }

        public void SelectClass(int classIndex)
        {
            resetAllClassBorders();
            currentlySelectedClass = classIndex;
            curClassSlots[classIndex].selectedBorder.color = slotSelectedColor;

            CharacterData.Instance.classDATA.classID = RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID)
                .availableClasses[currentlySelectedClass].classID;

            if (curPlayerModel == null) return;
            RPGClass classREF = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
            PlayerAppearanceHandler appearanceREF = curPlayerModel.GetComponent<PlayerAppearanceHandler>();
            InventoryManager.Instance.HideAllItemsMainMenu(appearanceREF);
            for (var i = 0; i < classREF.startItems.Count; i++)
                if (classREF.startItems[i].itemID != -1)
                    InventoryManager.Instance.InitEquipClassItemMainMenu(
                        RPGBuilderUtilities.GetItemFromID(classREF.startItems[i].itemID),
                        appearanceREF, i);
        }

        bool isCharacterNameAvailable(string charName)
        {
            return allCharacters.Any(t => t.CharacterName == charName);
        }

        public void CreateCharacter()
        {
            if (characterNameIF.text == "") return;
            if (isCharacterNameAvailable(characterNameIF.text)) return;
            CharacterData.Instance.CharacterName = characterNameIF.text;

            var classRef = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
            CharacterData.Instance.talentTrees.Clear();
            foreach (var t in classRef.talentTrees)
            {
                var newTalentTreeDATA = new CharacterData.TalentTrees_DATA();
                newTalentTreeDATA.treeID = t.talentTreeID;
                var talentTreeREF = RPGBuilderUtilities.GetTalentTreeFromID(newTalentTreeDATA.treeID);

                foreach (var t1 in talentTreeREF.nodeList)
                {
                    var newNodeDATA = new CharacterData.TalentTrees_DATA.TalentTreeNode_DATA();
                    newNodeDATA.nodeData = new RPGTalentTree.Node_DATA();
                    var learnedByDefault = false;
                    switch (t1.nodeType)
                    {
                        case RPGTalentTree.TalentTreeNodeType.ability:
                        {
                            newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.ability;
                            newNodeDATA.nodeData.abilityID = t1.abilityID;
                            if (RPGBuilderUtilities.GetAbilityFromID(t1.abilityID).learnedByDefault)
                                learnedByDefault = true;
                            break;
                        }
                        case RPGTalentTree.TalentTreeNodeType.recipe:
                        {
                            newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.recipe;
                            newNodeDATA.nodeData.recipeID = t1.recipeID;
                            if (RPGBuilderUtilities.GetCraftingRecipeFromID(t1.recipeID)
                                .learnedByDefault) learnedByDefault = true;
                            break;
                        }
                        case RPGTalentTree.TalentTreeNodeType.resourceNode:
                        {
                            newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.resourceNode;
                            newNodeDATA.nodeData.resourceNodeID = t1.resourceNodeID;
                            if (RPGBuilderUtilities.GetResourceNodeFromID(t1.resourceNodeID)
                                .learnedByDefault) learnedByDefault = true;
                            break;
                        }
                        case RPGTalentTree.TalentTreeNodeType.bonus:
                        {
                            newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.bonus;
                            newNodeDATA.nodeData.bonusID = t1.bonusID;
                            if (RPGBuilderUtilities.GetBonusFromID(t1.bonusID).learnedByDefault)
                                learnedByDefault = true;
                            break;
                        }
                    }

                    if (learnedByDefault)
                    {
                        newNodeDATA.known = true;
                        newNodeDATA.rank = 1;
                    }
                    else
                    {
                        newNodeDATA.known = false;
                        newNodeDATA.rank = 0;
                    }

                    newTalentTreeDATA.nodes.Add(newNodeDATA);
                }

                CharacterData.Instance.talentTrees.Add(newTalentTreeDATA);
            }


            CharacterData.Instance.bonusesDATA.Clear();
            foreach (var t in RPGBuilderEssentials.Instance.allBonuses)
            {
                var newBonus = new CharacterData.BONUSES_DATA();
                newBonus.bonusID = t.ID;

                newBonus.known = RPGBuilderUtilities.GetBonusFromID(newBonus.bonusID).learnedByDefault;

                newBonus.On = false;
                CharacterData.Instance.bonusesDATA.Add(newBonus);
            }

            CharacterData.Instance.treePoints.Clear();
            foreach (var t in RPGBuilderEssentials.Instance.allTreePoints)
            {
                var newTreePointData = new CharacterData.TreePoints_DATA();
                newTreePointData.treePointID = t.ID;
                newTreePointData.amount = t.startAmount;
                CharacterData.Instance.treePoints.Add(newTreePointData);
            }

            CharacterData.Instance.currencies.Clear();

            foreach (var t in RPGBuilderEssentials.Instance.allCurrencies)
            {
                var newCurrencyData = new CharacterData.Currencies_DATA();
                newCurrencyData.currencyID = t.ID;
                newCurrencyData.amount = t.baseValue;
                CharacterData.Instance.currencies.Add(newCurrencyData);
            }

            for (var i = 0; i < RPGBuilderEssentials.Instance.itemSettings.InventorySlots; i++)
            {
                var newInvItemData = new CharacterData.InventoryItemsDATA();
                newInvItemData.itemID = -1;
                newInvItemData.itemStack = 0;
                newInvItemData.itemRandomID = -1;
                CharacterData.Instance.inventoryItemsDATA.Add(newInvItemData);
            }

            CharacterData.Instance.skillsDATA.Clear();
            foreach (var t1 in RPGBuilderEssentials.Instance.allSkills)
                if (t1.automaticlyAdded)
                {
                    var newSkillData = new CharacterData.SkillsDATA();
                    newSkillData.currentSkillLevel = 1;
                    newSkillData.currentSkillXP = 0;
                    newSkillData.skillID = t1.ID;
                    var skillREF = RPGBuilderUtilities.GetSkillFromID(newSkillData.skillID);
                    newSkillData.maxSkillXP = RPGBuilderUtilities.GetLevelTemplateFromID(skillREF.levelTemplateID)
                        .allLevels[0].XPRequired;
                    foreach (var t2 in skillREF.talentTrees)
                    {
                        var talentTreeREF =
                            RPGBuilderUtilities.GetTalentTreeFromID(t2.talentTreeID);
                        var newTalentTreeDATA = new CharacterData.TalentTrees_DATA();
                        newTalentTreeDATA.treeID = talentTreeREF.ID;

                        foreach (var t in talentTreeREF.nodeList)
                        {
                            CharacterData.TalentTrees_DATA.TalentTreeNode_DATA newNodeDATA =
                                new CharacterData.TalentTrees_DATA.TalentTreeNode_DATA();
                            newNodeDATA.nodeData = new RPGTalentTree.Node_DATA();
                            var learnedByDefault = false;
                            switch (t.nodeType)
                            {
                                case RPGTalentTree.TalentTreeNodeType.ability:
                                {
                                    newNodeDATA.nodeData.abilityID = t.abilityID;
                                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.ability;
                                    if (RPGBuilderUtilities.GetAbilityFromID(t.abilityID)
                                        .learnedByDefault) learnedByDefault = true;
                                    break;
                                }
                                case RPGTalentTree.TalentTreeNodeType.recipe:
                                {
                                    newNodeDATA.nodeData.recipeID = t.recipeID;
                                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.recipe;
                                    if (RPGBuilderUtilities.GetCraftingRecipeFromID(t.recipeID)
                                        .learnedByDefault) learnedByDefault = true;
                                    break;
                                }
                                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                                {
                                    newNodeDATA.nodeData.resourceNodeID = t.resourceNodeID;
                                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.resourceNode;
                                    if (RPGBuilderUtilities.GetResourceNodeFromID(t.resourceNodeID)
                                        .learnedByDefault) learnedByDefault = true;
                                    break;
                                }
                                case RPGTalentTree.TalentTreeNodeType.bonus:
                                {
                                    newNodeDATA.nodeData.bonusID = t.bonusID;
                                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.bonus;
                                    if (RPGBuilderUtilities.GetBonusFromID(t.bonusID)
                                        .learnedByDefault) learnedByDefault = true;
                                    break;
                                }
                            }

                            if (learnedByDefault)
                            {
                                newNodeDATA.known = true;
                                newNodeDATA.rank = 1;
                            }
                            else
                            {
                                newNodeDATA.known = false;
                                newNodeDATA.rank = 0;
                            }

                            newTalentTreeDATA.nodes.Add(newNodeDATA);
                        }

                        CharacterData.Instance.talentTrees.Add(newTalentTreeDATA);
                    }

                    CharacterData.Instance.skillsDATA.Add(newSkillData);
                }

            CharacterData.Instance.classDATA.currentClassLevel = 1;
            CharacterData.Instance.classDATA.maxClassXP = RPGBuilderUtilities
                .GetLevelTemplateFromID(RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID)
                    .levelTemplateID).allLevels[0].XPRequired;

            foreach (var t in CustomInputManager.Instance.abilityKeys)
            {
                var newKeybind = new CharacterData.KeybindDATA();
                newKeybind.keybindName = t.keyName;
                newKeybind.key = t.defaultKey;
                CharacterData.Instance.abilityKeybindsDATA.Add(newKeybind);
            }

            foreach (var t in CustomInputManager.Instance.itemKeys)
            {
                var newKeybind = new CharacterData.KeybindDATA();
                newKeybind.keybindName = t.keyName;
                newKeybind.key = t.defaultKey;
                CharacterData.Instance.itemKeybindsDATA.Add(newKeybind);
            }

            foreach (var t in CustomInputManager.Instance.UIKeys)
            {
                var newKeybind = new CharacterData.KeybindDATA();
                newKeybind.keybindName = t.keyName;
                newKeybind.key = t.defaultKey;
                CharacterData.Instance.UIKeybindsDATA.Add(newKeybind);
            }

            foreach (var t in CustomInputManager.Instance.controlsKeys)
            {
                var newKeybind = new CharacterData.KeybindDATA();
                newKeybind.keybindName = t.keyName;
                newKeybind.key = t.defaultKey;
                CharacterData.Instance.controlsKeybindsDATA.Add(newKeybind);
            }
            
            RPGBuilderJsonSaver.GenerateCharacterEquippedtemsData();

            RPGBuilderJsonSaver.SaveCharacterData(characterNameIF.text, CharacterData.Instance);

            SceneManager.LoadScene(RPGBuilderUtilities.GetGameSceneFromID(CharacterData.Instance.currentGameSceneID)
                ._name);
        }

        private void AssignRandomizedItemsData(RandomizedItemsData randomItemsData)
        {
            RandomizedItemsData.Instance.nextAvailableID = randomItemsData.nextAvailableID;
            RandomizedItemsData.Instance.allPlayerOwnedRandomItems = randomItemsData.allPlayerOwnedRandomItems;
            RandomizedItemsData.Instance.allRandomizedItems = randomItemsData.allRandomizedItems;
        }
        private void AssignCharacterDATA(CharacterData newCharCbtData)
        {
            CharacterData.Instance.created = newCharCbtData.created;
            CharacterData.Instance.CharacterName = newCharCbtData.CharacterName;
            CharacterData.Instance.raceID = newCharCbtData.raceID;
            CharacterData.Instance.gender = newCharCbtData.gender;

            CharacterData.Instance.actionBarAbilities = newCharCbtData.actionBarAbilities;
            CharacterData.Instance.actionBarItems = newCharCbtData.actionBarItems;

            CharacterData.Instance.abilityKeybindsDATA = newCharCbtData.abilityKeybindsDATA;
            CharacterData.Instance.itemKeybindsDATA = newCharCbtData.itemKeybindsDATA;
            CharacterData.Instance.UIKeybindsDATA = newCharCbtData.UIKeybindsDATA;
            CharacterData.Instance.controlsKeybindsDATA = newCharCbtData.controlsKeybindsDATA;

            CharacterData.Instance.classDATA.classID = newCharCbtData.classDATA.classID;
            CharacterData.Instance.classDATA.currentClassLevel = newCharCbtData.classDATA.currentClassLevel;
            CharacterData.Instance.classDATA.currentClassXP = newCharCbtData.classDATA.currentClassXP;
            CharacterData.Instance.classDATA.maxClassXP = newCharCbtData.classDATA.maxClassXP;

            CharacterData.Instance.bonusesDATA = newCharCbtData.bonusesDATA;

            CharacterData.Instance.armorsEquipped = newCharCbtData.armorsEquipped;
            CharacterData.Instance.weaponsEquipped = newCharCbtData.weaponsEquipped;
            CharacterData.Instance.inventoryItemsDATA = newCharCbtData.inventoryItemsDATA;

            CharacterData.Instance.position = newCharCbtData.position;

            CharacterData.Instance.currentGameSceneID = newCharCbtData.currentGameSceneID;

            CharacterData.Instance.talentTrees = newCharCbtData.talentTrees;
            CharacterData.Instance.treePoints = newCharCbtData.treePoints;
            CharacterData.Instance.currencies = newCharCbtData.currencies;
            CharacterData.Instance.questsData = newCharCbtData.questsData;
            CharacterData.Instance.npcKilled = newCharCbtData.npcKilled;
            CharacterData.Instance.scenesEntered = newCharCbtData.scenesEntered;
            CharacterData.Instance.regionsEntered = newCharCbtData.regionsEntered;
            CharacterData.Instance.abilitiesLearned = newCharCbtData.abilitiesLearned;
            CharacterData.Instance.itemsGained = newCharCbtData.itemsGained;
            CharacterData.Instance.skillsDATA = newCharCbtData.skillsDATA;
            CharacterData.Instance.bonusLearned = newCharCbtData.bonusLearned;
        }

        public void PlaySelectedCharacter()
        {
            HandleBackdating();
            SceneManager.LoadScene(RPGBuilderUtilities.GetGameSceneFromID(CharacterData.Instance.currentGameSceneID)._name);
        }

        private bool classHasTalentTree(int ID, RPGClass _class)
        {
            foreach (var t in _class.talentTrees)
                if (t.talentTreeID == ID)
                    return true;

            return false;
        }

        private bool skillsHaveTalentTree(int ID)
        {
            foreach (var t1 in CharacterData.Instance.skillsDATA)
            {
                RPGSkill skillREF = RPGBuilderUtilities.GetSkillFromID(t1.skillID);
                foreach (var t in skillREF.talentTrees)
                {
                    if (t.talentTreeID == ID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool characterHasTalentTree(int ID, CharacterData charData)
        {
            foreach (var t in charData.talentTrees)
                if (t.treeID == ID)
                    return true;

            return false;
        }

        private bool characterHasBonus(int ID, CharacterData charData)
        {
            foreach (var t in charData.bonusesDATA)
                if (t.bonusID == ID)
                    return true;

            return false;
        }

        private bool bonusFromCharStillExist(int ID, CharacterData charData)
        {
            for (var i = 0; i < RPGBuilderEssentials.Instance.allBonuses.Count; i++)
                if (charData.bonusesDATA[i].bonusID == ID)
                    return true;
            return false;
        }

        private bool characterHasTreePoint(int ID, CharacterData charData)
        {
            foreach (var t in charData.treePoints)
                if (t.treePointID == ID)
                    return true;

            return false;
        }

        private bool treePointFromCharStillExist(int ID, CharacterData charData)
        {
            for (var i = 0; i < RPGBuilderEssentials.Instance.allTreePoints.Count; i++)
                if (charData.treePoints[i].treePointID == ID)
                    return true;
            return false;
        }

        private bool currencyFromCharStillExist(int ID, CharacterData charData)
        {
            for (var i = 0; i < RPGBuilderEssentials.Instance.allCurrencies.Count; i++)
                if (charData.currencies[i].currencyID == ID)
                    return true;
            return false;
        }

        private bool skillFromCharStillExist(int ID, CharacterData charData)
        {
            for (var i = 0; i < RPGBuilderEssentials.Instance.allSkills.Count; i++)
                if (charData.skillsDATA[i].skillID == ID)
                    return true;
            return false;
        }

        private bool characterHasCurrency(int ID, CharacterData charData)
        {
            foreach (var t in charData.currencies)
                if (t.currencyID == ID)
                    return true;

            return false;
        }

        private bool characterHasSkill(int ID, CharacterData charData)
        {
            foreach (var t in charData.skillsDATA)
                if (t.skillID == ID)
                    return true;

            return false;
        }

        private bool characterHasTalentTreeNode(RPGTalentTree tree, RPGTalentTree.Node_DATA nodeDATA,
            CharacterData charData)
        {
            foreach (var t in charData.talentTrees)
            foreach (var t1 in t.nodes)
                if (t1.nodeData == nodeDATA)
                    return true;

            return false;
        }

        private bool talentTreeHasTalentTreeNode(RPGTalentTree tree, RPGTalentTree.Node_DATA nodeDATA,
            CharacterData charData)
        {
            for (var i = 0; i < tree.nodeList.Count; i++)
                foreach (var t in charData.talentTrees)
                    if (RPGBuilderUtilities.GetTalentTreeFromID(t.treeID) == tree)
                        for (var u = 0; u < t.nodes.Count; u++)
                            if (t.nodes[u].nodeData == nodeDATA)
                                return true;

            return false;
        }

        private void RemoveTalentTreeNodeFromCharacter(RPGTalentTree tree, RPGTalentTree.Node_DATA nodeDATA)
        {
            foreach (var t in CharacterData.Instance.talentTrees)
                if (RPGBuilderUtilities.GetTalentTreeFromID(t.treeID) == tree)
                    for (var x = 0; x < t.nodes.Count; x++)
                        if (t.nodes[x].nodeData == nodeDATA)
                            t.nodes.RemoveAt(x);
        }

        private bool TalentTreeHasChanged(RPGTalentTree treeREF, int charTalentTreeIndex)
        {
            for (var i = 0; i < treeREF.nodeList.Count; i++)
            {
                if (treeREF.nodeList.Count != CharacterData.Instance.talentTrees[charTalentTreeIndex].nodes.Count)
                    return true;
                if (treeREF.nodeList[i].nodeType !=
                    CharacterData.Instance.talentTrees[charTalentTreeIndex].nodes[i].nodeData.nodeType) return true;

                if (treeREF.nodeList[i].nodeType == RPGTalentTree.TalentTreeNodeType.ability &&
                    treeREF.nodeList[i].abilityID != CharacterData.Instance.talentTrees[charTalentTreeIndex].nodes[i]
                        .nodeData.abilityID
                    || treeREF.nodeList[i].nodeType == RPGTalentTree.TalentTreeNodeType.recipe &&
                    treeREF.nodeList[i].recipeID !=
                    CharacterData.Instance.talentTrees[charTalentTreeIndex].nodes[i].nodeData.recipeID
                    || treeREF.nodeList[i].nodeType == RPGTalentTree.TalentTreeNodeType.resourceNode &&
                    treeREF.nodeList[i].resourceNodeID != CharacterData.Instance.talentTrees[charTalentTreeIndex].nodes[i]
                        .nodeData.resourceNodeID
                    || treeREF.nodeList[i].nodeType == RPGTalentTree.TalentTreeNodeType.bonus &&
                    treeREF.nodeList[i].bonusID !=
                    CharacterData.Instance.talentTrees[charTalentTreeIndex].nodes[i].nodeData.bonusID
                )
                    return true;
            }

            return false;
        }

        private void AddTalentTree(int treeID)
        {
            var newTalentTreeDATA = new CharacterData.TalentTrees_DATA();
            newTalentTreeDATA.treeID = treeID;
            var talentTreeREF = RPGBuilderUtilities.GetTalentTreeFromID(newTalentTreeDATA.treeID);

            foreach (var t in talentTreeREF.nodeList)
            {
                var newNodeDATA = new CharacterData.TalentTrees_DATA.TalentTreeNode_DATA();
                newNodeDATA.nodeData = new RPGTalentTree.Node_DATA();
                var learnedByDefault = false;
                if (t.nodeType == RPGTalentTree.TalentTreeNodeType.ability)
                {
                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.ability;
                    newNodeDATA.nodeData.abilityID = t.abilityID;
                    if (RPGBuilderUtilities.GetAbilityFromID(t.abilityID).learnedByDefault)
                        learnedByDefault = true;
                }
                else if (t.nodeType == RPGTalentTree.TalentTreeNodeType.recipe)
                {
                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.recipe;
                    newNodeDATA.nodeData.recipeID = t.recipeID;
                    if (RPGBuilderUtilities.GetCraftingRecipeFromID(t.recipeID).learnedByDefault)
                        learnedByDefault = true;
                }
                else if (t.nodeType == RPGTalentTree.TalentTreeNodeType.resourceNode)
                {
                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.resourceNode;
                    newNodeDATA.nodeData.resourceNodeID = t.resourceNodeID;
                    if (RPGBuilderUtilities.GetResourceNodeFromID(t.resourceNodeID).learnedByDefault
                    ) learnedByDefault = true;
                }
                else if (t.nodeType == RPGTalentTree.TalentTreeNodeType.bonus)
                {
                    newNodeDATA.nodeData.nodeType = RPGTalentTree.TalentTreeNodeType.bonus;
                    newNodeDATA.nodeData.bonusID = t.bonusID;
                    if (RPGBuilderUtilities.GetBonusFromID(t.bonusID).learnedByDefault)
                        learnedByDefault = true;
                }

                if (learnedByDefault)
                {
                    newNodeDATA.known = true;
                    newNodeDATA.rank = 1;
                }
                else
                {
                    newNodeDATA.known = false;
                    newNodeDATA.rank = 0;
                }

                newTalentTreeDATA.nodes.Add(newNodeDATA);
            }

            CharacterData.Instance.talentTrees.Add(newTalentTreeDATA);
        }

        private void AddBonus(int bonusID)
        {
            var newBonus = new CharacterData.BONUSES_DATA();
            newBonus.bonusID = bonusID;

            newBonus.known = RPGBuilderUtilities.GetBonusFromID(newBonus.bonusID).learnedByDefault;

            newBonus.On = false;
            CharacterData.Instance.bonusesDATA.Add(newBonus);
        }

        private void AddTreePoint(RPGTreePoint treePoint)
        {
            var newTreePointData = new CharacterData.TreePoints_DATA();
            newTreePointData.treePointID = treePoint.ID;
            newTreePointData.amount = treePoint.startAmount;
            CharacterData.Instance.treePoints.Add(newTreePointData);
        }

        private void AddCurrency(RPGCurrency currency)
        {
            var newCurrencyData = new CharacterData.Currencies_DATA();
            newCurrencyData.currencyID = currency.ID;
            newCurrencyData.amount = currency.baseValue;
            CharacterData.Instance.currencies.Add(newCurrencyData);
        }

        private void AddSkill(RPGSkill skillREF)
        {
            var newSkillData = new CharacterData.SkillsDATA();
            newSkillData.currentSkillLevel = 1;
            newSkillData.currentSkillXP = 0;
            newSkillData.skillID = skillREF.ID;
            newSkillData.maxSkillXP =
                RPGBuilderUtilities.GetLevelTemplateFromID(skillREF.levelTemplateID).allLevels[0].XPRequired;
            foreach (var t in skillREF.talentTrees)
                AddTalentTree(t.talentTreeID);

            CharacterData.Instance.skillsDATA.Add(newSkillData);
        }

        private void HandleBackdating()
        {
            var classRef = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
            for (var i = 0; i < CharacterData.Instance.talentTrees.Count; i++)
                if (!classHasTalentTree(CharacterData.Instance.talentTrees[i].treeID, classRef) && !skillsHaveTalentTree(CharacterData.Instance.talentTrees[i].treeID))
                {
                    // If character had a talent tree that was not anymore existing on this class or skills, remove it
                    CharacterData.Instance.talentTrees.RemoveAt(i);
                }
                else
                {
                    // If character had a talent tree that has been modified, remove it and add the modified one instead, also refund points spent
                    var treeREF = RPGBuilderUtilities.GetTalentTreeFromID(CharacterData.Instance.talentTrees[i].treeID);
                    if (!TalentTreeHasChanged(treeREF, i)) continue;
                    TreePointsManager.Instance.AddTreePoint(treeREF.treePointAcceptedID,
                        CharacterData.Instance.talentTrees[i].pointsSpent);
                    CharacterData.Instance.talentTrees.RemoveAt(i);
                    AddTalentTree(treeREF.ID);
                }

            foreach (var t in classRef.talentTrees)
                if (!characterHasTalentTree(t.talentTreeID, CharacterData.Instance))
                    // If class has a talent tree that is not on this character, add it
                    AddTalentTree(t.talentTreeID);

            for (int i = 0; i < CharacterData.Instance.skillsDATA.Count; i++)
            {
                RPGSkill skillREF = RPGBuilderUtilities.GetSkillFromID(CharacterData.Instance.skillsDATA[i].skillID);
                foreach (var t in skillREF.talentTrees)
                    if (!characterHasTalentTree(t.talentTreeID, CharacterData.Instance))
                        // If skill has a talent tree that is not on this character, add it
                        AddTalentTree(t.talentTreeID);
            }

            foreach (var t in RPGBuilderEssentials.Instance.allBonuses)
                if (!characterHasBonus(t.ID, CharacterData.Instance))
                    AddBonus(t.ID);

            for (var i = 0; i < CharacterData.Instance.bonusesDATA.Count; i++)
                if (!bonusFromCharStillExist(CharacterData.Instance.bonusesDATA[i].bonusID, CharacterData.Instance))
                    CharacterData.Instance.bonusesDATA.RemoveAt(i);

            foreach (var t in RPGBuilderEssentials.Instance.allTreePoints)
                if (!characterHasTreePoint(t.ID, CharacterData.Instance))
                    AddTreePoint(t);

            for (var i = 0; i < CharacterData.Instance.treePoints.Count; i++)
                if (!treePointFromCharStillExist(CharacterData.Instance.treePoints[i].treePointID, CharacterData.Instance))
                    CharacterData.Instance.treePoints.RemoveAt(i);

            foreach (var t in RPGBuilderEssentials.Instance.allCurrencies)
                if (!characterHasCurrency(t.ID, CharacterData.Instance))
                    AddCurrency(t);

            for (var i = 0; i < CharacterData.Instance.currencies.Count; i++)
                if (!currencyFromCharStillExist(CharacterData.Instance.currencies[i].currencyID, CharacterData.Instance))
                    CharacterData.Instance.currencies.RemoveAt(i);

            foreach (var t in RPGBuilderEssentials.Instance.allSkills)
                if (!characterHasSkill(t.ID, CharacterData.Instance))
                    if (t.automaticlyAdded)
                        AddSkill(t);

            for (var i = 0; i < CharacterData.Instance.skillsDATA.Count; i++)
                if (!skillFromCharStillExist(CharacterData.Instance.skillsDATA[i].skillID, CharacterData.Instance))
                    CharacterData.Instance.skillsDATA.RemoveAt(i);

            RPGBuilderJsonSaver.SaveCharacterData(CharacterData.Instance.CharacterName, CharacterData.Instance);
        }

        public void SelectCharacter(string characterName)
        {
            CharacterData.Instance.RESET_CHARACTER_DATA(false);
            var curCharCbtData = RPGBuilderJsonSaver.LoadCharacterData(characterName);
            curSelectedCharacterNameText.text = curCharCbtData.CharacterName;
            AssignCharacterDATA(curCharCbtData);

            if (curPlayerModel != null) Destroy(curPlayerModel);

            if (CharacterData.Instance.gender == RPGRace.RACE_GENDER.Male)
            {
                curSexSlots[0].selectedBorder.color = slotSelectedColor;
                curPlayerModel = Instantiate(
                    RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID).malePrefab,
                    Vector3.zero, Quaternion.identity);
            }
            else
            {
                curSexSlots[1].selectedBorder.color = slotSelectedColor;
                curPlayerModel = Instantiate(
                    RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID).femalePrefab,
                    Vector3.zero, Quaternion.identity);
            }

            curPlayerModel.transform.SetParent(characterModelSpot);
            curPlayerModel.transform.localPosition = Vector3.zero;
            curPlayerModel.transform.localRotation = Quaternion.identity;
            Destroy(curPlayerModel.GetComponent<CharacterController>());
            Destroy(curPlayerModel.GetComponent<CombatNode>());
            Destroy(curPlayerModel.GetComponent<RPGBCharacterController>());

            var appearanceref = curPlayerModel.GetComponent<PlayerAppearanceHandler>();

            for (var i = 0; i < CharacterData.Instance.armorsEquipped.Count; i++)
                if (CharacterData.Instance.armorsEquipped[i].itemID != -1)
                    InventoryManager.Instance.InitEquipItemMainMenu(
                        RPGBuilderUtilities.GetItemFromID(CharacterData.Instance.armorsEquipped[i].itemID),
                        appearanceref, i);

            for (var i = 0; i < CharacterData.Instance.weaponsEquipped.Count; i++)
                if (CharacterData.Instance.weaponsEquipped[i].itemID != -1)
                    InventoryManager.Instance.InitEquipItemMainMenu(
                        RPGBuilderUtilities.GetItemFromID(CharacterData.Instance.weaponsEquipped[i].itemID),
                        appearanceref, i);
        }

        private int getRaceIDByName(string raceName)
        {
            for (var i = 0; i < allRaces.Count; i++)
                if (allRaces[i]._name == raceName)
                    return i;
            return -1;
        }

        private CharacterData getCharacterDataByName(string characterName)
        {
            foreach (var t in allCharacters)
                if (t.CharacterName == characterName)
                    return t;

            return null;
        }
    }
}