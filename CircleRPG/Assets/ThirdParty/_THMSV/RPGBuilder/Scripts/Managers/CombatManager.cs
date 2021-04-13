using System.Collections;
using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.AI;
using THMSV.RPGBuilder.Character;
using THMSV.RPGBuilder.CombatVisuals;
using THMSV.RPGBuilder.DisplayHandler;
using THMSV.RPGBuilder.Logic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UI;
using THMSV.RPGBuilder.UIElements;
using THMSV.RPGBuilder.World;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class CombatManager : MonoBehaviour
    {
        public LayerMask ProjectileCheckLayer;
        public LayerMask ProjectileDestroyLayer;
        public List<CombatNode> allCombatNodes = new List<CombatNode>();
        public static GroundIndicatorManager groundIndicatorManager;
        public static CombatNode playerCombatInfo;
        public bool playerIsGroundCasting;
        public float NPC_GCD_DURATION;
        private StatBarDisplayHandler[] allStatsBar;
        private EXPBarDisplayHandler[] allExpBar;
        public List<GraveyardHandler> allGraveyards = new List<GraveyardHandler>();
        public CombatNode currentTarget;
        public CanvasGroup deathPopupCG;
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;

            allStatsBar = FindObjectsOfType<StatBarDisplayHandler>();
            allExpBar = FindObjectsOfType<EXPBarDisplayHandler>();
        }

        private void RemoveCombatNodeFromThreatTables(CombatNode cbtNode)
        {
            foreach (var t in allCombatNodes)
                if (t != cbtNode &&
                    (t.nodeType == CombatNode.COMBAT_NODE_TYPE.pet || t.nodeType == CombatNode.COMBAT_NODE_TYPE.mob)
                    && t.agentREF != null)
                    t.agentREF.RemoveCombatNodeFromThreatTabble(cbtNode);
        }

        private void DestroyDeadNodeCombatEntities(CombatNode cbtNode)
        {
            CombatVisualEffect[] entities = FindObjectsOfType<CombatVisualEffect>();
            foreach (var t in entities)
            {
                if (t.OwnerNode != cbtNode) continue;
                foreach (var t1 in t.allGOs)
                {
                    Destroy(t1);
                }
                Destroy(t.gameObject);
            }
        }

        public void HandleCombatNodeDEATH(CombatNode cbtNode)
        {
            RemoveCombatNodeFromThreatTables(cbtNode);
            cbtNode.InterruptCastActions();
            DestroyDeadNodeCombatEntities(cbtNode);

            // CLEARING ALL STATES COMING FROM THE DEAD NODE ON OTHER NODES
            foreach (var t in allCombatNodes)
            {
                for (var x = 0; x < t.nodeStateData.Count; x++)
                {
                    if (t.nodeStateData[x].casterNode == cbtNode)
                    {
                        t.CancelState(x);
                    }
                }
            }

            // CLEARING ALL STATES FOR THE DEAD NODE
            for (var x = 0; x < cbtNode.nodeStateData.Count; x++)
            {
                cbtNode.CancelState(x);
            }

            if (cbtNode == currentTarget)
            {
                TargetInfoDisplayManager.Instance.ResetTarget();
            }

            switch (cbtNode.nodeType)
            {
                case CombatNode.COMBAT_NODE_TYPE.mob:
                case CombatNode.COMBAT_NODE_TYPE.pet:
                {
                    if (cbtNode.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                        !playerCombatInfo.currentPets.Contains(cbtNode))
                    {
                        LevelingManager.Instance.GenerateMobEXP(cbtNode.npcDATA, cbtNode);
                        InventoryManager.Instance.GenerateDroppedLoot(cbtNode.npcDATA, cbtNode);
                        CharacterEventsManager.Instance.NPCKilled(cbtNode.npcDATA);
                    }

                    if (cbtNode.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                    {
                        cbtNode.ownerCombatInfo.currentPets.Remove(cbtNode);
                        if (cbtNode.ownerCombatInfo == playerCombatInfo)
                        {
                            if (cbtNode.ownerCombatInfo.currentPets.Count == 0)
                            {
                                PetPanelDisplayManager.Instance.Hide();
                            }
                            else
                            {
                                PetPanelDisplayManager.Instance.UpdateSummonCountText();
                                PetPanelDisplayManager.Instance.UpdateHealthBar();
                            }
                        }
                    }
                    
                    foreach (var t in cbtNode.currentPets)
                    {
                        if (allCombatNodes.Contains(t))
                            allCombatNodes.Remove(t);
                        Destroy(t.gameObject);
                    }

                    if (allCombatNodes.Contains(cbtNode)) allCombatNodes.Remove(cbtNode);

                    Destroy(cbtNode.gameObject);
                    break;
                }
                case CombatNode.COMBAT_NODE_TYPE.player:
                {
                    if (playerCombatInfo.playerControllerREF.CurrentController ==
                        RPGBCharacterController.ControllerType.ThirdPerson &&
                        !playerCombatInfo.playerControllerREF.ClickToRotate)
                    {
                        playerCombatInfo.playerControllerREF.ClickToRotate = true;
                    }
                    playerCombatInfo.playerControllerREF.InitDeath();
                    
                    for (var i = 0; i < cbtNode.currentPets.Count; i++)
                    {
                        if (allCombatNodes.Contains(cbtNode.currentPets[i]))
                            allCombatNodes.Remove(cbtNode.currentPets[i]);
                        Destroy(cbtNode.currentPets[i].gameObject);

                        cbtNode.currentPets.Remove(cbtNode.currentPets[i]);
                        if (cbtNode.currentPets.Count == 0)
                        {
                            PetPanelDisplayManager.Instance.Hide();
                        }
                        else
                        {
                            PetPanelDisplayManager.Instance.UpdateSummonCountText();
                            PetPanelDisplayManager.Instance.UpdateHealthBar();
                        }
                    }

                    LootPanelDisplayManager.Instance.Hide();
                    MerchantPanelDisplayManager.Instance.Hide();
                    QuestInteractionDisplayManager.Instance.Hide();
                    
                    RPGBuilderUtilities.EnableCG(deathPopupCG);
                    break;
                }
            }
        }



        public void SetPlayerTarget(CombatNode cbtNode)
        {
            if (cbtNode == null) return;
            if (Cursor.lockState == CursorLockMode.Locked && cbtNode == playerCombatInfo) return;
            currentTarget = cbtNode;
            TargetInfoDisplayManager.Instance.InitTargetUI(currentTarget);
            ScreenSpaceNameplates.Instance.SetNPToFocused(currentTarget);
        }

        public void ResetPlayerTarget()
        {
            currentTarget = null;
            TargetInfoDisplayManager.Instance.ResetTarget();
            ScreenSpaceNameplates.Instance.SetNPToFocused(null);
        }

        public void HandleHealthStatChange(CombatNode cbtNode)
        {
            if (cbtNode == currentTarget) TargetInfoDisplayManager.Instance.UpdateTargetHealthBar();

            switch (cbtNode.nodeType)
            {
                case CombatNode.COMBAT_NODE_TYPE.mob:
                case CombatNode.COMBAT_NODE_TYPE.pet:
                    ScreenSpaceNameplates.Instance.UpdateNPBar(cbtNode);
                    if (cbtNode.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                        if (playerCombatInfo.currentPets.Contains(cbtNode))
                            PetPanelDisplayManager.Instance.UpdateHealthBar();
                    if (cbtNode.npcDATA.npcType == RPGNpc.NPC_TYPE.BOSS)
                    {
                        if (BossUISlotHolder.Instance.thisNode == cbtNode)
                        {
                            BossUISlotHolder.Instance.UpdateHealth();
                        }
                    }

                    break;

                case CombatNode.COMBAT_NODE_TYPE.player:
                    StatBarUpdate("HEALTH");
                    break;
            }
        }

        public void InitAllStatBar()
        {
            foreach (var t in allStatsBar)
                t.UpdateBar();

            foreach (var t in allExpBar)
                t.UpdateBar();
        }

        public void StatBarUpdate(string statName)
        {
            foreach (var t in allStatsBar)
                if (t.STAT_NAME == statName)
                    t.UpdateBar();
        }

        public void EXPBarUpdate()
        {
            foreach (var t in allExpBar)
                if (t.expBarType == EXPBarDisplayHandler.EXPBar_Type.ClassEXP)
                    t.UpdateBar();
        }

        public void EXPBarUpdate(RPGSkill _skill)
        {
            foreach (var t in allExpBar)
                if (t._skill == _skill)
                    t.UpdateBar();
        }

        public static CombatManager Instance { get; private set; }

        public bool LayerContains(LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        private void ResetAnAbility(int abilityIndex, int slotIndex)
        {
            playerCombatInfo.abilitiesData[abilityIndex].currentAbility = null;
            playerCombatInfo.abilitiesData[abilityIndex].curAbilityID = -1;
            ActionBarDisplayManager.Instance.abilitySlots[slotIndex].curAb = null;
        }

        private bool isGraveyardAccepted(GraveyardHandler graveyard)
        {
            if (graveyard.requiredClasses.Count <= 0)
                return graveyard.requiredRaces.Count <= 0 ||
                       graveyard.requiredRaces.Contains(
                           RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID));
            if (!graveyard.requiredClasses.Contains(
                RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID)))
                return false;
            return graveyard.requiredRaces.Count <= 0 || graveyard.requiredRaces.Contains(RPGBuilderUtilities.GetRaceFromID(CharacterData.Instance.raceID));
        }

        public void HandleCombatAction(CombatNode cbtNode)
        {
            if (!cbtNode.inCombat)
            {
                cbtNode.inCombat = true;
                if(cbtNode == playerCombatInfo)ErrorEventsDisplayManager.Instance.ShowErrorEvent("Entered Combat", 3);
            }
            playerCombatInfo.appearanceREF.UpdateWeaponStates(cbtNode.inCombat);
            cbtNode.lastCombatActionTimer = 0;
        }

        public void ResetCombat(CombatNode cbtNode)
        {
            if (cbtNode == null) return;
            cbtNode.inCombat = false;
            cbtNode.lastCombatActionTimer = 0;
            if (cbtNode == playerCombatInfo)
            {
                ErrorEventsDisplayManager.Instance.ShowErrorEvent("Out of combat", 3);
                playerCombatInfo.appearanceREF.UpdateWeaponStates(cbtNode.inCombat);
            }
            else
            {
                foreach (var t in cbtNode.nodeStats.Where(t =>
                    t.stat.statType == RPGStat.STAT_TYPE.VITALITY))
                    t.curValue = (int) t.curMaxValue;
                
                HandleHealthStatChange(cbtNode);

                if(cbtNode.agentREF)cbtNode.agentREF.ClearThreatTable();

            }
        }

        public void RespawnPlayerToGraveyard()
        {
            RPGBuilderUtilities.DisableCG(deathPopupCG);
            Vector3 respawnPOS = Vector3.zero;
            if (allGraveyards.Count > 0)
            {
                GraveyardHandler closestGraveyard = null;
                var closestDist = Mathf.Infinity;
                foreach (var t in allGraveyards)
                    if (isGraveyardAccepted(t))
                    {
                        var dist = Vector3.Distance(playerCombatInfo.transform.position, t.transform.position);
                        if (!(dist < closestDist)) continue;
                        closestDist = dist;
                        closestGraveyard = t;
                    }

                if (closestGraveyard != null)
                {
                    respawnPOS = closestGraveyard.transform.position;
                }
                else
                {
                    respawnPOS = RPGBuilderUtilities.GetWorldPositionFromID(RPGBuilderUtilities
                            .GetGameSceneFromName(RPGBuilderEssentials.Instance.getCurrentScene().name).startPositionID)
                        .position;
                }
            }
            else
            {
                respawnPOS = RPGBuilderUtilities.GetWorldPositionFromID(RPGBuilderUtilities
                        .GetGameSceneFromName(RPGBuilderEssentials.Instance.getCurrentScene().name).startPositionID)
                    .position;
            }

            StatCalculator.ResetPlayerStatsAfterRespawn();
            playerCombatInfo.InitRespawn(respawnPOS);
        }

        private void InitNodeState(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, Sprite icon, RPGAbilityRankData rankREF)
        {
            var hasSameEffectType = false;
            var curStateIndex = -1;
            var allNodeStates = targetInfo.nodeStateData;
            for (var i = 0; i < allNodeStates.Count; i++)
                if (allNodeStates[i].stateEffect.effectType == effect.effectType)
                {
                    hasSameEffectType = true;
                    curStateIndex = i;
                    break;
                }

            if (hasSameEffectType)
            {
                // IF STUN, SLEEP, ROOT, SILENCE, IMMUNE, TAUNT TYPE = We replace the effect
                if (effect.effectType == RPGEffect.EFFECT_TYPE.immune || effect.effectType == RPGEffect.EFFECT_TYPE.root ||
                    effect.effectType == RPGEffect.EFFECT_TYPE.sleep
                    || effect.effectType == RPGEffect.EFFECT_TYPE.stun ||
                    effect.effectType == RPGEffect.EFFECT_TYPE.taunt || effect.effectType == RPGEffect.EFFECT_TYPE.silence)
                {
                    if (targetInfo == playerCombatInfo)
                    {
                        targetInfo.nodeStateData.RemoveAt(curStateIndex);
                        PlayerStatesDisplayHandler.Instance.RemoveState(curStateIndex);
                    }
                    else
                    {
                        ScreenSpaceNameplates.Instance.RemoveState(targetInfo, targetInfo.nodeStateData[curStateIndex]);
                        targetInfo.nodeStateData.RemoveAt(curStateIndex);
                    }

                    var newState = new CombatNode.NodeStatesDATA
                    {
                        stateName = effect._name,
                        casterNode = casterInfo,
                        stateMaxDuration = effect.duration,
                        stateCurDuration = 0,
                        curStack = 1,
                        maxStack = effect.stackLimit,
                        stateEffect = effect,
                        stateIcon = icon
                    };

                    switch (effect.effectType)
                    {
                        case RPGEffect.EFFECT_TYPE.root:
                        case RPGEffect.EFFECT_TYPE.sleep:
                        case RPGEffect.EFFECT_TYPE.stun:
                        case RPGEffect.EFFECT_TYPE.silence:
                            float CC_POWER = GetTotalOfStatType(casterInfo, RPGStat.STAT_TYPE.CC_POWER);
                            float CC_RES = GetTotalOfStatType(targetInfo, RPGStat.STAT_TYPE.RESISTANCE);

                            newState.stateMaxDuration += newState.stateMaxDuration * (CC_POWER / 100);
                            newState.stateMaxDuration = newState.stateMaxDuration * (CC_RES / 100);
                            break;
                        case RPGEffect.EFFECT_TYPE.damageOverTime:
                        case RPGEffect.EFFECT_TYPE.healOverTime:
                        case RPGEffect.EFFECT_TYPE.stat:
                            newState.curPulses = 0;
                            newState.maxPulses = effect.pulses;
                            newState.pulseInterval = effect.duration / effect.pulses;
                            break;
                    }

                    targetInfo.nodeStateData.Add(newState);
                    if (targetInfo == playerCombatInfo)
                        PlayerStatesDisplayHandler.Instance.DisplayState(newState);
                    else
                        ScreenSpaceNameplates.Instance.InitNameplateState(targetInfo, newState);

                    if (targetInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.mob &&
                        targetInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.pet ||
                        effect.effectType != RPGEffect.EFFECT_TYPE.root &&
                        effect.effectType != RPGEffect.EFFECT_TYPE.sleep &&
                        effect.effectType != RPGEffect.EFFECT_TYPE.stun) return;
                    if (targetInfo.agentREF != null) targetInfo.agentREF.InitStun();
                }
                else
                {
                    //if (!hasSameEffect) return;
                    if (effect.effectType != RPGEffect.EFFECT_TYPE.damageOverTime &&
                        effect.effectType != RPGEffect.EFFECT_TYPE.healOverTime &&
                        effect.effectType != RPGEffect.EFFECT_TYPE.stat) return;
                    if (targetInfo.nodeStateData[curStateIndex].casterNode == casterInfo ||
                        targetInfo.nodeStateData[curStateIndex].stateEffect.allowMixedCaster
                    ) // same effect from same caster
                    {
                        if (targetInfo.nodeStateData[curStateIndex].curStack <
                            targetInfo.nodeStateData[curStateIndex].maxStack)
                            targetInfo.nodeStateData[curStateIndex].curStack++;

                        // REFRESH THE EFFECT
                        targetInfo.nodeStateData[curStateIndex].curPulses = 0;
                        targetInfo.nodeStateData[curStateIndex].nextPulse = 0;
                        targetInfo.nodeStateData[curStateIndex].stateCurDuration = 0;
                        if (targetInfo == playerCombatInfo)
                            PlayerStatesDisplayHandler.Instance.UpdateState(curStateIndex);
                        else
                            ScreenSpaceNameplates.Instance.UpdateNameplateState(targetInfo,
                                targetInfo.nodeStateData[curStateIndex]);
                    }
                    else if (targetInfo.nodeStateData[curStateIndex].stateEffect.allowMultiple)
                    {
                        var newState = new CombatNode.NodeStatesDATA
                        {
                            stateName = effect._name,
                            casterNode = casterInfo,
                            stateMaxDuration = effect.duration,
                            stateCurDuration = 0,
                            curStack = 1,
                            maxStack = effect.stackLimit,
                            stateEffect = effect,
                            stateIcon = icon
                        };

                        if (effect.effectType == RPGEffect.EFFECT_TYPE.damageOverTime ||
                            effect.effectType == RPGEffect.EFFECT_TYPE.healOverTime ||
                            effect.effectType == RPGEffect.EFFECT_TYPE.stat)
                        {
                            newState.curPulses = 0;
                            newState.maxPulses = effect.pulses;
                            newState.pulseInterval = effect.duration / effect.pulses;
                        }

                        targetInfo.nodeStateData.Add(newState);
                        Debug.LogError("add state " + newState.stateName);
                        if (targetInfo == playerCombatInfo)
                            PlayerStatesDisplayHandler.Instance.DisplayState(newState);
                        else
                            ScreenSpaceNameplates.Instance.InitNameplateState(targetInfo, newState);
                    }
                }
            }
            else
            {
                var newState = new CombatNode.NodeStatesDATA
                {
                    stateName = effect._name,
                    casterNode = casterInfo,
                    stateMaxDuration = effect.duration,
                    stateCurDuration = 0,
                    curStack = 1,
                    maxStack = effect.stackLimit,
                    stateEffect = effect,
                    stateIcon = icon
                };

                if (effect.effectType == RPGEffect.EFFECT_TYPE.damageOverTime ||
                    effect.effectType == RPGEffect.EFFECT_TYPE.healOverTime ||
                    effect.effectType == RPGEffect.EFFECT_TYPE.stat)
                {
                    newState.curPulses = 0;
                    newState.maxPulses = effect.pulses;
                    newState.pulseInterval = effect.duration / effect.pulses;
                }

                targetInfo.nodeStateData.Add(newState);
                if (targetInfo == playerCombatInfo)
                    PlayerStatesDisplayHandler.Instance.DisplayState(newState);
                else
                    ScreenSpaceNameplates.Instance.InitNameplateState(targetInfo, newState);


                if (effect.effectType == RPGEffect.EFFECT_TYPE.stun)
                {
                    targetInfo.InterruptCastActions();
                }
                
                if (targetInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.mob &&
                    targetInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.pet ||
                    effect.effectType != RPGEffect.EFFECT_TYPE.root && effect.effectType != RPGEffect.EFFECT_TYPE.sleep &&
                    effect.effectType != RPGEffect.EFFECT_TYPE.stun) return;
                if (targetInfo.agentREF != null) targetInfo.agentREF.InitStun();
            }
        }

        public float GetTotalOfStatType(CombatNode nodeInfo, RPGStat.STAT_TYPE statType)
        {
            float total = 0;
            foreach (var t in nodeInfo.nodeStats)
            {
                if (t.stat.statType == statType)
                {
                    total += t.curValue;
                }
            }

            return total;
        }

        public float CalculateCastTime(CombatNode cbtNode, float baseCastTime)
        {
            float curCastMod = GetTotalOfStatType(cbtNode, RPGStat.STAT_TYPE.CAST_SPEED);

            if (curCastMod == 0) return baseCastTime;
            curCastMod /= 100;
            if (curCastMod > 0)
            {
                curCastMod = 1 - curCastMod;
                if (curCastMod < 0) curCastMod = 0;
                baseCastTime *= curCastMod;
                return baseCastTime;
            }

            curCastMod = Mathf.Abs(curCastMod);
            baseCastTime += baseCastTime * curCastMod;
            return baseCastTime;

        }

        public void LeapEnded(CombatNode nodeCombatInfo)
        {
            var curRank = 0;
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(playerCombatInfo.currentAbilityCasted);
            var rankREF =
                RPGBuilderUtilities.GetAbilityRankFromID(playerCombatInfo.currentAbilityCasted.ranks[curRank].rankID);

            if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP) return;
            if (rankREF.extraAbilityExecuted != null) InitExtraAbility(nodeCombatInfo, rankREF.extraAbilityExecuted);
        }

        public void TRIGGER_PLAYER_ABILITY(RPGAbility ability)
        {
            if (playerCombatInfo.IsCasting || playerIsGroundCasting || playerCombatInfo.IsChanneling ||
                playerCombatInfo.isLeaping || !RPGBuilderUtilities.isAbilityKnown(ability.ID))
                return;
            InitAbility(playerCombatInfo, ability, true);
        }


        public void TRIGGER_NPC_ABILITY(CombatNode npcInfo, RPGAbility ability)
        {
            if (npcInfo.isSleeping() || npcInfo.isStunned() || npcInfo.isSilenced() || npcInfo.IsCasting ||
                npcInfo.IsChanneling)
                return;
            InitAbility(npcInfo, ability, true);
        }

        private bool checkCooldown(CombatNode nodeCombatInfo, RPGAbility thisAbility)
        {
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
                return CharacterData.Instance.isAbilityCDReady(thisAbility);
            return nodeCombatInfo.getAbilityDATA(thisAbility).CDLeft == 0;
        }

        public void InitExtraAbility(CombatNode nodeCombatInfo, RPGAbility thisAbility)
        {
            var curRank = 0;
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(thisAbility.ranks[curRank].rankID);

            if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND &&
                rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP)
                ExecuteCombatVisuals(RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate, thisAbility, nodeCombatInfo,
                    Vector3.zero, rankREF);
            switch (rankREF.targetType)
            {
                case RPGAbility.TARGET_TYPES.AOE:
                    EXECUTE_AOE_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.CONE:
                    EXECUTE_CONE_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.PROJECTILE:
                    if (nodeCombatInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.objectAction)
                        EXECUTE_PROJECTILE_ABILITY(nodeCombatInfo, thisAbility,
                            RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.LINEAR:
                    EXECUTE_LINEAR_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                    break;
            }
        }

        private bool checkTarget(CombatNode casterInfo, RPGAbilityRankData rankREF)
        {
            if (rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_INSTANT ||
                rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_PROJECTILE)
            {
                CombatNode checkedTarget = null;
                if (casterInfo == playerCombatInfo)
                {
                    if (currentTarget != null)
                    {
                        checkedTarget = currentTarget;
                    }
                    else
                    {
                        ErrorEventsDisplayManager.Instance.ShowErrorEvent("This ability requires a target", 3);
                        return false;
                    }
                }
                else
                {
                    if (casterInfo.agentREF.target != null)
                        checkedTarget = casterInfo.agentREF.target;
                    else
                        return false;
                }

                if (checkedTarget != null)
                {
                    if (checkedTarget == playerCombatInfo)
                    {
                        if (casterInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
                        {
                            if (rankREF.canHitSelf) return true;
                            ErrorEventsDisplayManager.Instance.ShowErrorEvent("This ability cannot be used on yourself", 3);
                            return false;
                        }

                        switch (casterInfo.npcDATA.alignmentType)
                        {
                            case RPGNpc.ALIGNMENT_TYPE.ALLY:
                                if (!rankREF.canHitAlly) return false;
                                break;
                            case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                                if (!rankREF.canHitNeutral) return false;
                                break;
                            case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                                if (!rankREF.canHitAlly) return false;
                                break;
                        }
                    }
                    else
                    {
                        var dist = Vector3.Distance(casterInfo.transform.position, checkedTarget.transform.position);
                        float totalMinRange = rankREF.minRange + (rankREF.minRange *
                                                                  (GetTotalOfStatType(casterInfo,
                                                                      RPGStat.STAT_TYPE.ABILITY_TARGET_MIN_RANGE) / 100));
                        if (dist < totalMinRange)
                        {
                            if (casterInfo == playerCombatInfo)
                                ErrorEventsDisplayManager.Instance.ShowErrorEvent("The target is too close", 3);
                            return false;
                        }

                        float totalMaxRange = rankREF.maxRange + (rankREF.maxRange *
                                                                  (GetTotalOfStatType(casterInfo,
                                                                      RPGStat.STAT_TYPE.ABILITY_TARGET_MAX_RANGE) / 100));
                        
                        if (dist > totalMaxRange)
                        {
                            if (casterInfo == playerCombatInfo)
                                ErrorEventsDisplayManager.Instance.ShowErrorEvent("The target is too far", 3);
                            return false;
                        }

                        switch (checkedTarget.npcDATA.alignmentType)
                        {
                            case RPGNpc.ALIGNMENT_TYPE.ALLY:
                                if (!rankREF.canHitAlly)
                                {
                                    if (casterInfo == playerCombatInfo)
                                        ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                            "This ability cannot be used on allied targets", 3);
                                    return false;
                                }

                                break;
                            case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                                if (!rankREF.canHitNeutral)
                                {
                                    if (casterInfo == playerCombatInfo)
                                        ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                            "This ability cannot be used on neutral targets", 3);
                                    return false;
                                }

                                break;
                            case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                                if (!rankREF.canHitEnemy)
                                {
                                    if (casterInfo == playerCombatInfo)
                                        ErrorEventsDisplayManager.Instance.ShowErrorEvent(
                                            "This ability cannot be used on enemy targets", 3);
                                    return false;
                                }

                                break;
                        }
                    }
                }
                else
                {
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent("This ability requires a target", 3);
                    return false;
                }
            }
            else
            {
                return true;
            }

            return true;
        }

        private bool UseRequirementsMet(CombatNode casterInfo, RPGAbility ability, int curRank, bool abMustBeKnown)
        {
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);
            if (abMustBeKnown)
            {
                if (!checkCooldown(casterInfo, ability))
                {
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent("This ability is not ready yet", 3);
                    return false;
                }
            }

            if (casterInfo == playerCombatInfo && rankREF.castTime > 0 && !rankREF.castInRun)
            {
                if (!playerCombatInfo.playerControllerREF.GetIsGrounded()) return false;
                if (playerCombatInfo.playerControllerREF.GetDesiredSpeed() > 0) return false;
            }

            return checkTarget(casterInfo, rankREF) && rankREF.useRequirements.All(t => RequirementsManager.Instance.HandleAbilityRequirementUseType(casterInfo, t, true));
        }

        public void HandleAbilityCost(CombatNode nodeCombatInfo, RPGAbility ab, RPGAbilityRankData rankREF)
        {
            foreach (var t in rankREF.useRequirements)
                if (t.requirementType == RequirementsManager.AbilityUseRequirementType.statCost)
                {
                    var statREF = RPGBuilderUtilities.GetStatFromID(t.statCostID);
                    var newUseCostValue = (int) nodeCombatInfo.getCurrentValue(statREF._name) - t.useCost;
                    nodeCombatInfo.setCurrentValue(statREF._name, newUseCostValue);
                    nodeCombatInfo.HandleActionsForSpecialStats(statREF._name);
                }
        }


        public void HandleAbilityTypeActions(CombatNode casterInfo, RPGAbility thisAbility, RPGAbilityRankData rankREF,
            bool OnStart)
        {
            switch (rankREF.targetType)
            {
                case RPGAbility.TARGET_TYPES.SELF:
                    EXECUTE_SELF_ABILITY(casterInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.AOE:
                    EXECUTE_AOE_ABILITY(casterInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.CONE:
                    EXECUTE_CONE_ABILITY(casterInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.PROJECTILE:
                    EXECUTE_PROJECTILE_ABILITY(casterInfo, thisAbility,
                        OnStart
                            ? RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate
                            : RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.completed, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.LINEAR:
                    EXECUTE_LINEAR_ABILITY(casterInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.GROUND:
                    playerIsGroundCasting = true;
                    playerCombatInfo.playerControllerREF.StartCoroutine(
                        playerCombatInfo.playerControllerREF.UpdateCachedGroundCasting(playerIsGroundCasting));
                    playerCombatInfo.currentAbilityCastedSlot = 0;
                    playerCombatInfo.currentAbilityCasted = thisAbility;
                    playerCombatInfo.currentAbilityCastedCurRank = rankREF;
                    INIT_GROUND_ABILITY(casterInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.GROUND_LEAP:
                    playerIsGroundCasting = true;
                    playerCombatInfo.playerControllerREF.StartCoroutine(
                        playerCombatInfo.playerControllerREF.UpdateCachedGroundCasting(playerIsGroundCasting));
                    playerCombatInfo.currentAbilityCastedSlot = 0;
                    playerCombatInfo.currentAbilityCasted = thisAbility;
                    playerCombatInfo.currentAbilityCastedCurRank = rankREF;
                    INIT_GROUND_ABILITY(casterInfo, thisAbility, rankREF);
                    break;

                case RPGAbility.TARGET_TYPES.TARGET_INSTANT:
                    if (currentTarget != null && !currentTarget.dead)
                        EXECUTE_TARGET_INSTANT_ABILITY(casterInfo, currentTarget, thisAbility, rankREF);
                    break;
            }
        }

        public void InitAbility(CombatNode nodeCombatInfo, RPGAbility thisAbility, bool abMustBeKnown)
        {
            var curRank = 0;
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
            {
                curRank = 0;
            }
            else
            {
                curRank = RPGBuilderUtilities.getNodeCurrentRank(thisAbility);
                if (!abMustBeKnown) curRank = 0;
            }

            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(thisAbility.ranks[curRank].rankID);

            if (nodeCombatInfo.isSleeping() || nodeCombatInfo.isStunned() || nodeCombatInfo.isSilenced()) return;

            var abSlotIndex = nodeCombatInfo.getAbilityDATAIndex(thisAbility);
            switch (nodeCombatInfo.nodeType)
            {
                case CombatNode.COMBAT_NODE_TYPE.player:
                {
                    if (UseRequirementsMet(nodeCombatInfo, thisAbility, curRank, abMustBeKnown))
                    {
                        if (playerCombatInfo.playerControllerREF.CurrentController == RPGBCharacterController.ControllerType.ClickMove
                        && ( rankREF.targetType == RPGAbility.TARGET_TYPES.CONE || rankREF.targetType == RPGAbility.TARGET_TYPES.LINEAR || rankREF.targetType == RPGAbility.TARGET_TYPES.PROJECTILE
                        || rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_INSTANT || rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_PROJECTILE))
                        {
                            playerCombatInfo.playerControllerREF.PlayerLookAtCursor();
                        }
                        
                        if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND &&
                            rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP)
                        {
                            if (rankREF.targetType != RPGAbility.TARGET_TYPES.PROJECTILE)
                                ExecuteCombatVisuals(RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate, thisAbility,
                                    nodeCombatInfo, Vector3.zero, rankREF);

                            if (rankREF.standTimeDuration > 0)
                                playerCombatInfo.playerControllerREF.InitStandTime(rankREF.standTimeDuration);

                            if (rankREF.castSpeedSlowAmount > 0)
                                playerCombatInfo.playerControllerREF.InitCastMoveSlow(rankREF.castSpeedSlowAmount,
                                    rankREF.castSpeedSlowTime, rankREF.castSpeedSlowRate);
                        }

                        if (rankREF.castTime > 0)
                        {
                            if (rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_INSTANT ||
                                rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_PROJECTILE)
                            {
                                if (currentTarget.dead)
                                {
                                    ErrorEventsDisplayManager.Instance.ShowErrorEvent("The target is dead", 3);
                                    return;
                                }
                                nodeCombatInfo.currentTargetCasted = currentTarget;
                            }

                            nodeCombatInfo.InitCasting(thisAbility, abSlotIndex, rankREF);
                            if(rankREF.castBarVisible)PlayerInfoDisplayManager.Instance.InitCastBar(thisAbility);

                            if (rankREF.targetType == RPGAbility.TARGET_TYPES.PROJECTILE)
                                ExecuteCombatVisuals(RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate, thisAbility,
                                    nodeCombatInfo, Vector3.zero, rankREF);
                        }
                        else
                        {
                            HandleAbilityTypeActions(nodeCombatInfo, thisAbility, rankREF, true);

                            if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND &&
                                rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP)
                            {
                                if (!thisAbility.isPlayerAutoAttack)
                                    StartCooldown(nodeCombatInfo, abSlotIndex, rankREF.cooldown, thisAbility.ID);
                                else
                                    playerCombatInfo.InitAACooldown(1);
                            }

                            HandleAbilityCost(nodeCombatInfo, thisAbility, rankREF);

                            if (rankREF.channelTime > 0)
                            {
                                nodeCombatInfo.InitChanneling(thisAbility, abSlotIndex);
                                PlayerInfoDisplayManager.Instance.InitChannelBar(thisAbility);
                            }
                        }
                    }

                    break;
                }
                case CombatNode.COMBAT_NODE_TYPE.mob:
                case CombatNode.COMBAT_NODE_TYPE.pet:
                {
                    if (UseRequirementsMet(nodeCombatInfo, thisAbility, curRank, true))
                    {
                        if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND &&
                            rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP)
                        {
                            ExecuteCombatVisuals(RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate, thisAbility,
                                nodeCombatInfo, Vector3.zero, rankREF);

                            if (rankREF.standTimeDuration > 0)
                                nodeCombatInfo.agentREF.InitStandTime(rankREF.standTimeDuration, rankREF.canRotateInStandTime);

                            if (rankREF.castSpeedSlowAmount > 0)
                                playerCombatInfo.playerControllerREF.InitCastMoveSlow(rankREF.castSpeedSlowAmount,
                                    rankREF.castSpeedSlowTime, rankREF.castSpeedSlowRate);
                        }

                        if (rankREF.castTime > 0)
                        {
                            if (rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_INSTANT ||
                                rankREF.targetType == RPGAbility.TARGET_TYPES.TARGET_PROJECTILE)
                            {
                                if (nodeCombatInfo.agentREF.target.dead)
                                {
                                    return;
                                }
                                nodeCombatInfo.currentTargetCasted = nodeCombatInfo.agentREF.target;
                            }

                            nodeCombatInfo.InitCasting(thisAbility, abSlotIndex, rankREF);
                        }
                        else
                        {
                            switch (rankREF.targetType)
                            {
                                case RPGAbility.TARGET_TYPES.SELF:
                                    EXECUTE_SELF_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                                    break;

                                case RPGAbility.TARGET_TYPES.AOE:
                                    EXECUTE_AOE_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                                    break;

                                case RPGAbility.TARGET_TYPES.CONE:
                                    EXECUTE_CONE_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                                    break;

                                case RPGAbility.TARGET_TYPES.PROJECTILE:
                                    if (rankREF.castTime == 0)
                                    {
                                        EXECUTE_PROJECTILE_ABILITY(nodeCombatInfo, thisAbility,
                                            RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate, rankREF);
                                    }
                                    break;

                                case RPGAbility.TARGET_TYPES.LINEAR:
                                    EXECUTE_LINEAR_ABILITY(nodeCombatInfo, thisAbility, rankREF);
                                    break;

                                case RPGAbility.TARGET_TYPES.TARGET_INSTANT:
                                    if (nodeCombatInfo.agentREF.target != null && !nodeCombatInfo.agentREF.target.dead)
                                        EXECUTE_TARGET_INSTANT_ABILITY(nodeCombatInfo, nodeCombatInfo.agentREF.target,
                                            thisAbility, rankREF);
                                    break;
                            }

                            if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND &&
                                rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP)
                                StartCooldown(nodeCombatInfo, abSlotIndex, rankREF.cooldown, thisAbility.ID);

                            HandleAbilityCost(nodeCombatInfo, thisAbility, rankREF);

                            if (rankREF.channelTime > 0) nodeCombatInfo.InitChanneling(thisAbility, abSlotIndex);
                        }
                    }

                    break;
                }
            }
        }


        private void Update()
        {
            if (playerIsGroundCasting) HandleGroundCasting();
        }

        private void HandleGroundCasting()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                EXECUTE_GROUND_ABILITY(playerCombatInfo, playerCombatInfo.currentAbilityCasted,
                    RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.activate,
                    playerCombatInfo.currentAbilityCastedCurRank);
                playerIsGroundCasting = false;
                playerCombatInfo.currentAbilityCastedSlot = -1;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                groundIndicatorManager.HideIndicator();
                playerIsGroundCasting = false;
                playerCombatInfo.currentAbilityCastedSlot = -1;
            }

            playerCombatInfo.playerControllerREF.StartCoroutine(
                playerCombatInfo.playerControllerREF.UpdateCachedGroundCasting(playerIsGroundCasting));
        }


        public void StartCooldown(CombatNode casterInfo, int abilitySlotID, float CD, int abID)
        {
            var finalCD = CD;
            float cdrecoveryspeed = GetTotalOfStatType(casterInfo, RPGStat.STAT_TYPE.CAST_SPEED);

            if (cdrecoveryspeed != 0)
            {
                cdrecoveryspeed /= 100;
                finalCD -= finalCD * cdrecoveryspeed;
            }

            if (casterInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
            {
                CharacterData.Instance.InitAbilityCooldown(abID, finalCD);
            }
            else
            {
                casterInfo.abilitiesData[abilitySlotID].NextTimeUse = finalCD;
                casterInfo.abilitiesData[abilitySlotID].CDLeft = finalCD;
            }
        }

        private void EXECUTE_LINEAR_ABILITY(CombatNode nodeCombatInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {

            if (rankREF.ConeHitCount == 1)
            {
                var hitColliders = Physics.OverlapSphere(nodeCombatInfo.transform.position, rankREF.linearLength,
                    rankREF.hitLayers);
                var unitInCone = new List<Collider>();

                double Fi, cs, sn;

                foreach (var t in hitColliders)
                {
                    Fi = nodeCombatInfo.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
                    cs = Mathf.Cos((float) Fi);
                    sn = Mathf.Sin((float) Fi);

                    var tx = t.gameObject.transform.position.x - nodeCombatInfo.transform.position.x;
                    var tz = t.gameObject.transform.position.z - nodeCombatInfo.transform.position.z;

                    var ptx = (float) (cs * tx - sn * tz);
                    var ptz = (float) (sn * tx + cs * tz);

                    if (!(-(rankREF.linearWidth / 2) <= ptx) || !(ptx <= rankREF.linearWidth / 2)) continue;
                    if (ptz >= 0 && ptz <= rankREF.linearLength) unitInCone.Add(t);
                }

                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestUnits(nodeCombatInfo, unitInCone, totalUnitHit);
                foreach (var t in closestUnits)
                    ExecuteAbilityEffects(nodeCombatInfo, t, ability, rankREF);
            }
            else
            {
                StartCoroutine(EXECUTE_LINEAR_ABILITY_PULSE(nodeCombatInfo, ability));
            }
        }

        private IEnumerator EXECUTE_LINEAR_ABILITY_PULSE(CombatNode nodeCombatInfo, RPGAbility ability)
        {
            var curRank = 0;
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(ability);
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);

            for (var x = 0; x < rankREF.ConeHitCount; x++)
            {
                if(nodeCombatInfo.dead) yield break;
                var hitColliders = Physics.OverlapSphere(nodeCombatInfo.transform.position, rankREF.linearLength,
                    rankREF.hitLayers);
                var unitInCone = new List<Collider>();

                double Fi, cs, sn;

                foreach (var t in hitColliders)
                {
                    Fi = nodeCombatInfo.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
                    cs = Mathf.Cos((float) Fi);
                    sn = Mathf.Sin((float) Fi);

                    var tx = t.gameObject.transform.position.x - nodeCombatInfo.transform.position.x;
                    var tz = t.gameObject.transform.position.z - nodeCombatInfo.transform.position.z;

                    var ptx = (float) (cs * tx - sn * tz);
                    var ptz = (float) (sn * tx + cs * tz);

                    if (!(-(rankREF.linearWidth / 2) <= ptx) || !(ptx <= rankREF.linearWidth / 2)) continue;
                    if (ptz >= 0 && ptz <= rankREF.linearLength) unitInCone.Add(t);
                }

                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestUnits(nodeCombatInfo, unitInCone, totalUnitHit);
                foreach (var t in closestUnits)
                    ExecuteAbilityEffects(nodeCombatInfo, t, ability, rankREF);

                yield return new WaitForSeconds(rankREF.ConeHitInterval);
            }
        }

        private List<Collider> RemoveAlreadyHitTargetsFromArray(ProjectileHitDetection projREF, Collider[] hitColliders)
        {
            var colList = hitColliders.ToList();
            foreach (var t in projREF.alreadyHitNodes)
            {
                var thisCol = t.GetComponent<Collider>();
                if (colList.Contains(thisCol)) colList.Remove(thisCol);
            }

            return colList;
        }

        public void FIND_NEARBY_UNITS(CombatNode nodeCombatInfo, GameObject projectileGO, RPGAbility ability,
            ProjectileHitDetection projREF)
        {
            var curRank = 0;
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(ability);
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);

            var hitColliders = Physics.OverlapSphere(projectileGO.transform.position,
                rankREF.projectileNearbyUnitDistanceMax, rankREF.hitLayers);

            var colList = RemoveAlreadyHitTargetsFromArray(projREF, hitColliders);
            if (colList.Count == 0)
            {
                Destroy(projectileGO);
            }
            else
            {
                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestNearbyUnits(projREF.gameObject, colList, totalUnitHit);
                if (closestUnits.Count > 0)
                    for (var i = 0; i < 1; i++)
                    {
                        projREF.curNearbyTargetGO = closestUnits[i].gameObject;
                        return;
                    }
                else
                    Destroy(projectileGO);
            }
        }

        private void EXECUTE_SELF_ABILITY(CombatNode casterInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {
            ExecuteAbilityEffects(casterInfo, casterInfo, ability, rankREF);
        }

        private void EXECUTE_AOE_ABILITY(CombatNode nodeCombatInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {
            ExecuteCombatVisuals(RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE.completed, ability, nodeCombatInfo,
                Vector3.zero, rankREF);

            if (rankREF.AOEHitCount == 1)
            {
                float totalRadius = rankREF.AOERadius +
                                    (rankREF.AOERadius * (GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.AOE_RADIUS)/100));
                var hitColliders =
                    Physics.OverlapSphere(nodeCombatInfo.transform.position, totalRadius, rankREF.hitLayers);
                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestUnits(nodeCombatInfo, hitColliders, totalUnitHit);
                foreach (var t in closestUnits)
                    ExecuteAbilityEffects(nodeCombatInfo, t, ability, rankREF);
            }
            else
            {
                StartCoroutine(EXECUTE_AOE_ABILITY_PULSE(nodeCombatInfo, ability));
            }
        }

        private IEnumerator EXECUTE_AOE_ABILITY_PULSE(CombatNode nodeCombatInfo, RPGAbility ability)
        {
            if (nodeCombatInfo == null) yield break;
            var curRank = 0;
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(ability);
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);

            for (var x = 0; x < rankREF.AOEHitCount; x++)
            {
                if(nodeCombatInfo.dead) yield break;
                if (nodeCombatInfo == null) yield break;
                
                float totalRadius = rankREF.AOERadius +
                                    (rankREF.AOERadius * (GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.AOE_RADIUS)/100));
                var hitColliders =
                    Physics.OverlapSphere(nodeCombatInfo.transform.position, totalRadius, rankREF.hitLayers);
                
                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestUnits(nodeCombatInfo, hitColliders, totalUnitHit);
                foreach (var t in closestUnits)
                    ExecuteAbilityEffects(nodeCombatInfo, t, ability, rankREF);

                yield return new WaitForSeconds(rankREF.AOEHitInterval);
            }
        }

        private void EXECUTE_CONE_ABILITY(CombatNode nodeCombatInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {

            if (rankREF.ConeHitCount == 1)
            {
                var hitColliders =
                    Physics.OverlapSphere(nodeCombatInfo.transform.position, rankREF.minRange, rankREF.hitLayers);
                var unitInCone = new List<Collider>();
                foreach (var t in hitColliders)
                {
                    var heading = (t.gameObject.transform.position - nodeCombatInfo.transform.position).normalized;
                    var forward = nodeCombatInfo.transform.TransformDirection(Vector3.forward);
                    var dot = Vector3.Dot(forward, heading);

                    var UnitIsInCone = false;

                    if (rankREF.coneDegree == 30)
                    {
                        if (dot > 0.88f) UnitIsInCone = true;
                    }
                    else if (rankREF.coneDegree == 90)
                    {
                        if (dot > 0.5f) UnitIsInCone = true;
                    }
                    else if (rankREF.coneDegree == 180)
                    {
                        if (dot > 0f) UnitIsInCone = true;
                    }

                    if (UnitIsInCone) unitInCone.Add(t);
                }

                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestUnits(nodeCombatInfo, unitInCone, totalUnitHit);
                foreach (var t in closestUnits)
                    ExecuteAbilityEffects(nodeCombatInfo, t, ability, rankREF);
            }
            else
            {
                StartCoroutine(EXECUTE_CONE_ABILITY_PULSE(nodeCombatInfo, ability, rankREF));
            }
        }

        private IEnumerator EXECUTE_CONE_ABILITY_PULSE(CombatNode nodeCombatInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {

            for (var x = 0; x < rankREF.ConeHitCount; x++)
            {
                if(nodeCombatInfo.dead) yield break;
                var hitColliders =
                    Physics.OverlapSphere(nodeCombatInfo.transform.position, rankREF.minRange, rankREF.hitLayers);
                var unitInCone = new List<Collider>();
                foreach (var t in hitColliders)
                {
                    var heading = (t.gameObject.transform.position - nodeCombatInfo.transform.position).normalized;
                    var forward = nodeCombatInfo.transform.TransformDirection(Vector3.forward);
                    var dot = Vector3.Dot(forward, heading);

                    var UnitIsInCone = false;

                    switch (rankREF.coneDegree)
                    {
                        case 30:
                        {
                            if (dot > 0.88f) UnitIsInCone = true;
                            break;
                        }
                        case 90:
                        {
                            if (dot > 0.5f) UnitIsInCone = true;
                            break;
                        }
                        case 180:
                        {
                            if (dot > 0f) UnitIsInCone = true;
                            break;
                        }
                    }

                    if (UnitIsInCone) unitInCone.Add(t);
                }

                int totalUnitHit = rankREF.MaxUnitHit +
                                   (int)CombatManager.Instance.GetTotalOfStatType(nodeCombatInfo, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
                var closestUnits = getClosestUnits(nodeCombatInfo, unitInCone, totalUnitHit);
                foreach (var t in closestUnits)
                    ExecuteAbilityEffects(nodeCombatInfo, t, ability, rankREF);

                yield return new WaitForSeconds(rankREF.ConeHitInterval);
            }
        }

        private void EXECUTE_TARGET_INSTANT_ABILITY(CombatNode casterInfo, CombatNode targetInfo, RPGAbility ability,
            RPGAbilityRankData rankREF)
        {
            ExecuteAbilityEffects(casterInfo, targetInfo, ability, rankREF);
        }

        private void ExecuteCombatVisuals(RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE activationType,
            RPGAbility ability, CombatNode nodeCombatInfo, Vector3 POS, RPGAbilityRankData rankREF)
        {

            foreach (var t in rankREF.visualsData)
            {
                if (t.activationType != activationType) continue;

                switch (t.type)
                {
                    case RPGAbility.COMBAT_VISUAL_EFFECT_TYPE.effect:
                        var cbtVisualEffect = Instantiate(t.effect.gameObject,
                            nodeCombatInfo.transform.position, Quaternion.identity);
                        var cbtVisualEff = cbtVisualEffect.GetComponent<CombatVisualEffect>();
                        if (cbtVisualEff != null)
                        {
                            if (rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND &&
                                rankREF.targetType != RPGAbility.TARGET_TYPES.GROUND_LEAP)
                                cbtVisualEff.InitCombatVisual(nodeCombatInfo, ability, nodeCombatInfo.appearanceREF,
                                    rankREF);
                            else
                                cbtVisualEff.InitCombatVisual(nodeCombatInfo, ability, POS,
                                    nodeCombatInfo.appearanceREF, rankREF);
                        }

                        
                        if (cbtVisualEff != null)
                        {
                            cbtVisualEff.OwnerNode = nodeCombatInfo;
                        }
                        break;

                    case RPGAbility.COMBAT_VISUAL_EFFECT_TYPE.animation:
                        var cbtVisualAnimation = Instantiate(t.animation.gameObject,
                            nodeCombatInfo.transform.position, Quaternion.identity);
                        var cbtVisualAnim = cbtVisualAnimation.GetComponent<CombatVisualAnimation>();
                        if (cbtVisualAnim != null)
                            cbtVisualAnim.InitCombatAnimation(nodeCombatInfo.gameObject, ability);
                        break;
                }
            }
        }


        private void INIT_GROUND_ABILITY(CombatNode nodeCombatInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {
            groundIndicatorManager.ShowIndicator(rankREF.groundRadius * 1.25f, rankREF.groundRange);
        }

        private void EXECUTE_GROUND_ABILITY(CombatNode nodeCombatInfo, RPGAbility ability,
            RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE activationType, RPGAbilityRankData rankREF)
        {

            if (rankREF.targetType == RPGAbility.TARGET_TYPES.GROUND_LEAP)
                EXECUTE_GROUND_LEAP_MOVEMENT(nodeCombatInfo, ability, groundIndicatorManager.GetIndicatorPosition());

            ExecuteCombatVisuals(activationType, ability, nodeCombatInfo, groundIndicatorManager.GetIndicatorPosition(),
                rankREF);
            groundIndicatorManager.HideIndicator();

            StartCooldown(nodeCombatInfo, playerCombatInfo.currentAbilityCastedSlot, rankREF.cooldown, ability.ID);

            if (rankREF.standTimeDuration > 0)
                playerCombatInfo.playerControllerREF.InitStandTime(rankREF.standTimeDuration);

            if (rankREF.castSpeedSlowAmount > 0)
                playerCombatInfo.playerControllerREF.InitCastMoveSlow(rankREF.castSpeedSlowAmount,
                    rankREF.castSpeedSlowTime, rankREF.castSpeedSlowRate);
        }

        private void EXECUTE_GROUND_LEAP_MOVEMENT(CombatNode nodeCombatInfo, RPGAbility ability, Vector3 LeapEndPOS)
        {
            var curRank = 0;
            if (nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.mob ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction ||
                nodeCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(ability);
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);

            playerCombatInfo.playerControllerREF.InitGroundLeap();
            nodeCombatInfo.InitLeap(nodeCombatInfo.transform.position, LeapEndPOS, rankREF.groundLeapHeight,
                rankREF.groundLeapSpeed);
        }

        public void EXECUTE_GROUND_ABILITY_HIT(CombatNode casterInfo, CombatNode targetInfo, RPGAbility ability,
            RPGAbilityRankData rankREF)
        {
            ExecuteAbilityEffects(casterInfo, targetInfo, ability, rankREF);
        }


        private void EXECUTE_PROJECTILE_ABILITY(CombatNode nodeCombatInfo, RPGAbility ability,
            RPGAbility.COMBAT_VISUAL_EFFECT_ACTIVATION_TYPE activationType, RPGAbilityRankData rankREF)
        {
            ExecuteCombatVisuals(activationType, ability, nodeCombatInfo, Vector3.zero, rankREF);
        }

        public void EXECUTE_PROJECTILE_ABILITY_HIT(CombatNode casterInfo, CombatNode targetInfo, RPGAbility ability,
            RPGAbilityRankData rankREF)
        {
            ExecuteAbilityEffects(casterInfo, targetInfo, ability, rankREF);
        }

        public void ExecuteDOTPulse(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, int curStack, RPGAbilityRankData rankREF)
        {
            if (targetInfo.dead) return;
            var dmg = damageCalculation(casterInfo, targetInfo, effect, rankREF, false);
            dmg /= effect.pulses;
            dmg *= curStack;
            string dmgTextType = effect.mainDamageType.ToString();
            if (targetInfo.isImmune()) dmgTextType = "IMMUNE";
            HandleCombatTextTrigger(casterInfo, targetInfo, dmg, dmgTextType);
            targetInfo.TakeDamage(casterInfo, dmg, rankREF);

            HandleLifesteal(casterInfo, targetInfo, effect, dmg);
            if(dmg > 0)HandleThorn(casterInfo, targetInfo, dmg);
            HandleMobAggro(casterInfo, targetInfo);
            HandlePetDefendOwner(targetInfo, casterInfo);
        }

        private void HandlePetDefendOwner(CombatNode owner, CombatNode attacker)
        {
            if (owner.currentPets.Count <= 0) return;
            foreach (var node in owner.currentPets)
                if (node.agentREF.target == null)
                    node.agentREF.SetTarget(attacker);
        }

        public void ExecuteHOTPulse(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, int curStack)
        {
            if (targetInfo.dead) return;
            var heal = healingCalculation(casterInfo, targetInfo, effect, 0,false);
            heal /= effect.pulses;
            heal *= curStack;
            HandleCombatTextTrigger(casterInfo, targetInfo, heal, "HEAL");
            targetInfo.Heal(heal);
        }

        public void ExecuteStatPulse(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, int curStack,
            int stateIndex)
        {
            if (targetInfo.dead) return;
            if (targetInfo.nodeStateData[stateIndex].modifiedStatValues.Count == 0)
                for (var i = 0; i < effect.statEffectsData.Count; i++)
                    targetInfo.nodeStateData[stateIndex].modifiedStatValues.Add(0);

            for (var i = 0; i < effect.statEffectsData.Count; i++)
            {
                var statREF = RPGBuilderUtilities.GetStatFromID(effect.statEffectsData[i].statID);
                float newValue = 0, modifiedValue = 0;
                if (RPGBuilderUtilities.GetStatFromID(effect.statEffectsData[i].statID).statType ==
                    RPGStat.STAT_TYPE.VITALITY)
                {
                    newValue = targetInfo.getCurrentMaxValue(statREF._name);
                    if (effect.statEffectsData[i].isPercent)
                        modifiedValue = newValue * (effect.statEffectsData[i].statEffectModification / 100);
                    else
                        modifiedValue = effect.statEffectsData[i].statEffectModification;
                    newValue += modifiedValue;
                    targetInfo.setCurrentMaxValue(statREF._name, newValue);
                }
                else
                {
                    newValue = targetInfo.getCurrentValue(statREF._name);
                    if (effect.statEffectsData[i].isPercent)
                        modifiedValue = newValue * (effect.statEffectsData[i].statEffectModification / 100);
                    else
                        modifiedValue = effect.statEffectsData[i].statEffectModification;
                    newValue += modifiedValue;
                    targetInfo.setCurrentValue(statREF._name, newValue);
                }

                targetInfo.nodeStateData[stateIndex].modifiedStatValues[i] += modifiedValue;

                TriggerUpdateStatUI(targetInfo, statREF);
            }
        }

        private void TriggerUpdateStatUI(CombatNode nodeInfo, RPGStat attribute)
        {
            if (nodeInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.player) return;
            switch (attribute._name)
            {
                case "HEALTH":
                    StatBarUpdate("HEALTH");
                    break;
            }

            if (CharacterPanelDisplayManager.Instance.thisCG.alpha == 1 &&
                CharacterPanelDisplayManager.Instance.curCharInfoType ==
                CharacterPanelDisplayManager.characterInfoTypes.stats)
                CharacterPanelDisplayManager.Instance.InitCharStats();
        }

        public void RemoveStatModification(CombatNode targetInfo, RPGStat stat, float statModValue)
        {
            float newValue = 0;
            if (stat.statType == RPGStat.STAT_TYPE.VITALITY)
            {
                newValue = targetInfo.getCurrentMaxValue(stat._name);
                newValue -= statModValue;
                targetInfo.setCurrentMaxValue(stat._name, newValue);
            }
            else
            {
                newValue = targetInfo.getCurrentValue(stat._name);
                newValue -= statModValue;
                targetInfo.setCurrentValue(stat._name, newValue);
            }

            TriggerUpdateStatUI(targetInfo, stat);
        }

        private void HandleLifesteal(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, int dmg)
        {
            // LIFESTEAL
            var lifesteal = getLifesteal(casterInfo, effect, dmg);
            if (lifesteal <= 0) return;
            casterInfo.Heal(lifesteal);
            HandleCombatTextTrigger(casterInfo == playerCombatInfo ? casterInfo : targetInfo, casterInfo, lifesteal,
                "HEAL");
        }

        private void HandleThorn(CombatNode casterInfo, CombatNode targetInfo, int dmg)
        {
            float thorn = GetTotalOfStatType(targetInfo, RPGStat.STAT_TYPE.THORN);
            if (isPet(targetInfo))
            {
                thorn += GetTotalOfStatType(targetInfo.ownerCombatInfo, RPGStat.STAT_TYPE.MINION_THORN);
            }

            if (!(thorn > 0)) return;
            thorn /= 100;
            var thornDamage = (int) (dmg * thorn);
            casterInfo.TakeDamage(targetInfo, thornDamage, null);
            HandlePetDefendOwner(casterInfo, targetInfo);
            if (casterInfo == playerCombatInfo || targetInfo == playerCombatInfo ||
                playerCombatInfo.currentPets.Contains(casterInfo))
                ScreenTextDisplayManager.Instance.ScreenEventHandler("THORN", thornDamage.ToString(),
                    casterInfo.gameObject);
        }

        private void HandleMobAggro(CombatNode casterInfo, CombatNode targetInfo)
        {
            if (targetInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.mob &&
                targetInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.pet) return;
            if (targetInfo.agentREF != null && targetInfo.agentREF.target == null)
                targetInfo.agentREF.SetTarget(casterInfo);
        }

        private void HandleCombatTextTrigger(CombatNode casterInfo, CombatNode targetInfo, int dmg, string CombatTextType)
        {
            if (casterInfo == playerCombatInfo || targetInfo == playerCombatInfo ||
                playerCombatInfo.currentPets.Contains(casterInfo))
                ScreenTextDisplayManager.Instance.ScreenEventHandler(CombatTextType, dmg.ToString(), targetInfo.gameObject);
        }

        private void EFFECTS_LOGIC(RPGEffect effect, CombatNode casterInfo, CombatNode targetInfo, RPGAbilityRankData rankREF)
        {
            if (targetInfo.dead) return;
            switch (effect.effectType)
            {
                case RPGEffect.EFFECT_TYPE.instantDamage:
                    var dmg = damageCalculation(casterInfo, targetInfo, effect, rankREF, true);
                    if (!targetInfo.isImmune())
                    {
                        targetInfo.TakeDamage(casterInfo, dmg, rankREF);
                        HandlePetDefendOwner(targetInfo, casterInfo);
                        HandleLifesteal(casterInfo, targetInfo, effect, dmg);
                    }
                    HandleMobAggro(casterInfo, targetInfo);
                    if(dmg > 0)HandleThorn(casterInfo, targetInfo, dmg);
                    break;
                case RPGEffect.EFFECT_TYPE.instantHeal:
                    var heal = healingCalculation(casterInfo, targetInfo, effect, 0,true);
                    targetInfo.Heal(heal);
                    break;
                case RPGEffect.EFFECT_TYPE.teleport:
                    switch (effect.teleportType)
                    {
                        case RPGEffect.TELEPORT_TYPE.gameScene:
                            if (targetInfo == playerCombatInfo)
                                RPGBuilderEssentials.Instance.TeleportToGameScene(effect.gameSceneID, effect.teleportPOS);
                            break;
                        case RPGEffect.TELEPORT_TYPE.position:
                            if (targetInfo == playerCombatInfo) playerCombatInfo.playerControllerREF.TeleportToTarget(effect.teleportPOS);
                            break;
                        case RPGEffect.TELEPORT_TYPE.target:
                            if (currentTarget != null && casterInfo == playerCombatInfo)
                                playerCombatInfo.playerControllerREF.TeleportToTarget(targetInfo);
                            break;
                    }

                    break;
                case RPGEffect.EFFECT_TYPE.pet:
                    var maxSummonCount = getCurrentSummonCount(casterInfo);
                    for (var x = 0; x < effect.petSPawnCount; x++)
                        if (casterInfo.currentPets.Count < maxSummonCount)
                        {
                            var newPet = Instantiate(effect.petPrefab, casterInfo.transform.position,
                                effect.petPrefab.transform.rotation);
                            var petNode = newPet.GetComponent<CombatNode>();
                            if(casterInfo!=playerCombatInfo)casterInfo.currentPetsCombatActionType = CombatNode.PET_COMBAT_ACTION_TYPES.aggro;
                            petNode.scaleWithOwner = effect.petScaleWithCharacter;
                            petNode.ownerCombatInfo = casterInfo;
                            casterInfo.currentPets.Add(petNode);
                            allCombatNodes.Add(petNode);
                            petNode.InitializeCombatNode();

                            float totalDuration = effect.petDuration + (effect.petDuration *
                                GetTotalOfStatType(casterInfo, RPGStat.STAT_TYPE.MINION_DURATION) / 100);
                            destroyPet(totalDuration, petNode);
                        }
                        else
                        {
                            break;
                        }

                    if (casterInfo == playerCombatInfo)
                    {
                        if (casterInfo.currentPets.Count == 1)
                        {
                            PetPanelDisplayManager.Instance.Show();
                        }
                        else if (casterInfo.currentPets.Count > 1)
                        {
                            if (PetPanelDisplayManager.Instance.thisCG.alpha == 0)
                            {
                                PetPanelDisplayManager.Instance.Show();
                            }
                            else
                            {
                                PetPanelDisplayManager.Instance.UpdateSummonCountText();
                                PetPanelDisplayManager.Instance.UpdateHealthBar();
                            }
                        }
                    }

                    break;
            }
        }

        private IEnumerator destroyPet(float duration, CombatNode nodeREF)
        {
            yield return  new WaitForSeconds(duration);
            
            
            HandleCombatNodeDEATH(nodeREF);
        }

        private void ExecuteAbilityEffects(CombatNode casterInfo, CombatNode targetInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {

            foreach (var t in rankREF.effectsApplied)
            {
                var thisEffect = RPGBuilderUtilities.GetEffectFromID(t.effectID);
                var rdmEffectChance = Random.Range(0f, 100f);
                if (!(rdmEffectChance <= t.chance)) continue;
                if (RPGBuilderUtilities.GetEffectFromID(t.effectID).isState)
                    InitNodeState(casterInfo, t.target == RPGCombatDATA.TARGET_TYPE.Target ? targetInfo : casterInfo, thisEffect, thisEffect.icon, rankREF);
                else
                    EFFECTS_LOGIC(thisEffect, casterInfo, t.target == RPGCombatDATA.TARGET_TYPE.Target ? targetInfo : casterInfo, rankREF);

                if (thisEffect.effectPulseGO == null) continue;
                TriggerPulseGO(thisEffect, targetInfo, t.target == RPGCombatDATA.TARGET_TYPE.Target ? targetInfo : casterInfo, ability, rankREF);
            }
        }

        public void TriggerPulseGO(RPGEffect effect, CombatNode targetInfo, CombatNode casterInfo, RPGAbility ability, RPGAbilityRankData rankREF)
        {
            var cbtVisualEffect =
                Instantiate(effect.effectPulseGO, targetInfo.transform.position, Quaternion.identity);
            var cbtVisualEff = cbtVisualEffect.GetComponent<CombatVisualEffect>();
            if (cbtVisualEff == null) return;
            if (effect.effectType == RPGEffect.EFFECT_TYPE.reflect)
                cbtVisualEff.InitCombatVisual(casterInfo, ability, casterInfo.appearanceREF, rankREF);
            else
                cbtVisualEff.InitCombatVisual(targetInfo, ability, targetInfo.appearanceREF, rankREF);
        }

        public void ExecuteEffect(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, RPGAbilityRankData rankREF)
        {
            if (effect.isState)
                InitNodeState(casterInfo, targetInfo, effect, effect.icon, rankREF);
            else
                EFFECTS_LOGIC(effect, casterInfo, targetInfo, rankREF);
        }

        public int getCurrentSummonCount(CombatNode nodeRef)
        {
            return (int)GetTotalOfStatType(nodeRef, RPGStat.STAT_TYPE.SUMMON_COUNT);
        }


        public bool isPet(CombatNode nodeInfo)
        {
            if (nodeInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
            {
                return nodeInfo.npcDATA != null && nodeInfo.ownerCombatInfo != null;
            }

            return false;
        }
        
        private int damageCalculation(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect,
            RPGAbilityRankData rankREF, bool showCombatText)
        {
            bool damageIsFromPet = isPet(casterInfo);
            float damage = effect.Damage;
            float elementalDMG = 0;
            float elementalRES = 0;
            float elementalPEN = 0;
            float resistanceTypeBonus = 0;
            float DMGDealtMOD = 0;
            float DMGTakenMOD = GetTotalOfStatType(targetInfo, RPGStat.STAT_TYPE.DMG_TAKEN);
            float CRITCHANCE = 0;
            float CRITPOWER = 0;
            var DamageTextType = effect.mainDamageType.ToString();
            if (DamageTextType == "NONE") DamageTextType = "NO_DAMAGE_TYPE";
            // CHECKING CASTER STATS
            foreach (var t in casterInfo.nodeStats)
            {
                switch (t.stat.statType)
                {
                    case RPGStat.STAT_TYPE.BASE_DAMAGE_TYPE:
                    {
                        if (effect.mainDamageType.ToString() == t.stat.StatFunction)
                        {
                            damage += t.curValue;

                            var oppositeStat = t.stat.OppositeStat;
                            if (oppositeStat != "NONE" && oppositeStat != "")
                                resistanceTypeBonus += targetInfo.getCurrentValue(oppositeStat);
                        }

                        break;
                    }
                    case RPGStat.STAT_TYPE.DMG_DEALT:
                        DMGDealtMOD += t.curValue;
                        break;
                    case RPGStat.STAT_TYPE.CRIT_CHANCE:
                        CRITCHANCE += t.curValue;
                        if (damageIsFromPet)
                        if (CRITCHANCE > 100) CRITCHANCE = 100;
                        break;
                    case RPGStat.STAT_TYPE.CRIT_POWER:
                        CRITPOWER += t.curValue;
                        break;
                }
            }


            if (damageIsFromPet)
            {
                CRITPOWER += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                    RPGStat.STAT_TYPE.MINION_CRIT_POWER);
                
                switch (effect.mainDamageType)
                {
                    case RPGEffect.MAIN_DAMAGE_TYPE.PHYSICAL_DAMAGE:
                        damage += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                            RPGStat.STAT_TYPE.MINION_PHYSICAL_DAMAGE);
                        break;
                    case RPGEffect.MAIN_DAMAGE_TYPE.MAGICAL_DAMAGE:
                        damage += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                            RPGStat.STAT_TYPE.MINION_MAGICAL_DAMAGE);
                        break;
                }
                
                
                DMGDealtMOD += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                    RPGStat.STAT_TYPE.MINION_DAMAGE);
                
                CRITCHANCE += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                    RPGStat.STAT_TYPE.MINION_CRIT_CHANCE);
                if (CRITCHANCE > 100) CRITCHANCE = 100;
                
                CRITPOWER += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                    RPGStat.STAT_TYPE.MINION_CRIT_POWER);
            }


            if (damage > 0)
                if (resistanceTypeBonus > 0)
                {
                    if (resistanceTypeBonus > 100) resistanceTypeBonus = 100;
                    resistanceTypeBonus /= 100;
                    resistanceTypeBonus = 1 - resistanceTypeBonus;

                    damage *= resistanceTypeBonus;
                }

            if (effect.secondaryDamageType != "NONE" && effect.secondaryDamageType != "")
            {
                // THIS EFFECT HAS A SECONDARY DAMAGE TYPE
                elementalDMG = casterInfo.getCurrentValue(effect.secondaryDamageType);

                var oppositeStat = targetInfo.nodeStats[casterInfo.getStatIndexFromName(effect.secondaryDamageType)]
                    .stat
                    .OppositeStat;
                if (oppositeStat != "NONE" && oppositeStat != "")
                {
                    // DAMAGE STAT HAS A RESISTANCE STAT
                    elementalRES = targetInfo.getCurrentValue(oppositeStat);

                    var penStat = casterInfo.nodeStats[casterInfo.getStatIndexFromName(oppositeStat)].stat.OppositeStat;
                    if (penStat != "NONE" && penStat != "")
                        // RESISTANCE STAT HAS A PENETRATION STAT
                        elementalPEN = casterInfo.getCurrentValue(penStat);
                }

                var finalRES = elementalRES - elementalPEN;
                if (finalRES < 0) finalRES = 0;
                finalRES /= 100;
                finalRES = 1 - finalRES;

                damage += elementalDMG;
                damage *= finalRES;
            }

            if (effect.skillModifierID != -1 && effect.skillModifier > 0)
            {
                damage += RPGBuilderUtilities.getSkillLevel(effect.skillModifierID);
            }

            if (isDamageCrit(CRITCHANCE))
            {
                var critBonus = RPGBuilderEssentials.Instance.combatSettings.CriticalDamageBonus + CRITPOWER;
                
                critBonus /= 100;
                damage *= critBonus;
                DamageTextType += "_CRITICAL";
            }
            
            if (effect.maxHealthModifier > 0)
            {
                damage += casterInfo.getCurrentMaxValue("HEALTH") * (effect.maxHealthModifier / 100);
            }

            if (effect.missingHealthModifier > 0)
            {
                float curMissingHealthPercent =
                    casterInfo.getCurrentValue("HEALTH") / casterInfo.getCurrentMaxValue("HEALTH");
                curMissingHealthPercent = 1 - curMissingHealthPercent;
                if (curMissingHealthPercent > 0)
                {
                    curMissingHealthPercent *= 100;
                    curMissingHealthPercent *= effect.missingHealthModifier;
                    curMissingHealthPercent /= 100;
                    damage += damage * curMissingHealthPercent;
                }
            }
            
            if (effect.effectType == RPGEffect.EFFECT_TYPE.damageOverTime)
            {
                float DOTBONUS = GetTotalOfStatType(casterInfo, RPGStat.STAT_TYPE.DOT_BONUS);

                if (DOTBONUS > 0)
                {
                    damage += damage * (DOTBONUS / 100);
                }
            }

            if (DMGDealtMOD != 0 || DMGTakenMOD != 0)
            {
                DMGDealtMOD /= 100;
                DMGTakenMOD /= 100;
                damage += damage * DMGDealtMOD;
                damage += damage * DMGTakenMOD;
            }

            switch (effect.mainDamageType)
            {
                case RPGEffect.MAIN_DAMAGE_TYPE.PHYSICAL_DAMAGE:
                {
                    
                    float BLOCKCHANCE = 0, BLOCKFLAT = 0, BLOCKMODIFIER = 0, DODGECHANCE = 0, GLANCINGBLOWCHANCE = 0;
                    foreach (var t in targetInfo.nodeStats)
                    {
                        switch (t.stat.statType)
                        {
                            case RPGStat.STAT_TYPE.BLOCK_CHANCE:
                                BLOCKCHANCE += t.curValue;
                                break;
                            case RPGStat.STAT_TYPE.BLOCK_FLAT:
                                BLOCKFLAT += t.curValue;
                                break;
                            case RPGStat.STAT_TYPE.BLOCK_MODIFIER:
                                BLOCKMODIFIER += t.curValue;
                                break;
                            case RPGStat.STAT_TYPE.DODGE_CHANCE:
                                DODGECHANCE += t.curValue;
                                break;
                            case RPGStat.STAT_TYPE.GLANCING_BLOW_CHANCE:
                                GLANCINGBLOWCHANCE += t.curValue;
                                break;
                        }
                    }
                    
                    if (damageIsFromPet)
                    {
                        DODGECHANCE += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                            RPGStat.STAT_TYPE.MINION_DODGE_CHANCE);
                        
                        GLANCINGBLOWCHANCE += GetTotalOfStatType(casterInfo.ownerCombatInfo,
                            RPGStat.STAT_TYPE.MINION_GLANCING_BLOW_CHANCE);
                    }

                    if (BLOCKCHANCE > 0)
                    {
                        if (Random.Range(0f, 100f) <= BLOCKCHANCE)
                        {
                            damage -= damage * (BLOCKMODIFIER / 100);
                            damage -= BLOCKFLAT;

                            if (damage < 0)
                            {
                                damage = 0;
                            }

                            DamageTextType = "BLOCKED";
                        
                            int HEALTHONBLOCK = (int) GetTotalOfStatType(targetInfo, RPGStat.STAT_TYPE.HEALTH_ON_BLOCK);
                            bool targetIsPet = isPet(targetInfo);
                            
                            if (targetIsPet)
                            {
                                HEALTHONBLOCK += (int)GetTotalOfStatType(targetInfo.ownerCombatInfo,
                                    RPGStat.STAT_TYPE.MINION_HEALTH_ON_BLOCK);
                            }
                            if (HEALTHONBLOCK > 0)
                            {
                                var heal = healingCalculation(targetInfo, targetInfo, null, HEALTHONBLOCK, true);
                                targetInfo.Heal(heal);
                            }
                        }
                    }
                    else if (DODGECHANCE > 0)
                    {
                        if (Random.Range(0f, 100f) <= DODGECHANCE)
                        {
                            damage = 0;
                            DamageTextType = "DODGED";
                        }
                    }
                    else if (GLANCINGBLOWCHANCE > 0)
                    {
                        if (Random.Range(0f, 100f) <= GLANCINGBLOWCHANCE)
                        {
                            damage /= 2;
                        }
                    }

                    break;
                }
                case RPGEffect.MAIN_DAMAGE_TYPE.MAGICAL_DAMAGE:
                    break;
            }

            if (targetInfo.isImmune())
            {
                DamageTextType = "IMMUNE";
            }
            else
            {

                if (rankREF != null)
                {
                    if (damage > 0 && AbilityHasTag(rankREF, RPGAbility.ABILITY_TAGS.onHit))
                    {
                        int HEALTHONHIT = (int) GetTotalOfStatType(casterInfo, RPGStat.STAT_TYPE.HEALTH_ON_HIT);
                        if (damageIsFromPet)
                        {
                            HEALTHONHIT += (int)GetTotalOfStatType(casterInfo.ownerCombatInfo,
                                RPGStat.STAT_TYPE.MINION_HEALTH_ON_HIT);
                        }
                        if (HEALTHONHIT > 0)
                        {
                            var heal = healingCalculation(casterInfo, casterInfo, null, HEALTHONHIT, true);
                            targetInfo.Heal(heal);
                        }

                        foreach (var t in casterInfo.nodeStats)
                        {
                            if (t.stat.statType != RPGStat.STAT_TYPE.EFFECT_TRIGGER) continue;
                            if (!(Random.Range(0f, 100f) <= t.curValue)) continue;
                            foreach (var t1 in t.stat.onHitEffectsData)
                            {
                                if (t1.tagType != RPGAbility.ABILITY_TAGS.onHit) continue;
                                if (Random.Range(0f, 100f) <= t1.chance)
                                {
                                    ExecuteEffect(casterInfo,
                                        t1.targetType == RPGCombatDATA.TARGET_TYPE.Caster ? casterInfo : targetInfo,
                                        RPGBuilderUtilities.GetEffectFromID(t1.effectID), null);
                                }
                            }
                        }
                    }
                }
            }

            if (showCombatText) HandleCombatTextTrigger(casterInfo, targetInfo, (int) damage, DamageTextType);

            return (int) damage;
        }

        public void HandleOnKillActions(CombatNode attacker, CombatNode deadUnit, RPGAbilityRankData rankREF)
        {
            if (rankREF == null) return;
            if (!AbilityHasTag(rankREF, RPGAbility.ABILITY_TAGS.onKill)) return;
            int HEALTHONKILL = (int) GetTotalOfStatType(attacker, RPGStat.STAT_TYPE.HEALTH_ON_KILL);
            if (isPet(attacker))
            {
                HEALTHONKILL += (int)GetTotalOfStatType(attacker.ownerCombatInfo,
                    RPGStat.STAT_TYPE.MINION_HEALTH_ON_KILL);
            }
            if (HEALTHONKILL > 0)
            {
                var heal = healingCalculation(attacker, attacker, null, HEALTHONKILL, true);
                attacker.Heal(heal);
            }

            foreach (var t in attacker.nodeStats)
            {
                if (t.stat.statType != RPGStat.STAT_TYPE.EFFECT_TRIGGER) continue;
                if (!(Random.Range(0f, 100f) <= t.curValue)) continue;
                foreach (var t1 in t.stat.onHitEffectsData)
                {
                    if (t1.tagType != RPGAbility.ABILITY_TAGS.onKill) continue;
                    if (Random.Range(0f, 100f) <= t1.chance)
                    {
                        ExecuteEffect(attacker, attacker, RPGBuilderUtilities.GetEffectFromID(t1.effectID), null);
                    }
                }
            }
        }

        private bool AbilityHasTag(RPGAbilityRankData rankREF, RPGAbility.ABILITY_TAGS tag)
        {
            foreach (var t in rankREF.tagsData)
            {
                if (t.tag == tag) return true;
            }

            return false;
        }

        private int healingCalculation(CombatNode casterInfo, CombatNode targetInfo, RPGEffect effect, int baseValue, bool showCombatText)
        {
            float heal = effect != null ? effect.Damage : baseValue;
            float elementalDMG = 0;
            float elementalAbsorb = 0;
            float DMGDealtMOD = 0;
            float DMGTakenMOD = GetTotalOfStatType(targetInfo, RPGStat.STAT_TYPE.HEAL_RECEIVED);
            float CRITCHANCE = 0;
            float CRITPOWER = 0;
            var DamageTextType = "HEAL";

            // CHECKING CASTER STATS
            foreach (var t in casterInfo.nodeStats)
                switch (t.stat.statType)
                {
                    case RPGStat.STAT_TYPE.HEAL_DONE:
                        DMGDealtMOD += t.curValue;
                        break;
                    case RPGStat.STAT_TYPE.CRIT_CHANCE:
                    {
                        CRITCHANCE += t.curValue;
                        if (CRITCHANCE > 100) CRITCHANCE = 100;
                        break;
                    }
                    case RPGStat.STAT_TYPE.CRIT_POWER:
                    {
                        CRITPOWER += t.curValue;
                        break;
                    }
                    case RPGStat.STAT_TYPE.GLOBAL_HEALING:
                        heal += t.curValue;
                        break;
                }


            if (effect!=null && effect.secondaryDamageType != "NONE" && effect.secondaryDamageType != "")
            {
                // THIS EFFECT HAS A SECONDARY DAMAGE TYPE
                elementalDMG = casterInfo.getCurrentValue(effect.secondaryDamageType);

                var oppositeStat = targetInfo.nodeStats[casterInfo.getStatIndexFromName(effect.secondaryDamageType)].stat
                    .OppositeStat;
                if (oppositeStat != "NONE" && oppositeStat != "")
                    // DAMAGE STAT HAS A RESISTANCE STAT
                    elementalAbsorb = targetInfo.getCurrentValue(oppositeStat);

                var finalRES = elementalAbsorb;
                if (finalRES < 0) finalRES = 0;
                finalRES /= 100;
                finalRES = 1 + finalRES;

                heal += elementalDMG;
                heal *= finalRES;
            }

            if (effect!=null && effect.skillModifierID != -1 && effect.skillModifier > 0)
            {
                heal += RPGBuilderUtilities.getSkillLevel(effect.skillModifierID);
            }
            
            if (isDamageCrit(CRITCHANCE))
            {
                var critBonus = RPGBuilderEssentials.Instance.combatSettings.CriticalDamageBonus + CRITPOWER;
                critBonus /= 100;
                heal *= critBonus;
                DamageTextType += "_CRITICAL";
            }

            if (DMGDealtMOD != 0 || DMGTakenMOD != 0)
            {
                DMGDealtMOD /= 100;
                DMGTakenMOD /= 100;
                heal += heal * DMGDealtMOD;
                heal += heal * DMGTakenMOD;
            }
            
            if (effect!=null && effect.effectType == RPGEffect.EFFECT_TYPE.healOverTime)
            {
                float HOTBONUS = GetTotalOfStatType(casterInfo, RPGStat.STAT_TYPE.HOT_BONUS);

                if (HOTBONUS > 0)
                {
                    heal += heal * (HOTBONUS / 100);
                }
            }

            if (showCombatText) HandleCombatTextTrigger(casterInfo, targetInfo, (int) heal, DamageTextType);

            return (int) heal;
        }

        private bool isDamageCrit(float critRate)
        {
            if (!(critRate > 0)) return false;
            float critChance = Random.Range(0, 100);
            return critChance <= critRate;

        }


        private int getLifesteal(CombatNode casterInfo, RPGEffect effect, int dmg)
        {
            float lifestealStat = 0;

            foreach (var t in casterInfo.nodeStats)
                if (t.stat.statType == RPGStat.STAT_TYPE.LIFESTEAL)
                    lifestealStat += t.curValue;

            if (isPet(casterInfo))
            {
                lifestealStat += (int)GetTotalOfStatType(casterInfo.ownerCombatInfo,
                    RPGStat.STAT_TYPE.MINION_LIFESTEAL);
            }

            var curLifesteal = (effect.lifesteal / 100) + (lifestealStat / 100);
            return (int) (dmg * curLifesteal);
        }


        private List<CombatNode> getClosestUnits(CombatNode playerCombatInfo, Collider[] hitColliders, int maxUnitHit)
        {
            var closestUnits = new List<CombatNode>();
            var allDistances = new List<float>();

            foreach (var t in hitColliders)
                if (allDistances.Count >= maxUnitHit)
                {
                    var dist = Vector3.Distance(playerCombatInfo.gameObject.transform.position, t.transform.position);
                    var CurBiggestDistanceInArray = Mathf.Max(allDistances.ToArray());
                    var IndexOfBiggest = allDistances.IndexOf(CurBiggestDistanceInArray);
                    if (!(dist < CurBiggestDistanceInArray)) continue;
                    allDistances[IndexOfBiggest] = dist;
                    closestUnits[IndexOfBiggest] = t.GetComponent<CombatNode>();
                }
                else
                {
                    allDistances.Add(Vector3.Distance(playerCombatInfo.gameObject.transform.position,
                        t.transform.position));
                    closestUnits.Add(t.GetComponent<CombatNode>());
                }

            return closestUnits;
        }

        private List<CombatNode> getClosestUnits(CombatNode playerCombatInfo, List<Collider> hitColliders, int maxUnitHit)
        {
            var closestUnits = new List<CombatNode>();
            var allDistances = new List<float>();

            foreach (var t in hitColliders)
                if (allDistances.Count >= maxUnitHit)
                {
                    var dist = Vector3.Distance(playerCombatInfo.gameObject.transform.position, t.transform.position);
                    var CurBiggestDistanceInArray = Mathf.Max(allDistances.ToArray());
                    var IndexOfBiggest = allDistances.IndexOf(CurBiggestDistanceInArray);
                    if (!(dist < CurBiggestDistanceInArray)) continue;
                    allDistances[IndexOfBiggest] = dist;
                    closestUnits[IndexOfBiggest] = t.GetComponent<CombatNode>();
                }
                else
                {
                    allDistances.Add(Vector3.Distance(playerCombatInfo.gameObject.transform.position,
                        t.transform.position));
                    closestUnits.Add(t.GetComponent<CombatNode>());
                }

            return closestUnits;
        }

        private List<CombatNode> getClosestNearbyUnits(GameObject projGO, List<Collider> hitColliders, int maxUnitHit)
        {
            var closestUnits = new List<CombatNode>();
            var allDistances = new List<float>();

            foreach (var t in hitColliders)
                if (allDistances.Count >= maxUnitHit)
                {
                    var dist = Vector3.Distance(projGO.transform.position, t.transform.position);
                    var CurBiggestDistanceInArray = Mathf.Max(allDistances.ToArray());
                    var IndexOfBiggest = allDistances.IndexOf(CurBiggestDistanceInArray);
                    if (!(dist < CurBiggestDistanceInArray)) continue;
                    allDistances[IndexOfBiggest] = dist;
                    closestUnits[IndexOfBiggest] = t.GetComponent<CombatNode>();
                }
                else
                {
                    allDistances.Add(Vector3.Distance(projGO.transform.position, t.transform.position));
                    closestUnits.Add(t.GetComponent<CombatNode>());
                }

            return closestUnits;
        }
    }
}