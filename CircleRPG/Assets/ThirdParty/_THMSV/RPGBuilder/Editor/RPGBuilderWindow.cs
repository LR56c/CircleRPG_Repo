using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using System.Reflection;
using System;
using System.IO;
using THMSV.RPGBuilder;
using THMSV.RPGBuilder.CombatVisuals;
using THMSV.RPGBuilder.Managers;

public class RPGBuilderWindow : EditorWindow
{
    
    private RPGBuilderEditorDATA editorDATA;
    private RPGBuilderEditorDATA.ThemeTypes cachedTheme;
    private GUISkin skin;

    private List<RPGAbility> allAbilities = new List<RPGAbility>();
    private RPGAbility currentlyViewedAbility;
    private List<RPGAbilityRankData> allAbilityRanks = new List<RPGAbilityRankData>();
    private List<RPGEffect> allEffects = new List<RPGEffect>();
    private RPGEffect currentlyViewedEffect;
    private List<RPGNpc> allNPCs = new List<RPGNpc>();
    private RPGNpc currentlyViewedNPC;
    private List<RPGStat> allStats = new List<RPGStat>();
    private RPGStat currentlyViewedStat;
    private List<RPGTreePoint> allTreePoints = new List<RPGTreePoint>();
    private RPGTreePoint currentlyViewedTreePoint;


    private List<RPGItem> allItems = new List<RPGItem>();
    private RPGItem currentlyViewedItem;
    private List<RPGSkill> allSkills = new List<RPGSkill>();
    private RPGSkill currentlyViewedSkill;
    private List<RPGLevelsTemplate> allLevelsTemplate = new List<RPGLevelsTemplate>();
    private RPGLevelsTemplate currentlyViewedLevelTemplate;
    private List<RPGRace> allRaces = new List<RPGRace>();
    private RPGRace currentlyViewedRace;
    private List<RPGClass> allClasses = new List<RPGClass>();
    private RPGClass currentlyViewedClass;
    private List<RPGLootTable> allLootTables = new List<RPGLootTable>();
    private RPGLootTable currentlyViewedLootTable;
    private List<RPGMerchantTable> allMerchantTables = new List<RPGMerchantTable>();
    private RPGMerchantTable currentlyViewedMerchantTable;
    private List<RPGCurrency> allCurrencies = new List<RPGCurrency>();
    private RPGCurrency currentlyViewedCurrency;
    private List<RPGCraftingRecipe> allCraftingRecipes = new List<RPGCraftingRecipe>();
    private RPGCraftingRecipe currentlyViewedCraftingRecipe;
    private List<RPGCraftingRecipeRankData> allCraftingRecipeRanks = new List<RPGCraftingRecipeRankData>();
    private List<RPGCraftingStation> allCraftingStations = new List<RPGCraftingStation>();
    private RPGCraftingStation currentlyViewedCraftingStation;
    private List<RPGTalentTree> allTalentTrees = new List<RPGTalentTree>();
    private RPGTalentTree currentlyViewedTalentTree;
    private List<RPGBonus> allBonuses = new List<RPGBonus>();
    private RPGBonus currentlyViewedBonus;
    private List<RPGBonusRankDATA> allBonusRanks = new List<RPGBonusRankDATA>();

    private List<RPGTask> allTasks = new List<RPGTask>();
    private RPGTask currentlyViewedTask;
    private List<RPGQuest> allQuests = new List<RPGQuest>();
    private RPGQuest currentlyViewedQuest;
    private List<RPGWorldPosition> allWorldPositions = new List<RPGWorldPosition>();
    private RPGWorldPosition currentlyViewedWorldPosition;
    private List<RPGResourceNode> allResourceNodes = new List<RPGResourceNode>();
    private RPGResourceNode currentlyViewedResourceNode;
    private List<RPGResourceNodeRankData> allResourceNodeRanks = new List<RPGResourceNodeRankData>();
    private RPGGameScene currentlyViewedGameScene;
    private List<RPGGameScene> allGameScenes = new List<RPGGameScene>();

    private RPGCombatDATA combatSettings;
    private RPGItemDATA itemSettings;
    private RPGGeneralDATA generalSettings;

    private GUIContent nameGUIContent = new GUIContent("Name", "The name of this element");
    private GUIContent fileNameGUIContent = new GUIContent("File Name", "The name of the file inside your Unity Project");
    private GUIContent displayNameGUIContent = new GUIContent("Display Name", "The name that will be displayed in the game UI");
    private GUIContent descriptionGUIContent = new GUIContent("Description", "The description of this element, displayed in the game UI");

    private class elementListDATA
    {
        public string name;
        public Texture texture;
        public bool showIcon;
    }

    private List<elementListDATA> curElementList = new List<elementListDATA>();

    public enum AssetType
    {
        Ability,
        Effect,
        Item,
        NPC,
        Stat,
        CombatSettings,
        ItemSettings,
        Skill,
        LevelTemplate,
        Race,
        Class,
        GeneralSettings,
        TalentTree,
        TreePoint,
        LootTable,
        WorldPosition,
        MerchantTable,
        Currency,
        Task,
        Quest,
        CraftingRecipe,
        CraftingStation,
        ResourceNode,
        Bonus,
        GameScene
    }

    public enum CategorySelectedType
    {
        Combat,
        General,
        World,
        Settings,
        Partners
    }
    public CategorySelectedType currentCategorySelected;
    public int curSubCategorySelected = 0;


    public enum CombatSubCategorySelectedType
    {
        Ability,
        Effect,
        NPCs,
        Stat,
        TreePoint
    }
    public CombatSubCategorySelectedType combatSubCurrentCategorySelected;

    public enum GeneralSubCategorySelectedType
    {
        Item,
        Skill,
        LevelTemplate,
        Race,
        Class,
        LootTable,
        MerchantTable,
        Currency,
        CraftingRecipe,
        CraftingStation,
        TalentTree,
        Bonus
    }
    public GeneralSubCategorySelectedType generalSubCurrentCategorySelected;

    public enum WorldSubCategorySelectedType
    {
        Task,
        Quest,
        WorldPosition,
        ResourceNode,
        GameScene
    }
    public WorldSubCategorySelectedType worldSubCurrentCategorySelected;

    public enum SettingsSubCategorySelectedType
    {
        General,
        Combat,
        Item,
        Editor
    }
    public SettingsSubCategorySelectedType settingsSubCurrentCategorySelected;
    
    public enum PartnersSubCategorySelectedType
    {
        PolytopeStudio,
        Cafofo,
        GabrielAguiar,
        RDR,
        TitanForge,
        PONETI,
        MalbersAnimation
    }
    public PartnersSubCategorySelectedType partnersSubCurrentCategorySelected;

    public int curViewElementIndex;

    public bool showSearch;
    private string curSearchText = "", curCraftingTreeSearchText = "";
    private Vector2 searchScrollPosition;
    private Vector2 viewScrollPosition;

    private string cachedFileName = "";
    private string cachedElementName = "";

    private static RPGBuilderWindow curwindow;

    public Vector3 CursorPOS = Vector3.zero;
    
    public List<RPGAbility.rankDATA> temporaryAbRankList = new List<RPGAbility.rankDATA>();
    public List<RPGCraftingRecipe.rankDATA> temporaryRecipeRankList = new List<RPGCraftingRecipe.rankDATA>();
    public List<RPGResourceNode.rankDATA> temporaryResourceNodeRankList = new List<RPGResourceNode.rankDATA>();
    public List<RPGBonus.rankDATA> temporaryBonusRankList = new List<RPGBonus.rankDATA>();

    [MenuItem("THMSV/RPG Builder")]
    private static void OpenWindow()
    {
        var window = (RPGBuilderWindow)GetWindow(typeof(RPGBuilderWindow), false, "RPG Builder");
        window.minSize = new Vector2(1050, 550);
        window.maxSize = new Vector2(1050, 550);
        GUI.contentColor = Color.white;
        window.Show();

        curwindow = window;
    }

    private void InitALLData()
    {
        LoadAbilities();
        LoadMerchantTables();
        LoadEffects();
        LoadNPCs();
        LoadItems();
        LoadStats();
        LoadSettings();
        LoadSkills();
        LoadLevelsTemplate();
        LoadRaces();
        LoadClasses();
        LoadTalentTrees();
        LoadTreePoints();
        LoadLootTables();
        LoadWorldPositions();
        LoadCurrencies();
        LoadTasks();
        LoadQuests();
        LoadCraftingRecipes();
        LoadCraftingStations();
        LoadResourceNodes();
        LoadBonuses();
        LoadAbilityRankData();
        LoadRecipeRankData();
        LoadResourceNodeRankData();
        LoadBonusRankData();
        LoadGameScenes();
    }

    private void OnEnable()
    {
        skin = Resources.Load<GUISkin>("THMSV/RPGBuilderEditor/GUIStyles/RPGBuilderSkin");
        editorDATA = Resources.Load<RPGBuilderEditorDATA>("THMSV/RPGBuilderEditor/Data/RPGBuilderEditorData");
        cachedTheme = editorDATA.curEditorTheme;
        InitALLData();
        if (currentCategorySelected == CategorySelectedType.Combat && curSubCategorySelected == 0) SelectCategory("Combat");
    }
    
    void  OnInspectorUpdate()
    {
        if (editorDATA.increasedEditorUpdate)
        {
            Repaint();
        }
    }

    private Rect getContainerRect(string containerName)
    {
        for (var i = 0; i < editorDATA.containersData.Length; i++)
            if (editorDATA.containersData[i].containerName == containerName) return editorDATA.containersData[i].containerRect;
        Debug.LogError("RECT NOT FOUND");
        return new Rect();
    }

    private void OnGUI()
    {
        DrawContainers();
        DrawCategories();
        DrawSubCategories();
        DrawView();
    }

    Texture2D getThemeTexture(int bg)
    {
        if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Dark)
        {
            switch (bg)
            {
                case 1:
                    return editorDATA.DarkThemeBackground1;
                case 2:
                    return editorDATA.DarkThemeBackground2;
                default:
                    return null;
                
            }
        }
        else
        {
            switch (bg)
            {
                case 1:
                    return editorDATA.LightThemeBackground1;
                case 2:
                    return editorDATA.LightThemeBackground2;
                default:
                    return null;
            }
        }
    }

    private void DrawContainers()
    {
        for (var i = 0; i < editorDATA.containersData.Length; i++)
            if (editorDATA.containersData[i].Draw)
            {
                var newRect = editorDATA.containersData[i].containerRect;
                if (editorDATA.containersData[i].widthScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    newRect.width = Screen.width;
                else if (editorDATA.containersData[i].widthScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    newRect.width = Screen.height;
                else
                    newRect.width = editorDATA.containersData[i].containerRect.width;

                if (editorDATA.containersData[i].heightScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    newRect.height = Screen.width;
                else if (editorDATA.containersData[i].heightScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    newRect.height = Screen.height;
                else
                    newRect.height = editorDATA.containersData[i].containerRect.height;
                
                GUI.DrawTexture(newRect, getThemeTexture(2));
                
            }
    }

    private void DrawCategories()
    {
        var containerRect = getContainerRect("Categories");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.CategoriesLeftMargin, containerRect.y + +editorDATA.CategoriesTopMargin, containerRect.width, containerRect.height));
        
        for (var i = 0; i < editorDATA.categoriesData.Length; i++)
            if (editorDATA.categoriesData[i].Active)
            {
                var selected = false;
                var buttonStyle = skin.GetStyle("ModuleButton");
                if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Dark)
                {
                    buttonStyle.normal.textColor = Color.white;
                } else if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Light)
                {
                    buttonStyle.normal.textColor = Color.black;
                }
                if (editorDATA.categoriesData[i].CategoryName == editorDATA.categoriesData[(int)currentCategorySelected].CategoryName) selected = true;
                if (selected)
                {
                    buttonStyle = skin.GetStyle("ModuleButtonSelected");
                    if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Dark)
                    {
                        buttonStyle.normal.textColor = Color.black;
                        buttonStyle.hover.textColor = Color.black;
                    } else if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Light)
                    {
                        buttonStyle.normal.textColor = Color.white;
                        buttonStyle.hover.textColor = Color.white;
                    }
                }
                
                
                if (GUILayout.Button(editorDATA.categoriesData[i].CategoryName, buttonStyle, GUILayout.Width(editorDATA.CategoriesX), GUILayout.Height(editorDATA.CategoriesY))) SelectCategory(editorDATA.categoriesData[i].CategoryName);
            }

        GUILayout.EndArea();
    }

    private void DrawSubCategories()
    {
        var containerRect = getContainerRect("SubCategories");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));
        
        for (var i = 0; i < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData.Length; i++)
            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[i].Active)
            {
                var selected = false;
                var buttonStyle = skin.GetStyle("ModuleButton");
                
                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[i].SubCategoryName == editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[curSubCategorySelected].SubCategoryName) selected = true;
                if (selected)
                {
                    buttonStyle = skin.GetStyle("ModuleButtonSelected");
                }
                if (GUILayout.Button(editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[i].SubCategoryName, buttonStyle, GUILayout.Width(editorDATA.SubCategoriesX), GUILayout.Height(editorDATA.SubCategoriesY))) SelectSubCategory(i);
            }

        GUILayout.EndArea();
    }

    private void DrawView()
    {
        switch (editorDATA.categoriesData[(int)currentCategorySelected].CategoryName)
        {
            case "Combat":
                DrawCombatView();
                break;
            case "General":
                DrawGeneralView();
                break;
            case "World":
                DrawWorldView();
                break;
            case "Settings":
                DrawSettingsView();
                break;
            case "Partners":
                DrawPartnersView();
                break;
        }
    }

    private void DrawCombatView()
    {
        switch (combatSubCurrentCategorySelected)
        {
            case CombatSubCategorySelectedType.Ability:
                DrawAbilityView();
                break;

            case CombatSubCategorySelectedType.Effect:
                DrawEffectView();
                break;

            case CombatSubCategorySelectedType.NPCs:
                DrawNPCsView();
                break;

            case CombatSubCategorySelectedType.Stat:
                DrawStatView();
                break;

            case CombatSubCategorySelectedType.TreePoint:
                DrawTreePointView();
                break;
        }
    }

    private void DrawActionButtons(AssetType assetType, Rect containerRect)
    {
        float leftMargin = 0;
        if (assetType == AssetType.CombatSettings || assetType == AssetType.ItemSettings ||
            assetType == AssetType.GeneralSettings)
        {
            leftMargin = 50;
        }

        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.actionButtonsLeftMargin + leftMargin, containerRect.y + editorDATA.actionButtonsTopMargin, containerRect.width, containerRect.height));
        GUILayout.BeginHorizontal();

        GUIStyle buttonStyle = skin.GetStyle("ActionButtons");

        if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Dark)
        {
            buttonStyle.normal.background = editorDATA.DarkThemeBackground1;
            buttonStyle.normal.textColor = Color.white;
        } else if (editorDATA.curEditorTheme == RPGBuilderEditorDATA.ThemeTypes.Light)
        {
            buttonStyle.normal.background = editorDATA.LightThemeBackground1;
            buttonStyle.normal.textColor = Color.black;
        }

        if (GUILayout.Button("Save", buttonStyle, GUILayout.Width(editorDATA.actionButtonsX), GUILayout.Height(editorDATA.actionButtonsY))) Save(assetType);

        if (assetType != AssetType.CombatSettings && assetType != AssetType.ItemSettings && assetType != AssetType.GeneralSettings)
        {
            if (GUILayout.Button("New", buttonStyle, GUILayout.Width(editorDATA.actionButtonsX), GUILayout.Height(editorDATA.actionButtonsY))) CreateNew(assetType);
            if (GUILayout.Button("Duplicate", buttonStyle, GUILayout.Width(editorDATA.actionButtonsX), GUILayout.Height(editorDATA.actionButtonsY))) Duplicate(assetType);
            if (GUILayout.Button("Delete", buttonStyle, GUILayout.Width(editorDATA.actionButtonsX), GUILayout.Height(editorDATA.actionButtonsY)))
                if (EditorUtility.DisplayDialog("Confirm DELETE", "Are you sure you want to delete this " + assetType.ToString(), "YES", "Cancel")) Delete(assetType);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void viewInit(int x, Rect containerRect, Rect subContainerRect, string moduleType)
    {
        switch (moduleType)
        {
            case "combat":
                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.width = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.width = Screen.height;
                else
                    subContainerRect.width = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.width;

                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.height = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.height = Screen.height;
                else
                    subContainerRect.height = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.height;
                GUI.DrawTexture(subContainerRect, getThemeTexture(1));

                GUILayout.BeginArea(new Rect(editorDATA.searchSlotLeftMargin, containerRect.y + editorDATA.searchSlotTopMargin, containerRect.width, containerRect.height));
                break;
            case "general":
                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.width = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.width = Screen.height;
                else
                    subContainerRect.width = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width;

                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.height = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.height = Screen.height;
                else
                    subContainerRect.height = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height;
                GUI.DrawTexture(subContainerRect, getThemeTexture(1));

                GUILayout.BeginArea(new Rect(editorDATA.searchSlotLeftMargin, containerRect.y + editorDATA.searchSlotTopMargin, containerRect.width, containerRect.height));
                break;
            case "world":
                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.width = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.width = Screen.height;
                else
                    subContainerRect.width = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.width;

                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.height = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.height = Screen.height;
                else
                    subContainerRect.height = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.height;
                GUI.DrawTexture(subContainerRect, getThemeTexture(1));

                GUILayout.BeginArea(new Rect(editorDATA.searchSlotLeftMargin, containerRect.y + editorDATA.searchSlotTopMargin, containerRect.width, containerRect.height));
                break;
            case "settings":
                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.width = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.width = Screen.height;
                else
                    subContainerRect.width = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.width;

                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.height = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.height = Screen.height;
                else
                    subContainerRect.height = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.height;
                GUI.DrawTexture(subContainerRect, getThemeTexture(1));

                GUILayout.BeginArea(new Rect(editorDATA.searchSlotLeftMargin, containerRect.y + editorDATA.searchSlotTopMargin, containerRect.width, containerRect.height));
                break;
            case "partners":
                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.width = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].widthScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.width = Screen.height;
                else
                    subContainerRect.width = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width;

                if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.width)
                    subContainerRect.height = Screen.width;
                else if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].heightScalingType == RPGBuilderEditorDATA.ScalingType.height)
                    subContainerRect.height = Screen.height;
                else
                    subContainerRect.height = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height;
                GUI.DrawTexture(subContainerRect, getThemeTexture(1));

                GUILayout.BeginArea(new Rect(editorDATA.searchSlotLeftMargin, containerRect.y + editorDATA.searchSlotTopMargin, containerRect.width, containerRect.height));
                break;
        }
    }

    private void DrawElementList (List<elementListDATA> elementLIST, AssetType assetType)
    {
        GUILayout.BeginHorizontal();
        curSearchText = GUILayout.TextArea(curSearchText, skin.GetStyle("SearchBar"), GUILayout.Width(220), GUILayout.Height(30));

        GUILayout.Box(editorDATA.searchIcon, skin.GetStyle("CustomImage"), GUILayout.Width(30), GUILayout.Height(30));
        GUILayout.EndHorizontal();

        searchScrollPosition = GUILayout.BeginScrollView(searchScrollPosition, GUILayout.Width(editorDATA.searchViewX), GUILayout.Height(editorDATA.searchViewY));

        for (var i = 0; i < elementLIST.Count; i++)
        {
            var tempSearchText = "";
            if (curSearchText != null && curSearchText.Length > 0) tempSearchText = curSearchText.ToLower();
            var abName = elementLIST[i].name;
            abName = abName.ToLower();
            if (curSearchText == null || curSearchText.Length < 1 || abName.Contains(tempSearchText))
            {
                var selected = false;
                var buttonStyle = skin.GetStyle("ModuleButton");
                if (curViewElementIndex == i) selected = true;
                if (selected)
                {
                    buttonStyle = skin.GetStyle("ModuleButtonSelected");
                }
                
                var abnamestring = elementLIST[i].name;
                if (abnamestring.Length > 23)
                {
                    abnamestring = abnamestring.Remove(22);
                    abnamestring += "...";
                }

                var cont = new GUIContent();
                if (elementLIST[i].texture != null)
                    cont.image = elementLIST[i].texture;
                else
                    if(elementLIST[i].showIcon) cont.image = editorDATA.defaultElementIcon.texture;
                
                cont.text = abnamestring;
                if (GUILayout.Button(cont, buttonStyle, GUILayout.Width(editorDATA.searchSlotX), GUILayout.Height(editorDATA.searchSlotY)))
                    switch (assetType)
                    {
                        case AssetType.Ability:
                            SelectAbility(i);
                            break;
                        case AssetType.Bonus:
                            SelectBonus(i);
                            break;
                        case AssetType.Class:
                            SelectClass(i);
                            break;
                        case AssetType.CraftingRecipe:
                            SelectCraftingRecipe(i);
                            break;
                        case AssetType.CraftingStation:
                            SelectCraftingStation(i);
                            break;
                        case AssetType.Currency:
                            SelectCurrency(i);
                            break;
                        case AssetType.Effect:
                            SelectEffect(i);
                            break;
                        case AssetType.Item:
                            SelectItem(i);
                            break;
                        case AssetType.LevelTemplate:
                            SelectLevelTemplate(i);
                            break;
                        case AssetType.LootTable:
                            SelectLootTable(i);
                            break;
                        case AssetType.MerchantTable:
                            SelectMerchantTable(i);
                            break;
                        case AssetType.NPC:
                            SelectNPC(i);
                            break;
                        case AssetType.Quest:
                            SelectQuest(i);
                            break;
                        case AssetType.Race:
                            SelectRace(i);
                            break;
                        case AssetType.ResourceNode:
                            SelectResourceNode(i);
                            break;
                        case AssetType.Skill:
                            SelectSkill(i);
                            break;
                        case AssetType.Stat:
                            SelectStat(i);
                            break;
                        case AssetType.TalentTree:
                            SelectTalentTree(i);
                            break;
                        case AssetType.Task:
                            SelectTask(i);
                            break;
                        case AssetType.TreePoint:
                            SelectTreePoint(i);
                            break;
                        case AssetType.WorldPosition:
                            SelectWorldPosition(i);
                            break;
                        case AssetType.GameScene:
                            SelectGameScene(i);
                            break;
                    }
            }
        }

        GUILayout.EndScrollView();
    }

    private void DrawAbilityView()
    {
        if (currentlyViewedAbility == null)
        {
            if (allAbilities.Count == 0)
            {
                CreateNew(AssetType.Ability);
                return;
            }
            currentlyViewedAbility = Instantiate(allAbilities[0]) as RPGAbility;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;

        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "combat");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allAbilities.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allAbilities[i]._name;
                            newElementDATA.showIcon = true;
                            if (allAbilities[i].icon != null) newElementDATA.texture = allAbilities[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }

                        DrawElementList(curElementList, AssetType.Ability);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedAbility.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedAbility.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedAbility.ID = EditorGUILayout.IntField(currentlyViewedAbility.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(nameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedAbility._name = GUILayout.TextField(currentlyViewedAbility._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedAbility.displayName = GUILayout.TextField(currentlyViewedAbility.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fileNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedAbility._fileName = GUILayout.TextField("RPG_ABILITY_" + currentlyViewedAbility._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent("Known", "Is this ability known by default by your character?"), GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedAbility.learnedByDefault = EditorGUILayout.Toggle(currentlyViewedAbility.learnedByDefault);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(new GUIContent("Is Player Auto Attack?", "Is this ability used as an auto attack for the player?"), GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedAbility.isPlayerAutoAttack = EditorGUILayout.Toggle(currentlyViewedAbility.isPlayerAutoAttack);
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("RANKS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Rank", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            var newRankData = new RPGAbilityRankData();
                            var newRankDataElement = new RPGAbility.rankDATA();
                            newRankDataElement.rankID = -1;
                            newRankDataElement.rankREF = newRankData;
                            temporaryAbRankList.Add(newRankDataElement);
                        }
                        GUILayout.Space(10);

                        if (temporaryAbRankList.Count > 0)
                            if (GUILayout.Button("- Remove Rank", skin.GetStyle("RemoveAbilityRankButton"), GUILayout.Width(440), GUILayout.Height(25)))
                            {
                                var rankNumber = temporaryAbRankList.Count + 1;
                                temporaryAbRankList.RemoveAt(temporaryAbRankList.Count - 1);
                                return;
                            }

                        GUILayout.Space(10);

                        for (var i = 0; i < temporaryAbRankList.Count; i++)
                        {
                            var rankNbr = i + 1;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Rank: " + rankNbr, skin.GetStyle("AbilityRankButton"), GUILayout.Width(150), GUILayout.Height(25))) temporaryAbRankList[i].ShowedInEditor = !temporaryAbRankList[i].ShowedInEditor;
                            if (i > 0)
                            {
                                GUILayout.Space(5);
                                if (GUILayout.Button("Copy Above", skin.GetStyle("AddButton"), GUILayout.Width(225), GUILayout.Height(25))) temporaryAbRankList[i].rankREF.copyData(temporaryAbRankList[i].rankREF, temporaryAbRankList[i - 1].rankREF);
                            }
                            GUILayout.EndHorizontal();

                            if (temporaryAbRankList[i].ShowedInEditor)
                            {

                                GUILayout.Space(10);
                                GUILayout.Label("UNLOCKING SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryAbRankList[i].rankREF.unlockCost = EditorGUILayout.IntField(new GUIContent("Unlock Cost", "The cost to unlock this ability inside your combat trees"), temporaryAbRankList[i].rankREF.unlockCost, GUILayout.Width(430));

                                GUILayout.Space(10);
                                GUILayout.Label("USE REQUIREMENTS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Requirement", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryAbRankList[i].rankREF.useRequirements.Add(new RequirementsManager.AbilityUseRequirementDATA());

                                ScriptableObject scriptableObj = temporaryAbRankList[i].rankREF;
                                var serialObj = new SerializedObject(scriptableObj);
                                var ThisList = serialObj.FindProperty("useRequirements");
                                temporaryAbRankList[i].rankREF.useRequirements = GetTargetObjectOfProperty(ThisList) as List<RequirementsManager.AbilityUseRequirementDATA>;

                                for (var a = 0; a < temporaryAbRankList[i].rankREF.useRequirements.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                                    temporaryAbRankList[i].rankREF.useRequirements[a].requirementType = (RequirementsManager.AbilityUseRequirementType)EditorGUILayout.EnumPopup(temporaryAbRankList[i].rankREF.useRequirements[a].requirementType, GUILayout.Width(250));
                                    GUILayout.Space(10);
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryAbRankList[i].rankREF.useRequirements.RemoveAt(a);
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginVertical();

                                    if (temporaryAbRankList[i].rankREF.useRequirements.Count > 0)
                                    {
                                        if (temporaryAbRankList[i].rankREF.useRequirements[a].requirementType == RequirementsManager.AbilityUseRequirementType.item)
                                        {
                                            temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredREF = (RPGItem)EditorGUILayout.ObjectField("Item:", RPGBuilderUtilities.GetItemFromIDEditor(temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                            temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredCount = EditorGUILayout.IntField("Count:", temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredCount, GUILayout.Width(400));
                                            temporaryAbRankList[i].rankREF.useRequirements[a].consumeItem = EditorGUILayout.Toggle("Consumed", temporaryAbRankList[i].rankREF.useRequirements[a].consumeItem);
                                            if (temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredREF != null)
                                                temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredID = temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredREF.ID;
                                            else
                                                temporaryAbRankList[i].rankREF.useRequirements[a].itemRequiredID = -1;
                                        }
                                        else if (temporaryAbRankList[i].rankREF.useRequirements[a].requirementType == RequirementsManager.AbilityUseRequirementType.statCost)
                                        {
                                            temporaryAbRankList[i].rankREF.useRequirements[a].statCostREF = (RPGStat)EditorGUILayout.ObjectField("Stat:", RPGBuilderUtilities.GetStatFromIDEditor(temporaryAbRankList[i].rankREF.useRequirements[a].statCostID, allStats), typeof(RPGStat), false, GUILayout.Width(400));
                                            temporaryAbRankList[i].rankREF.useRequirements[a].useCost = EditorGUILayout.IntField("Cost:", temporaryAbRankList[i].rankREF.useRequirements[a].useCost, GUILayout.Width(400));
                                            if (temporaryAbRankList[i].rankREF.useRequirements[a].statCostREF != null)
                                                temporaryAbRankList[i].rankREF.useRequirements[a].statCostID = temporaryAbRankList[i].rankREF.useRequirements[a].statCostREF.ID;
                                            else
                                                temporaryAbRankList[i].rankREF.useRequirements[a].statCostID = -1;
                                        }
                                        else if (temporaryAbRankList[i].rankREF.useRequirements[a].requirementType == RequirementsManager.AbilityUseRequirementType.weaponTypeEquipped)
                                        {
                                            var currentWeaponTypeIndex = getIndexFromName("WeaponType", temporaryAbRankList[i].rankREF.useRequirements[a].weaponRequired);
                                            var tempIndex2 = 0;
                                            tempIndex2 = EditorGUILayout.Popup("Weapon Type", currentWeaponTypeIndex, itemSettings.weaponType);
                                            if (itemSettings.weaponType.Length > 0) temporaryAbRankList[i].rankREF.useRequirements[a].weaponRequired = itemSettings.weaponType[tempIndex2];
                                        }
                                    }
                                    EditorGUILayout.EndVertical();

                                    GUILayout.Space(10);
                                }

                                GUILayout.Space(10);
                                GUILayout.Label("ACTIVATION", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryAbRankList[i].rankREF.castTime = EditorGUILayout.FloatField(new GUIContent("Cast Time", "Duration to cast the ability"), temporaryAbRankList[i].rankREF.castTime, GUILayout.Width(430));
                                if(temporaryAbRankList[i].rankREF.castTime > 0) temporaryAbRankList[i].rankREF.castBarVisible = EditorGUILayout.Toggle(new GUIContent("Cast Bar Visible?", "Should this ability display the cast bar during cast time??"), temporaryAbRankList[i].rankREF.castBarVisible, GUILayout.Width(430));
                                temporaryAbRankList[i].rankREF.castInRun = EditorGUILayout.Toggle(new GUIContent("Move and cast?", "Can this ability be casted while moving?"), temporaryAbRankList[i].rankREF.castInRun, GUILayout.Width(430));
                                temporaryAbRankList[i].rankREF.canBeUsedStunned = EditorGUILayout.Toggle(new GUIContent("Stunned and use?", "Can this ability be used while being stunned?"), temporaryAbRankList[i].rankREF.canBeUsedStunned, GUILayout.Width(430));
                                temporaryAbRankList[i].rankREF.channelTime = EditorGUILayout.FloatField(new GUIContent("Channel Time", "The duration of the channel"), temporaryAbRankList[i].rankREF.channelTime, GUILayout.Width(430));
                                temporaryAbRankList[i].rankREF.standTimeDuration = EditorGUILayout.FloatField(new GUIContent("Stand Time", "How long should the caster be locked in place when using this ability?"), temporaryAbRankList[i].rankREF.standTimeDuration);
                                temporaryAbRankList[i].rankREF.canRotateInStandTime = EditorGUILayout.Toggle(new GUIContent("Mob Rotate In Stand Time?", "Can the NPC rotate while in stand time?"), temporaryAbRankList[i].rankREF.canRotateInStandTime, GUILayout.Width(430));
                                temporaryAbRankList[i].rankREF.castSpeedSlowAmount = EditorGUILayout.FloatField(new GUIContent("Slow Amount", "How much should the caster be slowed while casting this ability?"), temporaryAbRankList[i].rankREF.castSpeedSlowAmount);
                                temporaryAbRankList[i].rankREF.castSpeedSlowTime = EditorGUILayout.FloatField(new GUIContent("Slow Duration", "How long should the caster be slowed?"), temporaryAbRankList[i].rankREF.castSpeedSlowTime);
                                temporaryAbRankList[i].rankREF.castSpeedSlowRate = EditorGUILayout.FloatField(new GUIContent("Slow Rate", "How fast should the movement speed be reduced?"), temporaryAbRankList[i].rankREF.castSpeedSlowRate);

                                GUILayout.Space(10);
                                GUILayout.Label("ABILITY TYPE", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryAbRankList[i].rankREF.targetType = (RPGAbility.TARGET_TYPES)EditorGUILayout.EnumPopup(new GUIContent("Ability Mechanic", "What type of ability is it?"), temporaryAbRankList[i].rankREF.targetType, GUILayout.Width(430));
                                if (temporaryAbRankList[i].rankREF.targetType != RPGAbility.TARGET_TYPES.SELF && temporaryAbRankList[i].rankREF.targetType != RPGAbility.TARGET_TYPES.TARGET_INSTANT && temporaryAbRankList[i].rankREF.targetType != RPGAbility.TARGET_TYPES.TARGET_PROJECTILE)
                                {
                                    LayerMask tempMask = EditorGUILayout.MaskField(new GUIContent("Hit Layers", "What layers are hit by this ability?"), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(temporaryAbRankList[i].rankREF.hitLayers), InternalEditorUtility.layers, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.hitLayers = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
                                }
                                if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.SELF)
                                {
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.CONE)
                                {
                                    temporaryAbRankList[i].rankREF.minRange = EditorGUILayout.FloatField(new GUIContent("Range", "The distance at which this cone hits"), temporaryAbRankList[i].rankREF.minRange, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.coneDegree = EditorGUILayout.FloatField(new GUIContent("Angle", "The angle of the cone"), temporaryAbRankList[i].rankREF.coneDegree, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.ConeHitCount = EditorGUILayout.IntField(new GUIContent("Hits", "How many times does this ability hit?"), temporaryAbRankList[i].rankREF.ConeHitCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.ConeHitInterval = EditorGUILayout.FloatField(new GUIContent("Hit Interval", "How much time between each hit?"), temporaryAbRankList[i].rankREF.ConeHitInterval, GUILayout.Width(430));
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.AOE)
                                {
                                    temporaryAbRankList[i].rankREF.AOERadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius of the AoE"), temporaryAbRankList[i].rankREF.AOERadius, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.AOEHitCount = EditorGUILayout.IntField(new GUIContent("Hits", "How many times does this ability hit?"), temporaryAbRankList[i].rankREF.AOEHitCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.AOEHitInterval = EditorGUILayout.FloatField(new GUIContent("Hit Interval", "How much time between each hit?"), temporaryAbRankList[i].rankREF.AOEHitInterval, GUILayout.Width(430));
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.LINEAR)
                                {
                                    temporaryAbRankList[i].rankREF.linearWidth = EditorGUILayout.FloatField(new GUIContent("Width", "How wide is the linear area?"), temporaryAbRankList[i].rankREF.linearWidth, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.linearLength = EditorGUILayout.FloatField(new GUIContent("Length", "How long is the linear area?"), temporaryAbRankList[i].rankREF.linearLength, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.linearHeight = EditorGUILayout.FloatField(new GUIContent("Height", "How high is the linear area?"), temporaryAbRankList[i].rankREF.linearHeight, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.ConeHitCount = EditorGUILayout.IntField(new GUIContent("Hits", "How many times does this ability hit?"), temporaryAbRankList[i].rankREF.ConeHitCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.ConeHitInterval = EditorGUILayout.FloatField(new GUIContent("Hit Interval", "How much time between each hit?"), temporaryAbRankList[i].rankREF.ConeHitInterval, GUILayout.Width(430));
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.PROJECTILE)
                                {
                                    temporaryAbRankList[i].rankREF.projectileSpeed = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast does the projectile move?"), temporaryAbRankList[i].rankREF.projectileSpeed, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileDistance = EditorGUILayout.FloatField(new GUIContent("Distance", "How far away can the projectile go?"), temporaryAbRankList[i].rankREF.projectileDistance, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileAngleSpread = EditorGUILayout.FloatField(new GUIContent("Angle Spread", "Does the projectiles have an angle spread?"), temporaryAbRankList[i].rankREF.projectileAngleSpread, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileCount = EditorGUILayout.FloatField(new GUIContent("Counts", "How many projectiles should be fired?"), temporaryAbRankList[i].rankREF.projectileCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileDelay = EditorGUILayout.FloatField(new GUIContent("Delay", "Is there a delay before each projectile?"), temporaryAbRankList[i].rankREF.projectileDelay, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileComeBackAfterTime = EditorGUILayout.FloatField(new GUIContent("Come Back Time", "After how long does the projectile comes back to the caster?"), temporaryAbRankList[i].rankREF.projectileComeBackAfterTime, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileComeBackSpeed = EditorGUILayout.FloatField(new GUIContent("Come Back Speed", "How fast does the projectile comes back to the caster?"), temporaryAbRankList[i].rankREF.projectileComeBackSpeed, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileNearbyUnitDistanceMax = EditorGUILayout.FloatField(new GUIContent("Nearby Hit Distance", "How far away can nearby units be hit from?"), temporaryAbRankList[i].rankREF.projectileNearbyUnitDistanceMax, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileNearbyUnitMaxHit = EditorGUILayout.FloatField(new GUIContent("Nearby Hit Count", "How many times maximum can nearby units be hit?"), temporaryAbRankList[i].rankREF.projectileNearbyUnitMaxHit, GUILayout.Width(430));
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.GROUND)
                                {
                                    temporaryAbRankList[i].rankREF.groundRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius"), temporaryAbRankList[i].rankREF.groundRadius, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundRange = EditorGUILayout.FloatField(new GUIContent("Range", "The maximum range at which the ground ability can be casted from the character"), temporaryAbRankList[i].rankREF.groundRange, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundHitTime = EditorGUILayout.FloatField(new GUIContent("Delay", "The delay before the ability hits the ground"), temporaryAbRankList[i].rankREF.groundHitTime, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundHitCount = EditorGUILayout.IntField(new GUIContent("Hits", "How many times does this ability hits"), temporaryAbRankList[i].rankREF.groundHitCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundHitInterval = EditorGUILayout.FloatField(new GUIContent("Hit Interval", "How much time between each hit?"), temporaryAbRankList[i].rankREF.groundHitInterval, GUILayout.Width(430));
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.GROUND_LEAP)
                                {
                                    temporaryAbRankList[i].rankREF.groundRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius"), temporaryAbRankList[i].rankREF.groundRadius, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundRange = EditorGUILayout.FloatField(new GUIContent("Range", "The maximum range at which the ground ability can be casted from the character"), temporaryAbRankList[i].rankREF.groundRange, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundHitTime = EditorGUILayout.FloatField(new GUIContent("Delay", "The delay before the ability hits the ground"), temporaryAbRankList[i].rankREF.groundHitTime, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundHitCount = EditorGUILayout.IntField(new GUIContent("Hits", "How many times does this ability hits"), temporaryAbRankList[i].rankREF.groundHitCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundHitInterval = EditorGUILayout.FloatField(new GUIContent("Hit Interval", "How much time between each hit?"), temporaryAbRankList[i].rankREF.groundHitInterval, GUILayout.Width(430));

                                    temporaryAbRankList[i].rankREF.groundLeapDuration = EditorGUILayout.FloatField(new GUIContent("Leap Duration", "How long should the leap last?"), temporaryAbRankList[i].rankREF.groundLeapDuration, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundLeapHeight = EditorGUILayout.FloatField(new GUIContent("Leap Height", "How high should the leap be?"), temporaryAbRankList[i].rankREF.groundLeapHeight, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.groundLeapSpeed = EditorGUILayout.FloatField(new GUIContent("Leap Speed", "The speed of the leap"), temporaryAbRankList[i].rankREF.groundLeapSpeed, GUILayout.Width(430));

                                    temporaryAbRankList[i].rankREF.extraAbilityExecuted = (RPGAbility)EditorGUILayout.ObjectField(new GUIContent("Leap Ability", "Extra ability triggered by the leap"), temporaryAbRankList[i].rankREF.extraAbilityExecuted, typeof(RPGAbility), false, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.extraAbilityExecutedActivationType = (RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Activation Type", "When is the leap ability triggered"), temporaryAbRankList[i].rankREF.extraAbilityExecutedActivationType, GUILayout.Width(430));
                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_PROJECTILE)
                                {
                                    temporaryAbRankList[i].rankREF.canHitSelf = EditorGUILayout.Toggle(new GUIContent("Can Hit Self?", "Can this ability hit the player?"), temporaryAbRankList[i].rankREF.canHitSelf, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.canHitAlly = EditorGUILayout.Toggle(new GUIContent("Can Hit Allies?", "Can this ability hit the allied target?"), temporaryAbRankList[i].rankREF.canHitAlly, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.canHitNeutral = EditorGUILayout.Toggle(new GUIContent("Can Hit Neutral?", "Can this ability hit the neutral targets?"), temporaryAbRankList[i].rankREF.canHitNeutral, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.canHitEnemy = EditorGUILayout.Toggle(new GUIContent("Can Hit Enemy?", "Can this ability hit the enemy targets?"), temporaryAbRankList[i].rankREF.canHitEnemy, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.minRange = EditorGUILayout.FloatField(new GUIContent("Min. Range", "Minimum range from the target to use the ability"), temporaryAbRankList[i].rankREF.minRange, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.maxRange = EditorGUILayout.FloatField(new GUIContent("Max. Range", "Maximum range from the target to use the ability"), temporaryAbRankList[i].rankREF.maxRange, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileSpeed = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast does the projectile move?"), temporaryAbRankList[i].rankREF.projectileSpeed, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileCount = EditorGUILayout.FloatField(new GUIContent("Counts", "How many projectiles should be fired?"), temporaryAbRankList[i].rankREF.projectileCount, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.projectileDelay = EditorGUILayout.FloatField(new GUIContent("Delay", "Is there a delay before each projectile?"), temporaryAbRankList[i].rankREF.projectileDelay, GUILayout.Width(430));

                                }
                                else if (temporaryAbRankList[i].rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_INSTANT)
                                {
                                    temporaryAbRankList[i].rankREF.canHitSelf = EditorGUILayout.Toggle(new GUIContent("Can Hit Self?", "Can this ability hit the player?"), temporaryAbRankList[i].rankREF.canHitSelf, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.canHitAlly = EditorGUILayout.Toggle(new GUIContent("Can Hit Allies?", "Can this ability hit the allied target?"), temporaryAbRankList[i].rankREF.canHitAlly, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.canHitNeutral = EditorGUILayout.Toggle(new GUIContent("Can Hit Neutral?", "Can this ability hit the neutral targets?"), temporaryAbRankList[i].rankREF.canHitNeutral, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.canHitEnemy = EditorGUILayout.Toggle(new GUIContent("Can Hit Enemy?", "Can this ability hit the enemy targets?"), temporaryAbRankList[i].rankREF.canHitEnemy, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.minRange = EditorGUILayout.FloatField(new GUIContent("Min. Range", "Minimum range from the target to use the ability"), temporaryAbRankList[i].rankREF.minRange, GUILayout.Width(430));
                                    temporaryAbRankList[i].rankREF.maxRange = EditorGUILayout.FloatField(new GUIContent("Max. Range", "Maximum range from the target to use the ability"), temporaryAbRankList[i].rankREF.maxRange, GUILayout.Width(430));
                                }

                                if (temporaryAbRankList[i].rankREF.targetType != RPGAbility.TARGET_TYPES.SELF) temporaryAbRankList[i].rankREF.MaxUnitHit = EditorGUILayout.IntField(new GUIContent("Max Hits", "The maximum amount of units that can be hit by this ability"), temporaryAbRankList[i].rankREF.MaxUnitHit);

                                GUILayout.Space(10);
                                GUILayout.Label("COOLDOWNS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryAbRankList[i].rankREF.cooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "How long should the ability be on cooldown after being used"), temporaryAbRankList[i].rankREF.cooldown);
                                temporaryAbRankList[i].rankREF.isGCD = EditorGUILayout.Toggle(new GUIContent("GCD", "Is this ability triggered the global cooldown?"), temporaryAbRankList[i].rankREF.isGCD);
                                temporaryAbRankList[i].rankREF.startCDOnActivate = EditorGUILayout.Toggle(new GUIContent("CD On Activate", "Is the cooldown started on activation?"), temporaryAbRankList[i].rankREF.startCDOnActivate);

                                GUILayout.Space(10);
                                GUILayout.Label("EFFECTS APPLIED", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Effect", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryAbRankList[i].rankREF.effectsApplied.Add(new RPGAbility.AbilityEffectsApplied());

                                var ThisList2 = serialObj.FindProperty("effectsApplied");
                                temporaryAbRankList[i].rankREF.effectsApplied = GetTargetObjectOfProperty(ThisList2) as List<RPGAbility.AbilityEffectsApplied>;

                                for (var a = 0; a < temporaryAbRankList[i].rankREF.effectsApplied.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryAbRankList[i].rankREF.effectsApplied.RemoveAt(a);
                                        return;
                                    }
                                    var effectName = "";
                                    if (temporaryAbRankList[i].rankREF.effectsApplied[a].effectREF != null) effectName = temporaryAbRankList[i].rankREF.effectsApplied[a].effectREF._name;
                                    EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Unit Hit", GUILayout.Width(100));
                                    temporaryAbRankList[i].rankREF.effectsApplied[a].target = (RPGCombatDATA.TARGET_TYPE)EditorGUILayout.EnumPopup(temporaryAbRankList[i].rankREF.effectsApplied[a].target, GUILayout.Width(250));
                                    GUILayout.Space(10);

                                    EditorGUILayout.EndHorizontal();
                                    
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Chance", GUILayout.Width(100));
                                    temporaryAbRankList[i].rankREF.effectsApplied[a].chance = EditorGUILayout.Slider(temporaryAbRankList[i].rankREF.effectsApplied[a].chance, 0f, 100f, GUILayout.Width(250));
                                    GUILayout.Space(10);

                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Effect", GUILayout.Width(100));
                                    temporaryAbRankList[i].rankREF.effectsApplied[a].effectREF = (RPGEffect)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetEffectFromIDEditor(temporaryAbRankList[i].rankREF.effectsApplied[a].effectID, allEffects), typeof(RPGEffect), false, GUILayout.Width(250));
                                    EditorGUILayout.EndHorizontal();
                                    if (temporaryAbRankList[i].rankREF.effectsApplied[a].effectREF != null)
                                        temporaryAbRankList[i].rankREF.effectsApplied[a].effectID = temporaryAbRankList[i].rankREF.effectsApplied[a].effectREF.ID;
                                    else
                                        temporaryAbRankList[i].rankREF.effectsApplied[a].effectID = -1;
                                    GUILayout.Space(10);
                                }
                                
                                GUILayout.Space(10);
                                GUILayout.Label("TAGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Tag", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryAbRankList[i].rankREF.tagsData.Add(new RPGAbility.AbilityTagsData());

                                var ThisList8 = serialObj.FindProperty("tagsData");
                                temporaryAbRankList[i].rankREF.tagsData = GetTargetObjectOfProperty(ThisList8) as List<RPGAbility.AbilityTagsData>;

                                for (var a = 0; a < temporaryAbRankList[i].rankREF.tagsData.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryAbRankList[i].rankREF.tagsData.RemoveAt(a);
                                        return;
                                    }
                                    EditorGUILayout.LabelField("Visual:" + requirementNumber + ":", GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Tag:", GUILayout.Width(100));
                                    temporaryAbRankList[i].rankREF.tagsData[a].tag = (RPGAbility.ABILITY_TAGS)EditorGUILayout.EnumPopup(temporaryAbRankList[i].rankREF.tagsData[a].tag);
                                    EditorGUILayout.EndHorizontal();
                                }
                                
                                GUILayout.Space(10);
                                GUILayout.Label("VISUALS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Visual", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryAbRankList[i].rankREF.visualsData.Add(new RPGAbility.AbilityVisualData());

                                var ThisList5 = serialObj.FindProperty("visualsData");
                                temporaryAbRankList[i].rankREF.visualsData = GetTargetObjectOfProperty(ThisList5) as List<RPGAbility.AbilityVisualData>;

                                for (var a = 0; a < temporaryAbRankList[i].rankREF.visualsData.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryAbRankList[i].rankREF.visualsData.RemoveAt(a);
                                        return;
                                    }
                                    EditorGUILayout.LabelField("Visual:" + requirementNumber + ":", GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Activate On", GUILayout.Width(100));
                                    temporaryAbRankList[i].rankREF.visualsData[a].activationType = (RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE)EditorGUILayout.EnumPopup(temporaryAbRankList[i].rankREF.visualsData[a].activationType);
                                    GUILayout.Space(10);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Type", GUILayout.Width(100));
                                    temporaryAbRankList[i].rankREF.visualsData[a].type = (RPGAbility.COMBAT_VISUAL_EFFECT_TYPE)EditorGUILayout.EnumPopup(temporaryAbRankList[i].rankREF.visualsData[a].type);
                                    GUILayout.Space(10);
                                    EditorGUILayout.EndHorizontal();
                                    if (temporaryAbRankList[i].rankREF.visualsData[a].type ==
                                        RPGAbility.COMBAT_VISUAL_EFFECT_TYPE.effect)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(new GUIContent("Effect", "The Combat Visual representation of this ability"), GUILayout.Width(100));
                                        temporaryAbRankList[i].rankREF.visualsData[a].effect = (CombatVisualEffect)EditorGUILayout.ObjectField(temporaryAbRankList[i].rankREF.visualsData[a].effect, typeof(CombatVisualEffect), false);
                                        EditorGUILayout.EndHorizontal();
                                    } else if (temporaryAbRankList[i].rankREF.visualsData[a].type ==
                                               RPGAbility.COMBAT_VISUAL_EFFECT_TYPE.animation)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(new GUIContent("Animation", "The Combat Animation of this ability"), GUILayout.Width(100));
                                        temporaryAbRankList[i].rankREF.visualsData[a].animation = (CombatVisualAnimation)EditorGUILayout.ObjectField(temporaryAbRankList[i].rankREF.visualsData[a].animation, typeof(CombatVisualAnimation), false);
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    GUILayout.Space(10);
                                }
                                

                                serialObj.ApplyModifiedProperties();
                                
                            }

                            GUILayout.Space(5);
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();

                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
        DrawActionButtons(AssetType.Ability, containerRect2);
    }

    private void DrawCraftingRecipeView()
    {
        if (currentlyViewedCraftingRecipe == null)
        {
            if (allCraftingRecipes.Count == 0)
            {
                CreateNew(AssetType.CraftingRecipe);
                return;
            }
            currentlyViewedCraftingRecipe = Instantiate(allCraftingRecipes[0]) as RPGCraftingRecipe;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allCraftingRecipes.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allCraftingRecipes[i]._name;
                            newElementDATA.showIcon = true;
                            if (allCraftingRecipes[i].icon != null) newElementDATA.texture = allCraftingRecipes[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.CraftingRecipe);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedCraftingRecipe.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedCraftingRecipe.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingRecipe.ID = EditorGUILayout.IntField(currentlyViewedCraftingRecipe.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingRecipe._name = GUILayout.TextField(currentlyViewedCraftingRecipe._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingRecipe.displayName = GUILayout.TextField(currentlyViewedCraftingRecipe.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedCraftingRecipe._fileName = GUILayout.TextField("RPG_CRAFTING_RECIPE_" + currentlyViewedCraftingRecipe._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Known Automatically", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingRecipe.learnedByDefault = EditorGUILayout.Toggle(currentlyViewedCraftingRecipe.learnedByDefault);
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        currentlyViewedCraftingRecipe.craftingSkillREF = (RPGSkill)EditorGUILayout.ObjectField("Skill", RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedCraftingRecipe.craftingSkillID, allSkills), typeof(RPGSkill), false, GUILayout.Width(400));
                        if (currentlyViewedCraftingRecipe.craftingSkillREF != null)
                            currentlyViewedCraftingRecipe.craftingSkillID = currentlyViewedCraftingRecipe.craftingSkillREF.ID;
                        else
                            currentlyViewedCraftingRecipe.craftingSkillID = -1;

                        currentlyViewedCraftingRecipe.craftingStationREF = (RPGCraftingStation)EditorGUILayout.ObjectField("Station", RPGBuilderUtilities.GetCraftingStationFromIDEditor(currentlyViewedCraftingRecipe.craftingStationID, allCraftingStations), typeof(RPGCraftingStation), false, GUILayout.Width(400));
                        if (currentlyViewedCraftingRecipe.craftingStationREF != null)
                            currentlyViewedCraftingRecipe.craftingStationID = currentlyViewedCraftingRecipe.craftingStationREF.ID;
                        else
                            currentlyViewedCraftingRecipe.craftingStationID = -1;

                        GUILayout.Space(10);
                        GUILayout.Label("RANKS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Rank", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            var newRankData = new RPGCraftingRecipeRankData();
                            var newRankDataElement = new RPGCraftingRecipe.rankDATA();
                            newRankDataElement.rankID = -1;
                            newRankDataElement.rankREF = newRankData;
                            temporaryRecipeRankList.Add(newRankDataElement);
                        }
                        GUILayout.Space(10);

                        if (GUILayout.Button("- Remove Rank", skin.GetStyle("RemoveAbilityRankButton"), GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            var rankNumber = temporaryRecipeRankList.Count + 1;
                            temporaryRecipeRankList.RemoveAt(temporaryRecipeRankList.Count - 1);
                            return;
                        }
                        GUILayout.Space(10);

                        for (var i = 0; i < temporaryRecipeRankList.Count; i++)
                        {
                            var rankNbr = i + 1;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Rank: " + rankNbr, skin.GetStyle("AbilityRankButton"), GUILayout.Width(150), GUILayout.Height(25))) temporaryRecipeRankList[i].ShowedInEditor = !temporaryRecipeRankList[i].ShowedInEditor;
                            if (i > 0)
                            {
                                GUILayout.Space(5);
                                if (GUILayout.Button("Copy Above", skin.GetStyle("AddButton"), GUILayout.Width(225), GUILayout.Height(25))) temporaryRecipeRankList[i].rankREF.copyData(temporaryRecipeRankList[i].rankREF, temporaryRecipeRankList[i - 1].rankREF);
                            }
                            GUILayout.EndHorizontal();

                            if (temporaryRecipeRankList[i].ShowedInEditor)
                            {
                                temporaryRecipeRankList[i].rankREF.unlockCost = EditorGUILayout.IntField(new GUIContent("Unlock Cost", "Cost in point inside the crafting tree"), temporaryRecipeRankList[i].rankREF.unlockCost, GUILayout.Width(430));

                                GUILayout.Space(10);
                                GUILayout.Label("SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryRecipeRankList[i].rankREF.Experience = EditorGUILayout.IntField("Experience", temporaryRecipeRankList[i].rankREF.Experience, GUILayout.Width(430));
                                temporaryRecipeRankList[i].rankREF.craftTime = EditorGUILayout.FloatField("Craft Duration", temporaryRecipeRankList[i].rankREF.craftTime, GUILayout.Width(430));

                                GUILayout.Space(10);
                                GUILayout.Label("CRAFTED ITEMS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                ScriptableObject scriptableObj = temporaryRecipeRankList[i].rankREF;
                                var serialObj = new SerializedObject(scriptableObj);

                                if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryRecipeRankList[i].rankREF.allCraftedItems.Add(new RPGCraftingRecipeRankData.CraftedItemsDATA());

                                var ThisList2 = serialObj.FindProperty("allCraftedItems");
                                temporaryRecipeRankList[i].rankREF.allCraftedItems = GetTargetObjectOfProperty(ThisList2) as List<RPGCraftingRecipeRankData.CraftedItemsDATA>;

                                for (var a = 0; a < temporaryRecipeRankList[i].rankREF.allCraftedItems.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryRecipeRankList[i].rankREF.allCraftedItems.RemoveAt(a);
                                        return;
                                    }
                                    var effectName = "";
                                    if (temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemREF != null) effectName = temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemREF._name;
                                    EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Item", GUILayout.Width(100));
                                    temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                                    EditorGUILayout.EndHorizontal();
                                    if (temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemREF != null)
                                        temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemID = temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemREF.ID;
                                    else
                                        temporaryRecipeRankList[i].rankREF.allCraftedItems[a].craftedItemID = -1;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Chance", GUILayout.Width(100));
                                    temporaryRecipeRankList[i].rankREF.allCraftedItems[a].chance = EditorGUILayout.Slider(temporaryRecipeRankList[i].rankREF.allCraftedItems[a].chance, 0f, 100f, GUILayout.Width(250));
                                    GUILayout.Space(10);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Count", GUILayout.Width(100));
                                    temporaryRecipeRankList[i].rankREF.allCraftedItems[a].count = EditorGUILayout.IntField(temporaryRecipeRankList[i].rankREF.allCraftedItems[a].count, GUILayout.Width(250));
                                    GUILayout.Space(10);
                                    EditorGUILayout.EndHorizontal();

                                    GUILayout.Space(10);
                                }

                                GUILayout.Space(10);
                                GUILayout.Label("COMPONENTS REQUIRED", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryRecipeRankList[i].rankREF.allComponents.Add(new RPGCraftingRecipeRankData.ComponentsRequired());

                                var ThisList3 = serialObj.FindProperty("allComponents");
                                temporaryRecipeRankList[i].rankREF.allComponents = GetTargetObjectOfProperty(ThisList3) as List<RPGCraftingRecipeRankData.ComponentsRequired>;

                                for (var a = 0; a < temporaryRecipeRankList[i].rankREF.allComponents.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryRecipeRankList[i].rankREF.allComponents.RemoveAt(a);
                                        return;
                                    }
                                    var effectName = "";
                                    if (temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemREF != null) effectName = temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemREF._name;
                                    EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Item", GUILayout.Width(100));
                                    temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                                    EditorGUILayout.EndHorizontal();
                                    if (temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemREF != null)
                                        temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemID = temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemREF.ID;
                                    else
                                        temporaryRecipeRankList[i].rankREF.allComponents[a].componentItemID = -1;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Count", GUILayout.Width(100));
                                    temporaryRecipeRankList[i].rankREF.allComponents[a].count = EditorGUILayout.IntField(temporaryRecipeRankList[i].rankREF.allComponents[a].count, GUILayout.Width(250));
                                    GUILayout.Space(10);
                                    EditorGUILayout.EndHorizontal();

                                    GUILayout.Space(10);
                                }

                                serialObj.ApplyModifiedProperties();
                            }

                            GUILayout.Space(5);
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();

                        break;
                }
                GUILayout.EndArea();
            }
        }

        GUILayout.EndArea();

        DrawActionButtons(AssetType.CraftingRecipe, containerRect2);
    }

    private void DrawBonusView()
    {
        if (currentlyViewedBonus == null)
        {
            if (allBonuses.Count == 0)
            {
                CreateNew(AssetType.Bonus);
                return;
            }
            currentlyViewedBonus = Instantiate(allBonuses[0]) as RPGBonus;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allBonuses.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allBonuses[i]._name;
                            newElementDATA.showIcon = true;
                            if (allBonuses[i].icon != null) newElementDATA.texture = allBonuses[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }

                        DrawElementList(curElementList, AssetType.Bonus);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedBonus.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedBonus.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedBonus.ID = EditorGUILayout.IntField(currentlyViewedBonus.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedBonus._name = GUILayout.TextField(currentlyViewedBonus._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Display Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedBonus.displayName = GUILayout.TextField(currentlyViewedBonus.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedBonus._fileName = GUILayout.TextField("RPG_BONUS_" + currentlyViewedBonus._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("RANKS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Rank", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            var newRankData = new RPGBonusRankDATA();
                            var newRankDataElement = new RPGBonus.rankDATA();
                            newRankDataElement.rankID = -1;
                            newRankDataElement.rankREF = newRankData;
                            temporaryBonusRankList.Add(newRankDataElement);
                        }
                        GUILayout.Space(10);

                        if (GUILayout.Button("- Remove Rank", skin.GetStyle("RemoveAbilityRankButton"), GUILayout.Width(440), GUILayout.Height(30)))
                        {
                            var rankNumber = temporaryBonusRankList.Count + 1;
                            temporaryBonusRankList.RemoveAt(temporaryBonusRankList.Count - 1);
                            return;
                        }
                        GUILayout.Space(10);

                        for (var i = 0; i < temporaryBonusRankList.Count; i++)
                        {
                            var rankNbr = i + 1;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Rank: " + rankNbr, skin.GetStyle("AbilityRankButton"), GUILayout.Width(150), GUILayout.Height(30))) temporaryBonusRankList[i].ShowedInEditor = !temporaryBonusRankList[i].ShowedInEditor;
                            if (i > 0)
                            {
                                GUILayout.Space(20);
                                if (GUILayout.Button("Copy Above", skin.GetStyle("AddButton"), GUILayout.Width(225), GUILayout.Height(30))) temporaryBonusRankList[i].rankREF.copyData(temporaryBonusRankList[i].rankREF, temporaryBonusRankList[i - 1].rankREF);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);

                            if (temporaryBonusRankList[i].ShowedInEditor)
                            {
                                temporaryBonusRankList[i].rankREF.unlockCost = EditorGUILayout.IntField(new GUIContent("Unlock Cost", "Cost in point inside the tree"), temporaryBonusRankList[i].rankREF.unlockCost, GUILayout.Width(430));


                                ScriptableObject scriptableObj = temporaryBonusRankList[i].rankREF;
                                var serialObj = new SerializedObject(scriptableObj);

                                GUILayout.Space(10);
                                GUILayout.Label("REQUIREMENTS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Requirement", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryBonusRankList[i].rankREF.activeRequirements.Add(new RequirementsManager.BonusRequirementDATA());

                                var ThisList = serialObj.FindProperty("activeRequirements");
                                temporaryBonusRankList[i].rankREF.activeRequirements = GetTargetObjectOfProperty(ThisList) as List<RequirementsManager.BonusRequirementDATA>;

                                for (var a = 0; a < temporaryBonusRankList[i].rankREF.activeRequirements.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                                    temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType = (RequirementsManager.BonusRequirementType)EditorGUILayout.EnumPopup(temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType, GUILayout.Width(250));
                                    GUILayout.Space(10);
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryBonusRankList[i].rankREF.activeRequirements.RemoveAt(a);
                                        return;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginVertical();

                                    if (temporaryBonusRankList[i].rankREF.activeRequirements.Count > 0)
                                    {
                                        if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.statState)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].statStateRequired = (RequirementsManager.StatStateType)EditorGUILayout.EnumPopup(temporaryBonusRankList[i].rankREF.activeRequirements[a].statStateRequired, GUILayout.Width(250));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].statREF = (RPGStat)EditorGUILayout.ObjectField("Stat:", RPGBuilderUtilities.GetStatFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].statID, allStats), typeof(RPGStat), false, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].statValue = EditorGUILayout.IntField("Amount:", temporaryBonusRankList[i].rankREF.activeRequirements[a].statValue, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].isStatValuePercent = EditorGUILayout.Toggle(new GUIContent("Is Percent?", "Is the Stat Value a percentage?"), temporaryBonusRankList[i].rankREF.activeRequirements[a].isStatValuePercent);
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].statREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].statID = temporaryBonusRankList[i].rankREF.activeRequirements[a].statREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].statID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.weaponTypeEquipped)
                                        {
                                            var currentWeaponTypeIndex = getIndexFromName("WeaponType", temporaryBonusRankList[i].rankREF.activeRequirements[a].weaponRequired);
                                            var tempIndex2 = 0;
                                            tempIndex2 = EditorGUILayout.Popup("Weapon Type", currentWeaponTypeIndex, itemSettings.weaponType);
                                            if (itemSettings.weaponType.Length > 0) temporaryBonusRankList[i].rankREF.activeRequirements[a].weaponRequired = itemSettings.weaponType[tempIndex2];
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.pointSpent)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].pointSpentValue = EditorGUILayout.IntField(new GUIContent("Points Spent", "How many points should already be spent in this tree for this bonus to be active?"), temporaryBonusRankList[i].rankREF.activeRequirements[a].pointSpentValue, GUILayout.Width(400));
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.classLevel)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].classLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The class level required"), temporaryBonusRankList[i].rankREF.activeRequirements[a].classLevelValue, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType._class)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].classRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.skillLevel)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill required for this bonus to be active"), RPGBuilderUtilities.GetSkillFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].skillLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The skill level required"), temporaryBonusRankList[i].rankREF.activeRequirements[a].skillLevelValue, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].skillRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].skillRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].skillRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].skillRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.itemOwned)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].itemRequiredREF = (RPGItem)EditorGUILayout.ObjectField(new GUIContent("Item", "The item required"), RPGBuilderUtilities.GetItemFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].itemRequiredID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].itemEquipped = EditorGUILayout.Toggle(new GUIContent("Equipped?", "Does the item needs to be equipped?"), temporaryBonusRankList[i].rankREF.activeRequirements[a].itemEquipped);
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].itemRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].itemRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].itemRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].itemRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.abilityKnown)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].abilityRequiredREF = (RPGAbility)EditorGUILayout.ObjectField(new GUIContent("Ability", "The ability required to be known for this bonus to be active"), RPGBuilderUtilities.GetAbilityFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].abilityRequiredID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].abilityRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].abilityRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].abilityRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].abilityRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.recipeKnown)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].recipeRequiredREF = (RPGCraftingRecipe)EditorGUILayout.ObjectField(new GUIContent("Recipe", "The crafting recipe required to be known for this bonus to be active"), RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].craftingRecipeRequiredID, allCraftingRecipes), typeof(RPGCraftingRecipe), false, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].recipeRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].craftingRecipeRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].recipeRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].craftingRecipeRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.resourceNodeKnown)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].resourceNodeRequiredREF = (RPGResourceNode)EditorGUILayout.ObjectField(new GUIContent("Resource Node", "The resource node required to be known for this bonus to be active"), RPGBuilderUtilities.GetResourceNodeFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].resourceNodeRequiredID, allResourceNodes), typeof(RPGResourceNode), false, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].resourceNodeRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].resourceNodeRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].resourceNodeRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].resourceNodeRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.race)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].raceRequiredREF = (RPGRace)EditorGUILayout.ObjectField(new GUIContent("Race", "The race required"), RPGBuilderUtilities.GetRaceFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].raceRequiredID, allRaces), typeof(RPGRace), false, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].raceRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].raceRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].raceRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].raceRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.questState)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].questRequiredREF = (RPGQuest)EditorGUILayout.ObjectField(new GUIContent("Quest", "The quest required for this bonus to be active"), RPGBuilderUtilities.GetQuestFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].questRequiredID, allQuests), typeof(RPGQuest), false, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].questStateRequired = (QuestManager.questState)EditorGUILayout.EnumPopup(new GUIContent("State", "The required state of the quest"), temporaryBonusRankList[i].rankREF.activeRequirements[a].questStateRequired, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].questRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].questRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].questRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].questRequiredID = -1;
                                        }
                                        else if (temporaryBonusRankList[i].rankREF.activeRequirements[a].requirementType == RequirementsManager.BonusRequirementType.npcKilled)
                                        {
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].npcRequiredREF = (RPGNpc)EditorGUILayout.ObjectField(new GUIContent("NPC", "The NPC required to be killed"), RPGBuilderUtilities.GetNPCFromIDEditor(temporaryBonusRankList[i].rankREF.activeRequirements[a].npcRequiredID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(400));
                                            temporaryBonusRankList[i].rankREF.activeRequirements[a].npcKillsRequired = EditorGUILayout.IntField(new GUIContent("Kills", "How many times this NPC should have been killed for the bonus to be active"), temporaryBonusRankList[i].rankREF.activeRequirements[a].npcKillsRequired, GUILayout.Width(400));
                                            if (temporaryBonusRankList[i].rankREF.activeRequirements[a].npcRequiredREF != null)
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].npcRequiredID = temporaryBonusRankList[i].rankREF.activeRequirements[a].npcRequiredREF.ID;
                                            else
                                                temporaryBonusRankList[i].rankREF.activeRequirements[a].npcRequiredID = -1;
                                        }
                                    }
                                    EditorGUILayout.EndVertical();

                                    GUILayout.Space(10);
                                }

                                GUILayout.Space(10);
                                GUILayout.Label("EFFECTS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                if (GUILayout.Button("+ Add Effect", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) temporaryBonusRankList[i].rankREF.effectsApplied.Add(new RPGBonus.BonusEffectsApplied());

                                var ThisList2 = serialObj.FindProperty("effectsApplied");
                                temporaryBonusRankList[i].rankREF.effectsApplied = GetTargetObjectOfProperty(ThisList2) as List<RPGBonus.BonusEffectsApplied>;

                                for (var a = 0; a < temporaryBonusRankList[i].rankREF.effectsApplied.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        temporaryBonusRankList[i].rankREF.effectsApplied.RemoveAt(a);
                                        return;
                                    }
                                    var effectName = "";
                                    if (temporaryBonusRankList[i].rankREF.effectsApplied[a].effectREF != null) effectName = temporaryBonusRankList[i].rankREF.effectsApplied[a].effectREF._name;
                                    EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Effect", GUILayout.Width(100));
                                    temporaryBonusRankList[i].rankREF.effectsApplied[a].effectREF = (RPGEffect)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetEffectFromIDEditor(temporaryBonusRankList[i].rankREF.effectsApplied[a].effectID, allEffects), typeof(RPGEffect), false, GUILayout.Width(250));
                                    EditorGUILayout.EndHorizontal();
                                    if (temporaryBonusRankList[i].rankREF.effectsApplied[a].effectREF != null)
                                        temporaryBonusRankList[i].rankREF.effectsApplied[a].effectID = temporaryBonusRankList[i].rankREF.effectsApplied[a].effectREF.ID;
                                    else
                                        temporaryBonusRankList[i].rankREF.effectsApplied[a].effectID = -1;
                                    GUILayout.Space(10);
                                }

                                serialObj.ApplyModifiedProperties();
                            }

                            GUILayout.Space(5);
                        }


                        GUILayout.EndScrollView();
                        GUILayout.EndArea();

                        break;
                }
                GUILayout.EndArea();
            }
        }

        GUILayout.EndArea();

        DrawActionButtons(AssetType.Bonus, containerRect2);
    }


    private void DrawCraftingStationView()
    {
        if (currentlyViewedCraftingStation == null)
        {
            if (allCraftingStations.Count == 0)
            {
                CreateNew(AssetType.CraftingStation);
                return;
            }

            currentlyViewedCraftingStation = Instantiate(allCraftingStations[0]) as RPGCraftingStation;
        }

        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin,
            containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0;
            x < editorDATA.categoriesData[(int) currentCategorySelected]
                .subCategoriesData[(int) generalSubCurrentCategorySelected].containersData.Length;
            x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int) currentCategorySelected]
                .subCategoriesData[(int) generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int) currentCategorySelected]
                .subCategoriesData[(int) generalSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int) currentCategorySelected]
                    .subCategoriesData[(int) generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allCraftingStations.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allCraftingStations[i]._name;
                            newElementDATA.showIcon = true;
                            if (allCraftingStations[i].icon != null)
                                newElementDATA.texture = allCraftingStations[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }

                        DrawElementList(curElementList, AssetType.CraftingStation);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin,
                            editorDATA.categoriesData[(int) currentCategorySelected]
                                .subCategoriesData[(int) generalSubCurrentCategorySelected].containersData[x]
                                .containerRect.width,
                            editorDATA.categoriesData[(int) currentCategorySelected]
                                .subCategoriesData[(int) generalSubCurrentCategorySelected].containersData[x]
                                .containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition,
                            GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450),
                            GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedCraftingStation.icon = (Sprite) EditorGUILayout.ObjectField(
                            currentlyViewedCraftingStation.icon, typeof(Sprite), false, GUILayout.Width(70),
                            GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingStation.ID = EditorGUILayout.IntField(currentlyViewedCraftingStation.ID,
                            GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingStation._name = GUILayout.TextField(currentlyViewedCraftingStation._name,
                            GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCraftingStation.displayName = GUILayout.TextField(currentlyViewedCraftingStation.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedCraftingStation._fileName = GUILayout.TextField(
                            "RPG_CRAFTING_STATION_" + currentlyViewedCraftingStation._name, GUILayout.Width(200),
                            GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("GENERAL DATA", skin.GetStyle("ViewTitle"), GUILayout.Width(450),
                            GUILayout.Height(40));
                        GUILayout.Space(10);
                        ScriptableObject scriptableObj = currentlyViewedCraftingStation;
                        var serialObj = new SerializedObject(scriptableObj);

                        if (GUILayout.Button("+ Add Skill", skin.GetStyle("AddButton"), GUILayout.Width(440),
                            GUILayout.Height(25)))
                            currentlyViewedCraftingStation.craftSkills.Add(new RPGCraftingStation.CraftSkillsDATA());

                        var ThisList2 = serialObj.FindProperty("craftSkills");
                        currentlyViewedCraftingStation.craftSkills =
                            GetTargetObjectOfProperty(ThisList2) as List<RPGCraftingStation.CraftSkillsDATA>;

                        for (var a = 0; a < currentlyViewedCraftingStation.craftSkills.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20),
                                GUILayout.Height(20)))
                            {
                                currentlyViewedCraftingStation.craftSkills.RemoveAt(a);
                                return;
                            }

                            var effectName = "";
                            if (currentlyViewedCraftingStation.craftSkills[a].craftSkillREF != null)
                                effectName = currentlyViewedCraftingStation.craftSkills[a].craftSkillREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName,
                                GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Skill", GUILayout.Width(100));
                            currentlyViewedCraftingStation.craftSkills[a].craftSkillREF =
                                (RPGSkill) EditorGUILayout.ObjectField(
                                    RPGBuilderUtilities.GetSkillFromIDEditor(
                                        currentlyViewedCraftingStation.craftSkills[a].craftSkillID, allSkills),
                                    typeof(RPGSkill), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedCraftingStation.craftSkills[a].craftSkillREF != null)
                                currentlyViewedCraftingStation.craftSkills[a].craftSkillID =
                                    currentlyViewedCraftingStation.craftSkills[a].craftSkillREF.ID;
                            else
                                currentlyViewedCraftingStation.craftSkills[a].craftSkillID = -1;
                            GUILayout.Space(10);
                        }

                        serialObj.ApplyModifiedProperties();
                        GUILayout.EndScrollView();
                        GUILayout.EndArea();

                        break;
                }

                GUILayout.EndArea();
            }
        }

        GUILayout.EndArea();

        DrawActionButtons(AssetType.CraftingStation, containerRect2);
    }


    private int getRealNodeIndex(int tempAbIndex)
    {
        for (var i = 0; i < currentlyViewedTalentTree.nodeList.Count; i++)
            if (tempNodeLIst[tempAbIndex] == currentlyViewedTalentTree.nodeList[i]) return i;
        return -1;
    }

    private List<RPGTalentTree.Node_DATA> tempNodeLIst = new List<RPGTalentTree.Node_DATA>();

    private void DrawTalentTreeView()
    {
        if (currentlyViewedTalentTree == null)
        {
            if (allTalentTrees.Count == 0)
            {
                CreateNew(AssetType.TalentTree);
                return;
            }
            currentlyViewedTalentTree = Instantiate(allTalentTrees[0]) as RPGTalentTree;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allTalentTrees.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allTalentTrees[i]._name;
                            newElementDATA.showIcon = true;
                            if (allTalentTrees[i].icon != null) newElementDATA.texture = allTalentTrees[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.TalentTree);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(440), GUILayout.Height(30));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedTalentTree.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedTalentTree.icon, typeof(Sprite), false, GUILayout.Width(80), GUILayout.Height(80));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTalentTree.ID = EditorGUILayout.IntField(currentlyViewedTalentTree.ID, GUILayout.Width(250), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTalentTree._name = GUILayout.TextField(currentlyViewedTalentTree._name, GUILayout.Width(250), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Display Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTalentTree.displayName = GUILayout.TextField(currentlyViewedTalentTree.displayName, GUILayout.Width(250), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedTalentTree._fileName = GUILayout.TextField("RPG_TALENT_TREE_" + currentlyViewedTalentTree._name, GUILayout.Width(250), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Tree Point:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTalentTree.treePointAcceptedREF = (RPGTreePoint)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetTreePointFromIDEditor(currentlyViewedTalentTree.treePointAcceptedID, allTreePoints), typeof(RPGTreePoint), false, GUILayout.Width(250), GUILayout.Height(15));
                        if (currentlyViewedTalentTree.treePointAcceptedREF != null)
                            currentlyViewedTalentTree.treePointAcceptedID = currentlyViewedTalentTree.treePointAcceptedREF.ID;
                        else
                            currentlyViewedTalentTree.treePointAcceptedID = -1;

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Tier Amount:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTalentTree.TiersAmount = EditorGUILayout.IntField(currentlyViewedTalentTree.TiersAmount, GUILayout.Width(250));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        ScriptableObject scriptableObj = currentlyViewedTalentTree;
                        var serialObj = new SerializedObject(scriptableObj);

                        var ThisList = serialObj.FindProperty("nodeList");

                        if (GUILayout.Button("+ Add Node", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedTalentTree.nodeList.Add(new RPGTalentTree.Node_DATA());
                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(50);
                        curCraftingTreeSearchText = GUILayout.TextArea(curCraftingTreeSearchText, skin.GetStyle("SearchBarSmall"), GUILayout.Width(350), GUILayout.Height(25));
                        GUILayout.Box(editorDATA.searchIcon, GUILayout.Width(35), GUILayout.Height(25));
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);

                        currentlyViewedTalentTree.nodeList = GetTargetObjectOfProperty(ThisList) as List<RPGTalentTree.Node_DATA>;

                        var baseHeight = editorDATA.abilityListYMargin;
                        var curAbilityElementOffset = baseHeight;

                        tempNodeLIst.Clear();
                        var tempSearchText2 = "";
                        if (curCraftingTreeSearchText != null && curCraftingTreeSearchText.Length > 0) tempSearchText2 = curCraftingTreeSearchText.ToLower();

                        for (var i = 0; i < currentlyViewedTalentTree.nodeList.Count; i++)
                        {
                            var nodeIsNull = false;
                            var nodeName2 = "";
                            
                            switch(currentlyViewedTalentTree.nodeList[i].nodeType)
                            {
                                case RPGTalentTree.TalentTreeNodeType.ability:
                                    nodeIsNull = currentlyViewedTalentTree.nodeList[i].abilityID == -1;
                                    nodeName2 = "";
                                    var abREF = RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedTalentTree.nodeList[i].abilityID, allAbilities);
                                    if (!nodeIsNull && abREF!= null)
                                    {
                                        nodeName2 = abREF._name;
                                        nodeName2 = nodeName2.ToLower();
                                    }
                                    break;
                                case RPGTalentTree.TalentTreeNodeType.recipe:
                                    nodeIsNull = currentlyViewedTalentTree.nodeList[i].recipeID == -1;
                                    nodeName2 = "";
                                    var recipeREF = RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(currentlyViewedTalentTree.nodeList[i].recipeID, allCraftingRecipes);
                                    if (!nodeIsNull && recipeREF != null)
                                    {
                                        nodeName2 = recipeREF._name;
                                        nodeName2 = nodeName2.ToLower();
                                    }
                                    break;
                                case RPGTalentTree.TalentTreeNodeType.resourceNode:
                                    nodeIsNull = currentlyViewedTalentTree.nodeList[i].resourceNodeID == -1;
                                    nodeName2 = "";
                                    var resourceREF = RPGBuilderUtilities.GetResourceNodeFromIDEditor(currentlyViewedTalentTree.nodeList[i].resourceNodeID, allResourceNodes);
                                    if (!nodeIsNull && resourceREF != null)
                                    {
                                        nodeName2 = resourceREF._name;
                                        nodeName2 = nodeName2.ToLower();
                                    }
                                    break;
                                case RPGTalentTree.TalentTreeNodeType.bonus:
                                    nodeIsNull = currentlyViewedTalentTree.nodeList[i].bonusID == -1;
                                    nodeName2 = "";
                                    var bonusREF = RPGBuilderUtilities.GetBonusFromIDEditor(currentlyViewedTalentTree.nodeList[i].bonusID, allBonuses);
                                    if (!nodeIsNull && bonusREF != null)
                                    {
                                        nodeName2 = bonusREF._name;
                                        nodeName2 = nodeName2.ToLower();
                                    }
                                    break;
                            }

                            if (curCraftingTreeSearchText == null || curCraftingTreeSearchText.Length < 1 || nodeName2.Contains(tempSearchText2)) tempNodeLIst.Add(currentlyViewedTalentTree.nodeList[i]);
                        }

                        for (var i = 0; i < tempNodeLIst.Count; i++)
                        {
                            var realAbTreeIndex = getRealNodeIndex(i);
                            /*var previousAbilityTotalHeight = editorDATA.baseAbilityElementHeight;
                            if (i > 0 && realAbTreeIndex > 0)
                            {
                                previousAbilityTotalHeight += getRequirementsTotalHeight(getRealNodeIndex(i - 1));
                                curAbilityElementOffset += previousAbilityTotalHeight;
                                curAbilityElementOffset += 15;
                            }
                            var thisHeight = editorDATA.baseAbilityElementHeight + getRequirementsTotalHeight(realAbTreeIndex);
                            var thisRect = new Rect(0, curAbilityElementOffset, 480, thisHeight);
                            #if UNITY_2018
                            GUI.DrawTexture(thisRect, editorDATA.abilityElementBackground);
                            #endif
                            */
                            var abName = "";
                            var abIcon = editorDATA.abilityNullSprite;
                            if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.ability)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID = -1;
                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID != -1)
                                {
                                    var abilityREF = RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID, allAbilities);
                                    if (abilityREF != null)
                                    {
                                        abName = abilityREF._name;
                                        if (abilityREF.icon != null)
                                            abIcon = abilityREF.icon.texture;
                                    }
                                }
                                else
                                {
                                    abName = "EMPTY NODE";
                                }
                            }
                            else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.bonus)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID = -1;
                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID != -1)
                                {
                                    var bonusREF = RPGBuilderUtilities.GetBonusFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID, allBonuses);
                                    if (bonusREF != null)
                                    {
                                        abName = bonusREF._name;
                                        if (bonusREF.icon != null)
                                            abIcon = bonusREF.icon.texture;
                                    }
                                }
                                else
                                {
                                    abName = "EMPTY NODE";
                                }
                            }
                            else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.recipe)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID = -1;
                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID != -1)
                                {
                                    var recipeREF = RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID, allCraftingRecipes);

                                    if (recipeREF != null)
                                    {
                                        abName = recipeREF._name;
                                        if (recipeREF.icon != null)
                                            abIcon = recipeREF.icon.texture;
                                    }
                                }
                                else
                                {
                                    abName = "EMPTY NODE";
                                }
                            }
                            else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.resourceNode)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID = -1;
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID = -1;
                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID != -1)
                                {
                                    var resourceNodeREF = RPGBuilderUtilities.GetResourceNodeFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID, allResourceNodes);

                                    if (resourceNodeREF != null)
                                    {
                                        abName = resourceNodeREF._name;
                                        if (resourceNodeREF.icon != null)
                                            abIcon = resourceNodeREF.icon.texture;
                                    }
                                }
                                else
                                {
                                    abName = "EMPTY NODE";
                                }
                            }

                            GUILayout.Space(5);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(8);
                            GUILayout.Box(abIcon, GUILayout.Width(70), GUILayout.Height(70));
                            GUILayout.Space(20);
                            EditorGUILayout.BeginVertical();
                            
                            GUILayout.Label(abName, skin.GetStyle("ViewTitle"), GUILayout.Width(320), GUILayout.Height(30));

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(new GUIContent("Type", "What type of node is it?"), GUILayout.Width(40), GUILayout.Height(15));
                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType = (RPGTalentTree.TalentTreeNodeType)EditorGUILayout.EnumPopup(currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType, GUILayout.Width(75), GUILayout.Height(15));
                            GUILayout.Space(10);
                            GUILayout.Label(new GUIContent("Tier", "What tier is this ability part of?"), GUILayout.Width(40), GUILayout.Height(15));
                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].Tier = EditorGUILayout.IntField(currentlyViewedTalentTree.nodeList[realAbTreeIndex].Tier, GUILayout.Width(25));
                            GUILayout.Space(10);
                            GUILayout.Label(new GUIContent("Slot", "The slot of the ability in its tier. FROM 1 to 7"), GUILayout.Width(40), GUILayout.Height(15));
                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].Row = EditorGUILayout.IntField(currentlyViewedTalentTree.nodeList[realAbTreeIndex].Row, GUILayout.Width(25));
                            GUILayout.EndHorizontal();

                            if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.ability)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityREF = (RPGAbility)EditorGUILayout.ObjectField(currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityREF, typeof(RPGAbility), false, GUILayout.Width(320), GUILayout.Height(27));
                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityREF != null)
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityREF.ID;
                                else
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID = -1;
                            }
                            else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.bonus)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusREF = (RPGBonus)EditorGUILayout.ObjectField(currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusREF, typeof(RPGBonus), false, GUILayout.Width(320), GUILayout.Height(27));
                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusREF != null)
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusREF.ID;
                                else
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID = -1;
                            }
                            else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.recipe)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeREF = (RPGCraftingRecipe)EditorGUILayout.ObjectField(currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeREF, typeof(RPGCraftingRecipe), false, GUILayout.Width(320), GUILayout.Height(27));
                                if(currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeREF != null)
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeREF.ID;
                                else
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID = -1;
                            }
                            else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.resourceNode)
                            {
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeREF = (RPGResourceNode)EditorGUILayout.ObjectField(currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeREF, typeof(RPGResourceNode), false, GUILayout.Width(320), GUILayout.Height(27));
                                if(currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeREF != null)
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeREF.ID;
                                else
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID = -1;
                            }
                            GUILayout.EndVertical();

                            if (GUILayout.Button("X", skin.GetStyle("RemoveButtonSmall"), GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                currentlyViewedTalentTree.nodeList.RemoveAt(realAbTreeIndex);
                                return;
                            }

                            GUILayout.Space(8);
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(10);
                            EditorGUILayout.BeginHorizontal();

                            if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.ability && currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID == -1
                                || currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.recipe && currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID == -1
                                || currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.resourceNode && currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID == -1
                                || currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.bonus && currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID == -1)
                                EditorGUI.BeginDisabledGroup(true);
                            GUILayout.Label("REQUIREMENTS:", skin.GetStyle("CenteredLabel"), GUILayout.Width(150), GUILayout.Height(20));
                            GUILayout.Space(20);
                            if (GUILayout.Button("+", skin.GetStyle("AddButtonSmall"), GUILayout.Width(25), GUILayout.Height(25))) currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements.Add(new RequirementsManager.RequirementDATA());
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(15);

                            for (var a = 0; a < currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements.Count; a++)
                            {
                                /*var thisReqBaseHeight = curAbilityElementOffset + editorDATA.baseAbilityElementHeight;
                                if (a == 0)
                                    thisReqBaseHeight += 5;
                                else
                                    thisReqBaseHeight += getRequirementsBelowHeight(realAbTreeIndex, a);
                                var thisReqRect = new Rect(4, thisReqBaseHeight, 460, getRequirementsHeight(realAbTreeIndex, a));
                                #if UNITY_2018
                                GUI.DrawTexture(thisReqRect, editorDATA.abilityRequirementElementBackground);
                                #endif
                                */
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                                currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType = (RequirementsManager.RequirementType)EditorGUILayout.EnumPopup(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType, GUILayout.Width(150));
                                GUILayout.Space(250);
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements.RemoveAt(a);
                                    return;
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical();

                                if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements.Count > 0)
                                {
                                    if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.pointSpent)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].pointSpentValue = EditorGUILayout.IntField(new GUIContent("Points Spent", "How many points should already be spent in this tree for this bonus to be active?"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].pointSpentValue, GUILayout.Width(400));
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.classLevel)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The class level required"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classLevelValue, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType._class)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].classRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.skillLevel)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill required for this bonus to be active"), RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(400));
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The skill level required"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillLevelValue, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].skillRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.itemOwned)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredREF = (RPGItem)EditorGUILayout.ObjectField(new GUIContent("Item", "The item required"), RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredCount = EditorGUILayout.IntField(new GUIContent("Count", "The amount of items required"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredCount, GUILayout.Width(400));
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].consumeItem = EditorGUILayout.Toggle(new GUIContent("Consumed?", "Is this item consumed?"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].consumeItem);
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].itemRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.abilityKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredREF = (RPGAbility)EditorGUILayout.ObjectField(new GUIContent("Ability", "The ability required to be known for this bonus to be active"), RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.recipeKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].recipeRequiredREF = (RPGCraftingRecipe)EditorGUILayout.ObjectField(new GUIContent("Recipe", "The crafting recipe required to be known for this bonus to be active"), RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].craftingRecipeRequiredID, allCraftingRecipes), typeof(RPGCraftingRecipe), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].recipeRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].craftingRecipeRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].recipeRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].craftingRecipeRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.resourceNodeKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredREF = (RPGResourceNode)EditorGUILayout.ObjectField(new GUIContent("Resource Node", "The resource node required to be known for this bonus to be active"), RPGBuilderUtilities.GetResourceNodeFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredID, allResourceNodes), typeof(RPGResourceNode), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.bonusKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].bonusRequiredREF = (RPGBonus)EditorGUILayout.ObjectField(new GUIContent("Bonus", "The bonus required to be known for this bonus to be active"), RPGBuilderUtilities.GetBonusFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].bonusRequiredID, allBonuses), typeof(RPGBonus), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].bonusRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].bonusRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].bonusRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].bonusRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.race)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].raceRequiredREF = (RPGRace)EditorGUILayout.ObjectField(new GUIContent("Race", "The race required"), RPGBuilderUtilities.GetRaceFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].raceRequiredID, allRaces), typeof(RPGRace), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].raceRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].raceRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].raceRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].raceRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.questState)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questRequiredREF = (RPGQuest)EditorGUILayout.ObjectField(new GUIContent("Quest", "The quest required for this bonus to be active"), RPGBuilderUtilities.GetQuestFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questRequiredID, allQuests), typeof(RPGQuest), false, GUILayout.Width(400));
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questStateRequired = (QuestManager.questState)EditorGUILayout.EnumPopup(new GUIContent("State", "The required state of the quest"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questStateRequired, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].questRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.npcKilled)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcRequiredREF = (RPGNpc)EditorGUILayout.ObjectField(new GUIContent("NPC", "The NPC required to be killed"), RPGBuilderUtilities.GetNPCFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcRequiredID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(400));
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcKillsRequired = EditorGUILayout.IntField(new GUIContent("Kills", "How many times this NPC should have been killed for the bonus to be active"), currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcKillsRequired, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].npcRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.abilityNotKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredREF = (RPGAbility)EditorGUILayout.ObjectField(new GUIContent("Ability", "The ability required to not be known for this bonus to be active"), RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].abilityRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.recipeNotKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].recipeRequiredREF = (RPGCraftingRecipe)EditorGUILayout.ObjectField(new GUIContent("Recipe", "The crafting recipe required to not be known for this bonus to be active"), RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].craftingRecipeRequiredID, allCraftingRecipes), typeof(RPGCraftingRecipe), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].recipeRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].craftingRecipeRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].recipeRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].craftingRecipeRequiredID = -1;
                                    }
                                    else if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].requirementType == RequirementsManager.RequirementType.resourceNodeNotKnown)
                                    {
                                        currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredREF = (RPGResourceNode)EditorGUILayout.ObjectField(new GUIContent("Resource Node", "The resource node required to not be known for this bonus to be active"), RPGBuilderUtilities.GetResourceNodeFromIDEditor(currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredID, allResourceNodes), typeof(RPGResourceNode), false, GUILayout.Width(400));
                                        if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredREF != null)
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredID = currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredREF.ID;
                                        else
                                            currentlyViewedTalentTree.nodeList[realAbTreeIndex].requirements[a].resourceNodeRequiredID = -1;
                                    }
                                }
                                EditorGUILayout.EndVertical();

                                GUILayout.Space(10);
                            }

                            if (currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.ability && currentlyViewedTalentTree.nodeList[realAbTreeIndex].abilityID == -1
                                || currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.recipe && currentlyViewedTalentTree.nodeList[realAbTreeIndex].recipeID == -1
                                || currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.resourceNode && currentlyViewedTalentTree.nodeList[realAbTreeIndex].resourceNodeID == -1
                                || currentlyViewedTalentTree.nodeList[realAbTreeIndex].nodeType == RPGTalentTree.TalentTreeNodeType.bonus && currentlyViewedTalentTree.nodeList[realAbTreeIndex].bonusID == -1)
                                EditorGUI.EndDisabledGroup();
                            GUILayout.Space(15);
                        }

                        if (tempNodeLIst.Count > 1)
                            if (GUILayout.Button("ADD", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedTalentTree.nodeList.Add(new RPGTalentTree.Node_DATA());

                        serialObj.ApplyModifiedProperties();


                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.TalentTree, containerRect2);
    }


    private void DrawTreePointView()
    {
        if (currentlyViewedTreePoint == null)
        {
            if (allTreePoints.Count == 0)
            {
                CreateNew(AssetType.TreePoint);
                return;
            }
            currentlyViewedTreePoint = Instantiate(allTreePoints[0]) as RPGTreePoint;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "combat");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allTreePoints.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allTreePoints[i]._name;
                            newElementDATA.showIcon = true;
                            if (allTreePoints[i].icon != null) newElementDATA.texture = allTreePoints[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.TreePoint);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(440), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedTreePoint.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedTreePoint.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTreePoint.ID = EditorGUILayout.IntField(currentlyViewedTreePoint.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(nameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTreePoint._name = GUILayout.TextField(currentlyViewedTreePoint._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTreePoint._displayName = GUILayout.TextField(currentlyViewedTreePoint._displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(descriptionGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTreePoint.description = GUILayout.TextField(currentlyViewedTreePoint.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fileNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedTreePoint._fileName = GUILayout.TextField("RPG_TREE_POINT_" + currentlyViewedTreePoint._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("GENERAL VALUES", skin.GetStyle("ViewTitle"), GUILayout.Width(440), GUILayout.Height(40));
                        GUILayout.Space(10);

                        currentlyViewedTreePoint.startAmount = EditorGUILayout.IntField(new GUIContent("Starts At", "The amount of points given when creating the character"), currentlyViewedTreePoint.startAmount, GUILayout.Width(400));
                        currentlyViewedTreePoint.maxPoints = EditorGUILayout.IntField(new GUIContent("Max", "Maximum amount of this point type that the character can have"), currentlyViewedTreePoint.maxPoints, GUILayout.Width(400));

                        GUILayout.Space(10);
                        GUILayout.Label("GAIN REQUIREMENTS", skin.GetStyle("ViewTitle"), GUILayout.Width(440), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Requirement", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedTreePoint.gainPointRequirements.Add(new RPGTreePoint.GainRequirements());

                        ScriptableObject scriptableObj = currentlyViewedTreePoint;
                        var serialObj = new SerializedObject(scriptableObj);

                        var ThisList = serialObj.FindProperty("gainPointRequirements");
                        currentlyViewedTreePoint.gainPointRequirements = GetTargetObjectOfProperty(ThisList) as List<RPGTreePoint.GainRequirements>;

                        for (var a = 0; a < currentlyViewedTreePoint.gainPointRequirements.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedTreePoint.gainPointRequirements[a].gainType = (RPGTreePoint.TreePointGainRequirementTypes)EditorGUILayout.EnumPopup(currentlyViewedTreePoint.gainPointRequirements[a].gainType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedTreePoint.gainPointRequirements.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedTreePoint.gainPointRequirements.Count > 0)
                            {
                                if (currentlyViewedTreePoint.gainPointRequirements[a].gainType == RPGTreePoint.TreePointGainRequirementTypes.classLevelUp)
                                {
                                    currentlyViewedTreePoint.gainPointRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required to level up"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedTreePoint.gainPointRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                    if (currentlyViewedTreePoint.gainPointRequirements[a].classRequiredREF != null)
                                        currentlyViewedTreePoint.gainPointRequirements[a].classRequiredID = currentlyViewedTreePoint.gainPointRequirements[a].classRequiredREF.ID;
                                    else
                                        currentlyViewedTreePoint.gainPointRequirements[a].classRequiredID = -1;
                                    currentlyViewedTreePoint.gainPointRequirements[a].amountGained = EditorGUILayout.IntField(new GUIContent("Gain", "How many points should be gained"), currentlyViewedTreePoint.gainPointRequirements[a].amountGained, GUILayout.Width(400));
                                }
                                else if (currentlyViewedTreePoint.gainPointRequirements[a].gainType == RPGTreePoint.TreePointGainRequirementTypes.itemGained)
                                {
                                    currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredREF = (RPGItem)EditorGUILayout.ObjectField(new GUIContent("Item", "The item required"), RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                    if (currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredREF != null)
                                        currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredID = currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredREF.ID;
                                    else
                                        currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredID = -1;
                                    currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredCount = EditorGUILayout.IntField(new GUIContent("Stacks", "How many of this items is required?"), currentlyViewedTreePoint.gainPointRequirements[a].itemRequiredCount, GUILayout.Width(400));
                                    currentlyViewedTreePoint.gainPointRequirements[a].amountGained = EditorGUILayout.IntField(new GUIContent("Gain", "How many points should be gained"), currentlyViewedTreePoint.gainPointRequirements[a].amountGained, GUILayout.Width(400));
                                }
                                else if (currentlyViewedTreePoint.gainPointRequirements[a].gainType == RPGTreePoint.TreePointGainRequirementTypes.npcKilled)
                                {
                                    currentlyViewedTreePoint.gainPointRequirements[a].npcRequiredREF = (RPGNpc)EditorGUILayout.ObjectField(new GUIContent("NPC", "The NPC Required to be killed"), RPGBuilderUtilities.GetNPCFromIDEditor(currentlyViewedTreePoint.gainPointRequirements[a].npcRequiredID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(400));
                                    if (currentlyViewedTreePoint.gainPointRequirements[a].npcRequiredREF != null)
                                        currentlyViewedTreePoint.gainPointRequirements[a].npcRequiredID = currentlyViewedTreePoint.gainPointRequirements[a].npcRequiredREF.ID;
                                    else
                                        currentlyViewedTreePoint.gainPointRequirements[a].npcRequiredID = -1;
                                    currentlyViewedTreePoint.gainPointRequirements[a].amountGained = EditorGUILayout.IntField(new GUIContent("Gain", "How many points should be gained"), currentlyViewedTreePoint.gainPointRequirements[a].amountGained, GUILayout.Width(400));
                                }
                                else if (currentlyViewedTreePoint.gainPointRequirements[a].gainType == RPGTreePoint.TreePointGainRequirementTypes.skillLevelUp)
                                {
                                    currentlyViewedTreePoint.gainPointRequirements[a].skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill required to level up"), RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedTreePoint.gainPointRequirements[a].skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(400));
                                    if (currentlyViewedTreePoint.gainPointRequirements[a].skillRequiredREF != null)
                                        currentlyViewedTreePoint.gainPointRequirements[a].skillRequiredID = currentlyViewedTreePoint.gainPointRequirements[a].skillRequiredREF.ID;
                                    else
                                        currentlyViewedTreePoint.gainPointRequirements[a].skillRequiredID = -1;
                                    currentlyViewedTreePoint.gainPointRequirements[a].amountGained = EditorGUILayout.IntField(new GUIContent("Gain", "How many points should be gained"), currentlyViewedTreePoint.gainPointRequirements[a].amountGained, GUILayout.Width(400));
                                }
                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);
                        }

                        serialObj.ApplyModifiedProperties();


                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.TreePoint, containerRect2);
    }

    public object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }

        return obj;
    }
    private object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (var i = 0; i <= index; i++)
            if (!enm.MoveNext()) return null;
        return enm.Current;
    }

    private void DrawStatView()
    {
        if (currentlyViewedStat == null)
        {
            if (allStats.Count == 0)
            {
                CreateNew(AssetType.Stat);
                return;
            }
            currentlyViewedStat = Instantiate(allStats[0]) as RPGStat;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));
        
        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "combat");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allStats.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allStats[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Stat);
                        break;
                        
                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedStat.ID = EditorGUILayout.IntField("ID", currentlyViewedStat.ID);
                        EditorGUI.EndDisabledGroup();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(nameGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedStat._name = GUILayout.TextField(currentlyViewedStat._name, GUILayout.Width(278), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedStat.displayName = GUILayout.TextField(currentlyViewedStat.displayName, GUILayout.Width(278), GUILayout.Height(15));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(descriptionGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedStat.description = GUILayout.TextField(currentlyViewedStat.description, GUILayout.Width(278), GUILayout.Height(15));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fileNameGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedStat._fileName = GUILayout.TextField("RPG_STAT_" + currentlyViewedStat._name, GUILayout.Width(278), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();


                        currentlyViewedStat.statType = (RPGStat.STAT_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Type", "The Stat Type, this will be used in the game logic to trigger some specific actions based on its type."), currentlyViewedStat.statType, GUILayout.Width(430));

                        var currentStatCategoryIndex = getIndexFromName("StatUICategory", currentlyViewedStat.StatUICategory);
                        var tempIndex3 = 0;
                        tempIndex3 = EditorGUILayout.Popup(new GUIContent("UI Category", "Where in the Stat UI should this be displayed"), currentStatCategoryIndex, combatSettings.UIStatsCategories);
                        currentlyViewedStat.StatUICategory = GetStatCategoryNameFromID(tempIndex3);

                        //currentlyViewedStat.playerOnly = EditorGUILayout.Toggle(new GUIContent("Player Only", "Is this Stat for players only?"), currentlyViewedStat.playerOnly);
                        if (currentlyViewedStat.statType == RPGStat.STAT_TYPE.DAMAGE || currentlyViewedStat.statType == RPGStat.STAT_TYPE.HEALING
                             || currentlyViewedStat.statType == RPGStat.STAT_TYPE.BASE_DAMAGE_TYPE)
                        {
                            var currentStatFunctionIndex = getIndexFromName("StatFunction", currentlyViewedStat.StatFunction);
                            var tempIndex = 0;
                            tempIndex = EditorGUILayout.Popup(new GUIContent("Function", "The function of this Stat, will be checked in the game logic to execute the function"), currentStatFunctionIndex, combatSettings.StatFunctions);
                            currentlyViewedStat.StatFunction = GetStatFunctionNameFromID(tempIndex);
                        }
                        if (currentlyViewedStat.statType == RPGStat.STAT_TYPE.DAMAGE || currentlyViewedStat.statType == RPGStat.STAT_TYPE.RESISTANCE
                             || currentlyViewedStat.statType == RPGStat.STAT_TYPE.BASE_DAMAGE_TYPE)
                        {
                            var correctOppositeStatList = getCorrectOppositeStatsList(currentlyViewedStat.statType);
                            var currentOppositeStatIndex = GetIndexFromStatName(correctOppositeStatList, currentlyViewedStat.OppositeStat);
                            var tempIndex2 = 0;
                            tempIndex2 = EditorGUILayout.Popup(new GUIContent("Opposite Stat", "If not empty, the opposite stat will be used to counter this main stat, such as Damage/Resistance"), currentOppositeStatIndex, correctOppositeStatList);
                            currentlyViewedStat.OppositeStat = correctOppositeStatList[tempIndex2];
                        }
                        if (currentlyViewedStat.statType == RPGStat.STAT_TYPE.VITALITY)
                        {
                            currentlyViewedStat.isShifting = EditorGUILayout.Toggle(new GUIContent("Is Shifting?", "Is this stat shifting?"), currentlyViewedStat.isShifting);
                            currentlyViewedStat.shiftAmount = EditorGUILayout.FloatField(new GUIContent("Shift Amount", "The amount that will be shifted"), currentlyViewedStat.shiftAmount);
                            currentlyViewedStat.shiftInterval = EditorGUILayout.FloatField(new GUIContent("Shift Interval", "The duration between each shift"), currentlyViewedStat.shiftInterval);
                        }
                        currentlyViewedStat.minCheck = EditorGUILayout.Toggle("Check. Min", currentlyViewedStat.minCheck);
                        if (currentlyViewedStat.minCheck) currentlyViewedStat.minValue = EditorGUILayout.FloatField(new GUIContent("Value. Min", "The minimum value this stat can get to"), currentlyViewedStat.minValue);
                        currentlyViewedStat.maxCheck = EditorGUILayout.Toggle("Check. Max", currentlyViewedStat.maxCheck);
                        if (currentlyViewedStat.maxCheck) currentlyViewedStat.maxValue = EditorGUILayout.FloatField(new GUIContent("Value. Max", "The maximum value this stat can get to"), currentlyViewedStat.maxValue);

                        currentlyViewedStat.baseValue = EditorGUILayout.FloatField(new GUIContent("Value. Base", "The initial value of the stat"), currentlyViewedStat.baseValue);
                        currentlyViewedStat.bonusPerLevel = EditorGUILayout.FloatField(new GUIContent("Level Bonus", "For each level, extra amount gained"), currentlyViewedStat.bonusPerLevel);

                        if (currentlyViewedStat.statType == RPGStat.STAT_TYPE.VITALITY_REGEN)
                        {
                            currentlyViewedStat.vitalityRegenBonusStatREF =
                                (RPGStat) EditorGUILayout.ObjectField("Stat Affected By Bonus:",
                                    RPGBuilderUtilities.GetStatFromIDEditor(
                                        currentlyViewedStat.vitalityRegenBonusStatID, allStats),
                                    typeof(RPGStat), false, GUILayout.Width(400));
                            if (currentlyViewedStat.vitalityRegenBonusStatREF != null)
                                currentlyViewedStat.vitalityRegenBonusStatID =
                                    currentlyViewedStat.vitalityRegenBonusStatREF.ID;
                            else
                                currentlyViewedStat.vitalityRegenBonusStatID = -1;
                        } else if (currentlyViewedStat.statType == RPGStat.STAT_TYPE.EFFECT_TRIGGER)
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("EFFECTS", skin.GetStyle("ViewTitle"), GUILayout.Width(450),
                                GUILayout.Height(40));
                            GUILayout.Space(10);

                            if (GUILayout.Button("+ Add Effect", skin.GetStyle("AddButton"), GUILayout.Width(440),
                                GUILayout.Height(25)))
                                currentlyViewedStat.onHitEffectsData.Add(new RPGStat.OnHitEffectsData());

                            
                            ScriptableObject scriptableObj = currentlyViewedStat;
                            var serialObj = new SerializedObject(scriptableObj);
                            var ThisList5 = serialObj.FindProperty("onHitEffectsData");
                            currentlyViewedStat.onHitEffectsData =
                                GetTargetObjectOfProperty(ThisList5) as List<RPGStat.OnHitEffectsData>;

                            for (var a = 0; a < currentlyViewedStat.onHitEffectsData.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20),
                                    GUILayout.Height(20)))
                                {
                                    currentlyViewedStat.onHitEffectsData.RemoveAt(a);
                                    return;
                                }

                                EditorGUILayout.LabelField("Visual:" + requirementNumber + ":", GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                currentlyViewedStat.onHitEffectsData[a].targetType =
                                    (RPGCombatDATA.TARGET_TYPE) EditorGUILayout.EnumPopup("Target Type:", currentlyViewedStat
                                        .onHitEffectsData[a].targetType, GUILayout.Width(400));
                                currentlyViewedStat.onHitEffectsData[a].tagType =
                                    (RPGAbility.ABILITY_TAGS) EditorGUILayout.EnumPopup("When:", currentlyViewedStat
                                        .onHitEffectsData[a].tagType, GUILayout.Width(400));
                                currentlyViewedStat.onHitEffectsData[a].effectREF =
                                    (RPGEffect) EditorGUILayout.ObjectField("Effect:",
                                        RPGBuilderUtilities.GetEffectFromIDEditor(
                                            currentlyViewedStat.onHitEffectsData[a].effectID, allEffects),
                                        typeof(RPGEffect), false, GUILayout.Width(400));
                                if (currentlyViewedStat.onHitEffectsData[a].effectREF != null)
                                    currentlyViewedStat.onHitEffectsData[a].effectID =
                                        currentlyViewedStat.onHitEffectsData[a].effectREF.ID;
                                else
                                    currentlyViewedStat.onHitEffectsData[a].effectID = -1;

                                currentlyViewedStat.onHitEffectsData[a].chance = EditorGUILayout.Slider("Chance", currentlyViewedStat.onHitEffectsData[a].chance, 0f, 100f, GUILayout.Width(400));
                                GUILayout.Space(10);
                            }
                        }


                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Stat, containerRect2);
    }

    private string[] getCorrectOppositeStatsList(RPGStat.STAT_TYPE type)
    {

        var oppositeType = RPGStat.STAT_TYPE.NONE;
        if(type == RPGStat.STAT_TYPE.DAMAGE)
            oppositeType = RPGStat.STAT_TYPE.RESISTANCE;
        else if (type == RPGStat.STAT_TYPE.RESISTANCE)
            oppositeType = RPGStat.STAT_TYPE.PENETRATION;
        else if (type == RPGStat.STAT_TYPE.PENETRATION)
            oppositeType = RPGStat.STAT_TYPE.RESISTANCE;
        else if (type == RPGStat.STAT_TYPE.HEALING)
            oppositeType = RPGStat.STAT_TYPE.ABSORBTION;
        else if (type == RPGStat.STAT_TYPE.CC_POWER)
            oppositeType = RPGStat.STAT_TYPE.CC_RESISTANCE;
        else if (type == RPGStat.STAT_TYPE.CC_RESISTANCE)
            oppositeType = RPGStat.STAT_TYPE.CC_POWER;
        else if (type == RPGStat.STAT_TYPE.DMG_DEALT)
            oppositeType = RPGStat.STAT_TYPE.DMG_TAKEN;
        else if (type == RPGStat.STAT_TYPE.DMG_TAKEN)
            oppositeType = RPGStat.STAT_TYPE.DMG_DEALT;
        else if (type == RPGStat.STAT_TYPE.HEAL_RECEIVED)
            oppositeType = RPGStat.STAT_TYPE.HEAL_DONE;
        else if (type == RPGStat.STAT_TYPE.HEAL_DONE)
            oppositeType = RPGStat.STAT_TYPE.HEAL_RECEIVED;
        else if (type == RPGStat.STAT_TYPE.BASE_DAMAGE_TYPE)
            oppositeType = RPGStat.STAT_TYPE.BASE_RESISTANCE_TYPE;
        else if (type == RPGStat.STAT_TYPE.BASE_RESISTANCE_TYPE) oppositeType = RPGStat.STAT_TYPE.BASE_DAMAGE_TYPE;

        var statList = new List<string>();
        statList.Add("NONE");
        for (var i = 0; i < allStats.Count; i++)
            if (allStats[i].statType == oppositeType) statList.Add(allStats[i]._name);
        return statList.ToArray();
    }

    private string GetStatFunctionNameFromID (int Index)
    {
        return combatSettings.StatFunctions[Index];
    }

    private string GetStatCategoryNameFromID(int Index)
    {
        if (Index != -1 && Index != 0)
            return combatSettings.UIStatsCategories[Index];
        else
            return "";
    }


    private int getIndexFromName(string dataType, string curName)
    {
        switch (dataType)
        {
            case "StatFunction":
                for (var i = 0; i < combatSettings.StatFunctions.Length; i++)
                    if (combatSettings.StatFunctions[i] == curName) return i;
                return 0;
                
            case "States":
                for (var i = 0; i < combatSettings.States.Length; i++)
                    if (combatSettings.States[i] == curName) return i;
                return 0;

            case "ItemType":
                for (var i = 0; i < itemSettings.itemType.Length; i++)
                    if (itemSettings.itemType[i] == curName) return i;
                return 0;

            case "ItemQuality":
                for (var i = 0; i < itemSettings.itemQuality.Length; i++)
                    if (itemSettings.itemQuality[i] == curName) return i;
                return 0;

            case "WeaponType":
                for (var i = 0; i < itemSettings.weaponType.Length; i++)
                    if (itemSettings.weaponType[i] == curName) return i;
                return 0;

            case "ArmorType":
                for (var i = 0; i < itemSettings.armorType.Length; i++)
                    if (itemSettings.armorType[i] == curName) return i;
                return 0;

            case "ArmorSlots":
                for (var i = 0; i < itemSettings.armorSlots.Length; i++)
                    if (itemSettings.armorSlots[i] == curName) return i;
                return 0;

            case "WeaponSlots":
                for (var i = 0; i < itemSettings.weaponSlots.Length; i++)
                    if (itemSettings.weaponSlots[i] == curName) return i;
                return 0;

            case "SlotType":
                for (var i = 0; i < itemSettings.slotType.Length; i++)
                    if (itemSettings.slotType[i] == curName) return i;
                return 0;

            case "StatUICategory":
                for (var i = 0; i < combatSettings.UIStatsCategories.Length; i++)
                    if (combatSettings.UIStatsCategories[i] == curName) return i;
                return 0;

            default:
                return -1;
        }
    }

    private int GetIndexFromStatName(string[] statList, string curName)
    {
        for (var i = 0; i < statList.Length; i++)
            if (statList[i] == curName) return i;
        return 0;
    }

    private string GetSecondaryDamageNameFromID(string[] statList, int Index)
    {
        return statList[Index];
    }

    private int GetIndexFromSecondaryDamageName(string[] statList, string curName)
    {
        for (var i = 0; i < statList.Length; i++)
            if (statList[i] == curName) return i;
        return 0;
    }

    private string[] getCorrectStatsList (RPGStat.STAT_TYPE type)
    {
        var statList = new List<string>();
        statList.Add("NONE");
        for (var i = 0; i < allStats.Count; i++)
            if(allStats[i].statType == type) statList.Add(allStats[i]._name);
        return statList.ToArray();
    }


    private void DrawSkillView()
    {
        if (currentlyViewedSkill == null)
        {
            if (allSkills.Count == 0)
            {
                CreateNew(AssetType.Skill);
                return;
            }
            currentlyViewedSkill = Instantiate(allSkills[0]) as RPGSkill;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allSkills.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allSkills[i]._name;
                            newElementDATA.showIcon = true;
                            if (allSkills[i].icon != null) newElementDATA.texture = allSkills[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Skill);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedSkill.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedSkill.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedSkill.ID = EditorGUILayout.IntField(currentlyViewedSkill.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedSkill._name = GUILayout.TextField(currentlyViewedSkill._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedSkill.displayName = GUILayout.TextField(currentlyViewedSkill.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedSkill._fileName = GUILayout.TextField("RPG_SKILL_" + currentlyViewedSkill._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedSkill.description = GUILayout.TextField(currentlyViewedSkill.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        
                        currentlyViewedSkill.levelTemplateREF = (RPGLevelsTemplate)EditorGUILayout.ObjectField("Levels Template", RPGBuilderUtilities.GetLevelTemplateFromIDEditor(currentlyViewedSkill.levelTemplateID, allLevelsTemplate), typeof(RPGLevelsTemplate), false);
                        if (currentlyViewedSkill.levelTemplateREF != null)
                            currentlyViewedSkill.levelTemplateID = currentlyViewedSkill.levelTemplateREF.ID;
                        else
                            currentlyViewedSkill.levelTemplateID = -1;

                        currentlyViewedSkill.automaticlyAdded = EditorGUILayout.Toggle("Automatically learn?", currentlyViewedSkill.automaticlyAdded);

                        ScriptableObject scriptableObj = currentlyViewedSkill;
                        var serialObj = new SerializedObject(scriptableObj);

                        GUILayout.Space(10);
                        GUILayout.Label("TALENT TREES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Talent Tree", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedSkill.talentTrees.Add(new RPGSkill.TalentTreesDATA());

                        var ThisList2 = serialObj.FindProperty("talentTrees");
                        currentlyViewedSkill.talentTrees = GetTargetObjectOfProperty(ThisList2) as List<RPGSkill.TalentTreesDATA>;

                        for (var a = 0; a < currentlyViewedSkill.talentTrees.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedSkill.talentTrees.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedSkill.talentTrees[a].talentTreeREF != null) talentTreeName = currentlyViewedSkill.talentTrees[a].talentTreeREF.displayName;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("TALENT TREE", GUILayout.Width(100));
                            currentlyViewedSkill.talentTrees[a].talentTreeREF = (RPGTalentTree)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetTalentTreeFromIDEditor(currentlyViewedSkill.talentTrees[a].talentTreeID, allTalentTrees), typeof(RPGTalentTree), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedSkill.talentTrees[a].talentTreeREF != null)
                                currentlyViewedSkill.talentTrees[a].talentTreeID = currentlyViewedSkill.talentTrees[a].talentTreeREF.ID;
                            else
                                currentlyViewedSkill.talentTrees[a].talentTreeID = -1;
                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("STARTING ITEMS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedSkill.startItems.Add(new RPGItemDATA.StartingItemsDATA());

                        var ThisList4 = serialObj.FindProperty("startItems");
                        currentlyViewedSkill.startItems = GetTargetObjectOfProperty(ThisList4) as List<RPGItemDATA.StartingItemsDATA>;

                        for (var a = 0; a < currentlyViewedSkill.startItems.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedSkill.startItems.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedSkill.startItems[a].itemREF != null) talentTreeName = currentlyViewedSkill.startItems[a].itemREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("ITEM", GUILayout.Width(100));
                            currentlyViewedSkill.startItems[a].itemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedSkill.startItems[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Count", GUILayout.Width(100));
                            currentlyViewedSkill.startItems[a].count = EditorGUILayout.IntField(currentlyViewedSkill.startItems[a].count);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Equipped?", GUILayout.Width(100));
                            currentlyViewedSkill.startItems[a].equipped = EditorGUILayout.Toggle(currentlyViewedSkill.startItems[a].equipped);
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedSkill.startItems[a].itemREF != null)
                                currentlyViewedSkill.startItems[a].itemID = currentlyViewedSkill.startItems[a].itemREF.ID;
                            else
                                currentlyViewedSkill.startItems[a].itemID = -1;
                            GUILayout.Space(10);
                        }
                        
                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Skill, containerRect2);
    }

    private void DrawRaceView()
    {
        if (currentlyViewedRace == null)
        {
            if (allRaces.Count == 0)
            {
                CreateNew(AssetType.Race);
                return;
            }
            currentlyViewedRace = Instantiate(allRaces[0]) as RPGRace;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allRaces.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allRaces[i]._name;
                            newElementDATA.showIcon = true;
                            if (allRaces[i].icon != null) newElementDATA.texture = allRaces[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Race);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedRace.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedRace.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedRace.ID = EditorGUILayout.IntField(currentlyViewedRace.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedRace._name = GUILayout.TextField(currentlyViewedRace._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedRace.displayName = GUILayout.TextField(currentlyViewedRace.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedRace._fileName = GUILayout.TextField("RPG_RACE_" + currentlyViewedRace._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedRace.description = GUILayout.TextField(currentlyViewedRace.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("VISUAL AND START", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedRace.malePrefab = (GameObject)EditorGUILayout.ObjectField("Male Prefab", currentlyViewedRace.malePrefab, typeof(GameObject), false);
                        currentlyViewedRace.femalePrefab = (GameObject)EditorGUILayout.ObjectField("Female Prefab", currentlyViewedRace.femalePrefab, typeof(GameObject), false);

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Starting Scene:", GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedRace.startingSceneREF = (RPGGameScene)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetGameSceneFromIDEditor(currentlyViewedRace.startingSceneID, allGameScenes), typeof(RPGGameScene), false, GUILayout.Width(278), GUILayout.Height(15));
                        if(currentlyViewedRace.startingSceneREF != null)
                            currentlyViewedRace.startingSceneID = currentlyViewedRace.startingSceneREF.ID;
                        else
                            currentlyViewedRace.startingSceneID = -1;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Position:", GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedRace.startingPositionREF = (RPGWorldPosition)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetWorldPositionFromIDEditor(currentlyViewedRace.startingPositionID, allWorldPositions), typeof(RPGWorldPosition), false, GUILayout.Width(278), GUILayout.Height(15));
                        if (currentlyViewedRace.startingPositionREF != null)
                            currentlyViewedRace.startingPositionID = currentlyViewedRace.startingPositionREF.ID;
                        else
                            currentlyViewedRace.startingPositionID = -1;
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("CLASSES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        ScriptableObject scriptableObj = currentlyViewedRace;
                        var serialObj = new SerializedObject(scriptableObj);

                        if (GUILayout.Button("+ Add Class", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedRace.availableClasses.Add(new RPGRace.RACE_CLASSES_DATA());

                        var ThisList5 = serialObj.FindProperty("availableClasses");
                        currentlyViewedRace.availableClasses = GetTargetObjectOfProperty(ThisList5) as List<RPGRace.RACE_CLASSES_DATA>;

                        for (var a = 0; a < currentlyViewedRace.availableClasses.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedRace.availableClasses.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedRace.availableClasses[a].classREF != null) talentTreeName = currentlyViewedRace.availableClasses[a].classREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("CLASS", GUILayout.Width(100));
                            currentlyViewedRace.availableClasses[a].classREF = (RPGClass)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedRace.availableClasses[a].classID, allClasses), typeof(RPGClass), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedRace.availableClasses[a].classREF != null)
                                currentlyViewedRace.availableClasses[a].classID = currentlyViewedRace.availableClasses[a].classREF.ID;
                            else
                                currentlyViewedRace.availableClasses[a].classID = -1;
                            GUILayout.Space(10);
                        }


                        GUILayout.Space(10);
                        GUILayout.Label("STATS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Stat", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedRace.stats.Add(new RPGRace.RACE_STATS_DATA());

                        var ThisList3 = serialObj.FindProperty("stats");
                        currentlyViewedRace.stats = GetTargetObjectOfProperty(ThisList3) as List<RPGRace.RACE_STATS_DATA>;

                        for (var a = 0; a < currentlyViewedRace.stats.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedRace.stats.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedRace.stats[a].statREF != null) talentTreeName = currentlyViewedRace.stats[a].statREF.displayName;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("STAT", GUILayout.Width(100));
                            currentlyViewedRace.stats[a].statREF = (RPGStat)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetStatFromIDEditor(currentlyViewedRace.stats[a].statID, allStats), typeof(RPGStat), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Value", GUILayout.Width(100));
                            currentlyViewedRace.stats[a].amount = EditorGUILayout.FloatField(currentlyViewedRace.stats[a].amount);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Is Percent?", GUILayout.Width(100));
                            currentlyViewedRace.stats[a].isPercent = EditorGUILayout.Toggle(currentlyViewedRace.stats[a].isPercent);
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedRace.stats[a].statREF != null)
                                currentlyViewedRace.stats[a].statID = currentlyViewedRace.stats[a].statREF.ID;
                            else
                                currentlyViewedRace.stats[a].statID = -1;
                            GUILayout.Space(10);
                        }
                        
                        GUILayout.Space(10);
                        GUILayout.Label("STARTING ITEMS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedRace.startItems.Add(new RPGItemDATA.StartingItemsDATA());

                        var ThisList4 = serialObj.FindProperty("startItems");
                        currentlyViewedRace.startItems = GetTargetObjectOfProperty(ThisList4) as List<RPGItemDATA.StartingItemsDATA>;

                        for (var a = 0; a < currentlyViewedRace.startItems.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedRace.startItems.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedRace.startItems[a].itemREF != null) talentTreeName = currentlyViewedRace.startItems[a].itemREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("ITEM", GUILayout.Width(100));
                            currentlyViewedRace.startItems[a].itemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedRace.startItems[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Count", GUILayout.Width(100));
                            currentlyViewedRace.startItems[a].count = EditorGUILayout.IntField(currentlyViewedRace.startItems[a].count);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Equipped?", GUILayout.Width(100));
                            currentlyViewedRace.startItems[a].equipped = EditorGUILayout.Toggle(currentlyViewedRace.startItems[a].equipped);
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedRace.startItems[a].itemREF != null)
                                currentlyViewedRace.startItems[a].itemID = currentlyViewedRace.startItems[a].itemREF.ID;
                            else
                                currentlyViewedRace.startItems[a].itemID = -1;
                            GUILayout.Space(10);
                        }

                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Race, containerRect2);
    }

    private void DrawClassView()
    {
        if (currentlyViewedClass == null)
        {
            if (allClasses.Count == 0)
            {
                CreateNew(AssetType.Class);
                return;
            }
            currentlyViewedClass = Instantiate(allClasses[0]) as RPGClass;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allClasses.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allClasses[i]._name;
                            newElementDATA.showIcon = true;
                            if (allClasses[i].icon != null) newElementDATA.texture = allClasses[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }

                        DrawElementList(curElementList, AssetType.Class);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedClass.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedClass.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedClass.ID = EditorGUILayout.IntField(currentlyViewedClass.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedClass._name = GUILayout.TextField(currentlyViewedClass._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedClass.displayName = GUILayout.TextField(currentlyViewedClass.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedClass._fileName = GUILayout.TextField("RPG_CLASS_" + currentlyViewedClass._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedClass.description = GUILayout.TextField(currentlyViewedClass.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("LEVELS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedClass.levelTemplateREF = (RPGLevelsTemplate)EditorGUILayout.ObjectField("Levels Template", RPGBuilderUtilities.GetLevelTemplateFromIDEditor(currentlyViewedClass.levelTemplateID, allLevelsTemplate), typeof(RPGLevelsTemplate), false);
                        if (currentlyViewedClass.levelTemplateREF != null)
                            currentlyViewedClass.levelTemplateID = currentlyViewedClass.levelTemplateREF.ID;
                        else
                            currentlyViewedClass.levelTemplateID = -1;

                        GUILayout.Space(10);
                        GUILayout.Label("STATS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        ScriptableObject scriptableObj = currentlyViewedClass;
                        var serialObj = new SerializedObject(scriptableObj);

                        if (GUILayout.Button("+ Add Stat", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedClass.stats.Add(new RPGClass.CLASS_STATS_DATA());

                        var ThisList3 = serialObj.FindProperty("stats");
                        currentlyViewedClass.stats = GetTargetObjectOfProperty(ThisList3) as List<RPGClass.CLASS_STATS_DATA>;

                        for (var a = 0; a < currentlyViewedClass.stats.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedClass.stats.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedClass.stats[a].statREF != null) talentTreeName = currentlyViewedClass.stats[a].statREF.displayName;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("STAT", GUILayout.Width(100));
                            currentlyViewedClass.stats[a].statREF = (RPGStat)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetStatFromIDEditor(currentlyViewedClass.stats[a].statID, allStats), typeof(RPGStat), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Value", GUILayout.Width(100));
                            currentlyViewedClass.stats[a].amount = EditorGUILayout.FloatField(currentlyViewedClass.stats[a].amount);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Is Percent?", GUILayout.Width(100));
                            currentlyViewedClass.stats[a].isPercent = EditorGUILayout.Toggle(currentlyViewedClass.stats[a].isPercent);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Level Gain", GUILayout.Width(100));
                            currentlyViewedClass.stats[a].bonusPerLevel = EditorGUILayout.FloatField(currentlyViewedClass.stats[a].bonusPerLevel);
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedClass.stats[a].statREF != null)
                                currentlyViewedClass.stats[a].statID = currentlyViewedClass.stats[a].statREF.ID;
                            else
                                currentlyViewedClass.stats[a].statID = -1;
                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("TALENT TREES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Talent Tree", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedClass.talentTrees.Add(new RPGClass.TalentTreesDATA());

                        var ThisList2 = serialObj.FindProperty("talentTrees");
                        currentlyViewedClass.talentTrees = GetTargetObjectOfProperty(ThisList2) as List<RPGClass.TalentTreesDATA>;

                        for (var a = 0; a < currentlyViewedClass.talentTrees.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedClass.talentTrees.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedClass.talentTrees[a].talentTreeREF != null) talentTreeName = currentlyViewedClass.talentTrees[a].talentTreeREF.displayName;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("TALENT TREE", GUILayout.Width(100));
                            currentlyViewedClass.talentTrees[a].talentTreeREF = (RPGTalentTree)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetTalentTreeFromIDEditor(currentlyViewedClass.talentTrees[a].talentTreeID, allTalentTrees), typeof(RPGTalentTree), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedClass.talentTrees[a].talentTreeREF != null)
                                currentlyViewedClass.talentTrees[a].talentTreeID = currentlyViewedClass.talentTrees[a].talentTreeREF.ID;
                            else
                                currentlyViewedClass.talentTrees[a].talentTreeID = -1;
                            GUILayout.Space(10);
                        }
                        
                        GUILayout.Space(10);
                        GUILayout.Label("COMBAT", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Auto Attack", GUILayout.Width(100));
                        currentlyViewedClass.autoAttackAbilityREF = (RPGAbility)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedClass.autoAttackAbilityID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(250));
                        if (currentlyViewedClass.autoAttackAbilityREF != null)
                            currentlyViewedClass.autoAttackAbilityID = currentlyViewedClass.autoAttackAbilityREF.ID;
                        else
                            currentlyViewedClass.autoAttackAbilityID = -1;
                        EditorGUILayout.EndHorizontal();
                        
                        GUILayout.Space(10);
                        GUILayout.Label("STARTING ITEMS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedClass.startItems.Add(new RPGItemDATA.StartingItemsDATA());

                        var ThisList4 = serialObj.FindProperty("startItems");
                        currentlyViewedClass.startItems = GetTargetObjectOfProperty(ThisList4) as List<RPGItemDATA.StartingItemsDATA>;

                        for (var a = 0; a < currentlyViewedClass.startItems.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedClass.startItems.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedClass.startItems[a].itemREF != null) talentTreeName = currentlyViewedClass.startItems[a].itemREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("ITEM", GUILayout.Width(100));
                            currentlyViewedClass.startItems[a].itemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedClass.startItems[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Count", GUILayout.Width(100));
                            currentlyViewedClass.startItems[a].count = EditorGUILayout.IntField(currentlyViewedClass.startItems[a].count);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Equipped?", GUILayout.Width(100));
                            currentlyViewedClass.startItems[a].equipped = EditorGUILayout.Toggle(currentlyViewedClass.startItems[a].equipped);
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedClass.startItems[a].itemREF != null)
                                currentlyViewedClass.startItems[a].itemID = currentlyViewedClass.startItems[a].itemREF.ID;
                            else
                                currentlyViewedClass.startItems[a].itemID = -1;
                            GUILayout.Space(10);
                        }
                        
                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Class, containerRect2);
    }

    private void DrawLootTableView()
    {
        if (currentlyViewedLootTable == null)
        {
            if (allLootTables.Count == 0)
            {
                CreateNew(AssetType.LootTable);
                return;
            }
            currentlyViewedLootTable = Instantiate(allLootTables[0]) as RPGLootTable;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allLootTables.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allLootTables[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.LootTable);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedLootTable.ID = EditorGUILayout.IntField(currentlyViewedLootTable.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedLootTable._name = GUILayout.TextField(currentlyViewedLootTable._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedLootTable._fileName = GUILayout.TextField("RPG_LOOT_TABLE_" + currentlyViewedLootTable._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("ITEMS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        
                        ScriptableObject scriptableObj = currentlyViewedLootTable;
                        var serialObj = new SerializedObject(scriptableObj);

                        if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedLootTable.lootItems.Add(new RPGLootTable.LOOT_ITEMS());

                        var ThisList2 = serialObj.FindProperty("lootItems");
                        currentlyViewedLootTable.lootItems = GetTargetObjectOfProperty(ThisList2) as List<RPGLootTable.LOOT_ITEMS>;

                        for (var a = 0; a < currentlyViewedLootTable.lootItems.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedLootTable.lootItems.RemoveAt(a);
                                return;
                            }
                            var effectName = "";
                            if (currentlyViewedLootTable.lootItems[a].itemREF != null) effectName = currentlyViewedLootTable.lootItems[a].itemREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Item", GUILayout.Width(100));
                            currentlyViewedLootTable.lootItems[a].itemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedLootTable.lootItems[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedLootTable.lootItems[a].itemREF != null)
                                currentlyViewedLootTable.lootItems[a].itemID = currentlyViewedLootTable.lootItems[a].itemREF.ID;
                            else
                                currentlyViewedLootTable.lootItems[a].itemID = -1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Chance", GUILayout.Width(100));
                            currentlyViewedLootTable.lootItems[a].dropRate = EditorGUILayout.Slider(currentlyViewedLootTable.lootItems[a].dropRate, 0f, 100f, GUILayout.Width(250));
                            GUILayout.Space(10);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Min.", GUILayout.Width(100));
                            currentlyViewedLootTable.lootItems[a].min = EditorGUILayout.IntField(currentlyViewedLootTable.lootItems[a].min, GUILayout.Width(250));
                            GUILayout.Space(10);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Max.", GUILayout.Width(100));
                            currentlyViewedLootTable.lootItems[a].max = EditorGUILayout.IntField(currentlyViewedLootTable.lootItems[a].max, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(10);
                        }

                        if (currentlyViewedLootTable.lootItems.Count >= 3)
                            if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedLootTable.lootItems.Add(new RPGLootTable.LOOT_ITEMS());

                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.LootTable, containerRect2);
    }

    private void DrawWorldPositionView()
    {
        if (currentlyViewedWorldPosition == null)
        {
            if (allWorldPositions.Count == 0)
            {
                CreateNew(AssetType.WorldPosition);
                return;
            }
            currentlyViewedWorldPosition = Instantiate(allWorldPositions[0]) as RPGWorldPosition;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "world");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allWorldPositions.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allWorldPositions[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.WorldPosition);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedWorldPosition.ID = EditorGUILayout.IntField(currentlyViewedWorldPosition.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedWorldPosition._name = GUILayout.TextField(currentlyViewedWorldPosition._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedWorldPosition.displayName = GUILayout.TextField(currentlyViewedWorldPosition.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedWorldPosition._fileName = GUILayout.TextField("RPG_WORLD_POSITION_" + currentlyViewedWorldPosition._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("POSITION", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);


                        currentlyViewedWorldPosition.position = EditorGUILayout.Vector3Field("Position", currentlyViewedWorldPosition.position);

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.WorldPosition, containerRect2);
    }

    private void DrawGameSceneView()
    {
        if (currentlyViewedGameScene == null)
        {
            if (allGameScenes.Count == 0)
            {
                CreateNew(AssetType.GameScene);
                return;
            }
            currentlyViewedGameScene = Instantiate(allGameScenes[0]) as RPGGameScene;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "world");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allGameScenes.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allGameScenes[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.GameScene);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(440), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene.ID = EditorGUILayout.IntField(currentlyViewedGameScene.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(nameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene._name = GUILayout.TextField(currentlyViewedGameScene._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene.displayName = GUILayout.TextField(currentlyViewedGameScene.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(descriptionGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene.description = GUILayout.TextField(currentlyViewedGameScene.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fileNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedGameScene._fileName = GUILayout.TextField("RPG_GAME_SCENE_" + currentlyViewedGameScene._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        GUILayout.Space(10);
                        GUILayout.Label("MINIMAP SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Loading Image", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene.loadingBG = (Sprite)EditorGUILayout.ObjectField(currentlyViewedGameScene.loadingBG, typeof(Sprite), false, GUILayout.Width(120), GUILayout.Height(120));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Minimap Image", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene.minimapImage = (Sprite)EditorGUILayout.ObjectField(currentlyViewedGameScene.minimapImage, typeof(Sprite), false, GUILayout.Width(120), GUILayout.Height(120));
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Map Bounds", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedGameScene.mapBounds = EditorGUILayout.BoundsField(currentlyViewedGameScene.mapBounds);
                        GUILayout.EndHorizontal();
                        currentlyViewedGameScene.mapSize = EditorGUILayout.Vector2Field("Map Size", currentlyViewedGameScene.mapSize);

                        GUILayout.Space(10);
                        GUILayout.Label("POSITIONS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedGameScene.startPositionREF = (RPGWorldPosition)EditorGUILayout.ObjectField("Start Position", RPGBuilderUtilities.GetWorldPositionFromIDEditor(currentlyViewedGameScene.startPositionID, allWorldPositions), typeof(RPGWorldPosition), false, GUILayout.Width(400), GUILayout.Height(15));
                        if(currentlyViewedGameScene.startPositionREF != null)
                            currentlyViewedGameScene.startPositionID = currentlyViewedGameScene.startPositionREF.ID;
                        else
                            currentlyViewedGameScene.startPositionID = -1;
                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.GameScene, containerRect2);
    }

    private void DrawResourceNodeView()
    {
        if (currentlyViewedResourceNode == null)
        {
            if (allResourceNodes.Count == 0)
            {
                CreateNew(AssetType.ResourceNode);
                return;
            }
            currentlyViewedResourceNode = Instantiate(allResourceNodes[0]) as RPGResourceNode;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "world");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allResourceNodes.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allResourceNodes[i]._name;
                            newElementDATA.showIcon = true;
                            if (allResourceNodes[i].icon != null) newElementDATA.texture = allResourceNodes[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.ResourceNode);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedResourceNode.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedResourceNode.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedResourceNode.ID = EditorGUILayout.IntField(currentlyViewedResourceNode.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedResourceNode._name = GUILayout.TextField(currentlyViewedResourceNode._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedResourceNode.displayName = GUILayout.TextField(currentlyViewedResourceNode.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedResourceNode._fileName = GUILayout.TextField("RPG_RESOURCE_NODE_" + currentlyViewedResourceNode._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Known Automatically", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedResourceNode.learnedByDefault = EditorGUILayout.Toggle(currentlyViewedResourceNode.learnedByDefault);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Skill:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedResourceNode.skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedResourceNode.skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(200), GUILayout.Height(15));
                        if (currentlyViewedResourceNode.skillRequiredREF != null)
                            currentlyViewedResourceNode.skillRequiredID = currentlyViewedResourceNode.skillRequiredREF.ID;
                        else
                            currentlyViewedResourceNode.skillRequiredID = -1;
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("RANKS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Rank", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            var newRankData = new RPGResourceNodeRankData();
                            var newRankDataElement = new RPGResourceNode.rankDATA();
                            newRankDataElement.rankID = -1;
                            newRankDataElement.rankREF = newRankData;
                            temporaryResourceNodeRankList.Add(newRankDataElement);
                        }
                        GUILayout.Space(10);

                        if (GUILayout.Button("- Remove Rank", skin.GetStyle("RemoveAbilityRankButton"), GUILayout.Width(440), GUILayout.Height(30)))
                        {
                            var rankNumber = temporaryResourceNodeRankList.Count + 1;
                            temporaryResourceNodeRankList.RemoveAt(temporaryResourceNodeRankList.Count - 1);
                            return;
                        }
                        GUILayout.Space(10);

                        for (var i = 0; i < temporaryResourceNodeRankList.Count; i++)
                        {
                            var rankNbr = i + 1;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Rank: " + rankNbr, skin.GetStyle("AbilityRankButton"), GUILayout.Width(150), GUILayout.Height(30))) temporaryResourceNodeRankList[i].ShowedInEditor = !temporaryResourceNodeRankList[i].ShowedInEditor;
                            if (i > 0)
                            {
                                GUILayout.Space(20);
                                if (GUILayout.Button("Copy Above", skin.GetStyle("AddButton"), GUILayout.Width(225), GUILayout.Height(30))) temporaryResourceNodeRankList[i].rankREF.copyData(temporaryResourceNodeRankList[i].rankREF, temporaryResourceNodeRankList[i - 1].rankREF);
                            }
                            GUILayout.EndHorizontal();

                            if (temporaryResourceNodeRankList[i].ShowedInEditor)
                            {
                                GUILayout.Space(10);
                                GUILayout.Label("UNLOCKING SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryResourceNodeRankList[i].rankREF.unlockCost = EditorGUILayout.IntField("Unlock Cost", temporaryResourceNodeRankList[i].rankREF.unlockCost, GUILayout.Width(430));

                                GUILayout.Space(10);
                                GUILayout.Label("LOOT SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryResourceNodeRankList[i].rankREF.lootTableREF = (RPGLootTable)EditorGUILayout.ObjectField("Loot Table", RPGBuilderUtilities.GetlootTableFromIDEditor(temporaryResourceNodeRankList[i].rankREF.lootTableID, allLootTables), typeof(RPGLootTable), false, GUILayout.Width(430), GUILayout.Height(15));
                                if (temporaryResourceNodeRankList[i].rankREF.lootTableREF != null)
                                    temporaryResourceNodeRankList[i].rankREF.lootTableID = temporaryResourceNodeRankList[i].rankREF.lootTableREF.ID;
                                else
                                    temporaryResourceNodeRankList[i].rankREF.lootTableID = -1;

                                //temporaryResourceNodeRankList[i].rankREF.distanceMax = EditorGUILayout.FloatField("Distance", temporaryResourceNodeRankList[i].rankREF.distanceMax, GUILayout.Width(430), GUILayout.Height(15));
                                temporaryResourceNodeRankList[i].rankREF.gatherTime = EditorGUILayout.FloatField("Gather Time", temporaryResourceNodeRankList[i].rankREF.gatherTime, GUILayout.Width(430), GUILayout.Height(15));
                                temporaryResourceNodeRankList[i].rankREF.respawnTime = EditorGUILayout.FloatField("Respawn Time", temporaryResourceNodeRankList[i].rankREF.respawnTime, GUILayout.Width(430), GUILayout.Height(15));

                                GUILayout.Space(10);
                                GUILayout.Label("SKILL SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                temporaryResourceNodeRankList[i].rankREF.skillLevelRequired = EditorGUILayout.IntField("Level", temporaryResourceNodeRankList[i].rankREF.skillLevelRequired, GUILayout.Width(430), GUILayout.Height(15));
                                temporaryResourceNodeRankList[i].rankREF.Experience = EditorGUILayout.IntField("Experience", temporaryResourceNodeRankList[i].rankREF.Experience, GUILayout.Width(430));
                                
                            }

                            GUILayout.Space(5);
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.ResourceNode, containerRect2);
    }


    private void DrawTaskView()
    {
        if (currentlyViewedTask == null)
        {
            if (allTasks.Count == 0)
            {
                CreateNew(AssetType.Task);
                return;
            }
            currentlyViewedTask = Instantiate(allTasks[0]) as RPGTask;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "world");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allTasks.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allTasks[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Task);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTask.ID = EditorGUILayout.IntField(currentlyViewedTask.ID, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTask._name = GUILayout.TextField(currentlyViewedTask._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Display Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTask.displayName = GUILayout.TextField(currentlyViewedTask.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedTask._fileName = GUILayout.TextField("RPG_TASK_" + currentlyViewedTask._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedTask.description = GUILayout.TextField(currentlyViewedTask.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("TASK DATA", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedTask.taskType = (RPGTask.TASK_TYPE)EditorGUILayout.EnumPopup("Task Type", currentlyViewedTask.taskType, GUILayout.Width(325));
                        if(currentlyViewedTask.taskType == RPGTask.TASK_TYPE.enterRegion)
                        {

                        } else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.enterScene)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scene Name:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.sceneName = GUILayout.TextField(currentlyViewedTask.sceneName, GUILayout.Width(278), GUILayout.Height(15));
                            GUILayout.EndHorizontal();
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.getItem)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Item:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.itemToGetREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedTask.itemToGetID, allItems), typeof(RPGItem), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.itemToGetREF != null)
                                currentlyViewedTask.itemToGetID = currentlyViewedTask.itemToGetREF.ID;
                            else
                                currentlyViewedTask.itemToGetID = -1;
                            currentlyViewedTask.taskValue = EditorGUILayout.IntField(currentlyViewedTask.taskValue, GUILayout.Width(75), GUILayout.Height(15));
                            GUILayout.EndHorizontal();
                            currentlyViewedTask.keepItems = EditorGUILayout.Toggle("Keep Items", currentlyViewedTask.keepItems);
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.killNPC)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("NPC:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.npcToKillREF = (RPGNpc)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetNPCFromIDEditor(currentlyViewedTask.npcToKillID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.npcToKillREF != null)
                                currentlyViewedTask.npcToKillID = currentlyViewedTask.npcToKillREF.ID;
                            else
                                currentlyViewedTask.npcToKillID = -1;
                            currentlyViewedTask.taskValue = EditorGUILayout.IntField(currentlyViewedTask.taskValue, GUILayout.Width(75), GUILayout.Height(15));
                            GUILayout.EndHorizontal();
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.learnAbility)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Ability:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.abilityToLearnREF = (RPGAbility)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedTask.abilityToLearnID, allAbilities), typeof(RPGNpc), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.abilityToLearnREF != null)
                                currentlyViewedTask.abilityToLearnID = currentlyViewedTask.abilityToLearnREF.ID;
                            else
                                currentlyViewedTask.abilityToLearnID = -1;
                            GUILayout.EndHorizontal();
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.learnRecipe)
                        {

                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.reachClassLevel)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Class:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedTask.classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.classRequiredREF != null)
                                currentlyViewedTask.classRequiredID = currentlyViewedTask.classRequiredREF.ID;
                            else
                                currentlyViewedTask.abilityToLearnID = -1;
                            currentlyViewedTask.taskValue = EditorGUILayout.IntField(currentlyViewedTask.taskValue, GUILayout.Width(75), GUILayout.Height(15));
                            GUILayout.EndHorizontal();
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.reachSkillLevel)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Skill:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedTask.skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.skillRequiredREF != null)
                                currentlyViewedTask.skillRequiredID = currentlyViewedTask.skillRequiredREF.ID;
                            else
                                currentlyViewedTask.skillRequiredID = -1;
                            currentlyViewedTask.taskValue = EditorGUILayout.IntField(currentlyViewedTask.taskValue, GUILayout.Width(75), GUILayout.Height(15));
                            GUILayout.EndHorizontal();
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.talkToNPC)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("NPC:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.npcToTalkToREF = (RPGNpc)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetNPCFromIDEditor(currentlyViewedTask.npcToTalkToID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.npcToTalkToREF != null)
                                currentlyViewedTask.npcToTalkToID = currentlyViewedTask.npcToTalkToREF.ID;
                            else
                                currentlyViewedTask.npcToKillID = -1;
                            GUILayout.EndHorizontal();
                        }
                        else if (currentlyViewedTask.taskType == RPGTask.TASK_TYPE.useItem)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Item:", GUILayout.Width(147), GUILayout.Height(15));
                            currentlyViewedTask.itemToUseREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedTask.itemToUseID, allItems), typeof(RPGItem), false, GUILayout.Width(150), GUILayout.Height(15));
                            if (currentlyViewedTask.itemToUseREF != null)
                                currentlyViewedTask.itemToUseID = currentlyViewedTask.itemToUseREF.ID;
                            else
                                currentlyViewedTask.itemToUseID = -1;
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Task, containerRect2);
    }

    private void DrawQuestView()
    {
        if (currentlyViewedQuest == null)
        {
            if (allQuests.Count == 0)
            {
                CreateNew(AssetType.Quest);
                return;
            }
            currentlyViewedQuest = Instantiate(allQuests[0]) as RPGQuest;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "world");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allQuests.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allQuests[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Quest);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)worldSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedQuest.ID = EditorGUILayout.IntField(currentlyViewedQuest.ID, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        EditorGUI.EndDisabledGroup();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedQuest._name = GUILayout.TextField(currentlyViewedQuest._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Display Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedQuest.displayName = GUILayout.TextField(currentlyViewedQuest.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedQuest._fileName = GUILayout.TextField("RPG_QUEST_" + currentlyViewedQuest._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("QUEST UI DATA", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedQuest.description = GUILayout.TextField(currentlyViewedQuest.description, GUILayout.Width(200), GUILayout.Height(45));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Objective:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedQuest.ObjectiveText = GUILayout.TextField(currentlyViewedQuest.ObjectiveText, GUILayout.Width(200), GUILayout.Height(45));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Progress:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedQuest.ProgressText = GUILayout.TextField(currentlyViewedQuest.ProgressText, GUILayout.Width(200), GUILayout.Height(45));
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("QUEST DATA", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        //currentlyViewedQuest.repeatable = EditorGUILayout.Toggle("Repeatable?", currentlyViewedQuest.repeatable);
                        currentlyViewedQuest.canBeTurnedInWithoutNPC = EditorGUILayout.Toggle("Turn in without NPC", currentlyViewedQuest.canBeTurnedInWithoutNPC);

                        ScriptableObject scriptableObj = currentlyViewedQuest;
                        var serialObj = new SerializedObject(scriptableObj);

                        GUILayout.Space(10);
                        GUILayout.Label("ITEMS GIVEN", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedQuest.itemsGiven.Add(new RPGQuest.QuestItemsGivenDATA());

                        var ThisList7 = serialObj.FindProperty("itemsGiven");
                        currentlyViewedQuest.itemsGiven = GetTargetObjectOfProperty(ThisList7) as List<RPGQuest.QuestItemsGivenDATA>;

                        for (var a = 0; a < currentlyViewedQuest.itemsGiven.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedQuest.itemsGiven.RemoveAt(a);
                                return;
                            }
                            var effectName = "";
                            if (currentlyViewedQuest.itemsGiven[a].itemREF != null) effectName = currentlyViewedQuest.itemsGiven[a].itemREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Item", GUILayout.Width(100));
                            currentlyViewedQuest.itemsGiven[a].itemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedQuest.itemsGiven[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedQuest.itemsGiven[a].itemREF != null)
                                currentlyViewedQuest.itemsGiven[a].itemID = currentlyViewedQuest.itemsGiven[a].itemREF.ID;
                            else
                                currentlyViewedQuest.itemsGiven[a].itemID = -1;

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Count", GUILayout.Width(100));
                            currentlyViewedQuest.itemsGiven[a].count = EditorGUILayout.IntField(currentlyViewedQuest.itemsGiven[a].count, GUILayout.Width(250));
                            GUILayout.Space(10);
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(10);

                        }
                        GUILayout.Space(10);
                        GUILayout.Label("REQUIREMENTS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        if (GUILayout.Button("+ Add Requirement", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedQuest.questRequirements.Add(new RequirementsManager.RequirementDATA());

                        var ThisList = serialObj.FindProperty("questRequirements");
                        currentlyViewedQuest.questRequirements = GetTargetObjectOfProperty(ThisList) as List<RequirementsManager.RequirementDATA>;

                        for (var a = 0; a < currentlyViewedQuest.questRequirements.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedQuest.questRequirements[a].requirementType = (RequirementsManager.RequirementType)EditorGUILayout.EnumPopup(currentlyViewedQuest.questRequirements[a].requirementType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedQuest.questRequirements.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedQuest.questRequirements.Count > 0)
                            {
                                if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.pointSpent)
                                {
                                    currentlyViewedQuest.questRequirements[a].pointSpentValue = EditorGUILayout.IntField(new GUIContent("Points Spent", "How many points should already be spent in this tree for this bonus to be active?"), currentlyViewedQuest.questRequirements[a].pointSpentValue, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.classLevel)
                                {
                                    currentlyViewedQuest.questRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedQuest.questRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                    currentlyViewedQuest.questRequirements[a].classLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The class level required"), currentlyViewedQuest.questRequirements[a].classLevelValue, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].classRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].classRequiredID = currentlyViewedQuest.questRequirements[a].classRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].classRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType._class)
                                {
                                    currentlyViewedQuest.questRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedQuest.questRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].classRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].classRequiredID = currentlyViewedQuest.questRequirements[a].classRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].classRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.skillLevel)
                                {
                                    currentlyViewedQuest.questRequirements[a].skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill required for this bonus to be active"), RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedQuest.questRequirements[a].skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(400));
                                    currentlyViewedQuest.questRequirements[a].skillLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The skill level required"), currentlyViewedQuest.questRequirements[a].skillLevelValue, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].skillRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].skillRequiredID = currentlyViewedQuest.questRequirements[a].skillRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].skillRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.itemOwned)
                                {
                                    currentlyViewedQuest.questRequirements[a].itemRequiredREF = (RPGItem)EditorGUILayout.ObjectField(new GUIContent("Item", "The item required"), RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedQuest.questRequirements[a].itemRequiredID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                    currentlyViewedQuest.questRequirements[a].consumeItem = EditorGUILayout.Toggle(new GUIContent("Consumed?", "Is this item consumed?"), currentlyViewedQuest.questRequirements[a].consumeItem);
                                    if (currentlyViewedQuest.questRequirements[a].itemRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].itemRequiredID = currentlyViewedQuest.questRequirements[a].itemRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].itemRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.abilityKnown)
                                {
                                    currentlyViewedQuest.questRequirements[a].abilityRequiredREF = (RPGAbility)EditorGUILayout.ObjectField(new GUIContent("Ability", "The ability required to be known for this bonus to be active"), RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedQuest.questRequirements[a].abilityRequiredID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].abilityRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].abilityRequiredID = currentlyViewedQuest.questRequirements[a].abilityRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].abilityRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.recipeKnown)
                                {
                                    currentlyViewedQuest.questRequirements[a].recipeRequiredREF = (RPGCraftingRecipe)EditorGUILayout.ObjectField(new GUIContent("Recipe", "The crafting recipe required to be known for this bonus to be active"), RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(currentlyViewedQuest.questRequirements[a].craftingRecipeRequiredID, allCraftingRecipes), typeof(RPGCraftingRecipe), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].recipeRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].craftingRecipeRequiredID = currentlyViewedQuest.questRequirements[a].recipeRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].craftingRecipeRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.resourceNodeKnown)
                                {
                                    currentlyViewedQuest.questRequirements[a].resourceNodeRequiredREF = (RPGResourceNode)EditorGUILayout.ObjectField(new GUIContent("Resource Node", "The resource node required to be known for this bonus to be active"), RPGBuilderUtilities.GetResourceNodeFromIDEditor(currentlyViewedQuest.questRequirements[a].resourceNodeRequiredID, allResourceNodes), typeof(RPGResourceNode), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].resourceNodeRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].resourceNodeRequiredID = currentlyViewedQuest.questRequirements[a].resourceNodeRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].resourceNodeRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.race)
                                {
                                    currentlyViewedQuest.questRequirements[a].raceRequiredREF = (RPGRace)EditorGUILayout.ObjectField(new GUIContent("Race", "The race required"), RPGBuilderUtilities.GetRaceFromIDEditor(currentlyViewedQuest.questRequirements[a].raceRequiredID, allRaces), typeof(RPGRace), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].raceRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].raceRequiredID = currentlyViewedQuest.questRequirements[a].raceRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].raceRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.questState)
                                {
                                    currentlyViewedQuest.questRequirements[a].questRequiredREF = (RPGQuest)EditorGUILayout.ObjectField(new GUIContent("Quest", "The quest required for this bonus to be active"), RPGBuilderUtilities.GetQuestFromIDEditor(currentlyViewedQuest.questRequirements[a].questRequiredID, allQuests), typeof(RPGQuest), false, GUILayout.Width(400));
                                    currentlyViewedQuest.questRequirements[a].questStateRequired = (QuestManager.questState)EditorGUILayout.EnumPopup(new GUIContent("State", "The required state of the quest"), currentlyViewedQuest.questRequirements[a].questStateRequired, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].questRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].questRequiredID = currentlyViewedQuest.questRequirements[a].questRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].questRequiredID = -1;
                                }
                                else if (currentlyViewedQuest.questRequirements[a].requirementType == RequirementsManager.RequirementType.npcKilled)
                                {
                                    currentlyViewedQuest.questRequirements[a].npcRequiredREF = (RPGNpc)EditorGUILayout.ObjectField(new GUIContent("NPC", "The NPC required to be killed"), RPGBuilderUtilities.GetNPCFromIDEditor(currentlyViewedQuest.questRequirements[a].npcRequiredID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(400));
                                    currentlyViewedQuest.questRequirements[a].npcKillsRequired = EditorGUILayout.IntField(new GUIContent("Kills", "How many times this NPC should have been killed for the bonus to be active"), currentlyViewedQuest.questRequirements[a].npcKillsRequired, GUILayout.Width(400));
                                    if (currentlyViewedQuest.questRequirements[a].npcRequiredREF != null)
                                        currentlyViewedQuest.questRequirements[a].npcRequiredID = currentlyViewedQuest.questRequirements[a].npcRequiredREF.ID;
                                    else
                                        currentlyViewedQuest.questRequirements[a].npcRequiredID = -1;
                                }
                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("OBJECTIVES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        if (GUILayout.Button("+ Add Objective", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedQuest.objectives.Add(new RPGQuest.QuestObjectiveDATA());

                        var ThisList2 = serialObj.FindProperty("objectives");
                        currentlyViewedQuest.objectives = GetTargetObjectOfProperty(ThisList2) as List<RPGQuest.QuestObjectiveDATA>;

                        for (var a = 0; a < currentlyViewedQuest.objectives.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedQuest.objectives[a].objectiveType = (RPGQuest.QuestObjectiveType)EditorGUILayout.EnumPopup(currentlyViewedQuest.objectives[a].objectiveType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedQuest.objectives.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedQuest.objectives.Count > 0)
                                if (currentlyViewedQuest.objectives[a].objectiveType == RPGQuest.QuestObjectiveType.task)
                                {
                                    currentlyViewedQuest.objectives[a].taskREF = (RPGTask)EditorGUILayout.ObjectField("Task:", RPGBuilderUtilities.GetTaskFromIDEditor(currentlyViewedQuest.objectives[a].taskID, allTasks), typeof(RPGTask), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.objectives[a].taskREF != null)
                                        currentlyViewedQuest.objectives[a].taskID = currentlyViewedQuest.objectives[a].taskREF.ID;
                                    else
                                        currentlyViewedQuest.objectives[a].taskID = -1;

                                    currentlyViewedQuest.objectives[a].timeLimit = EditorGUILayout.FloatField("Time Limit?", currentlyViewedQuest.objectives[a].timeLimit, GUILayout.Width(400));
                                }

                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("AUTOMATIC REWARDS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        if (GUILayout.Button("+ Add Reward", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedQuest.rewardsGiven.Add(new RPGQuest.QuestRewardDATA());

                        var ThisList3 = serialObj.FindProperty("rewardsGiven");
                        currentlyViewedQuest.rewardsGiven = GetTargetObjectOfProperty(ThisList3) as List<RPGQuest.QuestRewardDATA>;

                        for (var a = 0; a < currentlyViewedQuest.rewardsGiven.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedQuest.rewardsGiven[a].rewardType = (RPGQuest.QuestRewardType)EditorGUILayout.EnumPopup(currentlyViewedQuest.rewardsGiven[a].rewardType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedQuest.rewardsGiven.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedQuest.rewardsGiven.Count > 0)
                            {
                                if (currentlyViewedQuest.rewardsGiven[a].rewardType == RPGQuest.QuestRewardType.currency)
                                {
                                    currentlyViewedQuest.rewardsGiven[a].currencyREF = (RPGCurrency)EditorGUILayout.ObjectField("Currency", RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedQuest.rewardsGiven[a].currencyID, allCurrencies), typeof(RPGCurrency), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.rewardsGiven[a].currencyREF != null)
                                        currentlyViewedQuest.rewardsGiven[a].currencyID = currentlyViewedQuest.rewardsGiven[a].currencyREF.ID;
                                    else
                                        currentlyViewedQuest.rewardsGiven[a].currencyID = -1;
                                    currentlyViewedQuest.rewardsGiven[a].count = EditorGUILayout.IntField("Count", currentlyViewedQuest.rewardsGiven[a].count, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.rewardsGiven[a].rewardType == RPGQuest.QuestRewardType.Experience)
                                {
                                    currentlyViewedQuest.rewardsGiven[a].Experience = EditorGUILayout.IntField("Experience", currentlyViewedQuest.rewardsGiven[a].Experience, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.rewardsGiven[a].rewardType == RPGQuest.QuestRewardType.item)
                                {
                                    currentlyViewedQuest.rewardsGiven[a].itemREF = (RPGItem)EditorGUILayout.ObjectField("Item", RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedQuest.rewardsGiven[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.rewardsGiven[a].itemREF != null)
                                        currentlyViewedQuest.rewardsGiven[a].itemID = currentlyViewedQuest.rewardsGiven[a].itemREF.ID;
                                    else
                                        currentlyViewedQuest.rewardsGiven[a].itemID = -1;
                                    currentlyViewedQuest.rewardsGiven[a].count = EditorGUILayout.IntField("Count", currentlyViewedQuest.rewardsGiven[a].count, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.rewardsGiven[a].rewardType == RPGQuest.QuestRewardType.treePoint)
                                {
                                    currentlyViewedQuest.rewardsGiven[a].treePointREF = (RPGTreePoint)EditorGUILayout.ObjectField("Tree point", RPGBuilderUtilities.GetTreePointFromIDEditor(currentlyViewedQuest.rewardsGiven[a].treePointID, allTreePoints), typeof(RPGTreePoint), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.rewardsGiven[a].treePointREF != null)
                                        currentlyViewedQuest.rewardsGiven[a].treePointID = currentlyViewedQuest.rewardsGiven[a].treePointREF.ID;
                                    else
                                        currentlyViewedQuest.rewardsGiven[a].treePointID = -1;
                                    currentlyViewedQuest.rewardsGiven[a].count = EditorGUILayout.IntField("Count", currentlyViewedQuest.rewardsGiven[a].count, GUILayout.Width(400));
                                }

                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("REWARDS TO PICK", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        if (GUILayout.Button("+ Add Reward", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedQuest.rewardsToPick.Add(new RPGQuest.QuestRewardDATA());

                        var ThisList4 = serialObj.FindProperty("rewardsToPick");
                        currentlyViewedQuest.rewardsToPick = GetTargetObjectOfProperty(ThisList4) as List<RPGQuest.QuestRewardDATA>;

                        for (var a = 0; a < currentlyViewedQuest.rewardsToPick.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedQuest.rewardsToPick[a].rewardType = (RPGQuest.QuestRewardType)EditorGUILayout.EnumPopup(currentlyViewedQuest.rewardsToPick[a].rewardType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedQuest.rewardsToPick.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedQuest.rewardsToPick.Count > 0)
                            {
                                if (currentlyViewedQuest.rewardsToPick[a].rewardType == RPGQuest.QuestRewardType.currency)
                                {
                                    currentlyViewedQuest.rewardsToPick[a].currencyREF = (RPGCurrency)EditorGUILayout.ObjectField("Currency", RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedQuest.rewardsToPick[a].currencyID, allCurrencies), typeof(RPGCurrency), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.rewardsToPick[a].currencyREF != null)
                                        currentlyViewedQuest.rewardsToPick[a].currencyID = currentlyViewedQuest.rewardsToPick[a].currencyREF.ID;
                                    else
                                        currentlyViewedQuest.rewardsToPick[a].currencyID = -1;
                                    currentlyViewedQuest.rewardsToPick[a].count = EditorGUILayout.IntField("Count", currentlyViewedQuest.rewardsToPick[a].count, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.rewardsToPick[a].rewardType == RPGQuest.QuestRewardType.Experience)
                                {
                                    currentlyViewedQuest.rewardsToPick[a].Experience = EditorGUILayout.IntField("Experience", currentlyViewedQuest.rewardsToPick[a].Experience, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.rewardsToPick[a].rewardType == RPGQuest.QuestRewardType.item)
                                {
                                    currentlyViewedQuest.rewardsToPick[a].itemREF = (RPGItem)EditorGUILayout.ObjectField("Item", RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedQuest.rewardsToPick[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.rewardsToPick[a].itemREF != null)
                                        currentlyViewedQuest.rewardsToPick[a].itemID = currentlyViewedQuest.rewardsToPick[a].itemREF.ID;
                                    else
                                        currentlyViewedQuest.rewardsToPick[a].itemID = -1;
                                    currentlyViewedQuest.rewardsToPick[a].count = EditorGUILayout.IntField("Count", currentlyViewedQuest.rewardsToPick[a].count, GUILayout.Width(400));
                                }
                                else if (currentlyViewedQuest.rewardsToPick[a].rewardType == RPGQuest.QuestRewardType.treePoint)
                                {
                                    currentlyViewedQuest.rewardsToPick[a].treePointREF = (RPGTreePoint)EditorGUILayout.ObjectField("Tree point", RPGBuilderUtilities.GetTreePointFromIDEditor(currentlyViewedQuest.rewardsToPick[a].treePointID, allTreePoints), typeof(RPGTreePoint), false, GUILayout.Width(400));
                                    if (currentlyViewedQuest.rewardsToPick[a].treePointREF != null)
                                        currentlyViewedQuest.rewardsToPick[a].treePointID = currentlyViewedQuest.rewardsToPick[a].treePointREF.ID;
                                    else
                                        currentlyViewedQuest.rewardsToPick[a].treePointID = -1;
                                    currentlyViewedQuest.rewardsToPick[a].count = EditorGUILayout.IntField("Count", currentlyViewedQuest.rewardsToPick[a].count, GUILayout.Width(400));
                                }

                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);
                        }

                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Quest, containerRect2);
    }


    private void DrawMerchantTableView()
    {
        if (currentlyViewedMerchantTable == null)
        {
            if (allMerchantTables.Count == 0)
            {
                CreateNew(AssetType.MerchantTable);
                return;
            }
            currentlyViewedMerchantTable = Instantiate(allMerchantTables[0]) as RPGMerchantTable;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allMerchantTables.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allMerchantTables[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.MerchantTable);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedMerchantTable.ID = EditorGUILayout.IntField(currentlyViewedMerchantTable.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedMerchantTable._name = GUILayout.TextField(currentlyViewedMerchantTable._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedMerchantTable.displayName = GUILayout.TextField(currentlyViewedMerchantTable.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedMerchantTable._fileName = GUILayout.TextField("RPG_MERCHANT_TABLE_" + currentlyViewedMerchantTable._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("LISTED ITEMS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        ScriptableObject scriptableObj = currentlyViewedMerchantTable;
                        var serialObj = new SerializedObject(scriptableObj);

                        if (GUILayout.Button("+ Add Item", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedMerchantTable.onSaleItems.Add(new RPGMerchantTable.ON_SALE_ITEMS_DATA());

                        var ThisList2 = serialObj.FindProperty("onSaleItems");
                        currentlyViewedMerchantTable.onSaleItems = GetTargetObjectOfProperty(ThisList2) as List<RPGMerchantTable.ON_SALE_ITEMS_DATA>;

                        for (var a = 0; a < currentlyViewedMerchantTable.onSaleItems.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedMerchantTable.onSaleItems.RemoveAt(a);
                                return;
                            }
                            var effectName = "";
                            if (currentlyViewedMerchantTable.onSaleItems[a].itemREF != null) effectName = currentlyViewedMerchantTable.onSaleItems[a].itemREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Item", GUILayout.Width(100));
                            currentlyViewedMerchantTable.onSaleItems[a].itemREF = (RPGItem)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedMerchantTable.onSaleItems[a].itemID, allItems), typeof(RPGItem), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedMerchantTable.onSaleItems[a].itemREF != null)
                                currentlyViewedMerchantTable.onSaleItems[a].itemID = currentlyViewedMerchantTable.onSaleItems[a].itemREF.ID;
                            else
                                currentlyViewedMerchantTable.onSaleItems[a].itemID = -1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Currency", GUILayout.Width(100));
                            currentlyViewedMerchantTable.onSaleItems[a].currencyREF = (RPGCurrency)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedMerchantTable.onSaleItems[a].currencyID, allCurrencies), typeof(RPGCurrency), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedMerchantTable.onSaleItems[a].currencyREF != null)
                                currentlyViewedMerchantTable.onSaleItems[a].currencyID = currentlyViewedMerchantTable.onSaleItems[a].currencyREF.ID;
                            else
                                currentlyViewedMerchantTable.onSaleItems[a].currencyID = -1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Cost", GUILayout.Width(100));
                            currentlyViewedMerchantTable.onSaleItems[a].cost = EditorGUILayout.IntField(currentlyViewedMerchantTable.onSaleItems[a].cost, GUILayout.Width(250));
                            GUILayout.Space(10);
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(10);
                            if (currentlyViewedMerchantTable.onSaleItems[a].currencyREF != null)
                                if (RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedMerchantTable.onSaleItems[a].currencyREF.convertToCurrencyID, allCurrencies) == null)
                                    if (RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedMerchantTable.onSaleItems[a].currencyID, allCurrencies).maxValue > 0 && currentlyViewedMerchantTable.onSaleItems[a].cost > RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedMerchantTable.onSaleItems[a].currencyID, allCurrencies).maxValue) currentlyViewedMerchantTable.onSaleItems[a].cost = RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedMerchantTable.onSaleItems[a].currencyID, allCurrencies).maxValue - 1;
                        }

                        serialObj.ApplyModifiedProperties();
                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.MerchantTable, containerRect2);
    }

    private void DrawCurrencyView()
    {
        if (currentlyViewedCurrency == null)
        {
            if (allCurrencies.Count == 0)
            {
                CreateNew(AssetType.Currency);
                return;
            }
            currentlyViewedCurrency = Instantiate(allCurrencies[0]) as RPGCurrency;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allCurrencies.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allCurrencies[i]._name;
                            newElementDATA.showIcon = true;
                            if (allCurrencies[i].icon != null) newElementDATA.texture = allCurrencies[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Currency);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedCurrency.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedCurrency.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCurrency.ID = EditorGUILayout.IntField(currentlyViewedCurrency.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCurrency._name = GUILayout.TextField(currentlyViewedCurrency._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Display Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCurrency.displayName = GUILayout.TextField(currentlyViewedCurrency.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedCurrency.description = GUILayout.TextField(currentlyViewedCurrency.description, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedCurrency._fileName = GUILayout.TextField("RPG_CURRENCY_" + currentlyViewedCurrency._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("VALUES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedCurrency.minValue = EditorGUILayout.IntField("Min", currentlyViewedCurrency.minValue);
                        currentlyViewedCurrency.maxValue = EditorGUILayout.IntField("Max", currentlyViewedCurrency.maxValue);
                        if (currentlyViewedCurrency.maxValue < currentlyViewedCurrency.minValue) currentlyViewedCurrency.maxValue = currentlyViewedCurrency.minValue;
                        if (currentlyViewedCurrency.minValue > currentlyViewedCurrency.maxValue) currentlyViewedCurrency.maxValue = currentlyViewedCurrency.minValue;

                        currentlyViewedCurrency.baseValue = EditorGUILayout.IntField("Starts At", currentlyViewedCurrency.baseValue);
                        if(currentlyViewedCurrency.baseValue >= currentlyViewedCurrency.maxValue && currentlyViewedCurrency.maxValue > 0) currentlyViewedCurrency.baseValue = currentlyViewedCurrency.maxValue - 1;

                        GUILayout.Space(10);
                        GUILayout.Label("CONVERSION SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedCurrency.AmountToConvert = EditorGUILayout.IntField("Amt. For Conversion", currentlyViewedCurrency.AmountToConvert);
                        if (currentlyViewedCurrency.AmountToConvert >= currentlyViewedCurrency.maxValue) currentlyViewedCurrency.AmountToConvert = currentlyViewedCurrency.maxValue;
                        currentlyViewedCurrency.convertToCurrencyREF = (RPGCurrency)EditorGUILayout.ObjectField("To Currency:", RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedCurrency.convertToCurrencyID, allCurrencies), typeof(RPGCurrency), false, GUILayout.Width(400), GUILayout.Height(15));
                        if (currentlyViewedCurrency.convertToCurrencyREF != null)
                            currentlyViewedCurrency.convertToCurrencyID = currentlyViewedCurrency.convertToCurrencyREF.ID;
                        else
                            currentlyViewedCurrency.convertToCurrencyID = -1;

                        currentlyViewedCurrency.lowestCurrencyREF = (RPGCurrency)EditorGUILayout.ObjectField("Lowest Currency:", RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedCurrency.lowestCurrencyID, allCurrencies), typeof(RPGCurrency), false, GUILayout.Width(400), GUILayout.Height(15));
                        if (currentlyViewedCurrency.lowestCurrencyREF != null)
                            currentlyViewedCurrency.lowestCurrencyID = currentlyViewedCurrency.lowestCurrencyREF.ID;
                        else
                            currentlyViewedCurrency.lowestCurrencyID = -1;

                        if (currentlyViewedCurrency.lowestCurrencyREF != null && currentlyViewedCurrency.lowestCurrencyID == currentlyViewedCurrency.ID)
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("CURRENCIES ABOVE THIS ONE", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);
                            ScriptableObject scriptableObj = currentlyViewedCurrency;
                            var serialObj = new SerializedObject(scriptableObj);

                            if (GUILayout.Button("+ Add Currency", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedCurrency.aboveCurrencies.Add(new RPGCurrency.AboveCurrencyDATA());

                            var ThisList2 = serialObj.FindProperty("aboveCurrencies");
                            currentlyViewedCurrency.aboveCurrencies = GetTargetObjectOfProperty(ThisList2) as List<RPGCurrency.AboveCurrencyDATA>;

                            for (var a = 0; a < currentlyViewedCurrency.aboveCurrencies.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedCurrency.aboveCurrencies.RemoveAt(a);
                                    return;
                                }
                                var effectName = "";
                                if (currentlyViewedCurrency.aboveCurrencies[a].currencyREF != null) effectName = currentlyViewedCurrency.aboveCurrencies[a].currencyREF._name;
                                EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Currency", GUILayout.Width(75));
                                currentlyViewedCurrency.aboveCurrencies[a].currencyREF = (RPGCurrency)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedCurrency.aboveCurrencies[a].currencyID, allCurrencies), typeof(RPGCurrency), false, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                if (currentlyViewedCurrency.aboveCurrencies[a].currencyREF != null)
                                    currentlyViewedCurrency.aboveCurrencies[a].currencyID = currentlyViewedCurrency.aboveCurrencies[a].currencyREF.ID;
                                else
                                    currentlyViewedCurrency.aboveCurrencies[a].currencyID = -1;
                                GUILayout.Space(10);
                            }

                            serialObj.ApplyModifiedProperties();
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Currency, containerRect2);
    }

    private void DrawLevelTemplateView()
    {
        if (currentlyViewedLevelTemplate == null)
        {
            if (allLevelsTemplate.Count == 0)
            {
                CreateNew(AssetType.LevelTemplate);
                return;
            }
            currentlyViewedLevelTemplate = Instantiate(allLevelsTemplate[0]) as RPGLevelsTemplate;

        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allLevelsTemplate.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allLevelsTemplate[i]._name;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.LevelTemplate);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedLevelTemplate.ID = EditorGUILayout.IntField(currentlyViewedLevelTemplate.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedLevelTemplate._name = GUILayout.TextField(currentlyViewedLevelTemplate._name, GUILayout.Width(278), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(147), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedLevelTemplate._fileName = GUILayout.TextField("RPG_LEVEL_TEMPLATE_" + currentlyViewedLevelTemplate._name, GUILayout.Width(278), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("TEMPLATE", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        currentlyViewedLevelTemplate.levels = EditorGUILayout.IntField("Levels", currentlyViewedLevelTemplate.levels);
                        currentlyViewedLevelTemplate.baseXPValue = EditorGUILayout.IntField("Base XP", currentlyViewedLevelTemplate.baseXPValue);
                        currentlyViewedLevelTemplate.increaseAmount = EditorGUILayout.FloatField("Increase by %", currentlyViewedLevelTemplate.increaseAmount);

                        GUILayout.Space(10);
                        if (GUILayout.Button("Click to Generate", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            currentlyViewedLevelTemplate.allLevels = new List<RPGLevelsTemplate.LEVELS_DATA>();
                            float curXP = currentlyViewedLevelTemplate.baseXPValue;

                            var curIncreaseAmount = currentlyViewedLevelTemplate.increaseAmount / 100;

                            for (var i = 0; i < currentlyViewedLevelTemplate.levels; i++)
                            {
                                var newLevel = new RPGLevelsTemplate.LEVELS_DATA();
                                var currentLevel = i + 1;

                                if (i > 0) curXP += curXP * curIncreaseAmount;

                                if(curXP > 2000000000) curXP = 2000000000;

                                newLevel.XPRequired = (int)curXP;
                                newLevel.levelName = "" + currentLevel;
                                newLevel.level = currentLevel;

                                currentlyViewedLevelTemplate.allLevels.Add(newLevel);
                            }
                        }
                        GUILayout.Space(10);


                        ScriptableObject scriptableObj = currentlyViewedLevelTemplate;
                        var serialObj = new SerializedObject(scriptableObj);
                        var serialProp = serialObj.FindProperty("allLevels");
                        EditorGUILayout.PropertyField(serialProp, true);
                        
                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.LevelTemplate, containerRect2);
    }

    private void AssetActions (UnityEngine.Object dirtyObject, bool refresh)
    {
        EditorUtility.SetDirty(dirtyObject);
        AssetDatabase.SaveAssets();
        if(refresh) AssetDatabase.Refresh();
    }

    private int HandleAssetIDSaving (AssetIDHandler.ASSET_TYPE_ID AssetIDType)
    {
        var assetID = -1;
        var currentIDFile = DataSavingSystem.LoadAssetID(AssetIDType);
        if (currentIDFile != null)
        {
            assetID = currentIDFile.id;
            assetID++;
            currentIDFile.id = assetID;
            DataSavingSystem.SaveAssetID(currentIDFile);
        }
        else
        {
            var file = new AssetIDHandler(AssetIDType, 0);
            DataSavingSystem.SaveAssetID(file);
            assetID = 0;
        }
        return assetID;
    }

    private int GetNewID(AssetIDHandler.ASSET_TYPE_ID AssetIDType)
    {
        var assetID = -1;
        var currentIDFile = DataSavingSystem.LoadAssetID(AssetIDType);
        if (currentIDFile != null)
            assetID = currentIDFile.id + 1;
        else
            assetID = 0;
        return assetID;
    }

    private void AssetActionsAfterCreate ()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private UnityEngine.Object HandleRankCreation(AssetType assetType, UnityEngine.Object curOBJ)
    {
        switch (assetType)
        {
            case AssetType.Ability:
                var curAB = (RPGAbility)curOBJ;
                for (var i = 0; i < temporaryAbRankList.Count; i++)
                {
                    var rankNumber = i + 1;
                    var existingRankData = (RPGAbilityRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGAbilityRankData));
                    if (existingRankData != null)
                    {
                        if (existingRankData.ID == -1)
                        {
                            var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.abilityRank);
                            curAB.ranks[i].rankID = rankID;
                        }
                        existingRankData = (RPGAbilityRankData)ResetSOReferencesBeforeSaving(AssetIDHandler.ASSET_TYPE_ID.abilityRank, existingRankData);
                        existingRankData.updateThis(temporaryAbRankList[i].rankREF);

                        if(existingRankData.name != curAB._name + "_RankData_" + rankNumber) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", curAB._name + "_RankData_" + rankNumber);
                        AssetActions(existingRankData, false);
                    }
                    else
                    {
                        var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.abilityRank);
                        temporaryAbRankList[i].rankREF.ID = rankID;
                        temporaryAbRankList[i].rankID = rankID;
                        temporaryAbRankList[i].rankREF = (RPGAbilityRankData)ResetSOReferencesBeforeSaving(AssetIDHandler.ASSET_TYPE_ID.abilityRank, temporaryAbRankList[i].rankREF);
                        AssetDatabase.CreateAsset(temporaryAbRankList[i].rankREF, "Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + curAB._name + "_RankData_" + rankNumber + ".asset");
                        AssetActionsAfterCreate();
                        var newRank = (RPGAbilityRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + curAB._name + "_RankData_" + rankNumber + ".asset", typeof(RPGAbilityRankData));
                        var newRankData = new RPGAbility.rankDATA();
                        newRankData.rankID = rankID;
                        curAB.ranks.Add(newRankData);
                    }
                }

                if (curAB.ranks.Count > temporaryAbRankList.Count)
                {
                    int diff = curAB.ranks.Count - temporaryAbRankList.Count;
                    for (int i = 0; i < diff; i++)
                    {
                        curAB.ranks.RemoveAt(curAB.ranks.Count-1);
                    }
                }
                return curAB;
            case AssetType.CraftingRecipe:
                var curRECIPE = (RPGCraftingRecipe)curOBJ;
                for (var i = 0; i < temporaryRecipeRankList.Count; i++)
                {
                    var rankNumber = i + 1;
                    var existingRankData = (RPGCraftingRecipeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + curRECIPE._name + "_RankData_" + rankNumber + ".asset", typeof(RPGCraftingRecipeRankData));
                    if (existingRankData != null)
                    {
                        if (existingRankData.ID == -1)
                        {
                            var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.recipeRank);
                            curRECIPE.ranks[i].rankID = rankID;
                        }
                        existingRankData.updateThis(temporaryRecipeRankList[i].rankREF);
                        if (existingRankData.name != curRECIPE._name + "_RankData_" + rankNumber) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", curRECIPE._name + "_RankData_" + rankNumber);
                        AssetActions(existingRankData, false);
                    }
                    else
                    {
                        var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.recipeRank);
                        temporaryRecipeRankList[i].rankREF.ID = rankID;
                        temporaryRecipeRankList[i].rankID = rankID;
                        AssetDatabase.CreateAsset(temporaryRecipeRankList[i].rankREF, "Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + curRECIPE._name + "_RankData_" + rankNumber + ".asset");
                        AssetActionsAfterCreate();
                        var newRank = (RPGCraftingRecipeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + curRECIPE._name + "_RankData_" + rankNumber + ".asset", typeof(RPGCraftingRecipeRankData));
                        var newRankData = new RPGCraftingRecipe.rankDATA();
                        newRankData.rankID = rankID;
                        curRECIPE.ranks.Add(newRankData);
                    }
                }

                if (curRECIPE.ranks.Count > temporaryRecipeRankList.Count)
                {
                    int diff = curRECIPE.ranks.Count - temporaryRecipeRankList.Count;
                    for (int i = 0; i < diff; i++)
                    {
                        curRECIPE.ranks.RemoveAt(curRECIPE.ranks.Count-1);
                    }
                }
                return curRECIPE;
            case AssetType.ResourceNode:
                var curRESOURCENODE = (RPGResourceNode)curOBJ;
                for (var i = 0; i < temporaryResourceNodeRankList.Count; i++)
                {
                    var rankNumber = i + 1;
                    var existingRankData = (RPGResourceNodeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + curRESOURCENODE._name + "_RankData_" + rankNumber + ".asset", typeof(RPGResourceNodeRankData));
                    if (existingRankData != null)
                    {
                        if (existingRankData.ID == -1)
                        {
                            var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.resourceNodeRank);
                            curRESOURCENODE.ranks[i].rankID = rankID;
                        }
                        existingRankData.updateThis(temporaryResourceNodeRankList[i].rankREF);
                        if (existingRankData.name != curRESOURCENODE._name + "_RankData_" + rankNumber) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", curRESOURCENODE._name + "_RankData_" + rankNumber);
                        AssetActions(existingRankData, false);
                    }
                    else
                    {
                        var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.resourceNodeRank);
                        temporaryResourceNodeRankList[i].rankREF.ID = rankID;
                        temporaryResourceNodeRankList[i].rankID = rankID;
                        AssetDatabase.CreateAsset(temporaryResourceNodeRankList[i].rankREF, "Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + curRESOURCENODE._name + "_RankData_" + rankNumber + ".asset");
                        AssetActionsAfterCreate();
                        var newRank = (RPGResourceNodeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + curRESOURCENODE._name + "_RankData_" + rankNumber + ".asset", typeof(RPGResourceNodeRankData));
                        var newRankData = new RPGResourceNode.rankDATA();
                        newRankData.rankID = rankID;
                        curRESOURCENODE.ranks.Add(newRankData);
                    }
                }
                
                if (curRESOURCENODE.ranks.Count > temporaryResourceNodeRankList.Count)
                {
                    int diff = curRESOURCENODE.ranks.Count - temporaryResourceNodeRankList.Count;
                    for (int i = 0; i < diff; i++)
                    {
                        curRESOURCENODE.ranks.RemoveAt(curRESOURCENODE.ranks.Count-1);
                    }
                }
                return curRESOURCENODE;
            case AssetType.Bonus:
                var curBONUS = (RPGBonus)curOBJ;
                for (var i = 0; i < temporaryBonusRankList.Count; i++)
                {
                    var rankNumber = i + 1;
                    var existingRankData = (RPGBonusRankDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + curBONUS._name + "_RankData_" + rankNumber + ".asset", typeof(RPGBonusRankDATA));
                    if (existingRankData != null)
                    {
                        if (existingRankData.ID == -1)
                        {
                            var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.bonusRank);
                            curBONUS.ranks[i].rankID = rankID;
                        }
                        existingRankData.updateThis(temporaryBonusRankList[i].rankREF);
                        if (existingRankData.name != curBONUS._name + "_RankData_" + rankNumber) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", curBONUS._name + "_RankData_" + rankNumber);
                        AssetActions(existingRankData, false);
                    }
                    else
                    {
                        var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.bonusRank);
                        temporaryBonusRankList[i].rankREF.ID = rankID;
                        temporaryBonusRankList[i].rankID = rankID;
                        AssetDatabase.CreateAsset(temporaryBonusRankList[i].rankREF, "Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + curBONUS._name + "_RankData_" + rankNumber + ".asset");
                        AssetActionsAfterCreate();
                        var newRank = (RPGBonusRankDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + curBONUS._name + "_RankData_" + rankNumber + ".asset", typeof(RPGBonusRankDATA));
                        var newRankData = new RPGBonus.rankDATA();
                        newRankData.rankID = rankID;
                        curBONUS.ranks.Add(newRankData);
                    }
                }
                
                if (curBONUS.ranks.Count > temporaryBonusRankList.Count)
                {
                    int diff = curBONUS.ranks.Count - temporaryBonusRankList.Count;
                    for (int i = 0; i < diff; i++)
                    {
                        curBONUS.ranks.RemoveAt(curBONUS.ranks.Count-1);
                    }
                }
                return curBONUS;
            default:
                return null;
        }
    }

    private UnityEngine.Object ResetSOReferencesBeforeSaving (AssetIDHandler.ASSET_TYPE_ID assetType, UnityEngine.Object obj)
    {
        switch(assetType)
        {
            case AssetIDHandler.ASSET_TYPE_ID.abilityRank:
                {
                    var curREF = (RPGAbilityRankData)obj;
                    for (var i = 0; i < curREF.useRequirements.Count; i++)
                    {
                        curREF.useRequirements[i].statCostREF = null;
                        curREF.useRequirements[i].itemRequiredREF = null;
                    }
                    for (var i = 0; i < curREF.effectsApplied.Count; i++) curREF.effectsApplied[i].effectREF = null;
                    return curREF;
                }
            case AssetIDHandler.ASSET_TYPE_ID._class:
                {
                    var curREF = (RPGClass)obj;
                    for (var i = 0; i < curREF.stats.Count; i++) curREF.stats[i].statREF = null;
                    for (var i = 0; i < curREF.talentTrees.Count; i++) curREF.talentTrees[i].talentTreeREF = null;
                    return curREF;
                }
            default:
                return null;
        }
    }

    private void SaveAsset(string elementName, string fileName, UnityEngine.Object savedObject)
    {
        if (elementName != null && elementName.Length > 0 && fileName != null && fileName.Length > 0)
        {
            var directory = "";
            var AssetIDType = AssetIDHandler.ASSET_TYPE_ID.ability;
            UnityEngine.Object existingELEMENT = null;
            var thisAssetType = AssetType.Ability;
            switch (savedObject.GetType().ToString())
            {
                case "RPGAbility":
                    directory = "Abilities/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.ability;
                    thisAssetType = AssetType.Ability;
                    var curViewedAbility = (RPGAbility)savedObject;
                    existingELEMENT = (RPGAbility)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGAbility));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGAbility)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedAbility = (RPGAbility)HandleRankCreation(thisAssetType, curViewedAbility);
                        curViewedAbility.ID = thisElement.ID;
                        thisElement.updateThis(curViewedAbility);
                        if (cachedFileName != curViewedAbility._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedAbility._fileName);
                        AssetActions(thisElement, true);
                        LoadAbilities();
                        LoadAbilityRankData();
                        SelectAbility(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedAbility.ID = ID;
                        curViewedAbility = (RPGAbility)HandleRankCreation(thisAssetType, curViewedAbility);
                        AssetDatabase.CreateAsset(curViewedAbility, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedAbility._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadAbilities();
                        LoadAbilityRankData();
                        SelectNewAbility(curViewedAbility._fileName);
                    }
                    LoadAbilityRankData();
                    break;
                case "RPGEffect":
                    directory = "Effects/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.effect;
                    thisAssetType = AssetType.Effect;
                    var curViewedEffect = (RPGEffect)savedObject;
                    existingELEMENT = (RPGEffect)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGEffect));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGEffect)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedEffect.ID = thisElement.ID;
                        thisElement.updateThis(curViewedEffect);
                        if (cachedFileName != curViewedEffect._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedEffect._fileName);
                        AssetActions(thisElement, true);
                        LoadEffects();
                        SelectEffect(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedEffect.ID = ID;
                        AssetDatabase.CreateAsset(curViewedEffect, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedEffect._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadEffects();
                        SelectNewEffect(curViewedEffect._fileName);
                    }
                    break;
                case "RPGNpc":
                    directory = "NPCs/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.npc;
                    thisAssetType = AssetType.NPC;
                    var curViewedNPC = (RPGNpc)savedObject;
                    existingELEMENT = (RPGNpc)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGNpc));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGNpc)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedNPC.ID = thisElement.ID;
                        thisElement.updateThis(curViewedNPC);
                        if (cachedFileName != curViewedNPC._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedNPC._fileName);
                        AssetActions(thisElement, true);
                        LoadNPCs();
                        SelectNPC(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedNPC.ID = ID;
                        AssetDatabase.CreateAsset(curViewedNPC, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedNPC._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadNPCs();
                        SelectNewNPC(curViewedNPC._fileName);
                    }
                    break;
                case "RPGStat":
                    directory = "Stats/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.stat;
                    thisAssetType = AssetType.Stat;
                    var curViewedStat = (RPGStat)savedObject;
                    existingELEMENT = (RPGStat)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGStat));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGStat)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedStat.ID = thisElement.ID;
                        thisElement.updateThis(curViewedStat);
                        if (cachedFileName != curViewedStat._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedStat._fileName);
                        AssetActions(thisElement, true);
                        LoadStats();
                        SelectStat(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedStat.ID = ID;
                        AssetDatabase.CreateAsset(curViewedStat, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedStat._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadStats();
                        SelectNewStat(curViewedStat._fileName);
                    }
                    break;
                case "RPGTreePoint":
                    directory = "TreePoints/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.treePoint;
                    thisAssetType = AssetType.TreePoint;
                    var curViewedTreePoint = (RPGTreePoint)savedObject;
                    existingELEMENT = (RPGTreePoint)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGTreePoint));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGTreePoint)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedTreePoint.ID = thisElement.ID;
                        thisElement.updateThis(curViewedTreePoint);
                        if (cachedFileName != curViewedTreePoint._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedTreePoint._fileName);
                        AssetActions(thisElement, true);
                        LoadTreePoints();
                        SelectTreePoint(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedTreePoint.ID = ID;
                        AssetDatabase.CreateAsset(curViewedTreePoint, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedTreePoint._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadTreePoints();
                        SelectNewTreePoint(curViewedTreePoint._fileName);
                    }
                    break;
                case "RPGItem":
                    directory = "Items/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.item;
                    thisAssetType = AssetType.Item;
                    var curViewedItem = (RPGItem)savedObject;
                    existingELEMENT = (RPGItem)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGItem));
                    if (existingELEMENT != null)
                    {
                        Debug.LogError("item exist");
                        var thisElement = (RPGItem)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedItem.ID = thisElement.ID;
                        thisElement.updateThis(curViewedItem);
                        if (cachedFileName != curViewedItem._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedItem._fileName);
                        AssetActions(thisElement, true);
                        LoadItems();
                        SelectItem(curViewElementIndex);
                    }
                    else
                    {
                        Debug.LogError("item do not exist");
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedItem.ID = ID;
                        AssetDatabase.CreateAsset(curViewedItem, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedItem._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadItems();
                        SelectNewItem(curViewedItem._fileName);
                    }
                    break;
                case "RPGSkill":
                    directory = "Skills/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.skill;
                    thisAssetType = AssetType.Skill;
                    var curViewedSkill = (RPGSkill)savedObject;
                    existingELEMENT = (RPGSkill)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGSkill));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGSkill)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedSkill.ID = thisElement.ID;
                        thisElement.updateThis(curViewedSkill);
                        if (cachedFileName != curViewedSkill._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedSkill._fileName);
                        AssetActions(thisElement, true);
                        LoadSkills();
                        SelectSkill(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedSkill.ID = ID;
                        AssetDatabase.CreateAsset(curViewedSkill, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedSkill._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadSkills();
                        SelectNewSkill(curViewedSkill._fileName);
                    }
                    break;
                case "RPGLevelsTemplate":
                    directory = "LevelsTemplate/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.levelTemplate;
                    thisAssetType = AssetType.LevelTemplate;
                    var curViewedLevelTemplate = (RPGLevelsTemplate)savedObject;
                    existingELEMENT = (RPGLevelsTemplate)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGLevelsTemplate));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGLevelsTemplate)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedLevelTemplate.ID = thisElement.ID;
                        thisElement.updateThis(curViewedLevelTemplate);
                        if (cachedFileName != curViewedLevelTemplate._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedLevelTemplate._fileName);
                        AssetActions(thisElement, true);
                        LoadLevelsTemplate();
                        SelectLevelTemplate(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedLevelTemplate.ID = ID;
                        AssetDatabase.CreateAsset(curViewedLevelTemplate, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedLevelTemplate._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadLevelsTemplate();
                        SelectNewLevelTemplate(curViewedLevelTemplate._fileName);
                    }
                    break;
                case "RPGRace":
                    directory = "Races/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.race;
                    thisAssetType = AssetType.Race;
                    var curViewedRace = (RPGRace)savedObject;
                    existingELEMENT = (RPGRace)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGRace));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGRace)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedRace.ID = thisElement.ID;
                        thisElement.updateThis(curViewedRace);
                        if (cachedFileName != curViewedRace._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedRace._fileName);
                        AssetActions(thisElement, true);
                        LoadRaces();
                        SelectRace(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedRace.ID = ID;
                        AssetDatabase.CreateAsset(curViewedRace, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedRace._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadRaces();
                        SelectNewRace(curViewedRace._fileName);
                    }
                    break;
                case "RPGClass":
                    directory = "Classes/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID._class;
                    thisAssetType = AssetType.Class;
                    var curViewedClass = (RPGClass)savedObject;
                    existingELEMENT = (RPGClass)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGClass));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGClass)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedClass.ID = thisElement.ID;
                        curViewedClass = (RPGClass)ResetSOReferencesBeforeSaving(AssetIDHandler.ASSET_TYPE_ID._class, curViewedClass);
                        thisElement.updateThis(curViewedClass);
                        if (cachedFileName != curViewedClass._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedClass._fileName);
                        AssetActions(thisElement, true);
                        LoadClasses();
                        SelectClass(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedClass.ID = ID;
                        curViewedClass = (RPGClass)ResetSOReferencesBeforeSaving(AssetIDHandler.ASSET_TYPE_ID._class, curViewedClass);
                        AssetDatabase.CreateAsset(curViewedClass, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedClass._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadClasses();
                        SelectNewClass(curViewedClass._fileName);
                    }
                    break;
                case "RPGLootTable":
                    directory = "LootTables/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.lootTable;
                    thisAssetType = AssetType.LootTable;
                    var curViewedLootTable = (RPGLootTable)savedObject;
                    existingELEMENT = (RPGLootTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGLootTable));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGLootTable)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedLootTable.ID = thisElement.ID;
                        thisElement.updateThis(curViewedLootTable);
                        if (cachedFileName != curViewedLootTable._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedLootTable._fileName);
                        AssetActions(thisElement, true);
                        LoadLootTables();
                        SelectLootTable(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedLootTable.ID = ID;
                        AssetDatabase.CreateAsset(curViewedLootTable, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedLootTable._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadLootTables();
                        SelectNewLootTable(curViewedLootTable._fileName);
                    }
                    break;
                case "RPGMerchantTable":
                    directory = "MerchantTables/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.merchantTable;
                    thisAssetType = AssetType.MerchantTable;
                    var curViewedMerchantTable = (RPGMerchantTable)savedObject;
                    existingELEMENT = (RPGMerchantTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGMerchantTable));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGMerchantTable)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedMerchantTable.ID = thisElement.ID;
                        thisElement.updateThis(curViewedMerchantTable);
                        if (cachedFileName != curViewedMerchantTable._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedMerchantTable._fileName);
                        AssetActions(thisElement, true);
                        LoadMerchantTables();
                        SelectMerchantTable(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedMerchantTable.ID = ID;
                        AssetDatabase.CreateAsset(curViewedMerchantTable, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedMerchantTable._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadMerchantTables();
                        SelectNewMerchantTable(curViewedMerchantTable._fileName);
                    }
                    break;
                case "RPGCurrency":
                    directory = "Currencies/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.currency;
                    thisAssetType = AssetType.Currency;
                    var curViewedCurrency = (RPGCurrency)savedObject;
                    existingELEMENT = (RPGCurrency)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGCurrency));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGCurrency)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedCurrency.ID = thisElement.ID;
                        thisElement.updateThis(curViewedCurrency);
                        if (cachedFileName != curViewedCurrency._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedCurrency._fileName);
                        AssetActions(thisElement, true);
                        LoadCurrencies();
                        SelectCurrency(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedCurrency.ID = ID;
                        AssetDatabase.CreateAsset(curViewedCurrency, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedCurrency._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadCurrencies();
                        SelectNewCurrency(curViewedCurrency._fileName);
                    }
                    break;
                case "RPGCraftingRecipe":
                    directory = "CraftingRecipes/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.craftingRecipe;
                    thisAssetType = AssetType.CraftingRecipe;
                    var curViewedCraftingRecipe = (RPGCraftingRecipe)savedObject;
                    existingELEMENT = (RPGCraftingRecipe)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGCraftingRecipe));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGCraftingRecipe)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedCraftingRecipe = (RPGCraftingRecipe)HandleRankCreation(thisAssetType, curViewedCraftingRecipe);
                        curViewedCraftingRecipe.ID = thisElement.ID;
                        thisElement.updateThis(curViewedCraftingRecipe);
                        if (cachedFileName != curViewedCraftingRecipe._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedCraftingRecipe._fileName);
                        AssetActions(thisElement, true);
                        LoadCraftingRecipes();
                        LoadRecipeRankData();
                        SelectCraftingRecipe(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedCraftingRecipe.ID = ID;
                        curViewedCraftingRecipe = (RPGCraftingRecipe)HandleRankCreation(thisAssetType, curViewedCraftingRecipe);
                        AssetDatabase.CreateAsset(curViewedCraftingRecipe, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedCraftingRecipe._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadCraftingRecipes();
                        LoadRecipeRankData();
                        SelectNewCraftingRecipe(curViewedCraftingRecipe._fileName);
                    }
                    LoadRecipeRankData();
                    break;
                case "RPGCraftingStation":
                    directory = "CraftingStations/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.craftingStation;
                    thisAssetType = AssetType.CraftingStation;
                    var curViewedCraftingStation = (RPGCraftingStation)savedObject;
                    existingELEMENT = (RPGCraftingStation)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGCraftingStation));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGCraftingStation)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedCraftingStation.ID = thisElement.ID;
                        thisElement.updateThis(curViewedCraftingStation);
                        if (cachedFileName != curViewedCraftingStation._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedCraftingStation._fileName);
                        AssetActions(thisElement, true);
                        LoadCraftingStations();
                        SelectCraftingStation(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedCraftingStation.ID = ID;
                        AssetDatabase.CreateAsset(curViewedCraftingStation, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedCraftingStation._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadCraftingStations();
                        SelectNewCraftingStation(curViewedCraftingStation._fileName);
                    }
                    break;
                case "RPGTalentTree":
                    directory = "TalentTrees/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.talentTree;
                    thisAssetType = AssetType.TalentTree;
                    var curViewedTalentTree = (RPGTalentTree)savedObject;
                    existingELEMENT = (RPGTalentTree)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGTalentTree));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGTalentTree)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedTalentTree.ID = thisElement.ID;
                        thisElement.updateThis(curViewedTalentTree);
                        if (cachedFileName != curViewedTalentTree._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedTalentTree._fileName);
                        AssetActions(thisElement, true);
                        LoadTalentTrees();
                        SelectTalentTree(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedTalentTree.ID = ID;
                        AssetDatabase.CreateAsset(curViewedTalentTree, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedTalentTree._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadTalentTrees();
                        SelectNewTalentTree(curViewedTalentTree._fileName);
                    }
                    break;
                case "RPGBonus":
                    directory = "Bonuses/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.bonus;
                    thisAssetType = AssetType.Bonus;
                    var curViewedBonus = (RPGBonus)savedObject;
                    existingELEMENT = (RPGBonus)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGBonus));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGBonus)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedBonus = (RPGBonus)HandleRankCreation(thisAssetType, curViewedBonus);
                        curViewedBonus.ID = thisElement.ID;
                        thisElement.updateThis(curViewedBonus);
                        if (cachedFileName != curViewedBonus._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedBonus._fileName);
                        AssetActions(thisElement, true);
                        LoadBonuses();
                        LoadBonusRankData();
                        SelectBonus(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedBonus.ID = ID;
                        curViewedBonus = (RPGBonus)HandleRankCreation(thisAssetType, curViewedBonus);
                        AssetDatabase.CreateAsset(curViewedBonus, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedBonus._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadBonuses();
                        LoadBonusRankData();
                        SelectNewBonus(curViewedBonus._fileName);
                    }
                    LoadBonusRankData();
                    break;
                case "RPGTask":
                    directory = "Tasks/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.task;
                    thisAssetType = AssetType.Task;
                    var curViewedTask = (RPGTask)savedObject;
                    existingELEMENT = (RPGTask)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGTask));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGTask)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedTask.ID = thisElement.ID;
                        thisElement.updateThis(curViewedTask);
                        if (cachedFileName != curViewedTask._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedTask._fileName);
                        AssetActions(thisElement, true);
                        LoadTasks();
                        SelectTask(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedTask.ID = ID;
                        AssetDatabase.CreateAsset(curViewedTask, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedTask._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadTasks();
                        SelectNewTask(curViewedTask._fileName);
                    }
                    break;
                case "RPGQuest":
                    directory = "Quests/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.quest;
                    thisAssetType = AssetType.Quest;
                    var curViewedQuest = (RPGQuest)savedObject;
                    existingELEMENT = (RPGQuest)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGQuest));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGQuest)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedQuest.ID = thisElement.ID;
                        thisElement.updateThis(curViewedQuest);
                        if (cachedFileName != curViewedQuest._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedQuest._fileName);
                        AssetActions(thisElement, true);
                        LoadQuests();
                        SelectQuest(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedQuest.ID = ID;
                        AssetDatabase.CreateAsset(curViewedQuest, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedQuest._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadQuests();
                        SelectNewQuest(curViewedQuest._fileName);
                    }
                    break;
                case "RPGWorldPosition":
                    directory = "WorldPositions/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.worldPosition;
                    thisAssetType = AssetType.WorldPosition;
                    var curViewedWorldPosition = (RPGWorldPosition)savedObject;
                    existingELEMENT = (RPGWorldPosition)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGWorldPosition));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGWorldPosition)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedWorldPosition.ID = thisElement.ID;
                        thisElement.updateThis(curViewedWorldPosition);
                        if (cachedFileName != curViewedWorldPosition._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedWorldPosition._fileName);
                        AssetActions(thisElement, true);
                        LoadWorldPositions();
                        SelectWorldPosition(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedWorldPosition.ID = ID;
                        AssetDatabase.CreateAsset(curViewedWorldPosition, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedWorldPosition._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadWorldPositions();
                        SelectNewWorldPosition(curViewedWorldPosition._fileName);
                    }
                    break;
                case "RPGResourceNode":
                    directory = "ResourceNodes/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.resourceNode;
                    thisAssetType = AssetType.ResourceNode;
                    var curViewedResourceNode = (RPGResourceNode)savedObject;
                    existingELEMENT = (RPGResourceNode)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGResourceNode));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGResourceNode)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedResourceNode = (RPGResourceNode)HandleRankCreation(thisAssetType, curViewedResourceNode);
                        curViewedResourceNode.ID = thisElement.ID;
                        thisElement.updateThis(curViewedResourceNode);
                        if (cachedFileName != curViewedResourceNode._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedResourceNode._fileName);
                        AssetActions(thisElement, true);
                        LoadResourceNodes();
                        LoadResourceNodeRankData();
                        SelectResourceNode(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedResourceNode.ID = ID;
                        curViewedResourceNode = (RPGResourceNode)HandleRankCreation(thisAssetType, curViewedResourceNode);
                        AssetDatabase.CreateAsset(curViewedResourceNode, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedResourceNode._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadResourceNodes();
                        LoadResourceNodeRankData();
                        SelectNewResourceNode(curViewedResourceNode._fileName);
                    }
                    LoadResourceNodeRankData();
                    break;
                case "RPGGameScene":
                    directory = "GameScenes/";
                    AssetIDType = AssetIDHandler.ASSET_TYPE_ID.gameScene;
                    thisAssetType = AssetType.GameScene;
                    var curViewedGameScene = (RPGGameScene)savedObject;
                    existingELEMENT = (RPGGameScene)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", typeof(RPGGameScene));
                    if (existingELEMENT != null)
                    {
                        var thisElement = (RPGGameScene)existingELEMENT;
                        if (thisElement.ID == -1) thisElement.ID = HandleAssetIDSaving(AssetIDType);
                        curViewedGameScene.ID = thisElement.ID;
                        thisElement.updateThis(curViewedGameScene);
                        if (cachedFileName != curViewedGameScene._fileName) AssetDatabase.RenameAsset("Assets/Resources/THMSV/RPGBuilderData/" + directory + cachedFileName + ".asset", curViewedGameScene._fileName);
                        AssetActions(thisElement, true);
                        LoadGameScenes();
                        SelectGameScene(curViewElementIndex);
                    }
                    else
                    {
                        var ID = HandleAssetIDSaving(AssetIDType);
                        curViewedGameScene.ID = ID;
                        AssetDatabase.CreateAsset(curViewedGameScene, "Assets/Resources/THMSV/RPGBuilderData/" + directory + curViewedGameScene._fileName + ".asset");
                        AssetActionsAfterCreate();
                        LoadGameScenes();
                        SelectNewGameScene(curViewedGameScene._fileName);
                    }
                    break;
            }
        }
    }


    private void Save(AssetType ASSET_TYPE)
    {
        switch (ASSET_TYPE)
        {
            case AssetType.Ability:
                SaveAsset(currentlyViewedAbility._name, currentlyViewedAbility._fileName, currentlyViewedAbility);
                break;
            case AssetType.Effect:
                SaveAsset(currentlyViewedEffect._name, currentlyViewedEffect._fileName, currentlyViewedEffect);
                break;
            case AssetType.NPC:
                SaveAsset(currentlyViewedNPC._name, currentlyViewedNPC._fileName, currentlyViewedNPC);
                break;
            case AssetType.Stat:
                SaveAsset(currentlyViewedStat._name, currentlyViewedStat._fileName, currentlyViewedStat);
                break;
            case AssetType.TreePoint:
                SaveAsset(currentlyViewedTreePoint._name, currentlyViewedTreePoint._fileName, currentlyViewedTreePoint);
                break;
            case AssetType.Item:
                SaveAsset(currentlyViewedItem._name, currentlyViewedItem._fileName, currentlyViewedItem);
                break;
            case AssetType.Skill:
                SaveAsset(currentlyViewedSkill._name, currentlyViewedSkill._fileName, currentlyViewedSkill);
                break;
            case AssetType.LevelTemplate:
                SaveAsset(currentlyViewedLevelTemplate._name, currentlyViewedLevelTemplate._fileName, currentlyViewedLevelTemplate);
                break;
            case AssetType.Race:
                SaveAsset(currentlyViewedRace._name, currentlyViewedRace._fileName, currentlyViewedRace);
                break;
            case AssetType.Class:
                SaveAsset(currentlyViewedClass._name, currentlyViewedClass._fileName, currentlyViewedClass);
                break;
            case AssetType.LootTable:
                SaveAsset(currentlyViewedLootTable._name, currentlyViewedLootTable._fileName, currentlyViewedLootTable);
                break;
            case AssetType.MerchantTable:
                SaveAsset(currentlyViewedMerchantTable._name, currentlyViewedMerchantTable._fileName, currentlyViewedMerchantTable);
                break;
            case AssetType.Currency:
                SaveAsset(currentlyViewedCurrency._name, currentlyViewedCurrency._fileName, currentlyViewedCurrency);
                break;
            case AssetType.CraftingRecipe:
                SaveAsset(currentlyViewedCraftingRecipe._name, currentlyViewedCraftingRecipe._fileName, currentlyViewedCraftingRecipe);
                break;
            case AssetType.CraftingStation:
                SaveAsset(currentlyViewedCraftingStation._name, currentlyViewedCraftingStation._fileName, currentlyViewedCraftingStation);
                break;
            case AssetType.TalentTree:
                SaveAsset(currentlyViewedTalentTree._name, currentlyViewedTalentTree._fileName, currentlyViewedTalentTree);
                break;
            case AssetType.Bonus:
                SaveAsset(currentlyViewedBonus._name, currentlyViewedBonus._fileName, currentlyViewedBonus);
                break;
            case AssetType.Task:
                SaveAsset(currentlyViewedTask._name, currentlyViewedTask._fileName, currentlyViewedTask);
                break;
            case AssetType.Quest:
                SaveAsset(currentlyViewedQuest._name, currentlyViewedQuest._fileName, currentlyViewedQuest);
                break;
            case AssetType.WorldPosition:
                SaveAsset(currentlyViewedWorldPosition._name, currentlyViewedWorldPosition._fileName, currentlyViewedWorldPosition);
                break;
            case AssetType.ResourceNode:
                SaveAsset(currentlyViewedResourceNode._name, currentlyViewedResourceNode._fileName, currentlyViewedResourceNode);
                break;
            case AssetType.GameScene:
                SaveAsset(currentlyViewedGameScene._name, currentlyViewedGameScene._fileName, currentlyViewedGameScene);
                break;
            case AssetType.CombatSettings:
                var existingCombatSettings = (RPGCombatDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Settings/" + "CombatSettings" + ".asset", typeof(RPGCombatDATA));
                if (existingCombatSettings != null)
                {
                    existingCombatSettings.updateThis(combatSettings);
                    AssetActions(existingCombatSettings, true);
                    LoadSettings();
                }
                else
                {
                    AssetDatabase.CreateAsset(combatSettings, "Assets/Resources/THMSV/RPGBuilderData/Settings/" + "CombatSettings" + ".asset");
                    AssetActionsAfterCreate();
                    LoadSettings();
                }
                break;
            case AssetType.ItemSettings:
                var existingItemSettings = (RPGItemDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Settings/" + "ItemSettings" + ".asset", typeof(RPGItemDATA));
                if (existingItemSettings != null)
                {
                    existingItemSettings.updateThis(itemSettings);
                    AssetActions(existingItemSettings, true);
                    LoadSettings();
                }
                else
                {
                    AssetDatabase.CreateAsset(itemSettings, "Assets/Resources/THMSV/RPGBuilderData/Settings/" + "ItemSettings" + ".asset");
                    AssetActionsAfterCreate();
                    LoadSettings();
                }
                break;
            case AssetType.GeneralSettings:
                var existingGeneralSettings = (RPGGeneralDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Settings/" + "GeneralSettings" + ".asset", typeof(RPGGeneralDATA));
                if (existingGeneralSettings != null)
                {
                    existingGeneralSettings.updateThis(generalSettings);
                    AssetActions(existingGeneralSettings, true);
                    LoadSettings();
                }
                else
                {
                    AssetDatabase.CreateAsset(combatSettings, "Assets/Resources/THMSV/RPGBuilderData/Settings/" + "GeneralSettings" + ".asset");
                    AssetActionsAfterCreate();
                    LoadSettings();
                }
                break;
        }
    }

    private void CreateNew(AssetType ASSET_TYPE)
    {
        temporaryAbRankList.Clear();
        temporaryRecipeRankList.Clear();
        temporaryResourceNodeRankList.Clear();
        temporaryBonusRankList.Clear();
        switch (ASSET_TYPE)
        {
            case AssetType.Ability:
                currentlyViewedAbility = new RPGAbility();
                break;
            case AssetType.Effect:
                currentlyViewedEffect = new RPGEffect();
                break;
            case AssetType.Item:
                currentlyViewedItem = new RPGItem();
                break;
            case AssetType.NPC:
                currentlyViewedNPC = new RPGNpc();
                break;
            case AssetType.Stat:
                currentlyViewedStat = new RPGStat();
                break;
            case AssetType.Skill:
                currentlyViewedSkill = new RPGSkill();
                break;
            case AssetType.LevelTemplate:
                currentlyViewedLevelTemplate = new RPGLevelsTemplate();
                break;
            case AssetType.Race:
                currentlyViewedRace = new RPGRace();
                break;
            case AssetType.Class:
                currentlyViewedClass = new RPGClass();
                break;
            case AssetType.TalentTree:
                currentlyViewedTalentTree = new RPGTalentTree();
                break;
            case AssetType.TreePoint:
                currentlyViewedTreePoint = new RPGTreePoint();
                break;
            case AssetType.LootTable:
                currentlyViewedLootTable = new RPGLootTable();
                break;
            case AssetType.WorldPosition:
                currentlyViewedWorldPosition = new RPGWorldPosition();
                break;
            case AssetType.MerchantTable:
                currentlyViewedMerchantTable = new RPGMerchantTable();
                break;
            case AssetType.Currency:
                currentlyViewedCurrency = new RPGCurrency();
                break;
            case AssetType.Task:
                currentlyViewedTask = new RPGTask();
                break;
            case AssetType.Quest:
                currentlyViewedQuest = new RPGQuest();
                break;
            case AssetType.CraftingRecipe:
                currentlyViewedCraftingRecipe = new RPGCraftingRecipe();
                break;
            case AssetType.CraftingStation:
                currentlyViewedCraftingStation = new RPGCraftingStation();
                break;
            case AssetType.ResourceNode:
                currentlyViewedResourceNode = new RPGResourceNode();
                break;
            case AssetType.Bonus:
                currentlyViewedBonus = new RPGBonus();
                break;
            case AssetType.GameScene:
                currentlyViewedGameScene = new RPGGameScene();
                break;
        }

        cachedFileName = "";
        cachedElementName = "";
        curViewElementIndex = -1;
    }

    private void Duplicate(AssetType ASSET_TYPE)
    {
        switch (ASSET_TYPE)
        {
            case AssetType.Ability:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGAbility)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Abilities/" + cachedFileName + ".asset", typeof(RPGAbility));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Abilities/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Abilities/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGAbility)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Abilities/" + cachedFileName + " Copy" + ".asset", typeof(RPGAbility));
                        duplicated._name = currentlyViewedAbility._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.ability);
                        duplicated.ID = ID;

                        for (var i = 0; i < duplicated.ranks.Count; i++)
                        {
                            var rankNumber = i + 1;
                            var rank = (RPGAbilityRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGAbilityRankData));
                            if (rank != null)
                            {
                                AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", "Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset");
                                
                                var duplicatedRank = (RPGAbilityRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset", typeof(RPGAbilityRankData));

                                var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.abilityRank);
                                duplicatedRank.ID = rankID;
                                duplicated.ranks[i].rankID = rankID;
                            } else
                            {
                                Debug.LogError("Could not find ability rank to duplicate");
                            }
                        }

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadAbilities();
                        LoadAbilityRankData();
                        SelectAbility(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.Effect:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGEffect)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Effects/" + cachedFileName + ".asset", typeof(RPGEffect));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Effects/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Effects/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGEffect)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Effects/" + cachedFileName + " Copy" + ".asset", typeof(RPGEffect));
                        duplicated._name = currentlyViewedEffect._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.effect);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadEffects();
                        SelectEffect(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.Item:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGItem)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Items/" + cachedFileName + ".asset", typeof(RPGItem));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Items/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Items/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGItem)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Items/" + cachedFileName + " Copy" + ".asset", typeof(RPGItem));
                        duplicated._name = currentlyViewedItem._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.item);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadItems();
                        SelectItem(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.NPC:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGNpc)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/NPCs/" + cachedFileName + ".asset", typeof(RPGNpc));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/NPCs/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/NPCs/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGNpc)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/NPCs/" + cachedFileName + " Copy" + ".asset", typeof(RPGNpc));
                        duplicated._name = currentlyViewedNPC._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.npc);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadNPCs();
                        SelectNPC(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.Stat:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGStat)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Stats/" + cachedFileName + ".asset", typeof(RPGStat));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Stats/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Stats/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGStat)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Stats/" + cachedFileName + " Copy" + ".asset", typeof(RPGStat));
                        duplicated._name = currentlyViewedStat._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.stat);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadStats();
                        SelectStat(curViewElementIndex + 1);
                    }
                    else
                    {
                        Debug.LogError("cannot find");
                    }
                }
                break;
            case AssetType.Skill:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGSkill)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Skills/" + cachedFileName + ".asset", typeof(RPGSkill));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Skills/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Skills/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGSkill)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Skills/" + cachedFileName + " Copy" + ".asset", typeof(RPGSkill));
                        duplicated._name = currentlyViewedAbility._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.skill);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadSkills();
                        SelectSkill(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.LevelTemplate:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGLevelsTemplate)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/LevelsTemplate/" + cachedFileName + ".asset", typeof(RPGLevelsTemplate));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/LevelsTemplate/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/LevelsTemplate/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGLevelsTemplate)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/LevelsTemplate/" + cachedFileName + " Copy" + ".asset", typeof(RPGLevelsTemplate));
                        duplicated._name = currentlyViewedLevelTemplate._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.levelTemplate);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadLevelsTemplate();
                        SelectLevelTemplate(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.Race:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGRace)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Races/" + cachedFileName + ".asset", typeof(RPGRace));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Races/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Races/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGRace)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Races/" + cachedFileName + " Copy" + ".asset", typeof(RPGRace));
                        duplicated._name = currentlyViewedRace._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.race);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadRaces();
                        SelectRace(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.Class:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGClass)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Classes/" + cachedFileName + ".asset", typeof(RPGClass));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Classes/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Classes/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGClass)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Classes/" + cachedFileName + " Copy" + ".asset", typeof(RPGClass));
                        duplicated._name = currentlyViewedClass._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID._class);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadClasses();
                        SelectClass(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.TalentTree:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGTalentTree)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/TalentTrees/" + cachedFileName + ".asset", typeof(RPGTalentTree));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/TalentTrees/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/TalentTrees/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGTalentTree)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/TalentTrees/" + cachedFileName + " Copy" + ".asset", typeof(RPGTalentTree));
                        duplicated._name = currentlyViewedTalentTree._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.talentTree);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadTalentTrees();
                        SelectTalentTree(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.TreePoint:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGTreePoint)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/TreePoints/" + cachedFileName + ".asset", typeof(RPGTreePoint));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/TreePoints/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/TreePoints/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGTreePoint)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/TreePoints/" + cachedFileName + " Copy" + ".asset", typeof(RPGTreePoint));
                        duplicated._name = currentlyViewedTreePoint._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.treePoint);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadTreePoints();
                        SelectTreePoint(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.LootTable:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGLootTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/LootTables/" + cachedFileName + ".asset", typeof(RPGLootTable));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/LootTables/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/LootTables/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGLootTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/LootTables/" + cachedFileName + " Copy" + ".asset", typeof(RPGLootTable));
                        duplicated._name = currentlyViewedLootTable._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.lootTable);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadLootTables();
                        SelectLootTable(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.WorldPosition:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGWorldPosition)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/WorldPositions/" + cachedFileName + ".asset", typeof(RPGWorldPosition));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/WorldPositions/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/WorldPositions/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGWorldPosition)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/WorldPositions/" + cachedFileName + " Copy" + ".asset", typeof(RPGWorldPosition));
                        duplicated._name = currentlyViewedWorldPosition._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.worldPosition);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadWorldPositions();
                        SelectWorldPosition(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.MerchantTable:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGMerchantTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/MerchantTables/" + cachedFileName + ".asset", typeof(RPGMerchantTable));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/MerchantTables/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/MerchantTables/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGMerchantTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/MerchantTables/" + cachedFileName + " Copy" + ".asset", typeof(RPGMerchantTable));
                        duplicated._name = currentlyViewedMerchantTable._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.merchantTable);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadMerchantTables();
                        SelectMerchantTable(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.Currency:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGCurrency)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Currencies/" + cachedFileName + ".asset", typeof(RPGCurrency));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Currencies/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Currencies/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGCurrency)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Currencies/" + cachedFileName + " Copy" + ".asset", typeof(RPGCurrency));
                        duplicated._name = currentlyViewedCurrency._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.currency);
                        duplicated.ID = ID;


                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadCurrencies();
                        SelectCurrency(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.Task:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingTask = (RPGTask)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Tasks/" + cachedFileName + ".asset", typeof(RPGTask));
                    if (existingTask != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Tasks/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Tasks/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGTask)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Tasks/" + cachedFileName + " Copy" + ".asset", typeof(RPGTask));
                        duplicated._name = currentlyViewedTask._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.task);
                        duplicated.ID = ID;
                        
                        EditorUtility.SetDirty(existingTask);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadTasks();
                        SelectTask(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.Quest:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingTask = (RPGQuest)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Quests/" + cachedFileName + ".asset", typeof(RPGQuest));
                    if (existingTask != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Quests/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Quests/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGQuest)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Quests/" + cachedFileName + " Copy" + ".asset", typeof(RPGQuest));
                        duplicated._name = currentlyViewedQuest._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.quest);
                        duplicated.ID = ID;


                        EditorUtility.SetDirty(existingTask);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadQuests();
                        SelectQuest(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.CraftingRecipe:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingTask = (RPGCraftingRecipe)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipes/" + cachedFileName + ".asset", typeof(RPGCraftingRecipe));
                    if (existingTask != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipes/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/CraftingRecipes/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGCraftingRecipe)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipes/" + cachedFileName + " Copy" + ".asset", typeof(RPGCraftingRecipe));
                        duplicated._name = currentlyViewedCraftingRecipe._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.craftingRecipe);
                        duplicated.ID = ID;

                        for (var i = 0; i < duplicated.ranks.Count; i++)
                        {
                            var rankNumber = i + 1;
                            var rank = (RPGCraftingRecipeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGCraftingRecipeRankData));
                            if (rank != null)
                            {
                                AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", "Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset");
                                var duplicatedRank = (RPGCraftingRecipeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset", typeof(RPGCraftingRecipeRankData));

                                var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.recipeRank);
                                duplicatedRank.ID = rankID;
                                duplicated.ranks[i].rankID = rankID;
                            }
                            else
                            {
                                Debug.LogError("Could not find recipe rank to duplicate");
                            }
                        }

                        EditorUtility.SetDirty(existingTask);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadCraftingRecipes();
                        LoadRecipeRankData();
                        SelectCraftingRecipe(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.CraftingStation:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingTask = (RPGCraftingStation)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingStations/" + cachedFileName + ".asset", typeof(RPGCraftingStation));
                    if (existingTask != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingStations/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/CraftingStations/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGCraftingStation)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingStations/" + cachedFileName + " Copy" + ".asset", typeof(RPGCraftingStation));
                        duplicated._name = currentlyViewedCraftingStation._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.craftingStation);
                        duplicated.ID = ID;


                        EditorUtility.SetDirty(existingTask);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadCraftingStations();
                        SelectCraftingStation(curViewElementIndex + 1);
                    }
                }
                break;

            case AssetType.ResourceNode:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingTask = (RPGResourceNode)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodes/" + cachedFileName + ".asset", typeof(RPGResourceNode));
                    if (existingTask != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/ResourceNodes/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/ResourceNodes/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGResourceNode)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodes/" + cachedFileName + " Copy" + ".asset", typeof(RPGResourceNode));
                        duplicated._name = currentlyViewedResourceNode._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.resourceNode);
                        duplicated.ID = ID;

                        for (var i = 0; i < duplicated.ranks.Count; i++)
                        {
                            var rankNumber = i + 1;
                            var rank = (RPGResourceNodeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGResourceNodeRankData));
                            if (rank != null)
                            {
                                AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", "Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset");
                                var duplicatedRank = (RPGResourceNodeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset", typeof(RPGResourceNodeRankData));

                                var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.resourceNodeRank);
                                duplicatedRank.ID = rankID;
                                duplicated.ranks[i].rankID = rankID;
                            }
                            else
                            {
                                Debug.LogError("Could not find resource node rank to duplicate");
                            }
                        }

                        EditorUtility.SetDirty(existingTask);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadResourceNodes();
                        LoadResourceNodeRankData();
                        SelectResourceNode(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.Bonus:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGBonus)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Bonuses/" + cachedFileName + ".asset", typeof(RPGBonus));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/Bonuses/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/Bonuses/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGBonus)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Bonuses/" + cachedFileName + " Copy" + ".asset", typeof(RPGBonus));
                        duplicated._name = currentlyViewedBonus._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.bonus);
                        duplicated.ID = ID;

                        for (var i = 0; i < duplicated.ranks.Count; i++)
                        {
                            var rankNumber = i + 1;
                            var rank = (RPGBonusRankDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGBonusRankDATA));
                            if (rank != null)
                            {
                                AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", "Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset");
                                var duplicatedRank = (RPGBonusRankDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + " Copy" + "_RankData_" + rankNumber + ".asset", typeof(RPGBonusRankDATA));

                                var rankID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.bonusRank);
                                duplicatedRank.ID = rankID;
                                duplicated.ranks[i].rankID = rankID;
                            }
                            else
                            {
                                Debug.LogError("Could not find bonus rank to duplicate");
                            }
                        }

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadBonuses();
                        LoadBonusRankData();
                        SelectBonus(curViewElementIndex + 1);
                    }
                }
                break;
            case AssetType.GameScene:
                if (cachedFileName != null && cachedFileName.Length > 0 && curViewElementIndex != -1)
                {
                    var existingAbility = (RPGGameScene)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/GameScenes/" + cachedFileName + ".asset", typeof(RPGGameScene));
                    if (existingAbility != null)
                    {
                        AssetDatabase.CopyAsset("Assets/Resources/THMSV/RPGBuilderData/GameScenes/" + cachedFileName + ".asset", "Assets/Resources/THMSV/RPGBuilderData/GameScenes/" + cachedFileName + " Copy" + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var duplicated = (RPGGameScene)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/GameScenes/" + cachedFileName + " Copy" + ".asset", typeof(RPGGameScene));
                        duplicated._name = currentlyViewedGameScene._name + " Copy";
                        duplicated._fileName = cachedFileName + " Copy";

                        var ID = HandleAssetIDSaving(AssetIDHandler.ASSET_TYPE_ID.gameScene);
                        duplicated.ID = ID;

                        EditorUtility.SetDirty(existingAbility);
                        EditorUtility.SetDirty(duplicated);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        LoadGameScenes();
                        SelectGameScene(curViewElementIndex + 1);
                    }
                }
                break;
        }
    }

    private void Delete(AssetType ASSET_TYPE)
    {
        switch (ASSET_TYPE)
        {
            case AssetType.Ability:
                var existingAbility = (RPGAbility)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Abilities/" + cachedFileName + ".asset", typeof(RPGAbility));
                if (existingAbility != null)
                {
                    for (var i = 0; i < existingAbility.ranks.Count; i++)
                    {
                        var rankNumber = i + 1;
                        var rank = (RPGAbilityRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGAbilityRankData));
                        if (rank != null)
                            AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/AbilityRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset");
                        else
                            Debug.LogError("Ability rank does not exist name = " + cachedElementName + rankNumber);
                    }

                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Abilities/" + cachedFileName + ".asset");
                }
                else
                {
                    Debug.LogError("Ability does not exist");
                }
                LoadAbilities();
                LoadAbilityRankData();
                SelectAbility(0);
                break;
            case AssetType.Effect:
                var existingEffect = (RPGEffect)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Effects/" + cachedFileName + ".asset", typeof(RPGEffect));
                if (existingEffect != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Effects/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Effect does not exist");
                LoadEffects();
                SelectEffect(0);
                break;
            case AssetType.Item:
                var existingItem = (RPGItem)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Items/" + cachedFileName + ".asset", typeof(RPGItem));
                if (existingItem != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Items/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Item does not exist");
                LoadItems();
                SelectItem(0);
                break;

            case AssetType.NPC:
                var existingNPC = (RPGNpc)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/NPCs/" + cachedFileName + ".asset", typeof(RPGNpc));
                if (existingNPC != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/NPCs/" + cachedFileName + ".asset");
                else
                    Debug.LogError("NPC does not exist");
                LoadNPCs();
                SelectNPC(0);
                break;
            case AssetType.Stat:
                var existingStats = (RPGStat)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Stats/" + cachedFileName + ".asset", typeof(RPGStat));
                if (existingStats != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Stats/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Stats does not exist");
                LoadStats();
                SelectStat(0);
                break;
            case AssetType.Skill:
                var existingSkill = (RPGSkill)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Skills/" + cachedFileName + ".asset", typeof(RPGSkill));
                if (existingSkill != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Skills/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Skill does not exist");
                LoadSkills();
                SelectSkill(0);
                break;
            case AssetType.LevelTemplate:
                var existingLevelTemplate = (RPGLevelsTemplate)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/LevelsTemplate/" + cachedFileName + ".asset", typeof(RPGLevelsTemplate));
                if (existingLevelTemplate != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/LevelsTemplate/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Levels Template does not exist");
                LoadLevelsTemplate();
                SelectLevelTemplate(0);
                break;
            case AssetType.Race:
                var existingRace = (RPGRace)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Races/" + cachedFileName + ".asset", typeof(RPGRace));
                if (existingRace != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Races/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Races does not exist");
                LoadRaces();
                SelectRace(0);
                break;
            case AssetType.Class:
                var existingClass = (RPGClass)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Classes/" + cachedFileName + ".asset", typeof(RPGClass));
                if (existingClass != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Classes/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Classes does not exist");
                LoadClasses();
                SelectClass(0);
                break;
            case AssetType.TalentTree:
                var existingAbilityTree = (RPGTalentTree)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/TalentTrees/" + cachedFileName + ".asset", typeof(RPGTalentTree));
                if (existingAbilityTree != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/TalentTrees/" + cachedFileName + ".asset");
                else
                    Debug.LogError("TalentTrees does not exist");
                LoadTalentTrees();
                SelectTalentTree(0);
                break;
            case AssetType.TreePoint:
                var existingTreePoint = (RPGTreePoint)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/TreePoints/" + cachedFileName + ".asset", typeof(RPGTreePoint));
                if (existingTreePoint != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/TreePoints/" + cachedFileName + ".asset");
                else
                    Debug.LogError("TreePoint does not exist");
                LoadTreePoints();
                SelectTreePoint(0);
                break;
            case AssetType.LootTable:
                var existingLootTable = (RPGLootTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/LootTables/" + cachedFileName + ".asset", typeof(RPGLootTable));
                if (existingLootTable != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/LootTables/" + cachedFileName + ".asset");
                else
                    Debug.LogError("LootTable does not exist");
                LoadLootTables();
                SelectLootTable(0);
                break;
            case AssetType.WorldPosition:
                var existingWorldPosition = (RPGWorldPosition)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/WorldPositions/" + cachedFileName + ".asset", typeof(RPGWorldPosition));
                if (existingWorldPosition != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/WorldPositions/" + cachedFileName + ".asset");
                else
                    Debug.LogError("WorldPosition does not exist");
                LoadWorldPositions();
                SelectWorldPosition(0);
                break;
            case AssetType.MerchantTable:
                var existingMerchantTable = (RPGMerchantTable)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/MerchantTables/" + cachedFileName + ".asset", typeof(RPGMerchantTable));
                if (existingMerchantTable != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/MerchantTables/" + cachedFileName + ".asset");
                else
                    Debug.LogError("MerchantTable does not exist");
                LoadMerchantTables();
                SelectMerchantTable(0);
                break;
            case AssetType.Currency:
                var existingCurrency = (RPGCurrency)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Currencies/" + cachedFileName + ".asset", typeof(RPGCurrency));
                if (existingCurrency != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Currencies/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Currency does not exist");
                LoadCurrencies();
                SelectCurrency(0);
                break;
            case AssetType.Task:
                var existingTask = (RPGTask)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Tasks/" + cachedFileName + ".asset", typeof(RPGTask));
                if (existingTask != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Tasks/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Task does not exist");
                LoadTasks();
                SelectTask(0);
                break;
            case AssetType.Quest:
                var existingQuest = (RPGQuest)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Quests/" + cachedFileName + ".asset", typeof(RPGQuest));
                if (existingQuest != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Quests/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Quest does not exist");
                LoadQuests();
                SelectQuest(0);
                break;
            case AssetType.CraftingRecipe:
                var existingCraftingRecipe = (RPGCraftingRecipe)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipes/" + cachedFileName + ".asset", typeof(RPGCraftingRecipe));
                if (existingCraftingRecipe != null)
                {
                    for (var i = 0; i < existingCraftingRecipe.ranks.Count; i++)
                    {
                        var rankNumber = i + 1;
                        var rank = (RPGCraftingRecipeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGCraftingRecipeRankData));
                        if (rank != null)
                            AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset");
                        else
                            Debug.LogError("Crafting Recipe rank does not exist name = " + cachedElementName + rankNumber);
                    }
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingRecipes/" + cachedFileName + ".asset");
                }
                else
                {
                    Debug.LogError("Crafting recipe does not exist");
                }
                LoadCraftingRecipes();
                LoadRecipeRankData();
                SelectCraftingRecipe(0);
                break;
            case AssetType.CraftingStation:
                var existingCraftingStation = (RPGCraftingStation)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/CraftingStations/" + cachedFileName + ".asset", typeof(RPGCraftingStation));
                if (existingCraftingStation != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/CraftingStations/" + cachedFileName + ".asset");
                else
                    Debug.LogError("Crafting station does not exist");
                LoadCraftingStations();
                SelectCraftingStation(0);
                break;
            case AssetType.ResourceNode:
                var existingResourceNode = (RPGResourceNode)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodes/" + cachedFileName + ".asset", typeof(RPGResourceNode));
                if (existingResourceNode != null)
                {
                    for (var i = 0; i < existingResourceNode.ranks.Count; i++)
                    {
                        var rankNumber = i + 1;
                        var rank = (RPGResourceNodeRankData)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGResourceNodeRankData));
                        if (rank != null)
                            AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/ResourceNodeRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset");
                        else
                            Debug.LogError("Resource Node rank does not exist name = " + cachedElementName + rankNumber);
                    }
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/ResourceNodes/" + cachedFileName + ".asset");
                }
                else
                {
                    Debug.LogError("Resource Node does not exist");
                }
                LoadResourceNodes();
                LoadResourceNodeRankData();
                SelectResourceNode(0);
                break;
            case AssetType.Bonus:
                var existingBonus = (RPGBonus)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/Bonuses/" + cachedFileName + ".asset", typeof(RPGBonus));
                if (existingBonus != null)
                {
                    for (var i = 0; i < existingBonus.ranks.Count; i++)
                    {
                        var rankNumber = i + 1;
                        var rank = (RPGBonusRankDATA)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset", typeof(RPGBonusRankDATA));
                        if (rank != null)
                            AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/BonusRankData/" + cachedElementName + "_RankData_" + rankNumber + ".asset");
                        else
                            Debug.LogError("Bonus rank does not exist name = " + cachedElementName + rankNumber);
                    }
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/Bonuses/" + cachedFileName + ".asset");
                }
                else
                {
                    Debug.LogError("Bonus Node does not exist");
                }
                LoadBonuses();
                LoadBonusRankData();
                SelectBonus(0);
                break;
            case AssetType.GameScene:
                var existingGameScene = (RPGGameScene)AssetDatabase.LoadAssetAtPath("Assets/Resources/THMSV/RPGBuilderData/GameScenes/" + cachedFileName + ".asset", typeof(RPGGameScene));
                if (existingGameScene != null)
                    AssetDatabase.DeleteAsset("Assets/Resources/THMSV/RPGBuilderData/GameScenes/" + cachedFileName + ".asset");
                else
                    Debug.LogError("GameScene does not exist");
                LoadGameScenes();
                SelectGameScene(0);
                break;
        }
        curwindow.Repaint();
    }

    private void SelectAbility(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allAbilities[abilityIndex]._fileName;
        cachedElementName = allAbilities[abilityIndex]._name;
        currentlyViewedAbility = Instantiate(allAbilities[abilityIndex]) as RPGAbility;
        GUI.FocusControl(null);

        temporaryAbRankList.Clear();
        for (var i = 0; i < currentlyViewedAbility.ranks.Count; i++)
        {
            var newRankData = new RPGAbility.rankDATA();
            newRankData.rankID = currentlyViewedAbility.ranks[i].rankID;
            newRankData.rankREF = Instantiate(RPGBuilderUtilities.GetAbilityRankFromIDEditor(currentlyViewedAbility.ranks[i].rankID, allAbilityRanks)) as RPGAbilityRankData;
            temporaryAbRankList.Add(newRankData);
        }
    }

    private void SelectNewAbility(string fileName)
    {
        RPGAbility newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allAbilities.Count; i++)
            if (allAbilities[i]._fileName == fileName)
            {
                newAb = allAbilities[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            cachedElementName = newAb._name;
            currentlyViewedAbility = Instantiate(allAbilities[abIndex]) as RPGAbility;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allAbilities[0]._fileName;
            cachedElementName = allAbilities[0]._name;
            currentlyViewedAbility = Instantiate(allAbilities[0]) as RPGAbility;
        }
        GUI.FocusControl(null);
    }

    private void SelectBonus(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allBonuses[abilityIndex]._fileName;
        cachedElementName = allBonuses[abilityIndex]._name;
        currentlyViewedBonus = Instantiate(allBonuses[abilityIndex]) as RPGBonus;
        GUI.FocusControl(null);

        temporaryBonusRankList.Clear();
        for (var i = 0; i < currentlyViewedBonus.ranks.Count; i++)
        {
            var newRankData = new RPGBonus.rankDATA();
            newRankData.rankID = currentlyViewedBonus.ranks[i].rankID;
            newRankData.rankREF = Instantiate(RPGBuilderUtilities.GetBonusRankFromIDEditor(currentlyViewedBonus.ranks[i].rankID, allBonusRanks)) as RPGBonusRankDATA;
            temporaryBonusRankList.Add(newRankData);
        }
    }

    private void SelectNewBonus(string fileName)
    {
        RPGBonus newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allBonuses.Count; i++)
            if (allBonuses[i]._fileName == fileName)
            {
                newAb = allBonuses[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            cachedElementName = newAb._name;
            currentlyViewedBonus = Instantiate(allBonuses[abIndex]) as RPGBonus;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allBonuses[0]._fileName;
            cachedElementName = allBonuses[0]._name;
            currentlyViewedBonus = Instantiate(allBonuses[0]) as RPGBonus;
        }
        GUI.FocusControl(null);
    }

    private void SelectTalentTree(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allTalentTrees[abilityIndex]._fileName;
        currentlyViewedTalentTree = Instantiate(allTalentTrees[abilityIndex]) as RPGTalentTree;
        GUI.FocusControl(null);
    }

    private void SelectNewTalentTree(string fileName)
    {
        RPGTalentTree newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allTalentTrees.Count; i++)
            if (allTalentTrees[i]._fileName == fileName)
            {
                newAb = allTalentTrees[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedTalentTree = Instantiate(allTalentTrees[abIndex]) as RPGTalentTree;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allTalentTrees[0]._fileName;
            currentlyViewedTalentTree = Instantiate(allTalentTrees[0]) as RPGTalentTree;
        }
        GUI.FocusControl(null);
    }

    private void SelectCraftingRecipe(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allCraftingRecipes[abilityIndex]._fileName;
        cachedElementName = allCraftingRecipes[abilityIndex]._name;
        currentlyViewedCraftingRecipe = Instantiate(allCraftingRecipes[abilityIndex]) as RPGCraftingRecipe;
        GUI.FocusControl(null);

        temporaryRecipeRankList.Clear();
        for (var i = 0; i < currentlyViewedCraftingRecipe.ranks.Count; i++)
        {
            var newRankData = new RPGCraftingRecipe.rankDATA();
            newRankData.rankID = currentlyViewedCraftingRecipe.ranks[i].rankID;
            newRankData.rankREF = Instantiate(RPGBuilderUtilities.GetCraftingRecipeRankFromIDEditor(currentlyViewedCraftingRecipe.ranks[i].rankID, allCraftingRecipeRanks)) as RPGCraftingRecipeRankData;
            temporaryRecipeRankList.Add(newRankData);
        }
    }

    private void SelectNewCraftingRecipe(string fileName)
    {
        RPGCraftingRecipe newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allCraftingRecipes.Count; i++)
            if (allCraftingRecipes[i]._fileName == fileName)
            {
                newAb = allCraftingRecipes[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            cachedElementName = newAb._name;
            currentlyViewedCraftingRecipe = Instantiate(allCraftingRecipes[abIndex]) as RPGCraftingRecipe;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allCraftingRecipes[0]._fileName;
            cachedElementName = allCraftingRecipes[0]._name;
            currentlyViewedCraftingRecipe = Instantiate(allCraftingRecipes[0]) as RPGCraftingRecipe;
        }
        GUI.FocusControl(null);
    }

    private void SelectCraftingStation(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allCraftingStations[abilityIndex]._fileName;
        currentlyViewedCraftingStation = Instantiate(allCraftingStations[abilityIndex]) as RPGCraftingStation;
        GUI.FocusControl(null);
    }

    private void SelectNewCraftingStation(string fileName)
    {
        RPGCraftingStation newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allCraftingStations.Count; i++)
            if (allCraftingStations[i]._fileName == fileName)
            {
                newAb = allCraftingStations[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedCraftingStation = Instantiate(allCraftingStations[abIndex]) as RPGCraftingStation;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allCraftingStations[0]._fileName;
            currentlyViewedCraftingStation = Instantiate(allCraftingStations[0]) as RPGCraftingStation;
        }
        GUI.FocusControl(null);
    }

    private void SelectResourceNode(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allResourceNodes[abilityIndex]._fileName;
        cachedElementName = allResourceNodes[abilityIndex]._name;
        currentlyViewedResourceNode = Instantiate(allResourceNodes[abilityIndex]) as RPGResourceNode;
        GUI.FocusControl(null);

        temporaryResourceNodeRankList.Clear();
        for (var i = 0; i < currentlyViewedResourceNode.ranks.Count; i++)
        {
            var newRankData = new RPGResourceNode.rankDATA();
            newRankData.rankID = currentlyViewedResourceNode.ranks[i].rankID;
            newRankData.rankREF = Instantiate(RPGBuilderUtilities.GetResourceNodeRankFromIDEditor(currentlyViewedResourceNode.ranks[i].rankID, allResourceNodeRanks)) as RPGResourceNodeRankData;
            temporaryResourceNodeRankList.Add(newRankData);
        }
    }

    private void SelectNewResourceNode(string fileName)
    {
        RPGResourceNode newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allResourceNodes.Count; i++)
            if (allResourceNodes[i]._fileName == fileName)
            {
                newAb = allResourceNodes[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            cachedElementName = newAb._name;
            currentlyViewedResourceNode = Instantiate(allResourceNodes[abIndex]) as RPGResourceNode;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allResourceNodes[0]._fileName;
            cachedElementName = allResourceNodes[0]._name;
            currentlyViewedResourceNode = Instantiate(allResourceNodes[0]) as RPGResourceNode;
        }
        GUI.FocusControl(null);
    }

    private void SelectGameScene(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allGameScenes[abilityIndex]._fileName;
        currentlyViewedGameScene = Instantiate(allGameScenes[abilityIndex]) as RPGGameScene;
        GUI.FocusControl(null);
    }

    private void SelectNewGameScene(string fileName)
    {
        RPGGameScene newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allGameScenes.Count; i++)
            if (allGameScenes[i]._fileName == fileName)
            {
                newAb = allGameScenes[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedGameScene = Instantiate(allGameScenes[abIndex]) as RPGGameScene;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allGameScenes[0]._fileName;
            currentlyViewedGameScene = Instantiate(allGameScenes[0]) as RPGGameScene;
        }
        GUI.FocusControl(null);
    }

    private void SelectTask(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allTasks[abilityIndex]._fileName;
        currentlyViewedTask = Instantiate(allTasks[abilityIndex]) as RPGTask;
        GUI.FocusControl(null);
    }

    private void SelectNewTask(string fileName)
    {
        RPGTask newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allTasks.Count; i++)
            if (allTasks[i]._fileName == fileName)
            {
                newAb = allTasks[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedTask = Instantiate(allTasks[abIndex]) as RPGTask;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allTasks[0]._fileName;
            currentlyViewedTask = Instantiate(allTasks[0]) as RPGTask;
        }
        GUI.FocusControl(null);
    }

    private void SelectQuest(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allQuests[abilityIndex]._fileName;
        currentlyViewedQuest = Instantiate(allQuests[abilityIndex]) as RPGQuest;
        GUI.FocusControl(null);
    }

    private void SelectNewQuest(string fileName)
    {
        RPGQuest newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allQuests.Count; i++)
            if (allQuests[i]._fileName == fileName)
            {
                newAb = allQuests[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedQuest = Instantiate(allQuests[abIndex]) as RPGQuest;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allQuests[0]._fileName;
            currentlyViewedQuest = Instantiate(allQuests[0]) as RPGQuest;
        }
        GUI.FocusControl(null);
    }

    private void SelectCurrency(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allCurrencies[abilityIndex]._fileName;
        currentlyViewedCurrency = Instantiate(allCurrencies[abilityIndex]) as RPGCurrency;
        GUI.FocusControl(null);
    }

    private void SelectNewCurrency(string fileName)
    {
        RPGCurrency newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allCurrencies.Count; i++)
            if (allCurrencies[i]._fileName == fileName)
            {
                newAb = allCurrencies[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedCurrency = Instantiate(allCurrencies[abIndex]) as RPGCurrency;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allCurrencies[0]._fileName;
            currentlyViewedCurrency = Instantiate(allCurrencies[0]) as RPGCurrency;
        }
        GUI.FocusControl(null);
    }

    private void SelectMerchantTable(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allMerchantTables[abilityIndex]._fileName;
        currentlyViewedMerchantTable = Instantiate(allMerchantTables[abilityIndex]) as RPGMerchantTable;
        GUI.FocusControl(null);
    }

    private void SelectNewMerchantTable(string fileName)
    {
        RPGMerchantTable newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allMerchantTables.Count; i++)
            if (allMerchantTables[i]._fileName == fileName)
            {
                newAb = allMerchantTables[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedMerchantTable = Instantiate(allMerchantTables[abIndex]) as RPGMerchantTable;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allMerchantTables[0]._fileName;
            currentlyViewedMerchantTable = Instantiate(allMerchantTables[0]) as RPGMerchantTable;
        }
        GUI.FocusControl(null);
    }

    private void SelectLootTable(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allLootTables[abilityIndex]._fileName;
        currentlyViewedLootTable = Instantiate(allLootTables[abilityIndex]) as RPGLootTable;
        GUI.FocusControl(null);
    }

    private void SelectNewLootTable(string fileName)
    {
        RPGLootTable newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allLootTables.Count; i++)
            if (allLootTables[i]._fileName == fileName)
            {
                newAb = allLootTables[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedLootTable = Instantiate(allLootTables[abIndex]) as RPGLootTable;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allLootTables[0]._fileName;
            currentlyViewedLootTable = Instantiate(allLootTables[0]) as RPGLootTable;
        }
        GUI.FocusControl(null);
    }


    private void SelectTreePoint(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allTreePoints[abilityIndex]._fileName;
        currentlyViewedTreePoint = Instantiate(allTreePoints[abilityIndex]) as RPGTreePoint;
        GUI.FocusControl(null);
    }

    private void SelectNewTreePoint(string fileName)
    {
        RPGTreePoint newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allTreePoints.Count; i++)
            if (allTreePoints[i]._fileName == fileName)
            {
                newAb = allTreePoints[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedTreePoint = Instantiate(allTreePoints[abIndex]) as RPGTreePoint;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allTreePoints[0]._fileName;
            currentlyViewedTreePoint = Instantiate(allTreePoints[0]) as RPGTreePoint;
        }
        GUI.FocusControl(null);
    }

    private void SelectRace(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allRaces[abilityIndex]._fileName;
        currentlyViewedRace = Instantiate(allRaces[abilityIndex]) as RPGRace;
        GUI.FocusControl(null);
    }

    private void SelectNewRace(string fileName)
    {
        RPGRace newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allRaces.Count; i++)
            if (allRaces[i]._fileName == fileName)
            {
                newAb = allRaces[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedRace = Instantiate(allRaces[abIndex]) as RPGRace;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allRaces[0]._fileName;
            currentlyViewedRace = Instantiate(allRaces[0]) as RPGRace;
        }
        GUI.FocusControl(null);
    }

    private void SelectClass(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allClasses[abilityIndex]._fileName;
        currentlyViewedClass = Instantiate(allClasses[abilityIndex]) as RPGClass;
        GUI.FocusControl(null);
    }

    private void SelectNewClass(string fileName)
    {
        RPGClass newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allClasses.Count; i++)
            if (allClasses[i]._fileName == fileName)
            {
                newAb = allClasses[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedClass = Instantiate(allClasses[abIndex]) as RPGClass;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allClasses[0]._fileName;
            currentlyViewedClass = Instantiate(allClasses[0]) as RPGClass;
        }
        GUI.FocusControl(null);
    }


    private void SelectWorldPosition(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allWorldPositions[abilityIndex]._fileName;
        currentlyViewedWorldPosition = Instantiate(allWorldPositions[abilityIndex]) as RPGWorldPosition;
        GUI.FocusControl(null);
    }

    private void SelectNewWorldPosition(string fileName)
    {
        RPGWorldPosition newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allWorldPositions.Count; i++)
            if (allWorldPositions[i]._fileName == fileName)
            {
                newAb = allWorldPositions[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedWorldPosition = Instantiate(allWorldPositions[abIndex]) as RPGWorldPosition;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allWorldPositions[0]._fileName;
            currentlyViewedWorldPosition = Instantiate(allWorldPositions[0]) as RPGWorldPosition;
        }
        GUI.FocusControl(null);
    }

    private void SelectLevelTemplate(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allLevelsTemplate[abilityIndex]._fileName;
        currentlyViewedLevelTemplate = Instantiate(allLevelsTemplate[abilityIndex]) as RPGLevelsTemplate;
        GUI.FocusControl(null);
    }

    private void SelectNewLevelTemplate(string fileName)
    {
        RPGLevelsTemplate newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allLevelsTemplate.Count; i++)
            if (allLevelsTemplate[i]._fileName == fileName)
            {
                newAb = allLevelsTemplate[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedLevelTemplate = Instantiate(allLevelsTemplate[abIndex]) as RPGLevelsTemplate;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allAbilities[0]._fileName;
            currentlyViewedLevelTemplate = Instantiate(allLevelsTemplate[0]) as RPGLevelsTemplate;
        }
        GUI.FocusControl(null);
    }

    private void SelectSkill(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allSkills[abilityIndex]._fileName;
        currentlyViewedSkill = Instantiate(allSkills[abilityIndex]) as RPGSkill;
        GUI.FocusControl(null);
    }

    private void SelectNewSkill(string fileName)
    {
        RPGSkill newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allSkills.Count; i++)
            if (allSkills[i]._fileName == fileName)
            {
                newAb = allSkills[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedSkill = Instantiate(allSkills[abIndex]) as RPGSkill;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allAbilities[0]._fileName;
            currentlyViewedSkill = Instantiate(allSkills[0]) as RPGSkill;
        }
        GUI.FocusControl(null);
    }

    private void SelectEffect(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allEffects[abilityIndex]._fileName;
        currentlyViewedEffect = Instantiate(allEffects[abilityIndex]) as RPGEffect;
        GUI.FocusControl(null);
    }

    private void SelectNPC(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allNPCs[abilityIndex]._fileName;
        currentlyViewedNPC = Instantiate(allNPCs[abilityIndex]) as RPGNpc;
        GUI.FocusControl(null);
    }

    private void SelectNewEffect(string fileName)
    {
        RPGEffect newEf = null;
        var efIndex = -1;
        for (var i = 0; i < allEffects.Count; i++)
            if (allEffects[i]._fileName == fileName)
            {
                newEf = allEffects[i];
                efIndex = i;
            }

        if (newEf != null)
        {
            curViewElementIndex = efIndex;
            cachedFileName = newEf._fileName;
            currentlyViewedEffect = Instantiate(allEffects[efIndex]) as RPGEffect;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allEffects[0]._fileName;
            currentlyViewedEffect = Instantiate(allEffects[0]) as RPGEffect;
        }
        GUI.FocusControl(null);
    }

    private void SelectNewNPC(string fileName)
    {
        RPGNpc newEf = null;
        var efIndex = -1;
        for (var i = 0; i < allNPCs.Count; i++)
            if (allNPCs[i]._fileName == fileName)
            {
                newEf = allNPCs[i];
                efIndex = i;
            }

        if (newEf != null)
        {
            curViewElementIndex = efIndex;
            cachedFileName = newEf._fileName;
            currentlyViewedNPC = Instantiate(allNPCs[efIndex]) as RPGNpc;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allNPCs[0]._fileName;
            currentlyViewedNPC = Instantiate(allNPCs[0]) as RPGNpc;
        }
        GUI.FocusControl(null);
    }


    private void SelectStat(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allStats[abilityIndex]._fileName;
        currentlyViewedStat = Instantiate(allStats[abilityIndex]) as RPGStat;
        GUI.FocusControl(null);
    }

    private void SelectNewStat(string fileName)
    {
        RPGStat newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allStats.Count; i++)
            if (allStats[i]._fileName == fileName)
            {
                newAb = allStats[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedStat = Instantiate(allStats[abIndex]) as RPGStat;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allStats[0]._fileName;
            currentlyViewedStat = Instantiate(allStats[0]) as RPGStat;
        }
        GUI.FocusControl(null);
    }


    private void SelectItem(int abilityIndex)
    {
        curViewElementIndex = abilityIndex;
        cachedFileName = allItems[abilityIndex]._fileName;
        currentlyViewedItem = Instantiate(allItems[abilityIndex]) as RPGItem;
        GUI.FocusControl(null);
    }

    private void SelectNewItem(string fileName)
    {
        RPGItem newAb = null;
        var abIndex = -1;
        for (var i = 0; i < allItems.Count; i++)
            if (allItems[i]._fileName == fileName)
            {
                newAb = allItems[i];
                abIndex = i;
            }

        if (newAb != null)
        {
            curViewElementIndex = abIndex;
            cachedFileName = newAb._fileName;
            currentlyViewedItem = Instantiate(allItems[abIndex]) as RPGItem;
        }
        else
        {
            curViewElementIndex = 0;
            cachedFileName = allItems[0]._fileName;
            currentlyViewedItem = Instantiate(allItems[0]) as RPGItem;
        }
        GUI.FocusControl(null);
    }

    private void DrawEffectView()
    {
        if (currentlyViewedEffect == null)
        {
            if (allEffects.Count == 0)
            {
                CreateNew(AssetType.Effect);
                return;
            }
            currentlyViewedEffect = Instantiate(allEffects[0]) as RPGEffect;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        //GUILayout.Label("View: Effects", skin.GetStyle("Header2"));

        

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "combat");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allEffects.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allEffects[i]._name;
                            newElementDATA.showIcon = true;
                            if (allEffects[i].icon != null) newElementDATA.texture = allEffects[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Effect);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedEffect.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedEffect.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedEffect.ID = EditorGUILayout.IntField(currentlyViewedEffect.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(nameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedEffect._name = GUILayout.TextField(currentlyViewedEffect._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedEffect.displayName = GUILayout.TextField(currentlyViewedEffect.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fileNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedEffect._fileName = GUILayout.TextField("RPG_EFFECT_" + currentlyViewedEffect._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                       
                        if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.damageOverTime || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.healOverTime
                            || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.immune || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.morph
                            || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.sleep || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.stat
                            || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.stun || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.taunt
                             || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.root || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.silence
                              || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.reflect)
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("CONDITIONS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);
                            currentlyViewedEffect.isState = true;
                            currentlyViewedEffect.isState = EditorGUILayout.Toggle(new GUIContent("Is State?", "Is this effect a state that stays on the target for a duration?"), currentlyViewedEffect.isState);
                            currentlyViewedEffect.isBuffOnSelf = EditorGUILayout.Toggle(new GUIContent("Buff?", "Is this effect a buff on self?"), currentlyViewedEffect.isBuffOnSelf);
                            currentlyViewedEffect.duration = EditorGUILayout.FloatField(new GUIContent("Duration", "How long does the effect last?"), currentlyViewedEffect.duration);
                            //currentlyViewedEffect.delay = EditorGUILayout.FloatField(new GUIContent("Hit Delay", "Should there be a delay before the effect actually hits?"), currentlyViewedEffect.delay);
                        }
                        else
                        {
                            currentlyViewedEffect.isState = false;
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("TYPE", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedEffect.effectType = (RPGEffect.EFFECT_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Type", "What type of effect is it?"), currentlyViewedEffect.effectType, GUILayout.Width(300));
                        if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.instantDamage || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.damageOverTime)
                        {
                            currentlyViewedEffect.mainDamageType = (RPGEffect.MAIN_DAMAGE_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Main Damage Type", "What is the main damage type of this effect?"), currentlyViewedEffect.mainDamageType);

                            var curStatList = getCorrectStatsList(RPGStat.STAT_TYPE.DAMAGE);
                            var currentSecondaryDamageIndex = GetIndexFromSecondaryDamageName(curStatList, currentlyViewedEffect.secondaryDamageType);
                            var tempIndex = 0;
                            tempIndex = EditorGUILayout.Popup(new GUIContent("Damage Type", "What type of damage is it?"), currentSecondaryDamageIndex, curStatList);
                            currentlyViewedEffect.secondaryDamageType = GetSecondaryDamageNameFromID(curStatList, tempIndex);

                            //currentlyViewedEffect.secondaryDamageType = EditorGUILayout.Popup("Main Damage Type", currentlyViewedEffect.mainDamageType);
                            currentlyViewedEffect.Damage = EditorGUILayout.IntField(new GUIContent("Damage", "What is the base damage value of this effect?"), currentlyViewedEffect.Damage);
                            currentlyViewedEffect.skillModifier = EditorGUILayout.FloatField(new GUIContent("Skill Modifier", "How much damage is added by skill level"), currentlyViewedEffect.skillModifier);

                            if (currentlyViewedEffect.skillModifier > 0)
                            {
                                currentlyViewedEffect.skillModifierREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill that will increase damage"), RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedEffect.skillModifierID, allSkills), typeof(RPGSkill), false);
                                if(currentlyViewedEffect.skillModifierREF != null)
                                    currentlyViewedEffect.skillModifierID = currentlyViewedEffect.skillModifierREF.ID;
                                else
                                    currentlyViewedEffect.skillModifierID = -1;
                            }
                            
                                currentlyViewedEffect.lifesteal = EditorGUILayout.FloatField(new GUIContent("Lifesteal", "How much of the damage dealt should be converted to self heals? 0.1 = 10%"), currentlyViewedEffect.lifesteal);
                                currentlyViewedEffect.maxHealthModifier = EditorGUILayout.FloatField(new GUIContent("Max Health Mod", "How much of the caster's maximum health should be converted to extra damage? 0.1 = 10%"), currentlyViewedEffect.maxHealthModifier);
                                currentlyViewedEffect.missingHealthModifier = EditorGUILayout.FloatField(new GUIContent("Missing Health Mod", "How much extra damage should be dealt for each percent of missing health? 0.1 = 10%"), currentlyViewedEffect.missingHealthModifier);
                            
                        }
                        if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.healOverTime || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.instantHeal)
                        {

                            var curStatList = getCorrectStatsList(RPGStat.STAT_TYPE.HEALING);
                            var currentSecondaryDamageIndex = GetIndexFromSecondaryDamageName(curStatList, currentlyViewedEffect.secondaryDamageType);
                            var tempIndex = 0;
                            tempIndex = EditorGUILayout.Popup(new GUIContent("Heal Type", "What type of heal is it?"), currentSecondaryDamageIndex, curStatList);
                            currentlyViewedEffect.secondaryDamageType = GetSecondaryDamageNameFromID(curStatList, tempIndex);
                            
                            currentlyViewedEffect.Damage = EditorGUILayout.IntField(new GUIContent("Healing", "What is the base healing value?"), currentlyViewedEffect.Damage);
                            currentlyViewedEffect.skillModifier = EditorGUILayout.FloatField(new GUIContent("Skill Modifier", "How much damage is added by skill level"), currentlyViewedEffect.skillModifier);

                            if (currentlyViewedEffect.skillModifier > 0)
                            {
                                currentlyViewedEffect.skillModifierREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill that will increase damage"), RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedEffect.skillModifierID, allSkills), typeof(RPGSkill), false);
                                if(currentlyViewedEffect.skillModifierREF != null)
                                    currentlyViewedEffect.skillModifierID = currentlyViewedEffect.skillModifierREF.ID;
                                else
                                    currentlyViewedEffect.skillModifierID = -1;
                            }
                        }
                        else if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.stat)
                        {
                            ScriptableObject scriptableObj = currentlyViewedEffect;
                            var serialObj = new SerializedObject(scriptableObj);

                            if (GUILayout.Button("+ Add Stat", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedEffect.statEffectsData.Add(new RPGEffect.STAT_EFFECTS_DATA());

                            var ThisList2 = serialObj.FindProperty("statEffectsData");
                            currentlyViewedEffect.statEffectsData = GetTargetObjectOfProperty(ThisList2) as List<RPGEffect.STAT_EFFECTS_DATA>;

                            for (var a = 0; a < currentlyViewedEffect.statEffectsData.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedEffect.statEffectsData.RemoveAt(a);
                                    return;
                                }
                                var statName = "";
                                if (currentlyViewedEffect.statEffectsData[a].statREF != null) statName = currentlyViewedEffect.statEffectsData[a].statREF.displayName;
                                EditorGUILayout.LabelField("" + requirementNumber + ": " + statName, GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Stat", GUILayout.Width(100));
                                currentlyViewedEffect.statEffectsData[a].statREF = (RPGStat)EditorGUILayout.ObjectField(currentlyViewedEffect.statEffectsData[a].statREF, typeof(RPGStat), false, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Value", GUILayout.Width(100));
                                currentlyViewedEffect.statEffectsData[a].statEffectModification = EditorGUILayout.FloatField(currentlyViewedEffect.statEffectsData[a].statEffectModification);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Is Percent?", GUILayout.Width(100));
                                currentlyViewedEffect.statEffectsData[a].isPercent = EditorGUILayout.Toggle(currentlyViewedEffect.statEffectsData[a].isPercent);
                                EditorGUILayout.EndHorizontal();

                                if (currentlyViewedEffect.statEffectsData[a].statREF != null)
                                    currentlyViewedEffect.statEffectsData[a].statID = currentlyViewedEffect.statEffectsData[a].statREF.ID;
                                else
                                    currentlyViewedEffect.statEffectsData[a].statID = -1;
                                GUILayout.Space(10);
                            }
                            
                            serialObj.ApplyModifiedProperties();
                        } else if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.teleport)
                        {
                            currentlyViewedEffect.teleportType = (RPGEffect.TELEPORT_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Type", "What type of teleport is it?"), currentlyViewedEffect.teleportType);
                            if (currentlyViewedEffect.teleportType == RPGEffect.TELEPORT_TYPE.gameScene)
                            {
                                currentlyViewedEffect.gameSceneREF = (RPGGameScene)EditorGUILayout.ObjectField(new GUIContent("Game Scene", "The game scene to teleport to"), RPGBuilderUtilities.GetGameSceneFromIDEditor(currentlyViewedEffect.gameSceneID, allGameScenes), typeof(RPGGameScene), false);
                                if(currentlyViewedEffect.gameSceneREF != null)
                                    currentlyViewedEffect.gameSceneID = currentlyViewedEffect.gameSceneREF.ID;
                                else
                                    currentlyViewedEffect.gameSceneID = -1;
                                currentlyViewedEffect.teleportPOS = EditorGUILayout.Vector3Field(new GUIContent("Location", "Position coordinates to teleport to"), currentlyViewedEffect.teleportPOS);
                            }
                            else if (currentlyViewedEffect.teleportType == RPGEffect.TELEPORT_TYPE.position)
                            {
                                currentlyViewedEffect.teleportPOS = EditorGUILayout.Vector3Field(new GUIContent("Location", "Position coordinates to teleport to"), currentlyViewedEffect.teleportPOS);
                            }
                        }
                        else if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.pet)
                        {
                            currentlyViewedEffect.petType = (RPGEffect.PET_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Type", "What type of pet is it?"), currentlyViewedEffect.petType);
                            currentlyViewedEffect.petPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab", "The NPC Prefab of the pet"), currentlyViewedEffect.petPrefab, typeof(GameObject), false);
                            currentlyViewedEffect.petSPawnCount = EditorGUILayout.IntField(new GUIContent("Spawn Count", "How many pets should this effect spawn at once?"), currentlyViewedEffect.petSPawnCount);
                            currentlyViewedEffect.petDuration = EditorGUILayout.FloatField(new GUIContent("Duration", "How long should the pet(s) stay active?"), currentlyViewedEffect.petDuration);
                            currentlyViewedEffect.petScaleWithCharacter = EditorGUILayout.Toggle(new GUIContent("Character Scaling?", "Does this pet scale with the character level?"), currentlyViewedEffect.petScaleWithCharacter);
                        }
                        else if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.reflect)
                        {
                            //currentlyViewedEffect.projectilesReflectedCount = EditorGUILayout.IntField(new GUIContent("Reflected Count", "How many projectiles max should this effect reflect?"), currentlyViewedEffect.projectilesReflectedCount);
                        }
                        if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.sleep
                            || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.morph || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.reflect
                            || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.root || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.silence
                            || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.taunt || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.dispel)
                            EditorGUILayout.LabelField("NOT YET IMPLEMENTED");


                        if (currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.damageOverTime || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.healOverTime
                                                                                                     || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.immune || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.morph
                                                                                                     || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.sleep || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.stat
                                                                                                     || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.stun || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.taunt
                                                                                                     || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.root || currentlyViewedEffect.effectType == RPGEffect.EFFECT_TYPE.silence)
                        {

                            GUILayout.Space(10);
                            GUILayout.Label("STACKING", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);
                            currentlyViewedEffect.allowMultiple = EditorGUILayout.Toggle(new GUIContent("Multiple?", "Can this effect be active multiple times on the same target?"), currentlyViewedEffect.allowMultiple);
                            currentlyViewedEffect.allowMixedCaster = EditorGUILayout.Toggle(new GUIContent("Mixed Caster?", "Can effects stack together even if they are not from the same caster?"), currentlyViewedEffect.allowMixedCaster);
                            currentlyViewedEffect.stackLimit = EditorGUILayout.IntField(new GUIContent("Max Stacks", "How many times maximum can it stack?"), currentlyViewedEffect.stackLimit);
                            currentlyViewedEffect.pulses = EditorGUILayout.IntField(new GUIContent("Pulses Count", "How many times will this effect pulse? Each pulse will trigger the effect again"), currentlyViewedEffect.pulses);
                        }
                        else
                        {
                            currentlyViewedEffect.isState = false;
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("VISUALS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedEffect.effectPulseGO = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Pulse Visual", "The Visual Effect triggered for each pulse of this effect"), currentlyViewedEffect.effectPulseGO, typeof(GameObject), false);

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();

                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Effect, containerRect2);
    }

    private void DrawNPCsView()
    {
        if (currentlyViewedNPC == null)
        {
            if (allNPCs.Count == 0)
            {
                CreateNew(AssetType.NPC);
                return;
            }
            currentlyViewedNPC = Instantiate(allNPCs[0]) as RPGNpc;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "combat");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allNPCs.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allNPCs[i]._name;
                            newElementDATA.showIcon = true;
                            if (allNPCs[i].icon != null) newElementDATA.texture = allNPCs[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.NPC);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)combatSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));
                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedNPC.ID = EditorGUILayout.IntField(currentlyViewedNPC.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(nameGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedNPC._name = GUILayout.TextField(currentlyViewedNPC._name, GUILayout.Width(278), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedNPC.displayName = GUILayout.TextField(currentlyViewedNPC.displayName, GUILayout.Width(278), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(fileNameGUIContent, GUILayout.Width(147), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedNPC._fileName = GUILayout.TextField("RPG_NPC_" + currentlyViewedNPC._name, GUILayout.Width(278), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        currentlyViewedNPC.NPCPrefab = (GameObject)EditorGUILayout.ObjectField("NPC Prefab", currentlyViewedNPC.NPCPrefab, typeof(GameObject), false, GUILayout.Width(430));
                        currentlyViewedNPC.isDummyTarget = EditorGUILayout.Toggle(new GUIContent("Dummy?","If this is on, the NPC will never die. Yep, quite OP."), currentlyViewedNPC.isDummyTarget, GUILayout.Width(430));
                        currentlyViewedNPC.isCombatEnabled = EditorGUILayout.Toggle(new GUIContent("Combat?","If this is on, the NPC will be able to fight and be attacked."), currentlyViewedNPC.isCombatEnabled, GUILayout.Width(430));
                        currentlyViewedNPC.isMovementEnabled = EditorGUILayout.Toggle(new GUIContent("Movement?","If this is on, the NPC will be able to perform movement actions."), currentlyViewedNPC.isMovementEnabled, GUILayout.Width(430));

                        GUILayout.Space(10);
                        GUILayout.Label("COMBAT", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedNPC.MinLevel = EditorGUILayout.IntField(new GUIContent("Level. Min", "The minimum level of the NPC"), currentlyViewedNPC.MinLevel, GUILayout.Width(430));
                        currentlyViewedNPC.MaxLevel = EditorGUILayout.IntField(new GUIContent("Level. Max", "The maximum level of the NPC"), currentlyViewedNPC.MaxLevel, GUILayout.Width(430));
                        if (currentlyViewedNPC.MaxLevel < currentlyViewedNPC.MinLevel) currentlyViewedNPC.MaxLevel = currentlyViewedNPC.MinLevel;
                        if (currentlyViewedNPC.MinEXP > currentlyViewedNPC.MaxEXP) currentlyViewedNPC.MaxLevel = currentlyViewedNPC.MinLevel;

                        currentlyViewedNPC.npcType = (RPGNpc.NPC_TYPE)EditorGUILayout.EnumPopup(new GUIContent("NPC Type", "The type of the NPC, will define interactions and actions"), currentlyViewedNPC.npcType, GUILayout.Width(430));
                        currentlyViewedNPC.alignmentType = (RPGNpc.ALIGNMENT_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Alignment", "The default alignment for this NPC to the player"), currentlyViewedNPC.alignmentType, GUILayout.Width(430));
                        
                        currentlyViewedNPC.AggroRange = EditorGUILayout.FloatField(new GUIContent("Aggro Range", "The aggro range of this NPC to its enemies"), currentlyViewedNPC.AggroRange, GUILayout.Width(430));
                        currentlyViewedNPC.distanceFromTarget = EditorGUILayout.FloatField(new GUIContent("Distance from target", "How far away should this NPC try to stay from his target in combat?"), currentlyViewedNPC.distanceFromTarget, GUILayout.Width(430));
                        currentlyViewedNPC.distanceFromOwner = EditorGUILayout.FloatField(new GUIContent("Distance from owner", "How far away should this NPC try to stay from his owner when following him?"), currentlyViewedNPC.distanceFromOwner, GUILayout.Width(430));
                        currentlyViewedNPC.DistanceToTargetReset = EditorGUILayout.FloatField(new GUIContent("Reset target distance", "How far away should the target be for the NPC to leave combat"), currentlyViewedNPC.DistanceToTargetReset, GUILayout.Width(430));

                        ScriptableObject scriptableObj = currentlyViewedNPC;
                        var serialObj = new SerializedObject(scriptableObj);

                        if (GUILayout.Button("+ Add Ability", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedNPC.abilities.Add(new RPGNpc.NPC_ABILITY_DATA());

                        var ThisList7 = serialObj.FindProperty("abilities");
                        currentlyViewedNPC.abilities = GetTargetObjectOfProperty(ThisList7) as List<RPGNpc.NPC_ABILITY_DATA>;

                        for (var a = 0; a < currentlyViewedNPC.abilities.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedNPC.abilities.RemoveAt(a);
                                return;
                            }
                            var effectName = "";
                            if (currentlyViewedNPC.abilities[a].abilityREF != null) effectName = currentlyViewedNPC.abilities[a].abilityREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Ability", GUILayout.Width(100));
                            currentlyViewedNPC.abilities[a].abilityREF = (RPGAbility)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedNPC.abilities[a].abilityID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedNPC.abilities[a].abilityREF != null)
                                currentlyViewedNPC.abilities[a].abilityID = currentlyViewedNPC.abilities[a].abilityREF.ID;
                            else
                                currentlyViewedNPC.abilities[a].abilityID = -1;
                            GUILayout.Space(10);
                        }


                        if (currentlyViewedNPC.npcType == RPGNpc.NPC_TYPE.MERCHANT)
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("MERCHANT", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);
                            currentlyViewedNPC.merchantTableREF = (RPGMerchantTable)EditorGUILayout.ObjectField(new GUIContent("Merchant Table", "The merchant table that this NPC will have"), RPGBuilderUtilities.GetMerchantTableFromIDEditor(currentlyViewedNPC.merchantTableID, allMerchantTables), typeof(RPGMerchantTable), false, GUILayout.Width(430));
                            if (currentlyViewedNPC.merchantTableREF != null)
                                currentlyViewedNPC.merchantTableID = currentlyViewedNPC.merchantTableREF.ID;
                            else
                                currentlyViewedNPC.merchantTableID = -1;
                        } else if (currentlyViewedNPC.npcType == RPGNpc.NPC_TYPE.QUEST_GIVER)
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("QUESTS: GIVEN", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);

                            if (GUILayout.Button("+ Add Quest", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedNPC.questGiven.Add(new RPGNpc.NPC_QUEST_DATA());

                            var ThisList2 = serialObj.FindProperty("questGiven");
                            currentlyViewedNPC.questGiven = GetTargetObjectOfProperty(ThisList2) as List<RPGNpc.NPC_QUEST_DATA>;

                            for (var a = 0; a < currentlyViewedNPC.questGiven.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedNPC.questGiven.RemoveAt(a);
                                    return;
                                }
                                var effectName = "";
                                if (currentlyViewedNPC.questGiven[a].questREF != null) effectName = currentlyViewedNPC.questGiven[a].questREF._name;
                                EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Quest", GUILayout.Width(100));
                                currentlyViewedNPC.questGiven[a].questREF = (RPGQuest)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetQuestFromIDEditor(currentlyViewedNPC.questGiven[a].questID, allQuests), typeof(RPGQuest), false, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                if (currentlyViewedNPC.questGiven[a].questREF != null)
                                    currentlyViewedNPC.questGiven[a].questID = currentlyViewedNPC.questGiven[a].questREF.ID;
                                else
                                    currentlyViewedNPC.questGiven[a].questID = -1;
                                GUILayout.Space(10);
                            }


                            GUILayout.Space(10);
                            GUILayout.Label("QUESTS: COMPLETING", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);

                            if (GUILayout.Button("+ Add Quest", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedNPC.questCompleted.Add(new RPGNpc.NPC_QUEST_DATA());

                            var ThisList3 = serialObj.FindProperty("questCompleted");
                            currentlyViewedNPC.questCompleted = GetTargetObjectOfProperty(ThisList3) as List<RPGNpc.NPC_QUEST_DATA>;

                            for (var a = 0; a < currentlyViewedNPC.questCompleted.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedNPC.questCompleted.RemoveAt(a);
                                    return;
                                }
                                var effectName = "";
                                if (currentlyViewedNPC.questCompleted[a].questREF != null) effectName = currentlyViewedNPC.questCompleted[a].questREF._name;
                                EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Quest", GUILayout.Width(100));
                                currentlyViewedNPC.questCompleted[a].questREF = (RPGQuest)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetQuestFromIDEditor(currentlyViewedNPC.questCompleted[a].questID, allQuests), typeof(RPGQuest), false, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                if (currentlyViewedNPC.questCompleted[a].questREF != null)
                                    currentlyViewedNPC.questCompleted[a].questID = currentlyViewedNPC.questCompleted[a].questREF.ID;
                                else
                                    currentlyViewedNPC.questCompleted[a].questID = -1;
                                GUILayout.Space(10);
                            }

                        }

                        GUILayout.Space(10);
                        GUILayout.Label("RESPAWN", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedNPC.MinRespawn = EditorGUILayout.FloatField(new GUIContent("Respawn. Min", "How long should the minimum respawn time be?"), currentlyViewedNPC.MinRespawn, GUILayout.Width(430));
                        currentlyViewedNPC.MaxRespawn = EditorGUILayout.FloatField(new GUIContent("Respawn. Max", "How long should the maximum respawn time be?"), currentlyViewedNPC.MaxRespawn, GUILayout.Width(430));
                        if(currentlyViewedNPC.MaxRespawn < currentlyViewedNPC.MinRespawn) currentlyViewedNPC.MaxRespawn = currentlyViewedNPC.MinRespawn;
                        if (currentlyViewedNPC.MinRespawn > currentlyViewedNPC.MaxRespawn) currentlyViewedNPC.MaxRespawn = currentlyViewedNPC.MinRespawn;

                        GUILayout.Space(10);
                        GUILayout.Label("REWARDS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedNPC.MinEXP = EditorGUILayout.IntField(new GUIContent("EXP. Min", "What is the minimum amount of Experience that this NPC will reward?"), currentlyViewedNPC.MinEXP, GUILayout.Width(430));
                        currentlyViewedNPC.MaxEXP = EditorGUILayout.IntField(new GUIContent("EXP. Max", "What is the maximum amount of Experience that this NPC will reward?"), currentlyViewedNPC.MaxEXP, GUILayout.Width(430));
                        currentlyViewedNPC.EXPBonusPerLevel = EditorGUILayout.IntField(new GUIContent("Level Bonus EXP", "For each level of the NPC, what extra amount of Experience will be given?"), currentlyViewedNPC.EXPBonusPerLevel, GUILayout.Width(430));
                        if (currentlyViewedNPC.MaxEXP < currentlyViewedNPC.MinEXP) currentlyViewedNPC.MaxEXP = currentlyViewedNPC.MinEXP;
                        if (currentlyViewedNPC.MinEXP > currentlyViewedNPC.MaxEXP) currentlyViewedNPC.MaxEXP = currentlyViewedNPC.MinEXP;

                        GUILayout.Space(10);
                        GUILayout.Label("LOOT TABLES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Loot Table", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedNPC.lootTables.Add(new RPGNpc.LOOT_TABLES());

                        var ThisList4 = serialObj.FindProperty("lootTables");
                        currentlyViewedNPC.lootTables = GetTargetObjectOfProperty(ThisList4) as List<RPGNpc.LOOT_TABLES>;

                        for (var a = 0; a < currentlyViewedNPC.lootTables.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedNPC.lootTables.RemoveAt(a);
                                return;
                            }
                            var effectName = "";
                            if (currentlyViewedNPC.lootTables[a].lootTableREF != null) effectName = currentlyViewedNPC.lootTables[a].lootTableREF._name;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Loot Table", GUILayout.Width(100));
                            currentlyViewedNPC.lootTables[a].lootTableREF = (RPGLootTable)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetlootTableFromIDEditor(currentlyViewedNPC.lootTables[a].lootTableID, allLootTables), typeof(RPGLootTable), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedNPC.lootTables[a].lootTableREF != null)
                                currentlyViewedNPC.lootTables[a].lootTableID = currentlyViewedNPC.lootTables[a].lootTableREF.ID;
                            else
                                currentlyViewedNPC.lootTables[a].lootTableID = -1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Chance", GUILayout.Width(100));
                            currentlyViewedNPC.lootTables[a].dropRate = EditorGUILayout.Slider(currentlyViewedNPC.lootTables[a].dropRate, 0f, 100f, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("STATS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Stat", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedNPC.stats.Add(new RPGNpc.NPC_STATS_DATA());

                        var ThisList5 = serialObj.FindProperty("stats");
                        currentlyViewedNPC.stats = GetTargetObjectOfProperty(ThisList5) as List<RPGNpc.NPC_STATS_DATA>;

                        for (var a = 0; a < currentlyViewedNPC.stats.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedNPC.stats.RemoveAt(a);
                                return;
                            }
                            var talentTreeName = "";
                            if (currentlyViewedNPC.stats[a].statREF != null) talentTreeName = currentlyViewedNPC.stats[a].statREF.displayName;
                            EditorGUILayout.LabelField("" + requirementNumber + ": " + talentTreeName, GUILayout.Width(400));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("STAT", GUILayout.Width(100));
                            currentlyViewedNPC.stats[a].statREF = (RPGStat)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetStatFromIDEditor(currentlyViewedNPC.stats[a].statID, allStats), typeof(RPGStat), false, GUILayout.Width(250));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Value", GUILayout.Width(100));
                            currentlyViewedNPC.stats[a].baseValue = EditorGUILayout.FloatField(currentlyViewedNPC.stats[a].baseValue);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Min. Value", GUILayout.Width(100));
                            currentlyViewedNPC.stats[a].minValue = EditorGUILayout.FloatField(currentlyViewedNPC.stats[a].minValue);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Max. Value", GUILayout.Width(100));
                            currentlyViewedNPC.stats[a].maxValue = EditorGUILayout.FloatField(currentlyViewedNPC.stats[a].maxValue);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Level Gain", GUILayout.Width(100));
                            currentlyViewedNPC.stats[a].bonusPerLevel = EditorGUILayout.FloatField(currentlyViewedNPC.stats[a].bonusPerLevel);
                            EditorGUILayout.EndHorizontal();
                            if (currentlyViewedNPC.stats[a].statREF != null)
                                currentlyViewedNPC.stats[a].statID = currentlyViewedNPC.stats[a].statREF.ID;
                            else
                                currentlyViewedNPC.stats[a].statID = -1;
                            GUILayout.Space(10);
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("MOVEMENT", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        currentlyViewedNPC.RoamRange = EditorGUILayout.FloatField(new GUIContent("Roam Range", "How far away can the NPC roam from its previous point?"), currentlyViewedNPC.RoamRange, GUILayout.Width(430));
                        currentlyViewedNPC.RoamDelay = EditorGUILayout.FloatField(new GUIContent("Roam Delay", "How long should the NPC stop between each different roaming path"), currentlyViewedNPC.RoamDelay, GUILayout.Width(430));
                        
                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.NPC, containerRect2);
    }


    private void DrawGeneralView()
    {
        switch (generalSubCurrentCategorySelected)
        {

            case GeneralSubCategorySelectedType.Item:
                DrawItemView();
                break;

            case GeneralSubCategorySelectedType.Skill:
                DrawSkillView();
                break;

            case GeneralSubCategorySelectedType.LevelTemplate:
                DrawLevelTemplateView();
                break;

            case GeneralSubCategorySelectedType.Race:
                DrawRaceView();
                break;

            case GeneralSubCategorySelectedType.Class:
                DrawClassView();
                break;

            case GeneralSubCategorySelectedType.LootTable:
                DrawLootTableView();
                break;

            case GeneralSubCategorySelectedType.MerchantTable:
                DrawMerchantTableView();
                break;

            case GeneralSubCategorySelectedType.Currency:
                DrawCurrencyView();
                break;

            case GeneralSubCategorySelectedType.CraftingRecipe:
                DrawCraftingRecipeView();
                break;

            case GeneralSubCategorySelectedType.CraftingStation:
                DrawCraftingStationView();
                break;

            case GeneralSubCategorySelectedType.TalentTree:
                DrawTalentTreeView();
                break;

            case GeneralSubCategorySelectedType.Bonus:
                DrawBonusView();
                break;
        }
    }

    private void DrawWorldView()
    {
        switch (worldSubCurrentCategorySelected)
        {
            case WorldSubCategorySelectedType.Task:
                DrawTaskView();
                break;

            case WorldSubCategorySelectedType.Quest:
                DrawQuestView();
                break;

            case WorldSubCategorySelectedType.WorldPosition:
                DrawWorldPositionView();
                break;

            case WorldSubCategorySelectedType.ResourceNode:
                DrawResourceNodeView();
                break;

            case WorldSubCategorySelectedType.GameScene:
                DrawGameSceneView();
                break;
        }
    }


    private void DrawItemView()
    {
        if (currentlyViewedItem == null)
        {
            if (allItems.Count == 0)
            {
                CreateNew(AssetType.Item);
                return;
            }
            currentlyViewedItem = Instantiate(allItems[0]) as RPGItem;
        }
        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));



        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "general");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].panelType)
                {
                    case RPGBuilderEditorDATA.PanelType.search:
                        curElementList.Clear();
                        for (var i = 0; i < allItems.Count; i++)
                        {
                            var newElementDATA = new elementListDATA();
                            newElementDATA.name = allItems[i]._name;
                            newElementDATA.showIcon = true;
                            if (allItems[i].icon != null) newElementDATA.texture = allItems[i].icon.texture;
                            curElementList.Add(newElementDATA);
                        }
                        DrawElementList(curElementList, AssetType.Item);
                        break;

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.ViewLeftMargin, editorDATA.ViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)generalSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.ViewX), GUILayout.Height(editorDATA.ViewY));

                        GUILayout.Space(10);
                        GUILayout.Label("BASE INFO", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        currentlyViewedItem.icon = (Sprite)EditorGUILayout.ObjectField(currentlyViewedItem.icon, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.BeginVertical();
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ID:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedItem.ID = EditorGUILayout.IntField(currentlyViewedItem.ID, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedItem._name = GUILayout.TextField(currentlyViewedItem._name, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(displayNameGUIContent, GUILayout.Width(100), GUILayout.Height(15));
                        currentlyViewedItem.displayName = GUILayout.TextField(currentlyViewedItem.displayName, GUILayout.Width(200), GUILayout.Height(15));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("File Name:", GUILayout.Width(100), GUILayout.Height(15));
                        EditorGUI.BeginDisabledGroup(true);
                        currentlyViewedItem._fileName = GUILayout.TextField("RPG_ITEM_" + currentlyViewedItem._name, GUILayout.Width(200), GUILayout.Height(15));
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                        GUILayout.Label("TYPES", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        ScriptableObject scriptableObj = currentlyViewedItem;
                        var serialObj = new SerializedObject(scriptableObj);
                        
                        var currentItemRarityIndex = getIndexFromName("ItemQuality", currentlyViewedItem.quality);
                        var tempIndex5 = 0;
                        tempIndex5 = EditorGUILayout.Popup("Quality", currentItemRarityIndex, itemSettings.itemQuality);
                        if (itemSettings.itemQuality.Length > 0) currentlyViewedItem.quality = itemSettings.itemQuality[tempIndex5];

                        var currentItemTypeIndex = getIndexFromName("ItemType", currentlyViewedItem.itemType);
                        var tempIndex = 0;
                        tempIndex = EditorGUILayout.Popup("Item Type", currentItemTypeIndex, itemSettings.itemType);
                        if (itemSettings.itemType.Length > 0) currentlyViewedItem.itemType = itemSettings.itemType[tempIndex];

                        if (currentlyViewedItem.itemType == "WEAPON")
                        {
                            var currentWeaponTypeIndex = getIndexFromName("WeaponType", currentlyViewedItem.weaponType);
                            var tempIndex2 = 0;
                            tempIndex2 = EditorGUILayout.Popup("Weapon Type", currentWeaponTypeIndex, itemSettings.weaponType);
                            if (itemSettings.weaponType.Length > 0) currentlyViewedItem.weaponType = itemSettings.weaponType[tempIndex2];


                            var currentSlotTypeIndex = getIndexFromName("SlotType", currentlyViewedItem.slotType);
                            var tempIndex3 = 0;
                            tempIndex3 = EditorGUILayout.Popup("Slot Type", currentSlotTypeIndex, itemSettings.slotType);
                            if (itemSettings.slotType.Length > 0) currentlyViewedItem.slotType = itemSettings.slotType[tempIndex3];
                        }
                        else if (currentlyViewedItem.itemType == "ARMOR")
                        {
                            var currentWeaponTypeIndex = getIndexFromName("ArmorType", currentlyViewedItem.armorType);
                            var tempIndex2 = 0;
                            tempIndex2 = EditorGUILayout.Popup("Armor Type", currentWeaponTypeIndex, itemSettings.armorType);
                            if (itemSettings.armorType.Length > 0) currentlyViewedItem.armorType = itemSettings.armorType[tempIndex2];

                            var currentEquipmentSlotIndex = getIndexFromName("ArmorSlots", currentlyViewedItem.equipmentSlot);
                            var tempIndex4 = 0;
                            tempIndex4 = EditorGUILayout.Popup("Armor Slot", currentEquipmentSlotIndex, itemSettings.armorSlots);
                            if (itemSettings.armorSlots.Length > 0) currentlyViewedItem.equipmentSlot = itemSettings.armorSlots[tempIndex4];
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("LOOT SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                            
                        currentlyViewedItem.dropInWorld = EditorGUILayout.Toggle("Drop in World?", currentlyViewedItem.dropInWorld, GUILayout.Width(250));
                        if (currentlyViewedItem.dropInWorld)
                        {
                            currentlyViewedItem.itemWorldModel = (GameObject)EditorGUILayout.ObjectField("In World Model", currentlyViewedItem.itemWorldModel, typeof(GameObject), false);
                            currentlyViewedItem.durationInWorld = EditorGUILayout.FloatField("Duration", currentlyViewedItem.durationInWorld, GUILayout.Width(430));
                        }
                        
                        if (currentlyViewedItem.itemType == "ARMOR" || currentlyViewedItem.itemType == "WEAPON")
                        {

                            GUILayout.Space(10);
                            GUILayout.Label("VISUALS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);
                            
                            if (currentlyViewedItem.itemType == "ARMOR")
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Model Name:", GUILayout.Width(147), GUILayout.Height(15));
                                currentlyViewedItem.itemModelName = GUILayout.TextField(currentlyViewedItem.itemModelName, GUILayout.Width(278), GUILayout.Height(15));
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                currentlyViewedItem.weaponModel = (GameObject)EditorGUILayout.ObjectField("Weapon Model", currentlyViewedItem.weaponModel, typeof(GameObject), false);
                                
                                
                                currentlyViewedItem.showWeaponPositionData = EditorGUILayout.Toggle("SHOW POSITIONS?", currentlyViewedItem.showWeaponPositionData, GUILayout.Width(250));
                                if(currentlyViewedItem.showWeaponPositionData){
                                GUILayout.Space(10);
                                GUILayout.Label("WEAPON POSITION DATA", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);

                                if (GUILayout.Button("+ Add Race Data", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedItem.weaponPositionDatas.Add(new RPGItem.WeaponPositionData());

                                var ThisList15 = serialObj.FindProperty("weaponPositionDatas");
                                currentlyViewedItem.weaponPositionDatas = GetTargetObjectOfProperty(ThisList15) as List<RPGItem.WeaponPositionData>;

                                for (var a = 0; a < currentlyViewedItem.weaponPositionDatas.Count; a++)
                                {
                                    GUILayout.Space(10);
                                    var requirementNumber = a + 1;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        currentlyViewedItem.weaponPositionDatas.RemoveAt(a);
                                        return;
                                    }
                                    var raceName = "";
                                    if (currentlyViewedItem.weaponPositionDatas[a].raceID != -1) raceName = RPGBuilderUtilities.GetRaceFromIDEditor(currentlyViewedItem.weaponPositionDatas[a].raceID, allRaces).displayName;
                                    EditorGUILayout.LabelField("" + requirementNumber + ": " + raceName, GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                    
                                    
                                    currentlyViewedItem.weaponPositionDatas[a].raceREF = (RPGRace)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetRaceFromIDEditor(currentlyViewedItem.weaponPositionDatas[a].raceID, allRaces), typeof(RPGRace), false, GUILayout.Width(250));
                                    
                                    if (currentlyViewedItem.weaponPositionDatas[a].raceREF != null)
                                        currentlyViewedItem.weaponPositionDatas[a].raceID = currentlyViewedItem.weaponPositionDatas[a].raceREF.ID;
                                    else
                                        currentlyViewedItem.weaponPositionDatas[a].raceID = -1;


                                    if (currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas.Count == 0)
                                    {
                                        currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas.Add(new RPGItem.WeaponPositionData.GenderPositionData());
                                        currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas.Add(new RPGItem.WeaponPositionData.GenderPositionData());

                                        currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[1].gender =
                                            RPGRace.RACE_GENDER.Female;
                                    }
                                    
                                    //if (GUILayout.Button("+ Add gender Data", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas.Add(new RPGItem.WeaponPositionData.GenderPositionData());
                                    for (var u = 0; u < currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas.Count; u++)
                                    {
                                        GUILayout.Space(10);
                                        var requirementNumber2 = u + 1;
                                        EditorGUILayout.BeginHorizontal();
                                        if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20),
                                            GUILayout.Height(20)))
                                        {
                                            currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas.RemoveAt(u);
                                            return;
                                        }

                                        var effectName2 = raceName + " " + currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].gender.ToString();
                                        
                                        EditorGUILayout.LabelField("" + requirementNumber2 + ": " + effectName2,
                                            GUILayout.Width(400));
                                        EditorGUILayout.EndHorizontal();
                                        
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].combatGORef = (GameObject)EditorGUILayout.ObjectField("Scene Reference", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].combatGORef, typeof(GameObject), true);
                                if (currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].combatGORef != null)
                                {
                                    currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatPositionInSlot = currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].combatGORef.transform.localPosition;
                                    currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatRotationInSlot = currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].combatGORef.transform.localEulerAngles;
                                    currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatScaleInSlot = currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].combatGORef.transform.localScale;
                                }
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatPositionInSlot = EditorGUILayout.Vector3Field("Combat POS", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatPositionInSlot);
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatRotationInSlot = EditorGUILayout.Vector3Field("Combat ROT", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatRotationInSlot);
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatScaleInSlot = EditorGUILayout.Vector3Field("Combat SCALE", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].CombatScaleInSlot);

                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].restGORef = (GameObject)EditorGUILayout.ObjectField("Scene Reference", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].restGORef, typeof(GameObject), true);
                                if (currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].restGORef != null)
                                {
                                    currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestPositionInSlot = currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].restGORef.transform.localPosition;
                                    currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestRotationInSlot = currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].restGORef.transform.localEulerAngles;
                                    currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestScaleInSlot = currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].restGORef.transform.localScale;
                                }
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestPositionInSlot = EditorGUILayout.Vector3Field("Rest POS", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestPositionInSlot);
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestRotationInSlot = EditorGUILayout.Vector3Field("Rest ROT", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestRotationInSlot);
                                currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestScaleInSlot = EditorGUILayout.Vector3Field("Rest SCALE", currentlyViewedItem.weaponPositionDatas[a].genderPositionDatas[u].RestScaleInSlot);
                                        
                                    }
                                } 
                                }
                                
                                GUILayout.Space(10);
                                GUILayout.Label("COMBAT", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                                GUILayout.Space(10);
                                currentlyViewedItem.AttackSpeed = EditorGUILayout.FloatField("Attack Speed", currentlyViewedItem.AttackSpeed, GUILayout.Width(430));
                                currentlyViewedItem.minDamage = EditorGUILayout.IntField("Min DMG", currentlyViewedItem.minDamage, GUILayout.Width(430));
                                currentlyViewedItem.maxDamage = EditorGUILayout.IntField("Max DMG", currentlyViewedItem.maxDamage, GUILayout.Width(430));
                                
                                currentlyViewedItem.autoAttackAbilityREF = (RPGAbility)EditorGUILayout.ObjectField("Auto Attack", RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedItem.autoAttackAbilityID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(430));
                                if (currentlyViewedItem.autoAttackAbilityREF != null)
                                    currentlyViewedItem.autoAttackAbilityID = currentlyViewedItem.autoAttackAbilityREF.ID;
                                else
                                    currentlyViewedItem.autoAttackAbilityID = -1;
                            }

                            GUILayout.Space(10);
                            GUILayout.Label("STATS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);

                            if (GUILayout.Button("+ Add Stat", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedItem.stats.Add(new RPGItem.ITEM_STATS());

                            var ThisList2 = serialObj.FindProperty("stats");
                            currentlyViewedItem.stats = GetTargetObjectOfProperty(ThisList2) as List<RPGItem.ITEM_STATS>;

                            for (var a = 0; a < currentlyViewedItem.stats.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedItem.stats.RemoveAt(a);
                                    return;
                                }
                                var effectName = "";
                                if (currentlyViewedItem.stats[a].statREF != null) effectName = currentlyViewedItem.stats[a].statREF._name;
                                EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Stat", GUILayout.Width(100));
                                currentlyViewedItem.stats[a].statREF = (RPGStat)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetStatFromIDEditor(currentlyViewedItem.stats[a].statID, allStats), typeof(RPGStat), false, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                if (currentlyViewedItem.stats[a].statREF != null)
                                    currentlyViewedItem.stats[a].statID = currentlyViewedItem.stats[a].statREF.ID;
                                else
                                    currentlyViewedItem.stats[a].statID = -1;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Amount", GUILayout.Width(100));
                                currentlyViewedItem.stats[a].amount = EditorGUILayout.FloatField(currentlyViewedItem.stats[a].amount, GUILayout.Width(250));
                                GUILayout.Space(10);

                                EditorGUILayout.EndHorizontal();
                                
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Is Percent?", GUILayout.Width(100));
                                currentlyViewedItem.stats[a].isPercent = EditorGUILayout.Toggle(currentlyViewedItem.stats[a].isPercent, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                GUILayout.Space(10);
                            }
                            
                            
                            GUILayout.Space(10);
                            GUILayout.Label("RANDOM STATS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                            GUILayout.Space(10);

                            if (GUILayout.Button("+ Add Random Stat", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedItem.randomStats.Add(new RPGItemDATA.RandomizedStatData());

                            var ThisList10 = serialObj.FindProperty("randomStats");
                            currentlyViewedItem.randomStats = GetTargetObjectOfProperty(ThisList10) as List<RPGItemDATA.RandomizedStatData>;

                            for (var a = 0; a < currentlyViewedItem.randomStats.Count; a++)
                            {
                                GUILayout.Space(10);
                                var requirementNumber = a + 1;
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    currentlyViewedItem.randomStats.RemoveAt(a);
                                    return;
                                }
                                var effectName = "";
                                if (currentlyViewedItem.randomStats[a].statREF != null) effectName = currentlyViewedItem.randomStats[a].statREF._name;
                                EditorGUILayout.LabelField("" + requirementNumber + ": " + effectName, GUILayout.Width(400));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Stat", GUILayout.Width(100));
                                currentlyViewedItem.randomStats[a].statREF = (RPGStat)EditorGUILayout.ObjectField(RPGBuilderUtilities.GetStatFromIDEditor(currentlyViewedItem.randomStats[a].statID, allStats), typeof(RPGStat), false, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                if (currentlyViewedItem.randomStats[a].statREF != null)
                                    currentlyViewedItem.randomStats[a].statID = currentlyViewedItem.randomStats[a].statREF.ID;
                                else
                                    currentlyViewedItem.randomStats[a].statID = -1;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Min", GUILayout.Width(100));
                                currentlyViewedItem.randomStats[a].minValue = EditorGUILayout.FloatField(currentlyViewedItem.randomStats[a].minValue, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Max", GUILayout.Width(100));
                                currentlyViewedItem.randomStats[a].maxValue = EditorGUILayout.FloatField(currentlyViewedItem.randomStats[a].maxValue, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Is Percent?", GUILayout.Width(100));
                                currentlyViewedItem.randomStats[a].isPercent = EditorGUILayout.Toggle(currentlyViewedItem.randomStats[a].isPercent, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Is Integer?", GUILayout.Width(100));
                                currentlyViewedItem.randomStats[a].isInt = EditorGUILayout.Toggle(currentlyViewedItem.randomStats[a].isInt, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Chance", GUILayout.Width(100));
                                currentlyViewedItem.randomStats[a].chance = EditorGUILayout.Slider(currentlyViewedItem.randomStats[a].chance, 0f, 100f, GUILayout.Width(250));
                                EditorGUILayout.EndHorizontal();
                                

                                GUILayout.Space(10);
                            }
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("REQUIREMENTS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);


                        if (GUILayout.Button("+ Add Requirement", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedItem.useRequirements.Add(new RequirementsManager.RequirementDATA());

                        var ThisList = serialObj.FindProperty("useRequirements");
                        currentlyViewedItem.useRequirements = GetTargetObjectOfProperty(ThisList) as List<RequirementsManager.RequirementDATA>;

                        for (var a = 0; a < currentlyViewedItem.useRequirements.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedItem.useRequirements[a].requirementType = (RequirementsManager.RequirementType)EditorGUILayout.EnumPopup(currentlyViewedItem.useRequirements[a].requirementType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                currentlyViewedItem.useRequirements.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedItem.useRequirements.Count > 0)
                            {
                                if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.pointSpent)
                                {
                                    currentlyViewedItem.useRequirements[a].pointSpentValue = EditorGUILayout.IntField(new GUIContent("Points Spent", "How many points should already be spent in this tree for this bonus to be active?"), currentlyViewedItem.useRequirements[a].pointSpentValue, GUILayout.Width(400));
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.classLevel)
                                {
                                    currentlyViewedItem.useRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedItem.useRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                    currentlyViewedItem.useRequirements[a].classLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The class level required"), currentlyViewedItem.useRequirements[a].classLevelValue, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].classRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].classRequiredID = currentlyViewedItem.useRequirements[a].classRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].classRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType._class)
                                {
                                    currentlyViewedItem.useRequirements[a].classRequiredREF = (RPGClass)EditorGUILayout.ObjectField(new GUIContent("Class", "The class required for this bonus to be active"), RPGBuilderUtilities.GetClassFromIDEditor(currentlyViewedItem.useRequirements[a].classRequiredID, allClasses), typeof(RPGClass), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].classRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].classRequiredID = currentlyViewedItem.useRequirements[a].classRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].classRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.skillLevel)
                                {
                                    currentlyViewedItem.useRequirements[a].skillRequiredREF = (RPGSkill)EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill required for this bonus to be active"), RPGBuilderUtilities.GetSkillFromIDEditor(currentlyViewedItem.useRequirements[a].skillRequiredID, allSkills), typeof(RPGSkill), false, GUILayout.Width(400));
                                    currentlyViewedItem.useRequirements[a].skillLevelValue = EditorGUILayout.IntField(new GUIContent("Level", "The skill level required"), currentlyViewedItem.useRequirements[a].skillLevelValue, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].skillRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].skillRequiredID = currentlyViewedItem.useRequirements[a].skillRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].skillRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.itemOwned)
                                {
                                    currentlyViewedItem.useRequirements[a].itemRequiredREF = (RPGItem)EditorGUILayout.ObjectField(new GUIContent("Item", "The item required"), RPGBuilderUtilities.GetItemFromIDEditor(currentlyViewedItem.useRequirements[a].itemRequiredID, allItems), typeof(RPGItem), false, GUILayout.Width(400));
                                    currentlyViewedItem.useRequirements[a].consumeItem = EditorGUILayout.Toggle(new GUIContent("Consumed?", "Is this item consumed?"), currentlyViewedItem.useRequirements[a].consumeItem);
                                    if (currentlyViewedItem.useRequirements[a].itemRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].itemRequiredID = currentlyViewedItem.useRequirements[a].itemRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].itemRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.abilityKnown)
                                {
                                    currentlyViewedItem.useRequirements[a].abilityRequiredREF = (RPGAbility)EditorGUILayout.ObjectField(new GUIContent("Ability", "The ability required to be known for this bonus to be active"), RPGBuilderUtilities.GetAbilityFromIDEditor(currentlyViewedItem.useRequirements[a].abilityRequiredID, allAbilities), typeof(RPGAbility), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].abilityRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].abilityRequiredID = currentlyViewedItem.useRequirements[a].abilityRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].abilityRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.recipeKnown)
                                {
                                    currentlyViewedItem.useRequirements[a].recipeRequiredREF = (RPGCraftingRecipe)EditorGUILayout.ObjectField(new GUIContent("Recipe", "The crafting recipe required to be known for this bonus to be active"), RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(currentlyViewedItem.useRequirements[a].craftingRecipeRequiredID, allCraftingRecipes), typeof(RPGCraftingRecipe), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].recipeRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].craftingRecipeRequiredID = currentlyViewedItem.useRequirements[a].recipeRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].craftingRecipeRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.resourceNodeKnown)
                                {
                                    currentlyViewedItem.useRequirements[a].resourceNodeRequiredREF = (RPGResourceNode)EditorGUILayout.ObjectField(new GUIContent("Resource Node", "The resource node required to be known for this bonus to be active"), RPGBuilderUtilities.GetResourceNodeFromIDEditor(currentlyViewedItem.useRequirements[a].resourceNodeRequiredID, allResourceNodes), typeof(RPGResourceNode), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].resourceNodeRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].resourceNodeRequiredID = currentlyViewedItem.useRequirements[a].resourceNodeRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].resourceNodeRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.race)
                                {
                                    currentlyViewedItem.useRequirements[a].raceRequiredREF = (RPGRace)EditorGUILayout.ObjectField(new GUIContent("Race", "The race required"), RPGBuilderUtilities.GetRaceFromIDEditor(currentlyViewedItem.useRequirements[a].raceRequiredID, allRaces), typeof(RPGRace), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].raceRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].raceRequiredID = currentlyViewedItem.useRequirements[a].raceRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].raceRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.questState)
                                {
                                    currentlyViewedItem.useRequirements[a].questRequiredREF = (RPGQuest)EditorGUILayout.ObjectField(new GUIContent("Quest", "The quest required for this bonus to be active"), RPGBuilderUtilities.GetQuestFromIDEditor(currentlyViewedItem.useRequirements[a].questRequiredID, allQuests), typeof(RPGQuest), false, GUILayout.Width(400));
                                    currentlyViewedItem.useRequirements[a].questStateRequired = (QuestManager.questState)EditorGUILayout.EnumPopup(new GUIContent("State", "The required state of the quest"), currentlyViewedItem.useRequirements[a].questStateRequired, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].questRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].questRequiredID = currentlyViewedItem.useRequirements[a].questRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].questRequiredID = -1;
                                }
                                else if (currentlyViewedItem.useRequirements[a].requirementType == RequirementsManager.RequirementType.npcKilled)
                                {
                                    currentlyViewedItem.useRequirements[a].npcRequiredREF = (RPGNpc)EditorGUILayout.ObjectField(new GUIContent("NPC", "The NPC required to be killed"), RPGBuilderUtilities.GetNPCFromIDEditor(currentlyViewedItem.useRequirements[a].npcRequiredID, allNPCs), typeof(RPGNpc), false, GUILayout.Width(400));
                                    currentlyViewedItem.useRequirements[a].npcKillsRequired = EditorGUILayout.IntField(new GUIContent("Kills", "How many times this NPC should have been killed for the bonus to be active"), currentlyViewedItem.useRequirements[a].npcKillsRequired, GUILayout.Width(400));
                                    if (currentlyViewedItem.useRequirements[a].npcRequiredREF != null)
                                        currentlyViewedItem.useRequirements[a].npcRequiredID = currentlyViewedItem.useRequirements[a].npcRequiredREF.ID;
                                    else
                                        currentlyViewedItem.useRequirements[a].npcRequiredID = -1;
                                }
                            }
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);
                        }


                        GUILayout.Space(10);
                        GUILayout.Label("ON USE ACTIONS", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);

                        if (GUILayout.Button("+ Add Action", skin.GetStyle("AddButton"), GUILayout.Width(440), GUILayout.Height(25))) currentlyViewedItem.onUseActions.Add(new RPGItem.OnUseActionDATA());

                        var ThisList3 = serialObj.FindProperty("onUseActions");
                        currentlyViewedItem.onUseActions = GetTargetObjectOfProperty(ThisList3) as List<RPGItem.OnUseActionDATA>;

                        for (var a = 0; a < currentlyViewedItem.onUseActions.Count; a++)
                        {
                            GUILayout.Space(10);
                            var requirementNumber = a + 1;
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
                            currentlyViewedItem.onUseActions[a].actionType =
                                (RPGItem.OnUseActionType) EditorGUILayout.EnumPopup(
                                    currentlyViewedItem.onUseActions[a].actionType, GUILayout.Width(250));
                            GUILayout.Space(10);
                            if (GUILayout.Button("X", skin.GetStyle("RemoveButton"), GUILayout.Width(20),
                                GUILayout.Height(20)))
                            {
                                currentlyViewedItem.onUseActions.RemoveAt(a);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginVertical();

                            if (currentlyViewedItem.onUseActions.Count > 0)
                            {
                                if (currentlyViewedItem.onUseActions[a].actionType ==
                                    RPGItem.OnUseActionType.gainTreePoint)
                                {
                                    currentlyViewedItem.onUseActions[a].treePointREF =
                                        (RPGTreePoint) EditorGUILayout.ObjectField(
                                            new GUIContent("Tree Point", "The tree point gained"),
                                            RPGBuilderUtilities.GetTreePointFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].treePointID, allTreePoints),
                                            typeof(RPGTreePoint), false, GUILayout.Width(400));
                                    currentlyViewedItem.onUseActions[a].treePointGained = EditorGUILayout.IntField(
                                        new GUIContent("Points Gained", "How many points should be gained?"),
                                        currentlyViewedItem.onUseActions[a].treePointGained, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].treePointREF != null)
                                        currentlyViewedItem.onUseActions[a].treePointID =
                                            currentlyViewedItem.onUseActions[a].treePointREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].treePointID = -1;
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));
                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.useAbility ||
                                         currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.learnAbility)
                                {
                                    currentlyViewedItem.onUseActions[a].abilityREF =
                                        (RPGAbility) EditorGUILayout.ObjectField(
                                            new GUIContent("Ability Used", "The ability used by the item"),
                                            RPGBuilderUtilities.GetAbilityFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].abilityID, allAbilities),
                                            typeof(RPGAbility), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].abilityREF != null)
                                        currentlyViewedItem.onUseActions[a].abilityID =
                                            currentlyViewedItem.onUseActions[a].abilityREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].abilityID = -1;
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.useEffect)
                                {
                                    currentlyViewedItem.onUseActions[a].effectREF =
                                        (RPGEffect) EditorGUILayout.ObjectField(
                                            new GUIContent("Effect", "The effect triggered"),
                                            RPGBuilderUtilities.GetEffectFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].effectID, allEffects),
                                            typeof(RPGEffect), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].effectREF != null)
                                        currentlyViewedItem.onUseActions[a].effectID =
                                            currentlyViewedItem.onUseActions[a].effectREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].effectID = -1;
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.learnRecipe)
                                {
                                    currentlyViewedItem.onUseActions[a].recipeREF =
                                        (RPGCraftingRecipe) EditorGUILayout.ObjectField(
                                            new GUIContent("Recipe", "The crafting recipe that will be learned"),
                                            RPGBuilderUtilities.GetCraftingRecipeFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].recipeID, allCraftingRecipes),
                                            typeof(RPGCraftingRecipe), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].recipeREF != null)
                                        currentlyViewedItem.onUseActions[a].recipeID =
                                            currentlyViewedItem.onUseActions[a].recipeREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].recipeID = -1;
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.learnResourceNode)
                                {
                                    currentlyViewedItem.onUseActions[a].resourceNodeREF =
                                        (RPGResourceNode) EditorGUILayout.ObjectField(
                                            new GUIContent("Resource Node", "The resource node that will be learned"),
                                            RPGBuilderUtilities.GetResourceNodeFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].resourceNodeID, allResourceNodes),
                                            typeof(RPGResourceNode), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].resourceNodeREF != null)
                                        currentlyViewedItem.onUseActions[a].resourceNodeID =
                                            currentlyViewedItem.onUseActions[a].resourceNodeREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].resourceNodeID = -1;
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.learnBonus)
                                {
                                    currentlyViewedItem.onUseActions[a].bonusREF =
                                        (RPGBonus) EditorGUILayout.ObjectField(
                                            new GUIContent("Bonus", "The bonus that will be learned"),
                                            RPGBuilderUtilities.GetBonusFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].bonusID, allBonuses),
                                            typeof(RPGBonus), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].bonusREF != null)
                                        currentlyViewedItem.onUseActions[a].bonusID =
                                            currentlyViewedItem.onUseActions[a].bonusREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].bonusID = -1;
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType == RPGItem.OnUseActionType.gainClassLevel ||
                                         currentlyViewedItem.onUseActions[a].actionType == RPGItem.OnUseActionType.gainClassXP)
                                {
                                    if (currentlyViewedItem.onUseActions[a].actionType ==
                                        RPGItem.OnUseActionType.gainClassLevel)
                                        currentlyViewedItem.onUseActions[a].classLevelGained = EditorGUILayout.IntField(
                                            new GUIContent("Level(s) Gained", "The amount of levels gained"),
                                            currentlyViewedItem.onUseActions[a].classLevelGained, GUILayout.Width(400));
                                    else if (currentlyViewedItem.onUseActions[a].actionType ==
                                             RPGItem.OnUseActionType.gainClassXP)
                                        currentlyViewedItem.onUseActions[a].classXPGained =
                                            EditorGUILayout.IntField(
                                                new GUIContent("XP Gained", "The amount of XP gained"),
                                                currentlyViewedItem.onUseActions[a].classXPGained,
                                                GUILayout.Width(400));
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.gainSkillLevel ||
                                         currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.gainSkillXP)
                                {
                                    currentlyViewedItem.onUseActions[a].skillREF =
                                        (RPGSkill) EditorGUILayout.ObjectField(new GUIContent("Skill", "The skill"),
                                            RPGBuilderUtilities.GetSkillFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].skillID, allSkills),
                                            typeof(RPGSkill), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].skillREF != null)
                                        currentlyViewedItem.onUseActions[a].skillID =
                                            currentlyViewedItem.onUseActions[a].skillREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].skillID = -1;

                                    if (currentlyViewedItem.onUseActions[a].actionType ==
                                        RPGItem.OnUseActionType.gainSkillLevel)
                                        currentlyViewedItem.onUseActions[a].skillLevelGained = EditorGUILayout.IntField(
                                            new GUIContent("Level(s) Gained", "The amount of levels gained"),
                                            currentlyViewedItem.onUseActions[a].skillLevelGained, GUILayout.Width(400));
                                    else if (currentlyViewedItem.onUseActions[a].actionType ==
                                             RPGItem.OnUseActionType.gainSkillXP)
                                        currentlyViewedItem.onUseActions[a].skillXPGained =
                                            EditorGUILayout.IntField(
                                                new GUIContent("XP Gained", "The amount of XP gained"),
                                                currentlyViewedItem.onUseActions[a].skillXPGained,
                                                GUILayout.Width(400));
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.acceptQuest)
                                {
                                    currentlyViewedItem.onUseActions[a].questREF =
                                        (RPGQuest) EditorGUILayout.ObjectField(
                                            new GUIContent("Quest", "The quest given by the item"),
                                            RPGBuilderUtilities.GetQuestFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].questID, allQuests),
                                            typeof(RPGQuest), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].questREF != null)
                                        currentlyViewedItem.onUseActions[a].questID =
                                            currentlyViewedItem.onUseActions[a].questREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].questID = -1;
                                    
                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));
                                }
                                else if (currentlyViewedItem.onUseActions[a].actionType ==
                                         RPGItem.OnUseActionType.currency)
                                {
                                    currentlyViewedItem.onUseActions[a].currencyREF =
                                        (RPGCurrency) EditorGUILayout.ObjectField(
                                            new GUIContent("Currency", "The Currency"),
                                            RPGBuilderUtilities.GetCurrencyFromIDEditor(
                                                currentlyViewedItem.onUseActions[a].currencyID, allCurrencies),
                                            typeof(RPGCurrency), false, GUILayout.Width(400));
                                    if (currentlyViewedItem.onUseActions[a].currencyREF != null)
                                        currentlyViewedItem.onUseActions[a].currencyID =
                                            currentlyViewedItem.onUseActions[a].currencyREF.ID;
                                    else
                                        currentlyViewedItem.onUseActions[a].currencyID = -1;

                                    currentlyViewedItem.onUseActions[a].isConsumed = EditorGUILayout.Toggle(
                                        new GUIContent("Consume Item?", "Should this action be consumed on use?"),
                                        currentlyViewedItem.onUseActions[a].isConsumed, GUILayout.Width(400));

                                }
                            }

                            EditorGUILayout.EndVertical();

                        }

                        GUILayout.Space(10);
                        GUILayout.Label("GENERAL", skin.GetStyle("ViewTitle"), GUILayout.Width(450), GUILayout.Height(40));
                        GUILayout.Space(10);
                        GUILayout.BeginVertical();
                        GUILayout.Label("Resell Settings:", GUILayout.Width(300), GUILayout.Height(15));
                        currentlyViewedItem.sellCurrencyREF = (RPGCurrency)EditorGUILayout.ObjectField("Currency", RPGBuilderUtilities.GetCurrencyFromIDEditor(currentlyViewedItem.sellCurrencyID, allCurrencies), typeof(RPGCurrency), false);
                        if (currentlyViewedItem.sellCurrencyREF != null)
                            currentlyViewedItem.sellCurrencyID = currentlyViewedItem.sellCurrencyREF.ID;
                        else
                            currentlyViewedItem.sellCurrencyID = -1;

                        currentlyViewedItem.sellPrice = EditorGUILayout.IntField("Amount", currentlyViewedItem.sellPrice, GUILayout.Width(430));
                        /*GUILayout.Label("Buy Settings:", GUILayout.Width(300), GUILayout.Height(15));
                        currentlyViewedItem.buyPrice = EditorGUILayout.IntField("Buy Price", currentlyViewedItem.buyPrice, GUILayout.Width(430));
                        currentlyViewedItem.buyCurrency = (RPGCurrency)EditorGUILayout.ObjectField("Currency", currentlyViewedItem.buyCurrency, typeof(RPGCurrency), false);*/
                        GUILayout.EndVertical();
                        GUILayout.Space(15);
                        currentlyViewedItem.stackLimit = EditorGUILayout.IntField("Stack Limit", currentlyViewedItem.stackLimit, GUILayout.Width(430));
                        if (currentlyViewedItem.randomStats.Count > 0)
                        {
                            currentlyViewedItem.stackLimit = 1;
                        }
                        
                        GUILayout.Label("Description:", GUILayout.Width(147), GUILayout.Height(15));
                        currentlyViewedItem.description = GUILayout.TextArea(currentlyViewedItem.description, GUILayout.Width(430), GUILayout.Height(40));

                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.Item, containerRect2);
    }


    private void DrawSettingsView()
    {
        switch (settingsSubCurrentCategorySelected)
        {
            case SettingsSubCategorySelectedType.General:
                DrawGeneralSettingsView();
                break;
            case SettingsSubCategorySelectedType.Combat:
                DrawCombatSettingsView();
                break;
            case SettingsSubCategorySelectedType.Item:
                DrawItemSettingsView();
                break;
            case SettingsSubCategorySelectedType.Editor:
                DrawEditorSettingsView();
                break;
        }
    }


    private void DrawPartnersView()
    {
        switch (partnersSubCurrentCategorySelected)
        {
            case PartnersSubCategorySelectedType.PolytopeStudio:
                DrawPolytopeStudioView();
                break;
            case PartnersSubCategorySelectedType.Cafofo:
                DrawCafofoView();
                break;
            case PartnersSubCategorySelectedType.GabrielAguiar:
                DrawGabrielAguiarView();
                break;
            case PartnersSubCategorySelectedType.RDR:
                DrawRDRView();
                break;
            case PartnersSubCategorySelectedType.TitanForge:
                DrawTitanForgeView();
                break;
            case PartnersSubCategorySelectedType.PONETI:
                DrawPONETIView();
                break;
            case PartnersSubCategorySelectedType.MalbersAnimation:
                DrawMalbersAnimationView();
                break;
        }
    }

    private void DrawGeneralSettingsView()
    {
        if (generalSettings == null)
        {
            generalSettings = Resources.Load<RPGGeneralDATA>("THMSV/RPGBuilderData/Settings/GeneralSettings");
            generalSettings = Instantiate(generalSettings) as RPGGeneralDATA;
            if (generalSettings == null)
            {
                Debug.LogError("COULD NOT FIND GENERAL SETTINGS");
                return;
            }
        }

        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        //GUILayout.Label("View: Combat Settings", skin.GetStyle("Header2"));
        GUILayout.BeginHorizontal();


        GUILayout.EndHorizontal();

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "settings");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, editorDATA.FullViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("SAVING SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        generalSettings.automaticSave = EditorGUILayout.Toggle("Auto Save?", generalSettings.automaticSave, GUILayout.Width(430));
                        if (generalSettings.automaticSave)
                        {
                            generalSettings.automaticSaveDelay = EditorGUILayout.FloatField("Save Delay", generalSettings.automaticSaveDelay, GUILayout.Width(430));
                            if(generalSettings.automaticSaveDelay < 1) generalSettings.automaticSaveDelay = 1;
                        }
                        generalSettings.automaticSaveOnQuit = EditorGUILayout.Toggle("Autom Save on Quit?", generalSettings.automaticSaveOnQuit, GUILayout.Width(430));

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.GeneralSettings, containerRect2);
    }


    private void DrawPolytopeStudioView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));
        

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));


                        GUILayout.Label("POLYTOPE STUDIO:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin+125, 40, 320, 180)
                            , editorDATA.polytopePartnerImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://bit.ly/2FsiWGj");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginVertical();


                        EditorGUI.BeginDisabledGroup(true);
                        for (var i = 0; i < editorDATA.polytopeStudioAssets_GO.Count; i++) EditorGUILayout.ObjectField(editorDATA.polytopeStudioAssets_GO[i].name, editorDATA.polytopeStudioAssets_GO[i], typeof(GameObject), false, GUILayout.Width(500), GUILayout.Height(20));
                        EditorGUI.EndDisabledGroup();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }

    private void DrawCafofoView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("CAFOFO:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin + 125, 40, 320, 180)
                            , editorDATA.cafofoPartnerImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://assetstore.unity.com/publishers/16881");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginVertical();


                        EditorGUI.BeginDisabledGroup(true);
                        for (var i = 0; i < editorDATA.cafofoAssets_AUDIO.Count; i++) EditorGUILayout.ObjectField(editorDATA.cafofoAssets_AUDIO[i].name, editorDATA.cafofoAssets_AUDIO[i], typeof(AudioClip), false, GUILayout.Width(500), GUILayout.Height(20));
                        EditorGUI.EndDisabledGroup();


                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }

    private void DrawGabrielAguiarView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("Gabriel Aguiar:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin + 125, 40, 320, 180)
                            , editorDATA.GabrielAguiarPartnerImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://assetstore.unity.com/publishers/31523");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginVertical();
                        
                        EditorGUI.BeginDisabledGroup(true);
                        for (var i = 0; i < editorDATA.GabbrielAguiarAsset_GO.Count; i++) EditorGUILayout.ObjectField(editorDATA.GabbrielAguiarAsset_GO[i].name, editorDATA.GabbrielAguiarAsset_GO[i], typeof(GameObject), false, GUILayout.Width(500), GUILayout.Height(20));
                        EditorGUI.EndDisabledGroup();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }

    private void DrawRDRView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("RDR:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin + 125, 40, 320, 180)
                            , editorDATA.RDRPartnerImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://assetstore.unity.com/publishers/19740");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);



                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.BeginHorizontal();
                        for (var i = 0; i < editorDATA.RDRAssets_SPRITE.Count; i++)
                        {
                            EditorGUILayout.ObjectField(editorDATA.RDRAssets_SPRITE[i], typeof(Sprite), false, GUILayout.Width(150), GUILayout.Height(150));
                            var curImageIndex = i + 1;
                            if (i > 0 && curImageIndex % 4 == 0)
                            {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.BeginHorizontal();
                            }
                        }
                        EditorGUILayout.BeginVertical();

                        for (var i = 0; i < editorDATA.RDRAssets_GO.Count; i++) EditorGUILayout.ObjectField(editorDATA.RDRAssets_GO[i].name, editorDATA.RDRAssets_GO[i], typeof(GameObject), false, GUILayout.Width(500), GUILayout.Height(20));
                        EditorGUILayout.EndVertical();
                        
                        EditorGUI.EndDisabledGroup();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }

    private void DrawTitanForgeView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("Titan Forge:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin + 125, 40, 320, 180)
                            , editorDATA.TitanForgePartnerImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://assetstore.unity.com/publishers/45702");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginVertical();

                        EditorGUI.BeginDisabledGroup(true);
                        for (var i = 0; i < editorDATA.TitanForgeAssets_GO.Count; i++) EditorGUILayout.ObjectField(editorDATA.TitanForgeAssets_GO[i].name, editorDATA.TitanForgeAssets_GO[i], typeof(GameObject), false, GUILayout.Width(500), GUILayout.Height(20));
                        EditorGUI.EndDisabledGroup();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }

    private void DrawPONETIView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("PONETI:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin + 125, 40, 320, 180)
                            , editorDATA.PONETIPartnerImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://assetstore.unity.com/publishers/38930");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        for (var i = 0; i < editorDATA.PONETIAssets_SPRITE.Count; i++)
                        {
                            EditorGUILayout.ObjectField(editorDATA.PONETIAssets_SPRITE[i], typeof(Sprite), false, GUILayout.Width(150), GUILayout.Height(150));
                            var curImageIndex = i + 1;
                            if(i>0 && curImageIndex % 4 == 0)
                            {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.BeginHorizontal();
                            }
                        }
                        EditorGUI.EndDisabledGroup();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }

    private void DrawMalbersAnimationView()
    {
        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));


        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "partners");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, 0, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)partnersSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("Malbers Animation:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginHorizontal();
                        GUI.DrawTexture(new Rect(editorDATA.FullViewLeftMargin + 125, 40, 320, 180)
                            , editorDATA.MalbersAnimationParterImage.texture);

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(190);

                        if (GUILayout.Button("> GO TO ASSET STORE PAGE <", skin.GetStyle("AddButton"), GUILayout.Width(650), GUILayout.Height(40))) Application.OpenURL("https://assetstore.unity.com/publishers/16163");
                        GUILayout.Space(10);

                        GUILayout.Label("ASSETS PROVIDED:", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);

                        EditorGUILayout.BeginVertical();

                        EditorGUI.BeginDisabledGroup(true);
                        for (var i = 0; i < editorDATA.MalbersAnimationAssets_GO.Count; i++) EditorGUILayout.ObjectField(editorDATA.MalbersAnimationAssets_GO[i].name, editorDATA.MalbersAnimationAssets_GO[i], typeof(GameObject), false, GUILayout.Width(500), GUILayout.Height(20));
                        EditorGUI.EndDisabledGroup();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();
    }
    
    private void LoadSettings()
    {
        combatSettings = Resources.Load<RPGCombatDATA>("THMSV/RPGBuilderData/Settings/CombatSettings");
        combatSettings = Instantiate(combatSettings) as RPGCombatDATA;
        itemSettings = Resources.Load<RPGItemDATA>("THMSV/RPGBuilderData/Settings/ItemSettings");
        itemSettings = Instantiate(itemSettings) as RPGItemDATA;
        generalSettings = Resources.Load<RPGGeneralDATA>("THMSV/RPGBuilderData/Settings/GeneralSettings");
        generalSettings = Instantiate(generalSettings) as RPGGeneralDATA;
    }

    private void DrawCombatSettingsView()
    {
        if(combatSettings == null)
        {
            combatSettings = Resources.Load<RPGCombatDATA>("THMSV/RPGBuilderData/Settings/CombatSettings");
            combatSettings = Instantiate(combatSettings) as RPGCombatDATA;
            if (combatSettings == null)
            {
                Debug.LogError("COULD NOT FIND COMBAT SETTINGS");
                return;
            }
        }

        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        //GUILayout.Label("View: Combat Settings", skin.GetStyle("Header2"));
        GUILayout.BeginHorizontal();
        
        
        GUILayout.EndHorizontal();

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "settings");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].panelType)
                {
                    
                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, editorDATA.FullViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));
                        

                        ScriptableObject scriptableObj = combatSettings;
                        var serialObj = new SerializedObject(scriptableObj);
                        var serialProp3 = serialObj.FindProperty("StatFunctions");
                        var serialProp4 = serialObj.FindProperty("States");
                        var serialProp5 = serialObj.FindProperty("UIStatsCategories");

                        GUILayout.Label("COMBAT RULES", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        combatSettings.CriticalDamageBonus = EditorGUILayout.FloatField("Critical Bonus (%)", combatSettings.CriticalDamageBonus, GUILayout.Width(430));
                        GUILayout.Label("ACTION BARS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        combatSettings.actionBarSlots = EditorGUILayout.IntField("Action Bar Slots", combatSettings.actionBarSlots, GUILayout.Width(430));
                        GUILayout.Label("STAT FUNCTIONS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp3, true, GUILayout.Width(450));
                        GUILayout.Label("STATES", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp4, true, GUILayout.Width(450));
                        GUILayout.Label("STAT UI CATEGORY", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp5, true, GUILayout.Width(450));


                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.CombatSettings, containerRect2);
    }

    private void DrawItemSettingsView()
    {
        if (itemSettings == null)
        {
            itemSettings = Resources.Load<RPGItemDATA>("THMSV/RPGBuilderData/Settings/ItemSettings");
            itemSettings = Instantiate(itemSettings) as RPGItemDATA;
            if (itemSettings == null)
            {
                Debug.LogError("COULD NOT FIND ITEM SETTINGS");
                return;
            }
        }

        var containerRect = getContainerRect("View");
        var containerRect2 = getContainerRect("ActionButtons");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin, containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        //GUILayout.Label("View: Item Settings", skin.GetStyle("Header2"));
        GUILayout.BeginHorizontal();
        

        GUILayout.EndHorizontal();

        for (var x = 0; x < editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData.Length; x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].Draw)
            {

                viewInit(x, containerRect, subContainerRect, "settings");

                switch (editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, editorDATA.FullViewTopMargin, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.width, editorDATA.categoriesData[(int)currentCategorySelected].subCategoriesData[(int)settingsSubCurrentCategorySelected].containersData[x].containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition, GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));


                        ScriptableObject scriptableObj = itemSettings;
                        var serialObj = new SerializedObject(scriptableObj);
                        var serialProp = serialObj.FindProperty("itemType");
                        var serialProp2 = serialObj.FindProperty("weaponType");
                        var serialProp3 = serialObj.FindProperty("armorType");
                        var serialProp6 = serialObj.FindProperty("buildingType");
                        var serialProp7 = serialObj.FindProperty("itemQuality");
                        var serialProp8 = serialObj.FindProperty("armorSlots");
                        var serialProp12 = serialObj.FindProperty("weaponSlots");
                        var serialProp9 = serialObj.FindProperty("slotType");
                        var serialProp10 = serialObj.FindProperty("itemQualityImages");
                        var serialProp11 = serialObj.FindProperty("itemQualityColors");

                        GUILayout.Label("INVENTORY SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        itemSettings.InventorySlots = EditorGUILayout.IntField("Slots", itemSettings.InventorySlots, GUILayout.Width(430));

                        GUILayout.Label("ITEM QUALITY", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp7, true, GUILayout.Width(450));
                        GUILayout.Label("ITEM QUALITY IMAGES", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp10, true, GUILayout.Width(450));
                        GUILayout.Label("ITEM QUALITY COLORS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp11, true, GUILayout.Width(450));
                        GUILayout.Label("ITEM TYPES", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp, true, GUILayout.Width(450));
                        GUILayout.Label("WEAPON TYPES", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp2, true, GUILayout.Width(450));
                        GUILayout.Label("ARMOR TYPES", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp3, true, GUILayout.Width(450));
                        GUILayout.Label("ARMOR SLOTS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp8, true, GUILayout.Width(450));
                        GUILayout.Label("WEAPON SLOTS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp12, true, GUILayout.Width(450));
                        GUILayout.Label("SLOT TYPE", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp9, true, GUILayout.Width(450));
                        GUILayout.Label("CRAFTING STATIONS", skin.GetStyle("ViewTitle"), GUILayout.Width(660), GUILayout.Height(30));
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(serialProp6, true, GUILayout.Width(450));


                        serialObj.ApplyModifiedProperties();

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }
                GUILayout.EndArea();
            }
        }
        GUILayout.EndArea();

        DrawActionButtons(AssetType.ItemSettings, containerRect2);
    }

    private void DrawEditorSettingsView()
    {
        if (editorDATA == null) return;

        var containerRect = getContainerRect("View");
        var categoryTextureSize = containerRect.width * 0.8f;
        GUILayout.BeginArea(new Rect(containerRect.x + editorDATA.SubCategoriesLeftMargin,
            containerRect.y + editorDATA.SubCategoriesTopMargin, containerRect.width, containerRect.height));

        for (var x = 0;
            x < editorDATA.categoriesData[(int) currentCategorySelected]
                .subCategoriesData[(int) settingsSubCurrentCategorySelected].containersData.Length;
            x++)
        {
            var subContainerRect = editorDATA.categoriesData[(int) currentCategorySelected]
                .subCategoriesData[(int) settingsSubCurrentCategorySelected].containersData[x].containerRect;

            if (editorDATA.categoriesData[(int) currentCategorySelected]
                .subCategoriesData[(int) settingsSubCurrentCategorySelected].containersData[x].Draw)
            {
                viewInit(x, containerRect, subContainerRect, "settings");

                switch (editorDATA.categoriesData[(int) currentCategorySelected]
                    .subCategoriesData[(int) settingsSubCurrentCategorySelected].containersData[x].panelType)
                {

                    case RPGBuilderEditorDATA.PanelType.view:
                        
                        cachedTheme = editorDATA.curEditorTheme;
                        
                        GUILayout.BeginArea(new Rect(editorDATA.FullViewLeftMargin, editorDATA.FullViewTopMargin,
                            editorDATA.categoriesData[(int) currentCategorySelected]
                                .subCategoriesData[(int) settingsSubCurrentCategorySelected].containersData[x]
                                .containerRect.width,
                            editorDATA.categoriesData[(int) currentCategorySelected]
                                .subCategoriesData[(int) settingsSubCurrentCategorySelected].containersData[x]
                                .containerRect.height));
                        viewScrollPosition = GUILayout.BeginScrollView(viewScrollPosition,
                            GUILayout.Width(editorDATA.FullViewX), GUILayout.Height(editorDATA.FullViewY));

                        GUILayout.Label("THEME SETTINGS", skin.GetStyle("ViewTitle"), GUILayout.Width(660),
                            GUILayout.Height(30));
                        GUILayout.Space(10);
                        editorDATA.curEditorTheme =
                            (RPGBuilderEditorDATA.ThemeTypes) EditorGUILayout.EnumPopup("Editor Theme",
                                editorDATA.curEditorTheme, GUILayout.Width(250));
                        
                        if (cachedTheme != editorDATA.curEditorTheme)
                        {
                            var buttonStyle = skin.GetStyle("ModuleButton");
                            buttonStyle.normal.background = getThemeTexture(1);
                            cachedTheme = editorDATA.curEditorTheme;
                            
                            EditorUtility.SetDirty(editorDATA);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }

                        GUILayout.Space(10);
                        GUILayout.Label("EDITOR UTILITIES", skin.GetStyle("ViewTitle"), GUILayout.Width(660),
                            GUILayout.Height(30));
                        GUILayout.Space(10);
                        if (GUILayout.Button("Delete All Characters", skin.GetStyle("RemoveAbilityRankButton"),
                            GUILayout.Width(440), GUILayout.Height(25)))
                        {
                            if (EditorUtility.DisplayDialog("Confirm DELETE", "Are you sure you want to delete ALL Characters ?", "YES", "Cancel"))
                            {
                                var path = Application.persistentDataPath;
                                var di = new DirectoryInfo(path);
                                var files = di.GetFiles().Where(o => o.Name.Contains("_CharacterData.txt")).ToArray();
                                foreach (var t in files)
                                {
                                    File.Delete(t.FullName);
                                }
                            }
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                        break;
                }

                GUILayout.EndArea();
            }
        }

        GUILayout.EndArea();
        DrawCategories();
    }


    private void SelectCategory(string categoryName)
    {
        curSubCategorySelected = 0;
        curSearchText = "";
        switch (categoryName)
        {
            case "Combat":
                currentCategorySelected = CategorySelectedType.Combat;
                combatSubCurrentCategorySelected = (CombatSubCategorySelectedType)0;
                if (combatSubCurrentCategorySelected == CombatSubCategorySelectedType.Ability && allAbilities.Count > 0) SelectAbility(0);
                break;
            case "General":
                currentCategorySelected = CategorySelectedType.General;
                generalSubCurrentCategorySelected = (GeneralSubCategorySelectedType)0;
                if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.Item && allItems.Count > 0) SelectItem(0);
                break;
            case "World":
                currentCategorySelected = CategorySelectedType.World;
                worldSubCurrentCategorySelected = (WorldSubCategorySelectedType)0;
                if (worldSubCurrentCategorySelected == WorldSubCategorySelectedType.Task && allTasks.Count > 0) SelectTask(0);
                break;
            case "Settings":
                currentCategorySelected = CategorySelectedType.Settings;
                settingsSubCurrentCategorySelected = (SettingsSubCategorySelectedType)0;
                break;
            case "Partners":
                currentCategorySelected = CategorySelectedType.Partners;
                partnersSubCurrentCategorySelected = (PartnersSubCategorySelectedType)0;
                break;
        }
    }

    private void SelectSubCategory(int subCategoryIndex)
    {
        curSubCategorySelected = subCategoryIndex;
        curSearchText = "";
        switch (editorDATA.categoriesData[(int)currentCategorySelected].CategoryName)
        {
            case "Combat":
                combatSubCurrentCategorySelected = (CombatSubCategorySelectedType)subCategoryIndex;
                if (combatSubCurrentCategorySelected == CombatSubCategorySelectedType.Ability && allAbilities.Count > 0)
                    SelectAbility(0);
                else if (combatSubCurrentCategorySelected == CombatSubCategorySelectedType.Effect && allEffects.Count > 0)
                    SelectEffect(0);
                else if (combatSubCurrentCategorySelected == CombatSubCategorySelectedType.NPCs && allNPCs.Count > 0)
                    SelectNPC(0);
                else if (combatSubCurrentCategorySelected == CombatSubCategorySelectedType.Stat && allStats.Count > 0)
                    SelectStat(0);
                else if (combatSubCurrentCategorySelected == CombatSubCategorySelectedType.TreePoint && allTreePoints.Count > 0)
                    SelectTreePoint(0);
                break;
            case "General":
                generalSubCurrentCategorySelected = (GeneralSubCategorySelectedType)subCategoryIndex;
                if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.Item && allItems.Count > 0)
                    SelectItem(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.Skill && allSkills.Count > 0)
                    SelectSkill(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.LevelTemplate && allLevelsTemplate.Count > 0)
                    SelectLevelTemplate(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.Race && allRaces.Count > 0)
                    SelectRace(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.Class && allClasses.Count > 0)
                    SelectClass(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.LootTable && allLootTables.Count > 0)
                    SelectLootTable(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.MerchantTable && allMerchantTables.Count > 0)
                    SelectMerchantTable(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.Currency && allCurrencies.Count > 0)
                    SelectCurrency(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.CraftingRecipe && allCraftingRecipes.Count > 0)
                    SelectCraftingRecipe(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.CraftingStation && allCraftingStations.Count > 0)
                    SelectCraftingStation(0);
                else if (generalSubCurrentCategorySelected == GeneralSubCategorySelectedType.TalentTree && allTalentTrees.Count > 0)
                    SelectTalentTree(0);
                break;
            case "World":
                worldSubCurrentCategorySelected = (WorldSubCategorySelectedType)subCategoryIndex;
                if (worldSubCurrentCategorySelected == WorldSubCategorySelectedType.Task && allTasks.Count > 0)
                    SelectTask(0);
                else if (worldSubCurrentCategorySelected == WorldSubCategorySelectedType.Quest && allQuests.Count > 0)
                    SelectQuest(0);
                else if (worldSubCurrentCategorySelected == WorldSubCategorySelectedType.WorldPosition && allWorldPositions.Count > 0)
                    SelectWorldPosition(0);
                else if (worldSubCurrentCategorySelected == WorldSubCategorySelectedType.ResourceNode && allResourceNodes.Count > 0)
                    SelectResourceNode(0);
                else if (worldSubCurrentCategorySelected == WorldSubCategorySelectedType.GameScene && allGameScenes.Count > 0)
                    SelectGameScene(0);
                break;
            case "Settings":
                settingsSubCurrentCategorySelected = (SettingsSubCategorySelectedType)subCategoryIndex;
                break;
            case "Partners":
                partnersSubCurrentCategorySelected = (PartnersSubCategorySelectedType)subCategoryIndex;
                break;
        }

        LoadData();
    }

    private void LoadData()
    {
        if (currentCategorySelected == CategorySelectedType.Combat)
            switch (combatSubCurrentCategorySelected)
            {
                case CombatSubCategorySelectedType.Ability:
                    LoadAbilities();
                    LoadAbilityRankData();
                    break;
                case CombatSubCategorySelectedType.Effect:
                    LoadEffects();
                    break;
                case CombatSubCategorySelectedType.NPCs:
                    LoadNPCs();
                    break;
                case CombatSubCategorySelectedType.Stat:
                    LoadStats();
                    break;
                case CombatSubCategorySelectedType.TreePoint:
                    LoadTreePoints();
                    break;
            }
        else if (currentCategorySelected == CategorySelectedType.General)
            switch (generalSubCurrentCategorySelected)
            {
                case GeneralSubCategorySelectedType.Bonus:
                    LoadBonuses();
                    LoadBonusRankData();
                    break;
                case GeneralSubCategorySelectedType.Class:
                    LoadClasses();
                    break;
                case GeneralSubCategorySelectedType.CraftingRecipe:
                    LoadCraftingRecipes();
                    LoadRecipeRankData();
                    break;
                case GeneralSubCategorySelectedType.CraftingStation:
                    LoadCraftingStations();
                    break;
                case GeneralSubCategorySelectedType.Currency:
                    LoadCurrencies();
                    break;
                case GeneralSubCategorySelectedType.Item:
                    LoadItems();
                    break;
                case GeneralSubCategorySelectedType.LevelTemplate:
                    LoadLevelsTemplate();
                    break;
                case GeneralSubCategorySelectedType.LootTable:
                    LoadLootTables();
                    break;
                case GeneralSubCategorySelectedType.MerchantTable:
                    LoadMerchantTables();
                    break;
                case GeneralSubCategorySelectedType.Race:
                    LoadRaces();
                    break;
                case GeneralSubCategorySelectedType.Skill:
                    LoadSkills();
                    break;
                case GeneralSubCategorySelectedType.TalentTree:
                    LoadTalentTrees();
                    break;
            }
        else if (currentCategorySelected == CategorySelectedType.World)
            switch (worldSubCurrentCategorySelected)
            {
                case WorldSubCategorySelectedType.Task:
                    LoadTasks();
                    break;
                case WorldSubCategorySelectedType.Quest:
                    LoadQuests();
                    break;
                case WorldSubCategorySelectedType.WorldPosition:
                    LoadWorldPositions();
                    break;
                case WorldSubCategorySelectedType.ResourceNode:
                    LoadResourceNodes();
                    LoadResourceNodeRankData();
                    break;
            }
    }


    private void LoadMerchantTables()
    {
        allMerchantTables = Resources.LoadAll<RPGMerchantTable>("THMSV/RPGBuilderData/MerchantTables").ToList();
    }

    private void LoadAbilities()
    {
        allAbilities = Resources.LoadAll<RPGAbility>("THMSV/RPGBuilderData/Abilities").ToList();
    }

    private void LoadAbilityRankData()
    {
        allAbilityRanks = Resources.LoadAll<RPGAbilityRankData>("THMSV/RPGBuilderData/AbilityRankData").ToList();
    }

    private void LoadRecipeRankData()
    {
        allCraftingRecipeRanks = Resources.LoadAll<RPGCraftingRecipeRankData>("THMSV/RPGBuilderData/CraftingRecipeRankData").ToList();
    }

    private void LoadResourceNodeRankData()
    {
        allResourceNodeRanks = Resources.LoadAll<RPGResourceNodeRankData>("THMSV/RPGBuilderData/ResourceNodeRankData").ToList();
    }

    private void LoadBonusRankData()
    {
        allBonusRanks = Resources.LoadAll<RPGBonusRankDATA>("THMSV/RPGBuilderData/BonusRankData").ToList();
    }

    private void LoadGameScenes()
    {
        allGameScenes = Resources.LoadAll<RPGGameScene>("THMSV/RPGBuilderData/GameScenes").ToList();
    }

    private void LoadCraftingRecipes()
    {
        allCraftingRecipes = Resources.LoadAll<RPGCraftingRecipe>("THMSV/RPGBuilderData/CraftingRecipes").ToList();
    }

    private void LoadCraftingStations()
    {
        allCraftingStations = Resources.LoadAll<RPGCraftingStation>("THMSV/RPGBuilderData/CraftingStations").ToList();
    }

    private void LoadResourceNodes()
    {
        allResourceNodes = Resources.LoadAll<RPGResourceNode>("THMSV/RPGBuilderData/ResourceNodes").ToList();
    }

    private void LoadBonuses()
    {
        allBonuses = Resources.LoadAll<RPGBonus>("THMSV/RPGBuilderData/Bonuses").ToList();
    }

    private void LoadTasks()
    {
        allTasks = Resources.LoadAll<RPGTask>("THMSV/RPGBuilderData/Tasks").ToList();
    }

    private void LoadQuests()
    {
        allQuests = Resources.LoadAll<RPGQuest>("THMSV/RPGBuilderData/Quests").ToList();
    }

    private void LoadWorldPositions()
    {
        allWorldPositions = Resources.LoadAll<RPGWorldPosition>("THMSV/RPGBuilderData/WorldPositions").ToList();
    }

    private void LoadCurrencies()
    {
        allCurrencies = Resources.LoadAll<RPGCurrency>("THMSV/RPGBuilderData/Currencies").ToList();
    }

    private void LoadTreePoints()
    {
        allTreePoints = Resources.LoadAll<RPGTreePoint>("THMSV/RPGBuilderData/TreePoints").ToList();
    }

    private void LoadLootTables()
    {
        allLootTables = Resources.LoadAll<RPGLootTable>("THMSV/RPGBuilderData/LootTables").ToList();
    }

    private void LoadEffects()
    {
        allEffects = Resources.LoadAll<RPGEffect>("THMSV/RPGBuilderData/Effects").ToList();
    }

    private void LoadNPCs()
    {
        allNPCs = Resources.LoadAll<RPGNpc>("THMSV/RPGBuilderData/NPCs").ToList();
    }

    private void LoadStats()
    {
        allStats = Resources.LoadAll<RPGStat>("THMSV/RPGBuilderData/Stats").ToList();
    }

    private void LoadItems()
    {
        allItems = Resources.LoadAll<RPGItem>("THMSV/RPGBuilderData/Items").ToList();
    }

    private void LoadSkills()
    {
        allSkills = Resources.LoadAll<RPGSkill>("THMSV/RPGBuilderData/Skills").ToList();
    }

    private void LoadLevelsTemplate()
    {
        allLevelsTemplate = Resources.LoadAll<RPGLevelsTemplate>("THMSV/RPGBuilderData/LevelsTemplate").ToList();
    }

    private void LoadRaces()
    {
        allRaces = Resources.LoadAll<RPGRace>("THMSV/RPGBuilderData/Races").ToList();
    }

    private void LoadClasses()
    {
        allClasses = Resources.LoadAll<RPGClass>("THMSV/RPGBuilderData/Classes").ToList();
    }

    private void LoadTalentTrees()
    {
        allTalentTrees = Resources.LoadAll<RPGTalentTree>("THMSV/RPGBuilderData/TalentTrees").ToList();
    }
}

