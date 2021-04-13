using System.Collections.Generic;
using THMSV.RPGBuilder.AI;
using THMSV.RPGBuilder.Character;
using THMSV.RPGBuilder.DisplayHandler;
using THMSV.RPGBuilder.Logic;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using THMSV.RPGBuilder.UIElements;
using THMSV.RPGBuilder.Utility;
using THMSV.RPGBuilder.World;
using UnityEngine;

namespace THMSV.RPGBuilder.LogicMono
{
    public class CombatNode : MonoBehaviour
    {
        public enum COMBAT_NODE_TYPE
        {
            mob,
            player,
            objectAction,
            pet
        }

        public COMBAT_NODE_TYPE nodeType;

        [System.Serializable]
        public class NodeStatesDATA
        {
            public string stateName;
            public CombatNode casterNode;
            public RPGEffect stateEffect;
            public RPGAbilityRankData rankREF;
            public Sprite stateIcon;
            public int maxPulses;
            public int curPulses;
            public float nextPulse;
            public float pulseInterval;
            public float stateMaxDuration;
            public float stateCurDuration;
            public int curStack;
            public int maxStack;
            public List<float> modifiedStatValues = new List<float>();
            public GameObject stateEffectGO;
        }

        public List<NodeStatesDATA> nodeStateData;
        public RPGNpc npcDATA;
        public NPCSpawner spawnerREF;
        public RPGBAIAgent agentREF;
        public int NPCLevel;
        public RPGBCharacterController playerControllerREF;
        public Renderer thisRendererREF;
        public float nameplateYOffset;
        public PlayerAppearanceHandler appearanceREF;
        public GroundIndicatorManager indicatorManagerREF;

        public enum PET_MOVEMENT_ACTION_TYPES
        {
            follow,
            stay
        }


        public PET_MOVEMENT_ACTION_TYPES currentPetsMovementActionType = PET_MOVEMENT_ACTION_TYPES.follow;

        public enum PET_COMBAT_ACTION_TYPES
        {
            defend,
            aggro
        }

        public PET_COMBAT_ACTION_TYPES currentPetsCombatActionType = PET_COMBAT_ACTION_TYPES.defend;
        public List<CombatNode> currentPets = new List<CombatNode>();
        public CombatNode ownerCombatInfo;
        public bool IsCasting, IsChanneling, isInteractiveNodeCasting;

        public float currentCastProgress,
            targetCastTime,
            currentChannelProgress,
            targetChannelTime,
            currentInteractionProgress,
            targetInteractionTime;

        public int currentAbilityCastedSlot;
        public RPGAbilityRankData currentAbilityCastedCurRank;
        public RPGAbility currentAbilityCasted;
        public CombatNode currentTargetCasted;
        public InteractiveNode currentInteractiveNodeCasted;
        public bool scaleWithOwner;
        public bool dead = false;
        public int currentAutoAttackAbilityID;
        private float nextAutoAttack;
        public float lastCombatActionTimer;
        private float outOfCombatDuration = 15;
        public bool inCombat;

        [System.Serializable]
        public class AbilitiesDATA
        {
            public string slot;
            public int curAbilityID;
            public RPGAbility currentAbility;
            public RPGAbilityRankData currentAbilityRankData;
            public float NextTimeUse, CDLeft;
        }
        public List<AbilitiesDATA> abilitiesData;

        public AbilitiesDATA getAbilityDATA(RPGAbility ab)
        {
            foreach (var t in abilitiesData)
                if (t.currentAbility == ab)
                    return t;

            return null;
        }

        public int getAbilityDATAIndex(RPGAbility ab)
        {
            for (var i = 0; i < abilitiesData.Count; i++)
                if (abilitiesData[i].currentAbility == ab)
                    return i;
            return -1;
        }

        [System.Serializable]
        public class NODE_STATS
        {
            public string _name;
            public RPGStat stat;
            public float curMinValue;
            public float curMaxValue;
            public float curValue;
            public float nextShift;
        }

        public List<NODE_STATS> nodeStats = new List<NODE_STATS>();

        public float getCurrentValue(string StatName)
        {
            foreach (var t in nodeStats)
                if (t.stat._name == StatName)
                    return t.curValue;
            return 0;
        }

        public void setCurrentValue(string StatName, float newValue)
        {
            foreach (var t in nodeStats)
                if (t.stat._name == StatName)
                    t.curValue = (int)newValue;
        }

