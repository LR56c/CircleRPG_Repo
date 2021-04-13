using THMSV.RPGBuilder.Logic;
using THMSV.RPGBuilder.UI;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class CharacterEventsManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static CharacterEventsManager Instance { get; private set; }

        public void ClassLevelUp()
        {
            // trigger actions if current level is required or asked for something
            var classREF = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);
            TaskCheckerManager.Instance.CheckQuestObjectives(classREF, CharacterData.Instance.classDATA.currentClassLevel);
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();

            TreePointsManager.Instance.CheckIfClassLevelUpGainPoints(classREF);
            StatCalculator.UpdateClassLevelUpStats();
            PlayerInfoDisplayManager.Instance.UpdateLevelText();
        }

        public void SkillLevelUp(int skillID)
        {
            var skillREF = RPGBuilderUtilities.GetSkillFromID(skillID);
            // trigger actions if current level is required or asked for something
            TaskCheckerManager.Instance.CheckQuestObjectives(skillREF, RPGBuilderUtilities.getSkillLevel(skillREF.ID));
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();
            TreePointsManager.Instance.CheckIfSkillLevelUpGainPoints(skillREF);
        }

        public void ItemUsed(RPGItem item)
        {
            // trigger actions if item use was required or asked for something
        }
        public void ItemEquipped(RPGItem item)
        {
            // trigger actions if this item was equipped
        }

        public void TalkedToNPC(RPGNpc npc)
        {
            // trigger actions if talking to this npc was required or asked for something
            TaskCheckerManager.Instance.CheckQuestObjectives(npc, true);
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();
        }
        public void ItemGain(RPGItem item)
        {
            var currentIndex = itemGainedBefore(item);
            if (currentIndex == -1)
            {
                var newDATA = new CharacterData.ITEM_GainedDATA();
                newDATA.itemID = item.ID;
                CharacterData.Instance.itemsGained.Add(newDATA);
            }

            TaskCheckerManager.Instance.CheckQuestObjectives(item);
            TreePointsManager.Instance.CheckIfItemGainPoints(item);
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();
        }

        private int itemGainedBefore(RPGItem item)
        {
            for (var i = 0; i < CharacterData.Instance.itemsGained.Count; i++)
                if (CharacterData.Instance.itemsGained[i].itemID == item.ID) return i;
            return -1;
        }

        public void AbilityLearned(RPGAbility ab)
        {
            var currentIndex = abilitylearnedBefore(ab);
            if (currentIndex == -1)
            {
                var newDATA = new CharacterData.ABILITY_LearnedDATA();
                newDATA.abilityID = ab.ID;
                CharacterData.Instance.abilitiesLearned.Add(newDATA);
            }
            TaskCheckerManager.Instance.CheckQuestObjectives(ab);
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();
        }

        public void BonusLearned(RPGBonus bonus)
        {
            var currentIndex = bonuslearnedBefore(bonus);
            if (currentIndex != -1) return;
            var newDATA = new CharacterData.BONUS_LearnedDATA();
            newDATA.bonusID = bonus.ID;
            CharacterData.Instance.bonusLearned.Add(newDATA);
            //TaskCheckerManager.Instance.CheckQuestObjectives(ab);
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();
        }

        public void RecipeLearned(RPGCraftingRecipe ab)
        {
            var currentIndex = recipelearnedBefore(ab);
            if (currentIndex != -1) return;
            var newDATA = new CharacterData.RECIPE_LearnedDATA();
            newDATA.recipeID = ab.ID;
            CharacterData.Instance.recipeslearned.Add(newDATA);
            //CheckQuestObjectives(ab);
            ScreenSpaceNameplates.Instance.TriggerNameplateInteractionIconUpdate();
        }

        private int recipelearnedBefore(RPGCraftingRecipe ab)
        {
            for (var i = 0; i < CharacterData.Instance.abilitiesLearned.Count; i++)
                if (CharacterData.Instance.abilitiesLearned[i].abilityID == ab.ID) return i;
            return -1;
        }

        public void ResourceNodeLearned(RPGResourceNode ab)
        {
            var currentIndex = resourceNodeLearnedBefore(ab);
            if (currentIndex != -1) return;
            var newDATA = new CharacterData.RESOURCENODE_LearnedDATA();
            newDATA.resourceNodeID = ab.ID;
            CharacterData.Instance.resourcenodeslearned.Add(newDATA);
            //CheckQuestObjectives(ab);
        }

        private int resourceNodeLearnedBefore(RPGResourceNode ab)
        {
            for (var i = 0; i < CharacterData.Instance.abilitiesLearned.Count; i++)
                if (CharacterData.Instance.abilitiesLearned[i].abilityID == ab.ID) return i;
            return -1;
        }

        private int abilitylearnedBefore(RPGAbility ab)
        {
            for (var i = 0; i < CharacterData.Instance.abilitiesLearned.Count; i++)
                if (CharacterData.Instance.abilitiesLearned[i].abilityID == ab.ID) return i;
            return -1;
        }

        private int bonuslearnedBefore(RPGBonus bonus)
        {
            for (var i = 0; i < CharacterData.Instance.bonusLearned.Count; i++)
                if (CharacterData.Instance.bonusLearned[i].bonusID == bonus.ID) return i;
            return -1;
        }

        public void SceneEntered(string _name)
        {
            var currentIndex = sceneAlreadyEnteredBefore(_name);
            if (currentIndex == -1)
            {
                var newDATA = new CharacterData.SCENE_EnteredDATA();
                newDATA.sceneName = _name;
                CharacterData.Instance.scenesEntered.Add(newDATA);
            }
            TaskCheckerManager.Instance.CheckQuestObjectives(_name);
        }

        private int sceneAlreadyEnteredBefore(string sceneName)
        {
            for (var i = 0; i < CharacterData.Instance.scenesEntered.Count; i++)
                if (CharacterData.Instance.scenesEntered[i].sceneName == sceneName) return i;
            return -1;
        }
        public void RegionEntered(string _name)
        {
            var currentIndex = regionAlreadyEnteredBefore(_name);
            if (currentIndex != -1) return;
            var newDATA = new CharacterData.REGION_EnteredDATA();
            newDATA.regionName = _name;
            CharacterData.Instance.regionsEntered.Add(newDATA);
        }

        private int regionAlreadyEnteredBefore(string _name)
        {
            for (var i = 0; i < CharacterData.Instance.regionsEntered.Count; i++)
                if (CharacterData.Instance.regionsEntered[i].regionName == _name) return i;
            return -1;
        }

        public void NPCKilled(RPGNpc npc)
        {
            var currentIndex = npcIsAlreadyKilledBefore(npc);
            if (currentIndex != -1)
            {
                CharacterData.Instance.npcKilled[currentIndex].killedAmount++;
            }
            else
            {
                var newDATA = new CharacterData.NPC_KilledDATA();
                newDATA.npcID = npc.ID;
                newDATA.killedAmount = 1;
                CharacterData.Instance.npcKilled.Add(newDATA);
            }
            TaskCheckerManager.Instance.CheckQuestObjectives(npc);
        }


        private int npcIsAlreadyKilledBefore(RPGNpc npc)
        {
            for (var i = 0; i < CharacterData.Instance.npcKilled.Count; i++)
                if (CharacterData.Instance.npcKilled[i].npcID == npc.ID) return i;
            return -1;
        }
    }
}
