using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;

namespace THMSV.RPGBuilder.Logic
{
    public static class StatCalculator
    {

        public static void ResetPlayerStatsAfterRespawn()
        {
            foreach (var t in CombatManager.playerCombatInfo.nodeStats)
                if (t.stat.statType == RPGStat.STAT_TYPE.VITALITY)
                    t.curValue = (int)t.curMaxValue;

            UpdateStatUI();
        }

        private static void UpdateStatUI()
        {
            CombatManager.Instance.StatBarUpdate("HEALTH");

            if (CharacterPanelDisplayManager.Instance.thisCG.alpha == 1)
                CharacterPanelDisplayManager.Instance.InitCharStats();
        }

        public static void UpdateClassLevelUpStats()
        {
            var thisClass = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);

            foreach (var t in thisClass.stats)
            foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
            {
                var statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                if (t1._name != statREF._name) continue;
                if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                {
                    t1.curMaxValue += t.bonusPerLevel;
                }
                else
                {
                    t1.curValue += t.bonusPerLevel;
                }
                ClampStat(statREF, CombatManager.playerCombatInfo);
                CombatManager.Instance.StatBarUpdate(t1._name);
            }

            UpdateStatUI();
        }


        public static void InitCharacterStats()
        {
            var race = RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID);
            var thisClass = RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID);


            foreach (var t in race.stats)
            foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
            {
                var statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                if (t1._name != statREF._name) continue;
                if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                {
                    t1.curMaxValue += t.amount;
                    t1.curValue = (int)t1.curMaxValue;
                }
                else
                {
                    t1.curValue += t.amount;
                }
                ClampStat(statREF, CombatManager.playerCombatInfo);
                CombatManager.Instance.StatBarUpdate(t1._name);
            }

            foreach (var t in thisClass.stats)
            foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
            {
                var statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                if (t1._name != statREF._name) continue;
                if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                {
                    t1.curMaxValue += t.amount;
                    t1.curValue = (int)t1.curMaxValue;

                    t1.curMaxValue += t.bonusPerLevel * CharacterData.Instance.classDATA.currentClassLevel;
                }
                else
                {
                    t1.curValue += t.amount;
                    t1.curValue += t.bonusPerLevel * CharacterData.Instance.classDATA.currentClassLevel;
                }
                ClampStat(statREF, CombatManager.playerCombatInfo);
                CombatManager.Instance.StatBarUpdate(t1._name);
            }

            UpdateStatUI();
        }

        public static void UpdateItemStats(RPGItem thisItem, int rdmItemID)
        {
            if (thisItem == null) return;
            foreach (var t in thisItem.stats)
            {
                foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
                {
                    var statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                    if (t1._name != statREF._name) continue;
                    float addedValue = t.amount;

                    if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                    {
                        if (t.isPercent)
                            addedValue = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name) *
                                         (t.amount / 100);
                        t1.curMaxValue += addedValue;
                    }
                    else
                    {
                        if (t.isPercent)
                            addedValue = CombatManager.playerCombatInfo.getCurrentValue(statREF._name) *
                                         (t.amount / 100);
                        t1.curValue += addedValue;
                    }

                    ClampStat(statREF, CombatManager.playerCombatInfo);
                    CombatManager.Instance.StatBarUpdate(t1._name);
                }
            }

            if (rdmItemID != -1)
            {
                int rdmItemIndex = RPGBuilderUtilities.getRandomPlayerOwnerItemIndexFromID(rdmItemID);
                foreach (var v in thisItem.randomStats)
                {
                    foreach (var t in RandomizedItemsData.Instance.allPlayerOwnedRandomItems[rdmItemIndex].randomStats)
                    {
                        if (v.statID != t.statID) continue;
                        foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
                        {
                            var statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                            if (t1._name != statREF._name) continue;
                            float addedValue = t.statValue;

                            if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                            {
                                if (v.isPercent)
                                    addedValue = CombatManager.playerCombatInfo.getCurrentMaxValue(statREF._name) *
                                                 (t.statValue / 100);
                                t1.curMaxValue += addedValue;
                            }
                            else
                            {
                                if (v.isPercent)
                                    addedValue = CombatManager.playerCombatInfo.getCurrentValue(statREF._name) *
                                                 (t.statValue / 100);
                                t1.curValue += addedValue;
                            }

                            ClampStat(statREF, CombatManager.playerCombatInfo);
                            CombatManager.Instance.StatBarUpdate(t1._name);
                        }
                    }
                }
            }

            UpdateStatUI();
        }

        public static void RemoveItemStats(RPGItem removedItem, int rdmItemID)
        {
            foreach (var t in removedItem.stats)
            {
                foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
                {
                    var statRef = RPGBuilderUtilities.GetStatFromID(t.statID);
                    if (t1._name != statRef._name) continue;
                    float removedValue = (int) t.amount;

                    float valueRef;
                    valueRef = statRef.statType == RPGStat.STAT_TYPE.VITALITY
                        ? CombatManager.playerCombatInfo.getCurrentMaxValue(statRef._name)
                        : CombatManager.playerCombatInfo.getCurrentValue(statRef._name);

                    if (t.isPercent)
                    {
                        var _decimal = 100 + t.amount;
                        _decimal /= 100;
                        removedValue = valueRef - valueRef / _decimal;
                    }

                    if (statRef.statType == RPGStat.STAT_TYPE.VITALITY)
                        t1.curMaxValue -= removedValue;
                    else
                        t1.curValue -= removedValue;

                    ClampStat(statRef, CombatManager.playerCombatInfo);
                    CombatManager.Instance.StatBarUpdate(t1._name);
                }
            }

            if (rdmItemID != -1)
            {
                int rdmItemIndex = RPGBuilderUtilities.getRandomPlayerOwnerItemIndexFromID(rdmItemID);
                foreach (var v in removedItem.randomStats)
                {
                    foreach (var t in RandomizedItemsData.Instance.allPlayerOwnedRandomItems[rdmItemIndex].randomStats)
                    {
                        if (v.statID != t.statID) continue;
                        foreach (var t1 in CombatManager.playerCombatInfo.nodeStats)
                        {
                            var statRef = RPGBuilderUtilities.GetStatFromID(t.statID);
                            if (t1._name != statRef._name) continue;
                            float removedValue = (int) t.statValue;

                            float valueRef;
                            valueRef = statRef.statType == RPGStat.STAT_TYPE.VITALITY
                                ? CombatManager.playerCombatInfo.getCurrentMaxValue(statRef._name)
                                : CombatManager.playerCombatInfo.getCurrentValue(statRef._name);

                            if (v.isPercent)
                            {
                                var _decimal = 100 + t.statValue;
                                _decimal /= 100;
                                removedValue = valueRef - valueRef / _decimal;
                            }

                            if (statRef.statType == RPGStat.STAT_TYPE.VITALITY)
                                t1.curMaxValue -= removedValue;
                            else
                                t1.curValue -= removedValue;

                            ClampStat(statRef, CombatManager.playerCombatInfo);
                            CombatManager.Instance.StatBarUpdate(t1._name);
                        }
                    }
                }
            }

            UpdateStatUI();
        }

        public static void ClampStat(RPGStat stat, CombatNode cbtNode)
        {
            if (stat.minCheck && cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curValue < cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curMinValue)
            {
                cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curValue = (int)cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curMinValue;
            }
            if (stat.maxCheck && cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curValue > cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curMaxValue)
            {
                cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curValue = (int)cbtNode.nodeStats[cbtNode.getStatIndexFromName(stat._name)].curMaxValue;
            }
        }
    }
}