        public float getCurrentMaxValue(string StatName)
        {
            foreach (var t in nodeStats)
                if (t.stat._name == StatName)
                    return t.curMaxValue;
            return 0;
        }

        public void setCurrentMaxValue(string StatName, float newValue)
        {
            foreach (var t in nodeStats)
                if (t.stat._name == StatName)
                    t.curMaxValue = (int)newValue;
        }

        public float getCurrentMinValue(string StatName)
        {
            foreach (var t in nodeStats)
                if (t.stat._name == StatName)
                    return t.curMinValue;
            return 0;
        }

        public void setCurrentMinValue(string StatName, float newValue)
        {
            foreach (var t in nodeStats)
                if (t.stat._name == StatName)
                    t.curMinValue = newValue;
        }

        public bool isLeaping;
        private Vector3 LeapEndPOS;
        private Vector3 startPos;
        private float leapHeight;
        private float leapSpeed;
        public CombatNode nodeTarget;

        public void InitLeap(Vector3 startpos, Vector3 endpos, float height, float speed)
        {
            isLeaping = true;
            startPos = startpos;
            LeapEndPOS = new Vector3(endpos.x, endpos.y - 0.04f, endpos.z);
            leapHeight = height;
            leapSpeed = speed;
        }

        private float leapAnimation;

        private void checkIsLeaping()
        {
            if (!isLeaping) return;
            leapAnimation += Time.deltaTime;
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 1000, playerControllerREF.GroundLayers))
            {
                if (leapAnimation > 0.04f && hit.distance <= 0.1f)
                {
                    CombatManager.Instance.LeapEnded(this);
                    CombatManager.playerCombatInfo.playerControllerREF.EndGroundLeap();
                    isLeaping = false;
                    leapAnimation = 0;
                    leapHeight = 0;
                    leapSpeed = 0;
                    Debug.LogError("too close from ground, we stop the leap");
                    return;
                }
            }

