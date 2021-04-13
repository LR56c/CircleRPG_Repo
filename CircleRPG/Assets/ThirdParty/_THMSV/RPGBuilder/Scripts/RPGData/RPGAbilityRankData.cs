using System.Collections.Generic;
using THMSV.RPGBuilder.CombatVisuals;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

public class RPGAbilityRankData : ScriptableObject
{
    public int ID = -1;
    public int unlockCost;

    public float castTime;
    public bool castInRun;
    public bool castBarVisible = true;
    public bool canBeUsedStunned;

    public List<RequirementsManager.AbilityUseRequirementDATA> useRequirements =
        new List<RequirementsManager.AbilityUseRequirementDATA>();

    public RPGEffect requiredEffectCaster;
    public bool consumeRequiredEffectCaster;

    public RPGEffect requiredEffectTarget;
    public bool consumeRequiredEffectTarget;


    public RPGAbility.TARGET_TYPES targetType;

    public LayerMask hitLayers;
    public bool canHitSelf;
    public bool canHitAlly;
    public bool canHitNeutral;
    public bool canHitEnemy;


    public int MaxUnitHit;
    public float minRange;
    public float maxRange;

    public float standTimeDuration;
    public bool canRotateInStandTime;
    public float castSpeedSlowAmount;
    public float castSpeedSlowTime;
    public float castSpeedSlowRate;

    public float coneDegree;
    public int ConeHitCount;
    public float ConeHitInterval;
    public float AOERadius;
    public int AOEHitCount;
    public float AOEHitInterval;
    public float linearWidth;
    public float linearHeight;
    public float linearLength;
    public float projectileSpeed;
    public float projectileDistance;
    public float projectileAngleSpread;
    public float projectileCount;
    public float projectileDelay;
    public float projectileComeBackAfterTime;
    public float projectileComeBackSpeed;
    public float projectileNearbyUnitDistanceMax;
    public float projectileNearbyUnitMaxHit;
    public float squareWidth;
    public float squareLength;
    public float squareHeight;
    public float groundRadius;
    public float groundRange;
    public float groundHitTime;
    public int groundHitCount;
    public float groundHitInterval;

    public float channelTime;
    public float groundLeapDuration;
    public float groundLeapHeight;
    public float groundLeapSpeed;

    public float cooldown;
    public bool isGCD;
    public bool startCDOnActivate;

    public List<RPGAbility.AbilityEffectsApplied> effectsApplied = new List<RPGAbility.AbilityEffectsApplied>();

    public List<RPGAbility.AbilityVisualData> visualsData = new List<RPGAbility.AbilityVisualData>();
    public List<RPGAbility.AbilityTagsData> tagsData = new List<RPGAbility.AbilityTagsData>();

    public RPGAbility extraAbilityExecuted;
    public RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE extraAbilityExecutedActivationType;

    public void copyData(RPGAbilityRankData original, RPGAbilityRankData copied)
    {
        original.unlockCost = copied.unlockCost;
        original.castTime = copied.castTime;
        original.castInRun = copied.castInRun;
        original.castBarVisible = copied.castBarVisible;
        original.canBeUsedStunned = copied.canBeUsedStunned;
        original.useRequirements = copied.useRequirements;

        original.requiredEffectCaster = copied.requiredEffectCaster;
        original.consumeRequiredEffectCaster = copied.consumeRequiredEffectCaster;

        original.requiredEffectTarget = copied.requiredEffectTarget;
        original.consumeRequiredEffectTarget = copied.consumeRequiredEffectTarget;

        original.targetType = copied.targetType;
        original.hitLayers = copied.hitLayers;
        original.canHitAlly = copied.canHitAlly;
        original.canHitSelf = copied.canHitSelf;
        original.canHitNeutral = copied.canHitNeutral;
        original.canHitEnemy = copied.canHitEnemy;

        original.MaxUnitHit = copied.MaxUnitHit;
        original.minRange = copied.minRange;
        original.maxRange = copied.maxRange;

        original.standTimeDuration = copied.standTimeDuration;
        original.canRotateInStandTime = copied.canRotateInStandTime;
        original.castSpeedSlowAmount = copied.castSpeedSlowAmount;
        original.castSpeedSlowTime = copied.castSpeedSlowTime;
        original.castSpeedSlowRate = copied.castSpeedSlowRate;

        original.coneDegree = copied.coneDegree;
        original.ConeHitCount = copied.ConeHitCount;
        original.ConeHitInterval = copied.ConeHitInterval;
        original.AOERadius = copied.AOERadius;
        original.AOEHitCount = copied.AOEHitCount;
        original.AOEHitInterval = copied.AOEHitInterval;
        original.linearWidth = copied.linearWidth;
        original.linearHeight = copied.linearHeight;
        original.linearLength = copied.linearLength;
        original.projectileSpeed = copied.projectileSpeed;
        original.projectileDistance = copied.projectileDistance;
        original.projectileAngleSpread = copied.projectileAngleSpread;
        original.projectileCount = copied.projectileCount;
        original.projectileDelay = copied.projectileDelay;
        original.projectileComeBackAfterTime = copied.projectileComeBackAfterTime;
        original.projectileComeBackSpeed = copied.projectileComeBackSpeed;
        original.projectileNearbyUnitDistanceMax = copied.projectileNearbyUnitDistanceMax;
        original.projectileNearbyUnitMaxHit = copied.projectileNearbyUnitMaxHit;
        original.squareWidth = copied.squareWidth;
        original.squareLength = copied.squareLength;
        original.squareHeight = copied.squareHeight;
        original.groundRadius = copied.groundRadius;
        original.groundRange = copied.groundRange;
        original.groundHitTime = copied.groundHitTime;
        original.groundHitCount = copied.groundHitCount;
        original.groundHitInterval = copied.groundHitInterval;

        original.channelTime = copied.channelTime;
        original.groundLeapDuration = copied.groundLeapDuration;
        original.groundLeapHeight = copied.groundLeapHeight;
        original.groundLeapSpeed = copied.groundLeapSpeed;

        original.cooldown = copied.cooldown;
        original.isGCD = copied.isGCD;
        original.startCDOnActivate = copied.startCDOnActivate;

        original.effectsApplied = copied.effectsApplied;

        original.visualsData = copied.visualsData;

        original.extraAbilityExecuted = copied.extraAbilityExecuted;
        original.extraAbilityExecutedActivationType = copied.extraAbilityExecutedActivationType;
        
        original.tagsData = copied.tagsData;
    }

