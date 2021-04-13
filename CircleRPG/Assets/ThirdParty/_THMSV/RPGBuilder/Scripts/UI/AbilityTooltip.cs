using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UI
{
    public class AbilityTooltip : MonoBehaviour
    {
        public CanvasGroup thisCG;


        public TextMeshProUGUI abilityNameText;
        public Image icon;
        private RPGAbility lastAbility;

        public Transform contentParent;
        private readonly List<GameObject> curElementsGO = new List<GameObject>();

        public GameObject CenteredTitleElementPrefab, DescriptionElementPrefab, SeparationElementPrefan;
        [SerializeField] private Color physicalDamageColor, magicalDamageColor, healingColor, durationColor;
        private string endColorString = "</color>";
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }


        public void Show(RPGAbility ability)
        {
            lastAbility = ability;
            RPGBuilderUtilities.EnableCG(thisCG);

            abilityNameText.text = ability.displayName;
            icon.sprite = ability.icon;

            GenerateTooltip(ability);
        }
        public void ShowBonus(RPGBonus bonus)
        {
            //lastAbility = ability;
            RPGBuilderUtilities.EnableCG(thisCG);

            abilityNameText.text = bonus._name;
            icon.sprite = bonus.icon;

            GenerateBonusTooltip(bonus);
        }

        private void GenerateBonusTooltip(RPGBonus bonus)
        {
            ClearAllAbilityTooltipElements();
            var curRank = RPGBuilderUtilities.getNodeCurrentRank(bonus);
            if (curRank == -1) curRank = 0;
            var bonusRank = RPGBuilderUtilities.GetBonusRankFromID(bonus.ranks[curRank].rankID);

            var description = "";
            
            foreach (var t in bonusRank.effectsApplied)
                description +=
                    generateEffectDescription(
                        RPGBuilderUtilities.GetEffectFromID(t.effectID), true);

            SpawnAbilityTooltipElement(AbilityTooltipElement.ABILITY_TOOLTIP_ELEMENT_TYPE.Description, description);
        }
        
        public void Hide()
        {
            RPGBuilderUtilities.DisableCG(thisCG);
        }

        private void Awake()
        {
            Hide();
        }


        private void ClearAllAbilityTooltipElements()
        {
            foreach (var t in curElementsGO)
                Destroy(t);

            curElementsGO.Clear();
        }

        string getColorHEX(Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
        }

        private void GenerateTooltip(RPGAbility ab)
        {
            ClearAllAbilityTooltipElements();
            var curRank = RPGBuilderUtilities.getNodeCurrentRank(ab);
            if (curRank == -1) curRank = 0;
            var abilityRankID = RPGBuilderUtilities.GetAbilityRankFromID(ab.ranks[curRank].rankID);

            var description = "";

            if (abilityRankID.castTime > 0)
            {
                description += $"After {getColorHEX(durationColor)}{abilityRankID.castTime}s{endColorString}, ";
            }
            
            foreach (var t in abilityRankID.effectsApplied)
                description +=
                    generateEffectDescription(
                        RPGBuilderUtilities.GetEffectFromID(t.effectID), false);

            SpawnAbilityTooltipElement(AbilityTooltipElement.ABILITY_TOOLTIP_ELEMENT_TYPE.Description, description);
        }

        private void SpawnAbilityTooltipElement(AbilityTooltipElement.ABILITY_TOOLTIP_ELEMENT_TYPE elementType, string Text)
        {
            switch (elementType)
            {
                case AbilityTooltipElement.ABILITY_TOOLTIP_ELEMENT_TYPE.CenteredTitle:

                    break;

                case AbilityTooltipElement.ABILITY_TOOLTIP_ELEMENT_TYPE.Description:
                    var newElement = Instantiate(DescriptionElementPrefab, contentParent);
                    var elementRef = newElement.GetComponent<AbilityTooltipElement>();
                    elementRef.InitDescription(Text, Color.white);
                    curElementsGO.Add(newElement);
                    break;
            }
        }

        private string getBaseDamageType(RPGEffect effect)
        {
            var baseDamageType = "";
            switch (effect.mainDamageType)
            {
                case RPGEffect.MAIN_DAMAGE_TYPE.PHYSICAL_DAMAGE:
                    baseDamageType = getColorHEX(physicalDamageColor);
                    break;
                case RPGEffect.MAIN_DAMAGE_TYPE.MAGICAL_DAMAGE:
                    baseDamageType = getColorHEX(magicalDamageColor);
                    break;
            }
            return baseDamageType;
        }

        private string getSecondaryDamageType(RPGEffect effect)
        {
            var secondaryDamageType = "";
            if (effect.secondaryDamageType != "NONE") secondaryDamageType = effect.secondaryDamageType;

            foreach (var t in RPGBuilderEssentials.Instance.allStats)
                if (t._name == effect.secondaryDamageType)
                    secondaryDamageType = t.displayName;

            return secondaryDamageType;
        }

        private string breakLine(string text)
        {
            return text + "\n";
        }

        bool isStatTypePercent(RPGStat stat)
        {
            return stat.statType == RPGStat.STAT_TYPE.THORN || stat.statType == RPGStat.STAT_TYPE.CC_POWER ||
                   stat.statType == RPGStat.STAT_TYPE.CRIT_CHANCE ||
                   stat.statType == RPGStat.STAT_TYPE.DMG_DEALT || stat.statType == RPGStat.STAT_TYPE.DMG_TAKEN ||
                   stat.statType == RPGStat.STAT_TYPE.HEAL_DONE ||
                   stat.statType == RPGStat.STAT_TYPE.CAST_SPEED || stat.statType == RPGStat.STAT_TYPE.LIFESTEAL ||
                   stat.statType == RPGStat.STAT_TYPE.ABSORBTION ||
                   stat.statType == RPGStat.STAT_TYPE.RESISTANCE || stat.statType == RPGStat.STAT_TYPE.PENETRATION ||
                   stat.statType == RPGStat.STAT_TYPE.CC_RESISTANCE ||
                   stat.statType == RPGStat.STAT_TYPE.HEAL_RECEIVED || stat.statType == RPGStat.STAT_TYPE.CD_RECOVERY_SPEED;
        }

        private string generateEffectDescription(RPGEffect effect, bool isBonus)
        {
            var effectDescription = "";
            var baseDamageType = "";
            var secondaryDamageType = "";
            var pulseDmg = 0;
            float delay = 0;

            switch (effect.effectType)
            {
                case RPGEffect.EFFECT_TYPE.instantDamage:
                    baseDamageType = getBaseDamageType(effect);
                    secondaryDamageType = getSecondaryDamageType(effect);

                    effectDescription = $"Deal {baseDamageType}{effect.Damage}{endColorString} damage. ";
                    if (secondaryDamageType != "") effectDescription += $"({secondaryDamageType})";
                    break;
                case RPGEffect.EFFECT_TYPE.instantHeal:
                    secondaryDamageType = getSecondaryDamageType(effect);

                    effectDescription = $"Heal for {effect.Damage} {baseDamageType}. ";
                    if (secondaryDamageType != "") effectDescription += $"({secondaryDamageType})";
                    break;
                case RPGEffect.EFFECT_TYPE.damageOverTime:
                    secondaryDamageType = getSecondaryDamageType(effect);
                    baseDamageType = getBaseDamageType(effect);
                    pulseDmg = effect.Damage / effect.pulses;
                    delay = effect.duration / effect.pulses;
                    effectDescription =
                        $"Deal {baseDamageType}{pulseDmg}{endColorString} damage every {getColorHEX(durationColor)}{delay:F1}s{endColorString} for {getColorHEX(durationColor)}{effect.duration}s{endColorString}. ";
                    if (secondaryDamageType != "") effectDescription += $"({secondaryDamageType})";
                    break;
                case RPGEffect.EFFECT_TYPE.healOverTime:
                    secondaryDamageType = getSecondaryDamageType(effect);
                    pulseDmg = effect.Damage / effect.pulses;
                    delay = effect.duration / effect.pulses;
                    effectDescription = $"Heal for {pulseDmg} every {delay:F1}s for {effect.duration}s. ";
                    if (secondaryDamageType != "") effectDescription += $"({secondaryDamageType})";
                    break;
                case RPGEffect.EFFECT_TYPE.stun:
                    effectDescription = $"Stun the target for {effect.duration}s. ";
                    break;
                case RPGEffect.EFFECT_TYPE.stat:
                    effectDescription = "";
                    if(isBonus)
                        effectDescription = "Permanently:";
                    else 
                        effectDescription = $"For the next {effect.duration}s:";
                    
                    foreach (var t in effect.statEffectsData)
                    {
                        string modifierText = "";
                        float statValue = t.statEffectModification;
                        if (t.statEffectModification > 0)
                        {
                            modifierText = "Increase";
                        }
                        else
                        {
                            modifierText = "Reduce";
                            statValue = Mathf.Abs(statValue);
                        }

                        string addText = $"{statValue}";
                        RPGStat statREF = RPGBuilderUtilities.GetStatFromID(t.statID);
                        if (t.isPercent || isStatTypePercent(statREF))
                        {
                            addText += " %";
                        }


                        effectDescription +=
                            $"\n{modifierText} {statREF.displayName} by {addText}";
                    }

                    break;
                case RPGEffect.EFFECT_TYPE.sleep:
                    effectDescription = $"Sleep the target for {effect.duration}s. ";
                    break;
                case RPGEffect.EFFECT_TYPE.teleport:
                    switch (effect.teleportType)
                    {
                        case RPGEffect.TELEPORT_TYPE.gameScene:
                            effectDescription =
                                $"Teleportation to {RPGBuilderUtilities.GetGameSceneFromID(effect.gameSceneID).displayName}. ";
                            break;
                        case RPGEffect.TELEPORT_TYPE.position:
                            effectDescription = "Teleportation to a coordinate in the map. ";
                            break;
                        case RPGEffect.TELEPORT_TYPE.target:
                            effectDescription = "Teleport to the target. ";
                            break;
                    }

                    break;
                case RPGEffect.EFFECT_TYPE.pet:
                    string durationText = $" for {effect.petDuration}.";
                    if (effect.petDuration == 0)
                    {
                        durationText = ".";
                    }

                    effectDescription =
                        $"Summon {effect.petSPawnCount} {effect.petPrefab.GetComponent<CombatNode>().npcDATA._name}{durationText}";
                    break;
            }

            effectDescription = breakLine(effectDescription);
            return effectDescription;
        }

        public static AbilityTooltip Instance { get; private set; }
    }
}