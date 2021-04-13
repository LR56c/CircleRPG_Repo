using System;
using System.Collections.Generic;
using THMSV.RPGBuilder.CombatVisuals;
using UnityEngine;

public class RPGAbility : ScriptableObject
{
    [Header("-----BASE DATA-----")] public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public Sprite icon;
    public bool learnedByDefault;
    public bool isPlayerAutoAttack;

    public enum TARGET_TYPES
    {
        SELF,
        CONE,
        AOE,
        LINEAR,
        PROJECTILE,
        SQUARE,
        GROUND,
        GROUND_LEAP,
        TARGET_PROJECTILE,
        TARGET_INSTANT
    }

    public enum COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE
    {
        activate,
        completed
    }
    public enum COMBAT_VISUAL_EFFECT_TYPE
    {
        effect,
        animation
    }

    public enum ABILITY_TAGS
    {
        onHit,
        onKill
    }

    [Serializable]
    public class rankDATA
    {
        public int rankID = -1;
        public RPGAbilityRankData rankREF;
        public bool ShowedInEditor;
    }

    public List<rankDATA> ranks = new List<rankDATA>();

    [Serializable]
    public class AbilityEffectsApplied
    {
        public int effectID = -1;
        public float chance = 100f;
        public RPGEffect effectREF;
        public RPGCombatDATA.TARGET_TYPE target;
    }
    
    [Serializable]
    public class AbilityVisualData
    {
        public COMBAT_VISUAL_EFFECT_TYPE type;
        public COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE activationType;
        public CombatVisualEffect effect;
        public CombatVisualAnimation animation;
    }
    [Serializable]
    public class AbilityTagsData
    {
        public ABILITY_TAGS tag;
    }

    public void updateThis(RPGAbility newAbilityDATA)
    {
        ID = newAbilityDATA.ID;
        _name = newAbilityDATA._name;
        _fileName = newAbilityDATA._fileName;
        icon = newAbilityDATA.icon;

        ranks = newAbilityDATA.ranks;
        learnedByDefault = newAbilityDATA.learnedByDefault;
        isPlayerAutoAttack = newAbilityDATA.isPlayerAutoAttack;
        displayName = newAbilityDATA.displayName;
    }
}