    public void updateThis(RPGAbilityRankData newAbilityDATA)
    {
        ID = newAbilityDATA.ID;
        unlockCost = newAbilityDATA.unlockCost;
        castTime = newAbilityDATA.castTime;
        castInRun = newAbilityDATA.castInRun;
        castBarVisible = newAbilityDATA.castBarVisible;
        canBeUsedStunned = newAbilityDATA.canBeUsedStunned;
        useRequirements = newAbilityDATA.useRequirements;

        requiredEffectCaster = newAbilityDATA.requiredEffectCaster;
        consumeRequiredEffectCaster = newAbilityDATA.consumeRequiredEffectCaster;

        requiredEffectTarget = newAbilityDATA.requiredEffectTarget;
        consumeRequiredEffectTarget = newAbilityDATA.consumeRequiredEffectTarget;

        targetType = newAbilityDATA.targetType;
        hitLayers = newAbilityDATA.hitLayers;
        canHitAlly = newAbilityDATA.canHitAlly;
        canHitSelf = newAbilityDATA.canHitSelf;
        canHitNeutral = newAbilityDATA.canHitNeutral;
        canHitEnemy = newAbilityDATA.canHitEnemy;

        MaxUnitHit = newAbilityDATA.MaxUnitHit;
        minRange = newAbilityDATA.minRange;
        maxRange = newAbilityDATA.maxRange;

        standTimeDuration = newAbilityDATA.standTimeDuration;
        canRotateInStandTime = newAbilityDATA.canRotateInStandTime;
        castSpeedSlowAmount = newAbilityDATA.castSpeedSlowAmount;
        castSpeedSlowTime = newAbilityDATA.castSpeedSlowTime;
        castSpeedSlowRate = newAbilityDATA.castSpeedSlowRate;

        coneDegree = newAbilityDATA.coneDegree;
        ConeHitCount = newAbilityDATA.ConeHitCount;
        ConeHitInterval = newAbilityDATA.ConeHitInterval;
        AOERadius = newAbilityDATA.AOERadius;
        AOEHitCount = newAbilityDATA.AOEHitCount;
        AOEHitInterval = newAbilityDATA.AOEHitInterval;
        linearWidth = newAbilityDATA.linearWidth;
        linearHeight = newAbilityDATA.linearHeight;
        linearLength = newAbilityDATA.linearLength;
        projectileSpeed = newAbilityDATA.projectileSpeed;
        projectileDistance = newAbilityDATA.projectileDistance;
        projectileAngleSpread = newAbilityDATA.projectileAngleSpread;
        projectileCount = newAbilityDATA.projectileCount;
        projectileDelay = newAbilityDATA.projectileDelay;
        projectileComeBackAfterTime = newAbilityDATA.projectileComeBackAfterTime;
        projectileComeBackSpeed = newAbilityDATA.projectileComeBackSpeed;
        projectileNearbyUnitDistanceMax = newAbilityDATA.projectileNearbyUnitDistanceMax;
        projectileNearbyUnitMaxHit = newAbilityDATA.projectileNearbyUnitMaxHit;
        squareWidth = newAbilityDATA.squareWidth;
        squareLength = newAbilityDATA.squareLength;
        squareHeight = newAbilityDATA.squareHeight;
        groundRadius = newAbilityDATA.groundRadius;
        groundRange = newAbilityDATA.groundRange;
        groundHitTime = newAbilityDATA.groundHitTime;
        groundHitCount = newAbilityDATA.groundHitCount;
        groundHitInterval = newAbilityDATA.groundHitInterval;

        channelTime = newAbilityDATA.channelTime;
        groundLeapDuration = newAbilityDATA.groundLeapDuration;
        groundLeapHeight = newAbilityDATA.groundLeapHeight;
        groundLeapSpeed = newAbilityDATA.groundLeapSpeed;

        cooldown = newAbilityDATA.cooldown;
        isGCD = newAbilityDATA.isGCD;
        startCDOnActivate = newAbilityDATA.startCDOnActivate;

        effectsApplied = newAbilityDATA.effectsApplied;


        visualsData = newAbilityDATA.visualsData;

        extraAbilityExecuted = newAbilityDATA.extraAbilityExecuted;
        extraAbilityExecutedActivationType = newAbilityDATA.extraAbilityExecutedActivationType;
        
        tagsData = newAbilityDATA.tagsData;
    }
}