            transform.position = MathParabola.Parabola(startPos, LeapEndPOS, leapHeight, leapAnimation / leapSpeed);
            if (!(Vector3.Distance(transform.position, LeapEndPOS) < 0.025f)) return;
            CombatManager.Instance.LeapEnded(this);
            CombatManager.playerCombatInfo.playerControllerREF.EndGroundLeap();
            isLeaping = false;
            leapAnimation = 0;
            leapHeight = 0;
            leapSpeed = 0;
        }

        private void checkIsCasting()
        {
            if (!IsCasting) return;
            currentCastProgress += Time.deltaTime;
            if (nodeType == COMBAT_NODE_TYPE.player)
            {
                if (!currentAbilityCastedCurRank.castInRun && playerControllerREF.GetDesiredSpeed() != 0)
                {
                    ResetCasting();
                    return;
                }

                if(currentAbilityCastedCurRank.castBarVisible) PlayerInfoDisplayManager.Instance.UpdateCastBar(currentCastProgress, targetCastTime);
            }

            if (!(currentCastProgress >= targetCastTime)) return;

            CombatManager.Instance.HandleAbilityTypeActions(this, currentAbilityCasted, currentAbilityCastedCurRank, false);

            if (!currentAbilityCastedCurRank.startCDOnActivate)
                CombatManager.Instance.StartCooldown(this, currentAbilityCastedSlot, currentAbilityCastedCurRank.cooldown,
                    currentAbilityCasted.ID);
            CombatManager.Instance.HandleAbilityCost(this, currentAbilityCasted, currentAbilityCastedCurRank);
            ResetCasting();
        }

        private void checkIsChanneling()
        {
            if (!IsChanneling) return;
            var curRank = 0;
            if (nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.objectAction ||
                nodeType == COMBAT_NODE_TYPE.pet) curRank = 0;
            else curRank = RPGBuilderUtilities.getNodeCurrentRank(currentAbilityCasted);
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(currentAbilityCasted.ranks[curRank].rankID);
            currentChannelProgress -= Time.deltaTime;
            if (nodeType == COMBAT_NODE_TYPE.player)
            {
                if (!rankREF.castInRun && playerControllerREF.GetDesiredSpeed() != 0)
                {
                    ResetChanneling();
                    return;
                }

                PlayerInfoDisplayManager.Instance.UpdateChannelBar(currentChannelProgress, rankREF.channelTime);
            }

            if (currentChannelProgress <= 0) ResetChanneling();
        }

        private void checkIsInteractiveNodeCasting()
        {
            if (!isInteractiveNodeCasting) return;
            currentInteractionProgress += Time.deltaTime;
            if (nodeType == COMBAT_NODE_TYPE.player)
                PlayerInfoDisplayManager.Instance.UpdateInteractionBar(currentInteractionProgress, targetInteractionTime);
        
            if (playerControllerREF.GetDesiredSpeed() != 0)
            {
                ResetInteractiveNodeCasting();
                return;
            }
        
            if (!(currentInteractionProgress >= targetInteractionTime)) return;
            currentInteractiveNodeCasted.UseNode();
            ResetInteractiveNodeCasting();
        }

        private void HandleCombatState()
        {
            lastCombatActionTimer += Time.deltaTime;
            if (!(lastCombatActionTimer >= outOfCombatDuration)) return;
            CombatManager.Instance.ResetCombat(this);
        }
        private void HandleStatShifting()
        {
            if (dead) return;
            foreach (var t in nodeStats)
                if (t.stat.statType == RPGStat.STAT_TYPE.VITALITY && t.stat.isShifting && Time.time >= t.nextShift)
                {
                    t.nextShift = Time.time + t.stat.shiftInterval;
                    UpdateStat(t.stat._name, "curBase", t.stat.shiftAmount + GetTotalAmountOfBonusShift(t.stat));
                }
        }

        float GetTotalAmountOfBonusShift(RPGStat statREF)
        {
            float total = 0;
            foreach (var t in nodeStats)
            {
                if (t.stat.statType == RPGStat.STAT_TYPE.VITALITY_REGEN && t.stat.vitalityRegenBonusStatID == statREF.ID)
                {
                    total += t.curValue;
                }
            }

            return total;
        }

        public bool autoAttackIsReady()
        {
            return Time.time >= nextAutoAttack;
        }

        public void InitAACooldown(float nextAA)
        {
            nextAutoAttack = Time.time + nextAA;
        }

        private void HandleAutoAttack()
        {
            if (currentAutoAttackAbilityID == -1) return;
            if (!(Time.time >= nextAutoAttack)) return;
            var abilityREF = RPGBuilderUtilities.GetAbilityFromID(currentAutoAttackAbilityID);
            if (abilityREF == null) return;
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(abilityREF.ranks[0].rankID);
            if (rankREF != null) CombatManager.Instance.InitAbility(CombatManager.playerCombatInfo, abilityREF, false);
        }

        private void FixedUpdate()
        {
            if (inCombat)
                HandleCombatState();
            else
                HandleStatShifting();

            checkIsLeaping();
            checkIsCasting();
            checkIsChanneling();
            checkIsInteractiveNodeCasting();
            UpdateStates();
            UpdateCooldowns();
            if (nodeType == COMBAT_NODE_TYPE.player && CombatManager.Instance.currentTarget != null) HandleAutoAttack();
        }


        private void ResetInteractiveNodeCasting()
        {
            isInteractiveNodeCasting = false;
            currentInteractiveNodeCasted = null;
            currentInteractionProgress = 0;
            targetInteractionTime = 0;
            if (nodeType == COMBAT_NODE_TYPE.player) PlayerInfoDisplayManager.Instance.ResetInteractionBarBar();
        }

        private void ResetCasting()
        {
            if (nodeType == COMBAT_NODE_TYPE.player && currentAbilityCastedCurRank.castBarVisible) PlayerInfoDisplayManager.Instance.ResetCastBar();
            IsCasting = false;
            currentAbilityCasted = null;
            currentTargetCasted = null;
            currentCastProgress = 0;
            targetCastTime = 0;
            currentAbilityCastedSlot = -1;
        }

        private void ResetChanneling()
        {
            IsChanneling = false;
            currentAbilityCasted = null;
            currentChannelProgress = 0;
            targetChannelTime = 0;
            currentAbilityCastedSlot = -1;
            PlayerInfoDisplayManager.Instance.ResetChannelBar();
        }

        private void UpdateStates()
        {
            for (var i = 0; i < nodeStateData.Count; i++)
            {
                nodeStateData[i].stateCurDuration += Time.deltaTime;
                if (nodeStateData[i].curPulses > 0) nodeStateData[i].nextPulse -= Time.deltaTime;
                if (nodeStateData[i].nextPulse <= 0 && nodeStateData[i].curPulses < nodeStateData[i].maxPulses)
                {
                    nodeStateData[i].nextPulse = nodeStateData[i].pulseInterval;
                    nodeStateData[i].curPulses++;
                    switch (nodeStateData[i].stateEffect.effectType)
                    {
                        case RPGEffect.EFFECT_TYPE.damageOverTime:
                            CombatManager.Instance.ExecuteDOTPulse(nodeStateData[i].casterNode, this,
                                nodeStateData[i].stateEffect, nodeStateData[i].curStack, nodeStateData[i].rankREF);
                            break;
                        case RPGEffect.EFFECT_TYPE.healOverTime:
                            CombatManager.Instance.ExecuteHOTPulse(nodeStateData[i].casterNode, this,
                                nodeStateData[i].stateEffect, nodeStateData[i].curStack);
                            break;
                        case RPGEffect.EFFECT_TYPE.stat:
                            CombatManager.Instance.ExecuteStatPulse(nodeStateData[i].casterNode, this,
                                nodeStateData[i].stateEffect, nodeStateData[i].curStack, i);
                            break;
                    }

                    if (i+1 > nodeStateData.Count) return;
                    if (nodeStateData[i].stateEffect.effectPulseGO != null)
                        CombatManager.Instance.TriggerPulseGO(nodeStateData[i].stateEffect, this,
                            nodeStateData[i].casterNode, null, null);
                }


                if (!(nodeStateData[i].stateCurDuration >= nodeStateData[i].stateMaxDuration)) continue;
                if (nodeStateData[i].stateEffect.effectType == RPGEffect.EFFECT_TYPE.stat)
                    for (var x = 0; x < nodeStateData[i].stateEffect.statEffectsData.Count; x++)
                        CombatManager.Instance.RemoveStatModification(this,
                            RPGBuilderUtilities.GetStatFromID(nodeStateData[i].stateEffect.statEffectsData[x].statID),
                            nodeStateData[i].modifiedStatValues[x]);
                if ((nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.pet) &&
                    (nodeStateData[i].stateEffect.effectType == RPGEffect.EFFECT_TYPE.stun ||
                     nodeStateData[i].stateEffect.effectType == RPGEffect.EFFECT_TYPE.sleep ||
                     nodeStateData[i].stateEffect.effectType == RPGEffect.EFFECT_TYPE.root))
                    if (agentREF != null) agentREF.ResetStun();

                nodeStateData.RemoveAt(i);
                return;
            }
        }

        public void CancelState(int stateIndex)
        {
            if (nodeStateData[stateIndex].stateEffect.effectType == RPGEffect.EFFECT_TYPE.stat)
                for (var x = 0; x < nodeStateData[stateIndex].stateEffect.statEffectsData.Count; x++)
                    CombatManager.Instance.RemoveStatModification(this,
                        RPGBuilderUtilities.GetStatFromID(nodeStateData[stateIndex].stateEffect.statEffectsData[x].statID),
                        nodeStateData[stateIndex].modifiedStatValues[x]);
            if ((nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.pet) &&
                (nodeStateData[stateIndex].stateEffect.effectType == RPGEffect.EFFECT_TYPE.stun ||
                 nodeStateData[stateIndex].stateEffect.effectType == RPGEffect.EFFECT_TYPE.sleep ||
                 nodeStateData[stateIndex].stateEffect.effectType == RPGEffect.EFFECT_TYPE.root))
                if (agentREF != null) agentREF.ResetStun();

            if (nodeType == COMBAT_NODE_TYPE.player)
                for (var i = 0; i < PlayerStatesDisplayHandler.Instance.curStatesSlots.Count; i++) PlayerStatesDisplayHandler.Instance.RemoveState(i);

            nodeStateData.RemoveAt(stateIndex);
        }

        public void CancelState(RPGEffect effect)
        {
            for (var i = 0; i < nodeStateData.Count; i++)
                if (nodeStateData[i].stateEffect == effect)
                {
                    if (nodeStateData[i].stateEffect.effectType == RPGEffect.EFFECT_TYPE.stat)
                        for (var x = 0; x < nodeStateData[i].stateEffect.statEffectsData.Count; x++)
                            CombatManager.Instance.RemoveStatModification(this,
                                RPGBuilderUtilities.GetStatFromID(nodeStateData[i].stateEffect.statEffectsData[x].statID),
                                nodeStateData[i].modifiedStatValues[x]);
                    nodeStateData.RemoveAt(i);
                }
        }

        public void InterruptCastActions()
        {
            GetComponent<Animator>().Rebind();
            if(IsCasting)ResetCasting();
            if(IsChanneling)ResetChanneling();
            if(isInteractiveNodeCasting)ResetInteractiveNodeCasting();
        }

        public void InitInteracting(InteractiveNode interactiveNode, float duration)
        {
            isInteractiveNodeCasting = true;
            currentInteractiveNodeCasted = interactiveNode;
            currentInteractionProgress = 0;
            targetInteractionTime = interactiveNode.interactionTime;
        }

        public void InitCasting(RPGAbility thisAbility, int abSlotIndex, RPGAbilityRankData rankREF)
        {
            IsCasting = true;
            currentAbilityCasted = thisAbility;
            currentAbilityCastedCurRank = rankREF;
            currentCastProgress = 0;
            targetCastTime = CombatManager.Instance.CalculateCastTime(this, rankREF.castTime);
            currentAbilityCastedSlot = abSlotIndex;
            if (rankREF.startCDOnActivate)
                CombatManager.Instance.StartCooldown(this, abSlotIndex, rankREF.cooldown, thisAbility.ID);
            if (nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.pet)
                ScreenSpaceNameplates.Instance.InitACastBar(this, thisAbility);
        }

        public void InitChanneling(RPGAbility thisAbility, int abSlotIndex)
        {
            var curRank = 0;
            if (nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.objectAction ||
                nodeType == COMBAT_NODE_TYPE.pet) curRank = 0;
            else curRank = RPGBuilderUtilities.getNodeCurrentRank(thisAbility);
            var rankREF = RPGBuilderUtilities.GetAbilityRankFromID(thisAbility.ranks[curRank].rankID);
            IsChanneling = true;
            currentAbilityCasted = thisAbility;
            currentChannelProgress = rankREF.channelTime;
            targetChannelTime = 0;
            currentAbilityCastedSlot = abSlotIndex;
            if (nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.pet)
                ScreenSpaceNameplates.Instance.InitAChannelBar(this, thisAbility);
        }

        public bool isStunned()
        {
            foreach (var t in nodeStateData)
                if (t.stateEffect.effectType == RPGEffect.EFFECT_TYPE.stun)
                    return true;

            return false;
        }

        public bool isSilenced()
        {
            foreach (var t in nodeStateData)
                if (t.stateEffect.effectType == RPGEffect.EFFECT_TYPE.silence)
                    return true;

            return false;
        }

        public bool isSleeping()
        {
            foreach (var t in nodeStateData)
                if (t.stateEffect.effectType == RPGEffect.EFFECT_TYPE.sleep)
                    return true;

            return false;
        }

        public bool isRooted()
        {
            foreach (var t in nodeStateData)
                if (t.stateEffect.effectType == RPGEffect.EFFECT_TYPE.root)
                    return true;

            return false;
        }

        public bool isTaunted()
        {
            foreach (var t in nodeStateData)
                if (t.stateEffect.effectType == RPGEffect.EFFECT_TYPE.taunt)
                    return true;

            return false;
        }

        public bool isImmune()
        {
            foreach (var t in nodeStateData)
                if (t.stateEffect.effectType == RPGEffect.EFFECT_TYPE.immune)
                    return true;

            return false;
        }

        private void UpdateCooldowns()
        {
            if (nodeType == COMBAT_NODE_TYPE.player) return;
            foreach (var t in abilitiesData)
                if (t.NextTimeUse > 0)
                {
                    t.CDLeft -= Time.deltaTime;
                    if (t.CDLeft <= 0)
                    {
                        t.CDLeft = 0;
                        t.NextTimeUse = 0;
                    }
                }
        }

        public void InitStats()
        {
            foreach (var t in RPGBuilderEssentials.Instance.allStats)
            {
                var newAttributeToLoad = new NODE_STATS();
                newAttributeToLoad._name = t._name;
                newAttributeToLoad.stat = t;
                if (nodeType == COMBAT_NODE_TYPE.player)
                {
                    newAttributeToLoad.curMinValue = t.minValue;
                    newAttributeToLoad.curValue = t.baseValue;
                    newAttributeToLoad.curMaxValue = t.maxValue;
                }
                else
                {
                    if (npcHasCustomStat(t))
                    {
                        var index = getCustomStatIndex(t);
                        if (index != -1)
                        {
                            newAttributeToLoad.curMinValue = npcDATA.stats[index].minValue;
                            newAttributeToLoad.curValue = npcDATA.stats[index].baseValue;
                            newAttributeToLoad.curMaxValue = npcDATA.stats[index].maxValue;
                            if (RPGBuilderUtilities.GetStatFromID(npcDATA.stats[index].statID).statType ==
                                RPGStat.STAT_TYPE.VITALITY)
                            {
                                newAttributeToLoad.curMaxValue += npcDATA.stats[index].bonusPerLevel * NPCLevel;
                                newAttributeToLoad.curValue = newAttributeToLoad.curMaxValue;
                            }
                            else
                            {
                                newAttributeToLoad.curValue += npcDATA.stats[index].bonusPerLevel * NPCLevel;
                            }

                            if (nodeType == COMBAT_NODE_TYPE.pet)
                            {
                                if (newAttributeToLoad.stat._name == "HEALTH")
                                {
                                    newAttributeToLoad.curMaxValue += newAttributeToLoad.curMaxValue * (CombatManager.Instance.GetTotalOfStatType(ownerCombatInfo, RPGStat.STAT_TYPE.MINION_HEALTH)/100);
                                    newAttributeToLoad.curValue = newAttributeToLoad.curMaxValue;
                                }
                            }
                        }
                    }
                    else
                    {
                        newAttributeToLoad.curMinValue = t.minValue;
                        newAttributeToLoad.curValue = t.baseValue;
                        newAttributeToLoad.curMaxValue = t.maxValue;
                        if (t.statType == RPGStat.STAT_TYPE.VITALITY)
                        {
                            newAttributeToLoad.curMaxValue +=
                                t.bonusPerLevel * NPCLevel;
                            newAttributeToLoad.curValue = newAttributeToLoad.curMaxValue;
                        }
                        else
                        {
                            newAttributeToLoad.curValue +=
                                t.bonusPerLevel * NPCLevel;
                        }

                        if (nodeType == COMBAT_NODE_TYPE.pet)
                        {
                            if (newAttributeToLoad.stat._name == "HEALTH")
                            {
                                newAttributeToLoad.curMaxValue += newAttributeToLoad.curMaxValue * (CombatManager.Instance.GetTotalOfStatType(ownerCombatInfo, RPGStat.STAT_TYPE.MINION_HEALTH)/100);
                                newAttributeToLoad.curValue = newAttributeToLoad.curMaxValue;
                            }
                        }
                    }
                }

                nodeStats.Add(newAttributeToLoad);
            }

            if (nodeType != COMBAT_NODE_TYPE.player) return;
            StatCalculator.InitCharacterStats();
            CombatManager.Instance.InitAllStatBar();
        }

        private int getCustomStatIndex(RPGStat stat)
        {
            for (var i = 0; i < npcDATA.stats.Count; i++)
                if (npcDATA.stats[i].statID == stat.ID)
                    return i;
            return -1;
        }

        private bool npcHasCustomStat(RPGStat stat)
        {
            foreach (var t in npcDATA.stats)
                if (t.statID == stat.ID)
                    return true;

            return false;
        }

        private void Start()
        {
            // INIT PLAYER UI
            if (nodeType != COMBAT_NODE_TYPE.player) return;
            CombatManager.playerCombatInfo = this;
            CombatManager.groundIndicatorManager = indicatorManagerREF;
        }

        public void InitializeCombatNode()
        {
            if (nodeType != COMBAT_NODE_TYPE.mob && nodeType != COMBAT_NODE_TYPE.pet) return;
            InitNPCAbilities();
            InitNPCLevel();
            InitStats();
            InitCollisions();
            if (thisRendererREF != null)
                ScreenSpaceNameplates.Instance.RegisterNewNameplate(thisRendererREF, this, gameObject, nameplateYOffset,
                    false);
            if (nodeType == COMBAT_NODE_TYPE.pet)
                agentREF.curMovementState = RPGBAIAgent.AGENT_MOVEMENT_STATES.followOwner;
        }

        private void InitCollisions()
        {
            Collider collider = gameObject.GetComponent<Collider>();
            foreach (var t in CombatManager.Instance.allCombatNodes)
            {
                Physics.IgnoreCollision(collider,
                    t.nodeType == COMBAT_NODE_TYPE.player
                        ? t.gameObject.GetComponent<CharacterController>()
                        : t.gameObject.GetComponent<Collider>());
            }
        }

        private void InitNPCLevel()
        {
            NPCLevel = scaleWithOwner ? CharacterData.Instance.classDATA.currentClassLevel : Random.Range(npcDATA.MinLevel, npcDATA.MaxLevel + 1);
        }

        private void InitNPCAbilities()
        {
            if (npcDATA == null) return;
            foreach (var t in npcDATA.abilities)
            {
                var abilityREF = RPGBuilderUtilities.GetAbilityFromID(t.abilityID);
                if (abilityREF == null) continue;
                var newAb = new AbilitiesDATA();
                newAb.slot = abilityREF._name;
                newAb.currentAbility = abilityREF;
                newAb.curAbilityID = abilityREF.ID;
                newAb.currentAbilityRankData = RPGBuilderUtilities.GetAbilityRankFromID(abilityREF.ranks[0].rankID);
                abilitiesData.Add(newAb);
            }
        }

        public void TakeDamage(CombatNode attacker, int Amount, RPGAbilityRankData rankREF)
        {
            bool died = false;
            ScreenSpaceNameplates.Instance.SetNPToVisible(this);
            if (nodeType == COMBAT_NODE_TYPE.mob || nodeType == COMBAT_NODE_TYPE.pet)
            {
                if (npcDATA.isDummyTarget)
                {
                    if (Amount >= nodeStats[getStatIndexFromName("HEALTH")].curValue)
                        Heal((int) nodeStats[getStatIndexFromName("HEALTH")].curMaxValue);
                    else 
                        died = UpdateStat("HEALTH", "curBase", -Amount);
                }
                else
                {
                    died = UpdateStat("HEALTH", "curBase", -Amount);
                }

                if (npcDATA.npcType == RPGNpc.NPC_TYPE.BOSS)
                {
                    if (BossUISlotHolder.Instance.thisNode == null)
                    {
                        BossUISlotHolder.Instance.Init(this);
                    }
                }

                if (agentREF != null) agentREF.AlterThreatTable(attacker, Amount);
            }
            else
            {
                died = UpdateStat("HEALTH", "curBase", -Amount);
            }

            CombatManager.Instance.HandleCombatAction(this);
            CombatManager.Instance.HandleCombatAction(attacker);

            if (died)
            {
                CombatManager.Instance.HandleOnKillActions(attacker, this, rankREF);
            }
        }


        public void Heal(int Amount)
        {
            UpdateStat("HEALTH", "curBase", Amount);
            ScreenSpaceNameplates.Instance.SetNPToVisible(this);
            ScreenSpaceNameplates.Instance.UpdateNPBar(this);
            if (nodeType == COMBAT_NODE_TYPE.pet)
                if (CombatManager.playerCombatInfo.currentPets.Contains(this))
                    PetPanelDisplayManager.Instance.UpdateHealthBar();
            if (this == CombatManager.Instance.currentTarget) TargetInfoDisplayManager.Instance.UpdateTargetHealthBar();
        }

        private bool UpdateStat(string _name, string valueType, float Amount)
        {
            var statIndex = getStatIndexFromName(_name);
            if (statIndex == -1) return false;
            bool died = false;
            float newValue = 0;
            switch (valueType)
            {
                case "curMin":
                    newValue = nodeStats[statIndex].curMinValue += Amount;
                    if (newValue < nodeStats[statIndex].stat.minValue) newValue = nodeStats[statIndex].stat.minValue;
                    nodeStats[statIndex].curMinValue = newValue;
                    break;
                case "curBase":
                    newValue = nodeStats[statIndex].curValue += Amount;
                    if (newValue < nodeStats[statIndex].curMinValue) newValue = nodeStats[statIndex].curMinValue;
                    else if (newValue > nodeStats[statIndex].curMaxValue) newValue = nodeStats[statIndex].curMaxValue;
                    nodeStats[statIndex].curValue = newValue;

                    // CHECK VITALITY ACTIONS
                    if (nodeStats[statIndex].curValue == 0 && nodeStats[statIndex].stat._name == "HEALTH")
                    {
                        DEATH();
                        died = true;
                    }

                    break;
                case "curMax":
                    newValue = nodeStats[statIndex].curMaxValue += Amount;
                    if (newValue > nodeStats[statIndex].stat.maxValue) newValue = nodeStats[statIndex].stat.maxValue;
                    nodeStats[statIndex].curMaxValue = newValue;
                    break;
                case "defaultMin":
                    nodeStats[statIndex].stat.minValue = newValue;
                    break;
                case "defaultBase":
                    nodeStats[statIndex].stat.baseValue = newValue;
                    break;
                case "defaultMax":
                    nodeStats[statIndex].stat.maxValue = newValue;
                    break;
            }

            if (nodeStats[statIndex].stat._name != "HEALTH")
            {
                HandleActionsForSpecialStats(nodeStats[statIndex].stat._name);
            }
            else
            {
                CombatManager.Instance.HandleHealthStatChange(this);
            }

            return died;
        }

        private void OnDestroy()
        {
            if (RPGBuilderEssentials.Instance.getCurrentScene().name == "MainMenu") return;
            if (CombatManager.Instance.allCombatNodes.Contains(this))
                CombatManager.Instance.allCombatNodes.Remove(this);
            OnMouseExit();
        }

        private void OnMouseDown()
        {
            if(!RPGBuilderUtilities.IsPointerOverUIObject())CombatManager.Instance.SetPlayerTarget(this);
        }

        private void OnMouseOver()
        {
            if (nodeType != COMBAT_NODE_TYPE.mob && nodeType != COMBAT_NODE_TYPE.pet) return;
            if (RPGBuilderUtilities.IsPointerOverUIObject()) return;
            if (Input.GetMouseButtonUp(1))
            {
                if (Vector3.Distance(transform.position, CombatManager.playerCombatInfo.transform.position) < 4)
                    switch (npcDATA.npcType)
                    {
                        case RPGNpc.NPC_TYPE.MERCHANT:
                        {
                            if (MerchantPanelDisplayManager.Instance.thisCG.alpha == 0)
                                MerchantPanelDisplayManager.Instance.Show(npcDATA);
                            break;
                        }
                        case RPGNpc.NPC_TYPE.QUEST_GIVER:
                        {
                            CharacterEventsManager.Instance.TalkedToNPC(npcDATA);
                            if (QuestInteractionDisplayManager.Instance.thisCG.alpha == 0)
                                QuestInteractionDisplayManager.Instance.Show(npcDATA);
                            break;
                        }
                    }
                else
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent("This is too far", 3);
            }

            switch (npcDATA.npcType)
            {
                case RPGNpc.NPC_TYPE.MERCHANT:
                    CursorManager.Instance.SetCursor(CursorManager.cursorType.merchant);
                    break;
                case RPGNpc.NPC_TYPE.QUEST_GIVER:
                    CursorManager.Instance.SetCursor(CursorManager.cursorType.questGiver);
                    break;
                case RPGNpc.NPC_TYPE.MOB when npcDATA.alignmentType == RPGNpc.ALIGNMENT_TYPE.ENEMY:
                    CursorManager.Instance.SetCursor(CursorManager.cursorType.enemy);
                    break;
                case RPGNpc.NPC_TYPE.RARE when npcDATA.alignmentType == RPGNpc.ALIGNMENT_TYPE.ENEMY:
                    CursorManager.Instance.SetCursor(CursorManager.cursorType.enemy);
                    break;
                case RPGNpc.NPC_TYPE.BOSS when npcDATA.alignmentType == RPGNpc.ALIGNMENT_TYPE.ENEMY:
                    CursorManager.Instance.SetCursor(CursorManager.cursorType.enemy);
                    break;
            }
        }

        private void OnMouseExit()
        {
            if (nodeType != COMBAT_NODE_TYPE.mob && nodeType != COMBAT_NODE_TYPE.pet) return;
            if (npcDATA.npcType == RPGNpc.NPC_TYPE.MERCHANT || npcDATA.npcType == RPGNpc.NPC_TYPE.QUEST_GIVER || npcDATA.alignmentType == RPGNpc.ALIGNMENT_TYPE.ENEMY)
                CursorManager.Instance.ResetCursor();
        }

        private void DEATH()
        {
            if (dead) return;
            dead = true;
            switch (nodeType)
            {
                case COMBAT_NODE_TYPE.mob:
                {
                    var respawnTime = Random.Range(npcDATA.MinRespawn, npcDATA.MaxRespawn);
                    spawnerREF.StartCoroutine(spawnerREF.ExecuteSpawner(respawnTime));
                    CombatManager.Instance.HandleCombatNodeDEATH(this);
                    break;
                }
                case COMBAT_NODE_TYPE.pet:
                case COMBAT_NODE_TYPE.player:
                    CombatManager.Instance.HandleCombatNodeDEATH(this);
                    break;
            }
        }

        public void InitRespawn(Vector3 respawnPOS)
        {
            playerControllerREF.CancelDeathAnim();
            playerControllerREF.TeleportToTarget(respawnPOS);
            dead = false;
        }

        public void HandleActionsForSpecialStats(string statName)
        {
            switch (statName)
            {
                case "MANA":
                    if (nodeType == COMBAT_NODE_TYPE.player) CombatManager.Instance.StatBarUpdate("MANA");
                    break;
            }
        }

        public int getStatIndexFromName(string statname)
        {
            for (var i = 0; i < nodeStats.Count; i++)
                if (nodeStats[i]._name == statname)
                    return i;
            return -1;
        }
    }
}