using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using THMSV.RPGBuilder.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace THMSV.RPGBuilder.LogicMono
{
    public class RPGBuilderEssentials : MonoBehaviour
    {
        private static RPGBuilderEssentials instance;
        public Canvas mainGameCanvas;

        private float nextAutomaticSave = 0;
    
        public RPGGeneralDATA generalSettings;
        public RPGItemDATA itemSettings;
        public RPGCombatDATA combatSettings;

        public List<RPGAbility> allAbilities;
        public List<RPGEffect> allEffects;
        public List<RPGNpc> allNPCs;
        public List<RPGStat> allStats;
        public List<RPGTreePoint> allTreePoints;

        public List<RPGItem> allItems;
        public List<RPGSkill> allSkills;
        public List<RPGLevelsTemplate> allLevelTemplates;
        public List<RPGRace> allRaces;
        public List<RPGClass> allClasses;
        public List<RPGLootTable> allLootTables;
        public List<RPGMerchantTable> allMerchantTables;
        public List<RPGCurrency> allCurrencies;
        public List<RPGCraftingRecipe> allCraftingRecipes;
        public List<RPGCraftingStation> allCraftingStation;
        public List<RPGTalentTree> allTalentTrees;
        public List<RPGBonus> allBonuses;

        public List<RPGTask> allTasks;
        public List<RPGQuest> allQuests;
        public List<RPGWorldPosition> allWorldPositions;
        public List<RPGResourceNode> allResourceNodes;
        public List<RPGGameScene> allGameScenes;

        public List<RPGAbilityRankData> allAbilityRanks;
        public List<RPGCraftingRecipeRankData> allRecipeRanks;
        public List<RPGResourceNodeRankData> allResourceNodeRanks;
        public List<RPGBonusRankDATA> allBonusRankData;

        public bool isInGame;
    
        public static RPGBuilderEssentials Instance => instance;

        private void LoadDATA()
        {
            allAbilities = Resources.LoadAll<RPGAbility>("THMSV/RPGBuilderData/Abilities").ToList();
            allEffects = Resources.LoadAll<RPGEffect>("THMSV/RPGBuilderData/Effects").ToList();
            allNPCs = Resources.LoadAll<RPGNpc>("THMSV/RPGBuilderData/NPCs").ToList();
            allStats = Resources.LoadAll<RPGStat>("THMSV/RPGBuilderData/Stats").ToList();
            allTreePoints = Resources.LoadAll<RPGTreePoint>("THMSV/RPGBuilderData/TreePoints").ToList();

            allItems = Resources.LoadAll<RPGItem>("THMSV/RPGBuilderData/Items").ToList();
            allSkills = Resources.LoadAll<RPGSkill>("THMSV/RPGBuilderData/Skills").ToList();
            allLevelTemplates = Resources.LoadAll<RPGLevelsTemplate>("THMSV/RPGBuilderData/LevelsTemplate").ToList();
            allRaces = Resources.LoadAll<RPGRace>("THMSV/RPGBuilderData/Races").ToList();
            allClasses = Resources.LoadAll<RPGClass>("THMSV/RPGBuilderData/Classes").ToList();
            allLootTables = Resources.LoadAll<RPGLootTable>("THMSV/RPGBuilderData/LootTables").ToList();
            allMerchantTables = Resources.LoadAll<RPGMerchantTable>("THMSV/RPGBuilderData/MerchantTables").ToList();
            allCurrencies = Resources.LoadAll<RPGCurrency>("THMSV/RPGBuilderData/Currencies").ToList();
            allCraftingRecipes = Resources.LoadAll<RPGCraftingRecipe>("THMSV/RPGBuilderData/CraftingRecipes").ToList();
            allCraftingStation = Resources.LoadAll<RPGCraftingStation>("THMSV/RPGBuilderData/CraftingStation").ToList();
            allTalentTrees = Resources.LoadAll<RPGTalentTree>("THMSV/RPGBuilderData/TalentTrees").ToList();
            allBonuses = Resources.LoadAll<RPGBonus>("THMSV/RPGBuilderData/Bonuses").ToList();

            allTasks = Resources.LoadAll<RPGTask>("THMSV/RPGBuilderData/Tasks").ToList();
            allQuests = Resources.LoadAll<RPGQuest>("THMSV/RPGBuilderData/Quests").ToList();
            allWorldPositions = Resources.LoadAll<RPGWorldPosition>("THMSV/RPGBuilderData/WorldPositions").ToList();
            allResourceNodes = Resources.LoadAll<RPGResourceNode>("THMSV/RPGBuilderData/ResourceNodes").ToList();
            allGameScenes = Resources.LoadAll<RPGGameScene>("THMSV/RPGBuilderData/GameScenes").ToList();

            allAbilityRanks = Resources.LoadAll<RPGAbilityRankData>("THMSV/RPGBuilderData/AbilityRankData").ToList();
            allRecipeRanks = Resources.LoadAll<RPGCraftingRecipeRankData>("THMSV/RPGBuilderData/CraftingRecipeRankData").ToList();
            allResourceNodeRanks = Resources.LoadAll<RPGResourceNodeRankData>("THMSV/RPGBuilderData/ResourceNodeRankData").ToList();
            allBonusRankData = Resources.LoadAll<RPGBonusRankDATA>("THMSV/RPGBuilderData/BonusRankData").ToList();

            itemSettings = Resources.Load<RPGItemDATA>("THMSV/RPGBuilderData/Settings/ItemSettings");
            generalSettings = Resources.Load<RPGGeneralDATA>("THMSV/RPGBuilderData/Settings/GeneralSettings");
            combatSettings = Resources.Load<RPGCombatDATA>("THMSV/RPGBuilderData/Settings/CombatSettings");
        }

        private void Start()
        {
            if (instance != null) return;
            instance = this;

            mainGameCanvas.enabled = false;
            LoadDATA();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    
        public Scene getCurrentScene()
        {
            return SceneManager.GetActiveScene();
        }

        public void TeleportToGameScene(int GameSceneID, Vector3 teleportPosition)
        {
            var gameSceneREF = RPGBuilderUtilities.GetGameSceneFromID(GameSceneID);
            CharacterData.Instance.position = teleportPosition;
            SceneManager.LoadScene(gameSceneREF._name);
        }

        public void HandleDATAReset ()
        {
            InventoryManager.Instance.bags[0].slots.Clear();

            CombatManager.playerCombatInfo = null;
            CombatManager.Instance.currentTarget = null;
            CombatManager.Instance.ResetPlayerTarget();
            CombatManager.Instance.allCombatNodes.Clear();

            CustomInputManager.Instance.InitializationCompleted = false;

            CursorManager.Instance.ResetCursor();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            QuestTrackerDisplayManager.Instance.trackedQuest.Clear();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "MainMenu")
            {
                CharacterData.Instance.currentGameSceneID = RPGBuilderUtilities.GetGameSceneFromName(scene.name).ID;
                mainGameCanvas.enabled = true;
                GameObject playerCharacter;
                playerCharacter = Instantiate(CharacterData.Instance.gender == RPGRace.RACE_GENDER.Male ? RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID).malePrefab : RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID).femalePrefab, CharacterData.Instance.position, Quaternion.identity);

                CombatManager.playerCombatInfo = playerCharacter.GetComponent<CombatNode>();
                CombatManager.playerCombatInfo.currentAutoAttackAbilityID = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID).autoAttackAbilityID;
                CombatManager.Instance.allCombatNodes.Add(CombatManager.playerCombatInfo);
                CombatManager.playerCombatInfo.InitStats();
                CombatManager.Instance.allGraveyards.Clear();
                CombatManager.Instance.allGraveyards = FindObjectsOfType<GraveyardHandler>().ToList();

                ScreenSpaceNameplates.Instance.InitCamera();
                ScreenSpaceWorldDroppedItems.Instance.InitCamera();
                
                PlayerInfoDisplayManager.Instance.Init();

                ActionBarDisplayManager.Instance.InitAbilityActionBar();
                if (CharacterData.Instance.actionBarAbilities.Count == 0)
                {
                    for (var i = 0; i < combatSettings.actionBarSlots; i++)
                        CharacterData.Instance.actionBarAbilities.Add(new CharacterData.ActionBarAbilitiesDATA());
                }
                else
                {
                    if (combatSettings.actionBarSlots > CharacterData.Instance.actionBarAbilities.Count)
                    {
                        int diff = combatSettings.actionBarSlots - CharacterData.Instance.actionBarAbilities.Count;
                        for (int i = 0; i < diff; i++)
                        {
                            CharacterData.Instance.actionBarAbilities.Add(new CharacterData.ActionBarAbilitiesDATA());
                        }
                    }
                    else if(combatSettings.actionBarSlots < CharacterData.Instance.actionBarAbilities.Count)
                    {
                        int diff = CharacterData.Instance.actionBarAbilities.Count - combatSettings.actionBarSlots;
                        for (int i = 0; i < diff; i++)
                        {
                            CharacterData.Instance.actionBarAbilities.RemoveAt(CharacterData.Instance.actionBarAbilities.Count-1);
                        }  
                    }

                    for (var i = 0; i < combatSettings.actionBarSlots; i++)
                    {
                        if (CharacterData.Instance.actionBarAbilities[i].curAbID == -1 ||
                            !RPGBuilderUtilities.isAbilityKnown(CharacterData.Instance.actionBarAbilities[i].curAbID))
                            continue;
                        ActionBarDisplayManager.Instance.abilitySlots[i].curAb =
                            RPGBuilderUtilities.GetAbilityFromID(CharacterData.Instance.actionBarAbilities[i].curAbID);
                        CombatManager.playerCombatInfo.abilitiesData[i].curAbilityID =
                            CharacterData.Instance.actionBarAbilities[i].curAbID;
                        CombatManager.playerCombatInfo.abilitiesData[i].currentAbility =
                            RPGBuilderUtilities.GetAbilityFromID(CharacterData.Instance.actionBarAbilities[i].curAbID);
                    }
                }

                if(CharacterData.Instance.actionBarItems.Count == 0)
                    for (var i = 0; i < ActionBarDisplayManager.Instance.itemSlots.Length; i++) CharacterData.Instance.actionBarItems.Add(new CharacterData.ActionBarItemsDATA());
                else
                    for (var i = 0; i < ActionBarDisplayManager.Instance.itemSlots.Length; i++)
                    {
                        if (CharacterData.Instance.actionBarItems[i].curItemID == -1) continue;
                        ActionBarDisplayManager.Instance.itemSlots[i].curItem = RPGBuilderUtilities.GetItemFromID(CharacterData.Instance.actionBarItems[i].curItemID);
                    }

                CustomInputManager.Instance.InitKEYS();
                InventoryManager.Instance.InitInventory();
                ActionBarDisplayManager.Instance.InitializeAbilitySlots();
                ActionBarDisplayManager.Instance.InitializeItemSlots();

                CharacterEventsManager.Instance.SceneEntered(scene.name);
                BonusManager.Instance.ResetAllOnBonuses();
                BonusManager.Instance.InitBonuses();

                MinimapDisplayManager.Instance.InitializeMinimap(RPGBuilderUtilities.GetGameSceneFromName(scene.name));
                Toolbar.Instance.InitToolbar();
                
                if (!CharacterData.Instance.created)
                {
                    InitializeNewCharacter();
                }
                else
                {
                    InventoryManager.Instance.InitEquippedItems();
                }
                
                
                isInGame = true;
            } else
            {
                isInGame = false;
            }
        }


       
        
        private void InitializeNewCharacter()
        {
            CharacterData.Instance.created = true;

            RPGRace raceREF = RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID);
            foreach (var t in raceREF.startItems)
            {
                InventoryManager.Instance.AddItem(t.itemID, t.count, t.equipped, RPGBuilderUtilities.GenerateRandomItemStats(t.itemID, t.equipped, true));
            }

            RPGClass classREF = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
            foreach (var t in classREF.startItems)
            {
                InventoryManager.Instance.AddItem(t.itemID, t.count, t.equipped, RPGBuilderUtilities.GenerateRandomItemStats(t.itemID, t.equipped, true));
            }

            foreach (var t1 in CharacterData.Instance.skillsDATA)
            {
                RPGSkill skillREF = RPGBuilderUtilities.GetSkillFromID(t1.skillID);
                foreach (var t in skillREF.startItems)
                {
                    InventoryManager.Instance.AddItem(t.itemID, t.count, t.equipped, RPGBuilderUtilities.GenerateRandomItemStats(t.itemID, t.equipped, true));
                }
            }

            List<RPGAbility> knownAbilities = new List<RPGAbility>();
            List<int> slots = new List<int>();
            int curAb = 0;
            foreach (var t1 in CharacterData.Instance.talentTrees.SelectMany(t => t.nodes.Where(t1 => t1.nodeData.nodeType == RPGTalentTree.TalentTreeNodeType.ability).Where(t1 => t1.known)))
            {
                knownAbilities.Add(RPGBuilderUtilities.GetAbilityFromID(t1.nodeData.abilityID));
                slots.Add(curAb);
                curAb++;
            }

            for (int i = 0; i < knownAbilities.Count; i++)
            {
                ActionBarDisplayManager.Instance.SetAbilityToSlot(knownAbilities[i], slots[i]);
            }

        }

        private void Update()
        {
            if (generalSettings == null || CombatManager.playerCombatInfo == null) return;
            if (!generalSettings.automaticSave) return;
            if (!(Time.time >= nextAutomaticSave)) return;
            nextAutomaticSave += generalSettings.automaticSaveDelay;
            RPGBuilderJsonSaver.SaveCharacterData(CharacterData.Instance.CharacterName, CharacterData.Instance);
            RPGBuilderJsonSaver.SaveRandomItemsData(RandomizedItemsData.Instance);
        }

        private void OnApplicationQuit()
        {
            if (generalSettings == null || !generalSettings.automaticSaveOnQuit || CombatManager.playerCombatInfo == null) return;
            RPGBuilderJsonSaver.SaveCharacterData(CharacterData.Instance.CharacterName, CharacterData.Instance);
            RPGBuilderJsonSaver.SaveRandomItemsData(RandomizedItemsData.Instance);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

    }
}
