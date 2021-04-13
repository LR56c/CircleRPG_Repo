using System.Collections.Generic;
using THMSV.RPGBuilder.Logic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UI;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class BonusManager : MonoBehaviour
    {
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static BonusManager Instance { get; private set; }

        public void InitBonuses()
        {
            foreach (var t in CharacterData.Instance.bonusesDATA)
                if (!t.On && RPGBuilderUtilities.isBonusKnown(t.bonusID)) InitBonus(RPGBuilderUtilities.GetBonusFromID(t.bonusID));
        }
    
        public void ResetAllOnBonuses()
        {
            foreach (var t in CharacterData.Instance.bonusesDATA) t.On = false;
        }


        private CharacterData.BONUSES_DATA getBonusDATAByBonus (RPGBonus bonus)
        {
            foreach (var t in CharacterData.Instance.bonusesDATA)
                if (t.bonusID == bonus.ID) return t;

            return null;
        }

        private void InitBonus(RPGBonus ab)
        {
            var bnsDATA = getBonusDATAByBonus(ab);
            if (!RPGBuilderUtilities.isBonusKnown(ab.ID) || bnsDATA.On) return;
            var curRank = 0;
            curRank = RPGBuilderUtilities.getNodeCurrentRank(ab);
            if (UseRequirementsMet(CombatManager.playerCombatInfo, ab, curRank)) HandleBonusActions(ab);
        }

        public bool UseRequirementsMet(CombatNode nodeCombatInfo, RPGBonus bonus, int curRank)
        {
            var rankREF = RPGBuilderUtilities.GetBonusRankFromID(bonus.ranks[curRank].rankID);
            foreach (var t in rankREF.activeRequirements)
            {
                var intValue1 = 0;
                switch (t.requirementType)
                {
                    case RequirementsManager.BonusRequirementType.classLevel:
                        intValue1 = CharacterData.Instance.classDATA.currentClassLevel;
                        break;
                    case RequirementsManager.BonusRequirementType.skillLevel:
                        intValue1 = RPGBuilderUtilities.getSkillLevel(t.skillRequiredID);
                        break;
                    case RequirementsManager.BonusRequirementType.pointSpent:
                        //intValue1 = RPGBuilderUtilities.getCombatTreePointSpentAmount(tree);
                        break;
                }
                if (!RequirementsManager.Instance.HandleBonusRequirementUseType(t, intValue1, false)) return false;
            }
            return true;
        }

        public void CancelBonus(RPGBonus ab, int curRank)
        {
            var rankREF = RPGBuilderUtilities.GetBonusRankFromID(ab.ranks[curRank].rankID);
            foreach (var t in rankREF.effectsApplied)
            {
                var effectREF = RPGBuilderUtilities.GetEffectFromID(t.effectID);
                if (effectREF.effectType != RPGEffect.EFFECT_TYPE.stat) continue;
                foreach (var t1 in effectREF.statEffectsData)
                {
                    var statREF = RPGBuilderUtilities.GetStatFromID(t1.statID);
                    var statDATA = t1;
                    float removedValue = (int) statDATA.statEffectModification;
                    if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                    {
                        if (statDATA.isPercent)
                        {
                            var _decimal = 100 + statDATA.statEffectModification;
                            _decimal /= 100;
                            var valueBeforeItem = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name) /
                                                  _decimal;
                            removedValue = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name) -
                                           valueBeforeItem;
                        }

                        CombatManager.playerCombatInfo
                            .nodeStats[CombatManager.playerCombatInfo.getStatIndexFromName(statREF._name)]
                            .curMaxValue -= removedValue;
                    }
                    else
                    {
                        if (statDATA.isPercent)
                        {
                            var _decimal = 100 + statDATA.statEffectModification;
                            _decimal /= 100;
                            var valueBeforeItem = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name) /
                                                  _decimal;
                            removedValue = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name) -
                                           valueBeforeItem;
                        }

                        var statIndex = CombatManager.playerCombatInfo.getStatIndexFromName(statREF._name);
                        CombatManager.playerCombatInfo.nodeStats[statIndex].curValue -= removedValue;
                    }

                    StatCalculator.ClampStat(statREF, CombatManager.playerCombatInfo);
                    CombatManager.Instance.StatBarUpdate(statREF._name);
                    if (CharacterPanelDisplayManager.Instance.thisCG.alpha == 1)
                        CharacterPanelDisplayManager.Instance.InitCharStats();
                }
            }

            AlterBonusState(ab, false);
        }

        private void AlterBonusState (RPGBonus bonus, bool isOn)
        {
            foreach (var bns in CharacterData.Instance.bonusesDATA)
                if(bns.bonusID == bonus.ID) bns.On = isOn;
        }

        public void CancelBonusFromUnequippedWeapon(string weaponType)
        {
            foreach (var t in CharacterData.Instance.bonusesDATA)
            {
                if (!t.On) continue;
                var bonusREF = RPGBuilderUtilities.GetBonusFromID(t.bonusID);
                var curRank = RPGBuilderUtilities.getNodeCurrentRank(bonusREF);
                if (bonusRequireThisWeaponType(weaponType, bonusREF, curRank)) CancelBonus(bonusREF, curRank);
            }
        }

        private bool bonusRequireThisWeaponType(string weaponType, RPGBonus ab, int curRank)
        {
            var rankREF = RPGBuilderUtilities.GetBonusRankFromID(ab.ranks[curRank].rankID);
            foreach (var t in rankREF.activeRequirements)
                if (t.requirementType == RequirementsManager.BonusRequirementType.weaponTypeEquipped
                    && t.weaponRequired == weaponType
                    && !RequirementsManager.Instance.isWeaponTypeEquipped(weaponType))
                    return true;
            return false;
        }

        public void HandleBonusActions(RPGBonus bonus)
        {
            var curRank = 0;
            curRank = RPGBuilderUtilities.getNodeCurrentRank(bonus);
            var rankREF = RPGBuilderUtilities.GetBonusRankFromID(bonus.ranks[curRank].rankID);

            for (var i = 0; i < rankREF.effectsApplied.Count; i++)
            {
                var effectREF = RPGBuilderUtilities.GetEffectFromID(rankREF.effectsApplied[i].effectID);
                if (effectREF.isState)
                    switch (effectREF.effectType)
                    {
                        case RPGEffect.EFFECT_TYPE.stat:
                            foreach (var t in effectREF.statEffectsData)
                            {
                                var statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                                float newValue = 0, modifiedValue = 0;
                                if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                                {
                                    newValue = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name);
                                    if (effectREF.statEffectsData[i].isPercent)
                                        modifiedValue = newValue * (effectREF.statEffectsData[i].statEffectModification / 100);
                                    else
                                        modifiedValue = effectREF.statEffectsData[i].statEffectModification;
                                    newValue += modifiedValue;
                                    CombatManager.playerCombatInfo.setCurrentMaxValue(statREF._name, newValue);
                                }
                                else
                                {
                                    newValue = CombatManager.playerCombatInfo.getCurrentValue(statREF._name);
                                    if (effectREF.statEffectsData[i].isPercent)
                                        modifiedValue = newValue * (effectREF.statEffectsData[i].statEffectModification / 100);
                                    else
                                        modifiedValue = effectREF.statEffectsData[i].statEffectModification;
                                    newValue += modifiedValue;
                                    CombatManager.playerCombatInfo.setCurrentValue(statREF._name, newValue);
                                }
                                CombatManager.Instance.StatBarUpdate(statREF._name);
                            }
                            break;
                    }
            }
            AlterBonusState(bonus, true);
        }


        public void RankDownBonus(RPGBonus bonus, RPGTalentTree tree)
        {
            foreach (var t in CharacterData.Instance.talentTrees)
            {
                if (tree.ID != t.treeID) continue;
                foreach (var t1 in t.nodes)
                {
                    if (t1.nodeData.bonusID != bonus.ID) continue;
                    if (t1.rank <= 0) continue;
                    if (!CheckBonusRankingDown(bonus, tree)) continue;
                    CancelBonus(bonus, t1.rank - 1);

                    var rankREF = RPGBuilderUtilities.GetBonusRankFromID(bonus.ranks[t1.rank - 1].rankID);
                    TreePointsManager.Instance.AddTreePoint(tree.treePointAcceptedID, rankREF.unlockCost);
                    t.pointsSpent -= rankREF.unlockCost;
                    t1.rank--;

                    if (t1.rank == 0)
                    {
                        t1.known = false;
                        CharacterData.Instance.bonusesDATA[CharacterData.Instance.getBonusIndex(bonus)].known = true;
                    }
                    else
                    {
                        InitBonus(bonus);
                    }
                    TreesDisplayManager.Instance.InitTree(tree);

                    AbilityTooltip.Instance.Hide();
                    TreesDisplayManager.Instance.HideRequirements();
                }
            }
        }


        public void RankUpBonus(RPGBonus bonus, RPGTalentTree tree)
        {
            for (var i = 0; i < CharacterData.Instance.talentTrees.Count; i++)
            {
                if (tree.ID != CharacterData.Instance.talentTrees[i].treeID) continue;
                for (var x = 0; x < CharacterData.Instance.talentTrees[i].nodes.Count; x++)
                {
                    if (bonus.ID != CharacterData.Instance.talentTrees[i].nodes[x].nodeData.bonusID) continue;
                    if (CharacterData.Instance.talentTrees[i].nodes[x].rank >= RPGBuilderUtilities
                        .GetBonusFromID(CharacterData.Instance.talentTrees[i].nodes[x].nodeData.bonusID).ranks
                        .Count) continue;
                    if (!CheckBonusRankingRequirements(bonus, tree, i, x)) continue;
                    var rankREF = RPGBuilderUtilities.GetBonusRankFromID(bonus.ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank].rankID);
                    TreePointsManager.Instance.RemoveTreePoint(tree.treePointAcceptedID, rankREF.unlockCost);
                    CharacterData.Instance.talentTrees[i].pointsSpent += rankREF.unlockCost;
                    CharacterData.Instance.talentTrees[i].nodes[x].rank++;
                    CharacterData.Instance.talentTrees[i].nodes[x].known = true;
                    CharacterData.Instance.bonusesDATA[CharacterData.Instance.getBonusIndex(bonus)].known = true;

                    TreesDisplayManager.Instance.InitTree(tree);

                    if (CharacterData.Instance.talentTrees[i].nodes[x].rank == 1)
                    {
                        CharacterEventsManager.Instance.BonusLearned(bonus);
                        InitBonus(bonus);
                    }
                    else if (CharacterData.Instance.talentTrees[i].nodes[x].rank > 1)
                    {
                        var previousRank = -1;
                        previousRank += CharacterData.Instance.talentTrees[i].nodes[x].rank - 1;
                        CancelBonus(bonus, previousRank);
                        InitBonus(bonus);
                    }

                    AbilityTooltip.Instance.Hide();
                    TreesDisplayManager.Instance.HideRequirements();
                }
            }
        }


        private bool CheckBonusRankingDown(RPGBonus bonus, RPGTalentTree tree)
        {
            foreach (var t in tree.nodeList)
            foreach (var t1 in t.requirements)
            {
                if (t1.requirementType != RequirementsManager.RequirementType.bonusKnown ||
                    t1.bonusRequiredID != bonus.ID || !RPGBuilderUtilities.isBonusKnown(bonus.ID) ||
                    RPGBuilderUtilities.getNodeCurrentRank(bonus) != 0) continue;
                ErrorEventsDisplayManager.Instance.ShowErrorEvent("Cannot unlearn a node that is required for others", 3);
                return false;
            }

            return true;
        }


        private bool CheckBonusRankingRequirements(RPGBonus bonus, RPGTalentTree tree, int i, int x)
        {
            var rankREF =
                RPGBuilderUtilities.GetBonusRankFromID(bonus.ranks[CharacterData.Instance.talentTrees[i].nodes[x].rank].rankID);
            if (CharacterData.Instance.getTreePointsAmountByPoint(tree.treePointAcceptedID) < rankREF.unlockCost)
            {
                //NOT ENOUGH POINTS
                ErrorEventsDisplayManager.Instance.ShowErrorEvent("Not enough points", 3);
                return false;
            }


            List<bool> reqResults = new List<bool>();
            foreach (var t in tree.nodeList[x].requirements)
            {
                var intValue1 = 0;
                switch (t.requirementType)
                {
                    case RequirementsManager.RequirementType.classLevel:
                        intValue1 = CharacterData.Instance.classDATA.currentClassLevel;
                        break;
                    case RequirementsManager.RequirementType.skillLevel:
                        intValue1 = RPGBuilderUtilities.getSkillLevel(t.skillRequiredID);
                        break;
                    case RequirementsManager.RequirementType.pointSpent:
                        intValue1 = RPGBuilderUtilities.getTreePointSpentAmount(tree);
                        break;
                    case RequirementsManager.RequirementType._class:
                        intValue1 = t.classRequiredID;
                        break;
                }
            
                reqResults.Add(RequirementsManager.Instance.HandleRequirementType(t, intValue1, true));
            }

            return !reqResults.Contains(false);
        }

    }
}
