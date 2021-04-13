using System.Collections.Generic;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

public class RPGBonusRankDATA : ScriptableObject
{
    public int ID = -1;
    public int unlockCost;

    public List<RequirementsManager.BonusRequirementDATA> activeRequirements =
        new List<RequirementsManager.BonusRequirementDATA>();

    public List<RPGBonus.BonusEffectsApplied> effectsApplied = new List<RPGBonus.BonusEffectsApplied>();

    public void copyData(RPGBonusRankDATA original, RPGBonusRankDATA copied)
    {
        original.activeRequirements = copied.activeRequirements;
        original.effectsApplied = copied.effectsApplied;
        original.unlockCost = copied.unlockCost;
    }

    public void updateThis(RPGBonusRankDATA newDATA)
    {
        ID = newDATA.ID;
        activeRequirements = newDATA.activeRequirements;
        effectsApplied = newDATA.effectsApplied;
        unlockCost = newDATA.unlockCost;
    }
}