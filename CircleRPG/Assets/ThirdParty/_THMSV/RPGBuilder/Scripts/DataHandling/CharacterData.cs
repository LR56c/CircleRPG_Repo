using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    private static CharacterData instance;

    public bool created;
    public int raceID = -1;
    public string CharacterName;
    public RPGRace.RACE_GENDER gender;
    public Vector3 position;
    public int currentGameSceneID = -1;

    [System.Serializable]
    public class KeybindDATA
    {
        public string keybindName;
        public KeyCode key;
    }

    public List<KeybindDATA> abilityKeybindsDATA = new List<KeybindDATA>();
    public List<KeybindDATA> itemKeybindsDATA = new List<KeybindDATA>();
    public List<KeybindDATA> UIKeybindsDATA = new List<KeybindDATA>();
    public List<KeybindDATA> controlsKeybindsDATA = new List<KeybindDATA>();

    [System.Serializable]
    public class ClassDATA
    {
        public int classID;
        public int currentClassLevel;
        public int currentClassXP, maxClassXP;

    }

    public ClassDATA classDATA;

    [System.Serializable]
    public class TalentTrees_DATA
    {
        public int treeID;
        public int pointsSpent;

        [System.Serializable]
        public class TalentTreeNode_DATA
        {
            public RPGTalentTree.Node_DATA nodeData;
            public int rank;
            public bool known;
            public float NextTimeUse, CDLeft;
        }

        public List<TalentTreeNode_DATA> nodes = new List<TalentTreeNode_DATA>();

    }

    public List<TalentTrees_DATA> talentTrees = new List<TalentTrees_DATA>();

    [System.Serializable]
    public class BONUSES_DATA
    {
        public int bonusID;
        public bool known;
        public bool On;
    }

    public List<BONUSES_DATA> bonusesDATA = new List<BONUSES_DATA>();

    [System.Serializable]
    public class ActionBarAbilitiesDATA
    {
        public int curAbID = -1;
    }

    public List<ActionBarAbilitiesDATA> actionBarAbilities = new List<ActionBarAbilitiesDATA>();

    [System.Serializable]
    public class ActionBarItemsDATA
    {
        public int curItemID = -1;
    }

    public List<ActionBarItemsDATA> actionBarItems = new List<ActionBarItemsDATA>();
    

    [System.Serializable]
    public class SkillsDATA
    {
        public int skillID;
        public int currentSkillLevel;
        public int currentSkillXP, maxSkillXP;
    }

    public List<SkillsDATA> skillsDATA = new List<SkillsDATA>();

    [System.Serializable]
    public class InventoryItemsDATA
    {
        public int itemID;
        public int itemStack;
        public int itemRandomID = -1;
    }

    public List<InventoryItemsDATA> inventoryItemsDATA = new List<InventoryItemsDATA>();

    [System.Serializable]
    public class ArmorsEquippedDATA
    {
        public int itemID = -1;
        public int randomItemID = -1;
    }
    public List<ArmorsEquippedDATA> armorsEquipped = new List<ArmorsEquippedDATA>();
    
    [System.Serializable]
    public class WeaponsEquippedDATA
    {
        public int itemID = -1;
        public int randomItemID = -1;
    }
    public List<WeaponsEquippedDATA> weaponsEquipped = new List<WeaponsEquippedDATA>();

    [System.Serializable]
    public class QuestDATA
    {
        public int questID;
        public QuestManager.questState state;

        [System.Serializable]
        public class Quest_ObjectiveDATA
        {
            public int taskID;
            public QuestManager.questObjectiveState state;
            public int currentProgressValue, maxProgressValue;
        }

        public List<Quest_ObjectiveDATA> objectives = new List<Quest_ObjectiveDATA>();
    }

    public List<QuestDATA> questsData = new List<QuestDATA>();

    [System.Serializable]
    public class TreePoints_DATA
    {
        public int treePointID;
        public int amount;
    }

    public List<TreePoints_DATA> treePoints = new List<TreePoints_DATA>();

    [System.Serializable]
    public class Currencies_DATA
    {
        public int currencyID;
        public int amount;
    }

    public List<Currencies_DATA> currencies = new List<Currencies_DATA>();

    [System.Serializable]
    public class NPC_KilledDATA
    {
        public int npcID;
        public int killedAmount;
    }

    public List<NPC_KilledDATA> npcKilled = new List<NPC_KilledDATA>();

    [System.Serializable]
    public class SCENE_EnteredDATA
    {
        public string sceneName;
    }

    public List<SCENE_EnteredDATA> scenesEntered = new List<SCENE_EnteredDATA>();

    [System.Serializable]
    public class REGION_EnteredDATA
    {
        public string regionName;
    }

    public List<REGION_EnteredDATA> regionsEntered = new List<REGION_EnteredDATA>();

    [System.Serializable]
    public class ABILITY_LearnedDATA
    {
        public int abilityID;
    }

    public List<ABILITY_LearnedDATA> abilitiesLearned = new List<ABILITY_LearnedDATA>();

    [System.Serializable]
    public class BONUS_LearnedDATA
    {
        public int bonusID;
    }

    public List<BONUS_LearnedDATA> bonusLearned = new List<BONUS_LearnedDATA>();

    [System.Serializable]
    public class RECIPE_LearnedDATA
    {
        public int recipeID;
    }

    public List<RECIPE_LearnedDATA> recipeslearned = new List<RECIPE_LearnedDATA>();

    [System.Serializable]
    public class RESOURCENODE_LearnedDATA
    {
        public int resourceNodeID;
    }

    public List<RESOURCENODE_LearnedDATA> resourcenodeslearned = new List<RESOURCENODE_LearnedDATA>();

    [System.Serializable]
    public class ITEM_GainedDATA
    {
        public int itemID;
    }

    public List<ITEM_GainedDATA> itemsGained = new List<ITEM_GainedDATA>();

    

    public static CharacterData Instance => instance;

    private void Start()
    {
        if (instance != null) return;
        instance = this;
    }

    public int getCurrencyAmount(RPGCurrency currency)
    {
        foreach (var t in currencies)
            if (t.currencyID == currency.ID)
                return t.amount;

        return -1;
    }

    public int getCurrencyIndex(RPGCurrency currency)
    {
        for (var i = 0; i < currencies.Count; i++)
            if (currencies[i].currencyID == currency.ID)
                return i;
        return -1;
    }

    public int getBonusIndex(RPGBonus bonus)
    {
        for (var i = 0; i < bonusesDATA.Count; i++)
            if (bonusesDATA[i].bonusID == bonus.ID)
                return i;
        return -1;
    }


    public BONUSES_DATA getBonusDataByBonus(RPGBonus bonus)
    {
        foreach (var t in bonusesDATA)
            if (t.bonusID == bonus.ID)
                return t;

        return null;
    }

    public TalentTrees_DATA.TalentTreeNode_DATA getTalentTreeNodeData(int ID)
    {
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.abilityID == ID)
                return t1;

        return null;
    }

    public TalentTrees_DATA.TalentTreeNode_DATA getTalentTreeNodeData(RPGCraftingRecipe ab)
    {
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.recipeID == ab.ID)
                return t1;

        return null;
    }

    public TalentTrees_DATA.TalentTreeNode_DATA getTalentTreeNodeData(RPGResourceNode ab)
    {
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.resourceNodeID == ab.ID)
                return t1;

        return null;
    }

    public TalentTrees_DATA.TalentTreeNode_DATA getTalentTreeNodeData(RPGBonus ab)
    {
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.bonusID == ab.ID)
                return t1;

        return null;
    }

    public int getTreePointsAmountByPoint(int ID)
    {
        foreach (var t in treePoints)
            if (t.treePointID == ID)
                return t.amount;

        return -1;
    }

    public QuestDATA getQuestDATA(RPGQuest quest)
    {
        foreach (var t in questsData)
            if (t.questID == quest.ID)
                return t;

        return null;
    }

    public int getQuestINDEX(RPGQuest quest)
    {
        for (var i = 0; i < questsData.Count; i++)
            if (questsData[i].questID == quest.ID)
                return i;
        return -1;
    }

    public bool isAbilityCDReady(RPGAbility ab)
    {
        if (ab.isPlayerAutoAttack)
            return CombatManager.playerCombatInfo.autoAttackIsReady();
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.nodeType == RPGTalentTree.TalentTreeNodeType.ability && t1.nodeData.abilityID == ab.ID)
                if (t1.NextTimeUse == 0)
                    return true;

        return false;
    }

    public void InitAbilityCooldown(int ID, float duration)
    {
        if (RPGBuilderUtilities.GetAbilityFromID(ID).isPlayerAutoAttack)
            CombatManager.playerCombatInfo.InitAACooldown(duration);
        else
            foreach (var t in talentTrees)
            foreach (var t1 in t.nodes)
                if (t1.nodeData.nodeType == RPGTalentTree.TalentTreeNodeType.ability && t1.nodeData.abilityID == ID)
                {
                    t1.NextTimeUse = duration;
                    t1.CDLeft = duration;
                }
    }

    public List<float> getAbilityCD(RPGAbility ab)
    {
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.nodeData.nodeType == RPGTalentTree.TalentTreeNodeType.ability && t1.nodeData.abilityID == ab.ID)
            {
                if (t1.NextTimeUse != 0)
                {
                    var cdDATA = new List<float>();
                    cdDATA.Add(t1.NextTimeUse);
                    cdDATA.Add(t1.CDLeft);
                    return cdDATA;
                }

                return null;
            }

        return null;
    }

    private void FixedUpdate()
    {
        if (Time.frameCount < 10)
            return;

        if (!RPGBuilderEssentials.Instance.isInGame) return;
        foreach (var t in talentTrees)
        foreach (var t1 in t.nodes)
            if (t1.NextTimeUse > 0)
            {
                t1.CDLeft -= Time.deltaTime;
                if (!(t1.CDLeft <= 0)) continue;
                t1.CDLeft = 0;
                t1.NextTimeUse = 0;
            }
    }

    public void RESET_CHARACTER_DATA(bool destroyEssentials)
    {
        created = false;
        raceID = -1;
        CharacterName = "";
        gender = RPGRace.RACE_GENDER.Male;
        position = Vector3.zero;
        currentGameSceneID = -1;

        abilityKeybindsDATA.Clear();
        itemKeybindsDATA.Clear();
        UIKeybindsDATA.Clear();
        controlsKeybindsDATA.Clear();
        
        classDATA.classID = -1;
        classDATA.currentClassLevel = -1;
        classDATA.currentClassLevel = 0;
        classDATA.maxClassXP = 0;

        armorsEquipped.Clear();
        weaponsEquipped.Clear();
        talentTrees.Clear();
        bonusesDATA.Clear();
        actionBarAbilities.Clear();
        actionBarItems.Clear();
        skillsDATA.Clear();
        inventoryItemsDATA.Clear();
        questsData.Clear();
        treePoints.Clear();
        currencies.Clear();
        npcKilled.Clear();
        scenesEntered.Clear();
        regionsEntered.Clear();
        abilitiesLearned.Clear();
        bonusLearned.Clear();
        recipeslearned.Clear();
        resourcenodeslearned.Clear();
        itemsGained.Clear();

        if (destroyEssentials) Destroy(RPGBuilderEssentials.Instance.gameObject);
    }
}
