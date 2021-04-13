using System;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New RPG Effect", menuName = "RPG BUILDER/Effect")]
public class RPGEffect : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public Sprite icon;

    public GameObject effectPulseGO;

    public enum EFFECT_TYPE
    {
        instantDamage,
        instantHeal,
        damageOverTime,
        healOverTime,
        stat,
        stun,
        sleep,
        immune,
        morph,
        dispel,
        teleport,
        taunt,
        root,
        silence,
        pet,
        reflect
    }

    public EFFECT_TYPE effectType;

    public enum MAIN_DAMAGE_TYPE
    {
        NONE,
        PHYSICAL_DAMAGE,
        MAGICAL_DAMAGE
    }

    public MAIN_DAMAGE_TYPE mainDamageType;

    public string secondaryDamageType;

    public bool isState, isBuffOnSelf;
    public int Damage;
    public float skillModifier;
    public int skillModifierID = -1;
    public RPGSkill skillModifierREF;
    public float lifesteal;
    public float maxHealthModifier;
    public float missingHealthModifier;
    public float UltimateGain;
    public float delay;
    public int stackLimit = 1;
    public bool allowMultiple, allowMixedCaster;
    public int pulses = 1;
    public float duration;

    public int projectilesReflectedCount;

    [Serializable]
    public class STAT_EFFECTS_DATA
    {
        public int statID;
        public RPGStat statREF;
        public float statEffectModification;
        public bool isPercent;
    }

    public List<STAT_EFFECTS_DATA> statEffectsData = new List<STAT_EFFECTS_DATA>();

    public enum TELEPORT_TYPE
    {
        gameScene,
        position,
        target
    }

    public TELEPORT_TYPE teleportType;
    public int gameSceneID = -1;
    public RPGGameScene gameSceneREF;
    public Vector3 teleportPOS;


    public enum PET_TYPE
    {
        combat
    }

    public PET_TYPE petType;
    public GameObject petPrefab;
    public float petDuration;
    public int petSPawnCount;
    public bool petScaleWithCharacter;


    public void updateThis(RPGEffect newEffectDATA)
    {
        ID = newEffectDATA.ID;
        _name = newEffectDATA._name;
        _fileName = newEffectDATA._fileName;
        icon = newEffectDATA.icon;

        effectPulseGO = newEffectDATA.effectPulseGO;

        effectType = newEffectDATA.effectType;

        mainDamageType = newEffectDATA.mainDamageType;
        secondaryDamageType = newEffectDATA.secondaryDamageType;
        isState = newEffectDATA.isState;
        isBuffOnSelf = newEffectDATA.isBuffOnSelf;
        Damage = newEffectDATA.Damage;
        lifesteal = newEffectDATA.lifesteal;
        maxHealthModifier = newEffectDATA.maxHealthModifier;
        missingHealthModifier = newEffectDATA.missingHealthModifier;
        UltimateGain = newEffectDATA.UltimateGain;
        delay = newEffectDATA.delay;
        stackLimit = newEffectDATA.stackLimit;
        allowMultiple = newEffectDATA.allowMultiple;
        allowMixedCaster = newEffectDATA.allowMixedCaster;
        pulses = newEffectDATA.pulses;
        duration = newEffectDATA.duration;

        statEffectsData = newEffectDATA.statEffectsData;

        teleportType = newEffectDATA.teleportType;
        gameSceneID = newEffectDATA.gameSceneID;
        teleportPOS = newEffectDATA.teleportPOS;

        petType = newEffectDATA.petType;
        petPrefab = newEffectDATA.petPrefab;
        petDuration = newEffectDATA.petDuration;
        petSPawnCount = newEffectDATA.petSPawnCount;
        projectilesReflectedCount = newEffectDATA.projectilesReflectedCount;
        petScaleWithCharacter = newEffectDATA.petScaleWithCharacter;
            
        skillModifier = newEffectDATA.skillModifier;
        skillModifierID = newEffectDATA.skillModifierID;
        displayName = newEffectDATA.displayName;
    }
}