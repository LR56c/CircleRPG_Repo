using System;
using System.Collections;
using System.Collections.Generic;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

namespace THMSV.RPGBuilder.World
{
    public class InteractiveNode : MonoBehaviour
    {
        public enum InteractiveNodeType
        {
            resourceNode,
            effectNode,
            abilityNode,
            questNode,
            giveTreePoint,
            teachSkill,
            giveClassEXP,
            giveSkillEXP,
            completeTask,
            container
        }

        public InteractiveNodeType nodeType;


        public enum InteractiveNodeCategory
        {
            combat,
            general,
            world
        }

        public InteractiveNodeCategory nodeCategory;

        [Serializable]
        public class containerLootTablesDATA
        {
            public RPGLootTable lootTable;
            public float chance;
        }

        public List<containerLootTablesDATA> containerTablesData = new List<containerLootTablesDATA>();

        [Serializable]
        public class effectsDATA
        {
            public RPGEffect effect;
            public float chance;
        }

        public List<effectsDATA> effectsData = new List<effectsDATA>();

        [Serializable]
        public class abilitiesDATA
        {
            public RPGAbility ability;
            public float chance;
        }

        public List<abilitiesDATA> abilitiesData = new List<abilitiesDATA>();

        [Serializable]
        public class questsDATA
        {
            public RPGQuest quest;
            public float chance;
        }

        public List<questsDATA> questsData = new List<questsDATA>();

        [Serializable]
        public class treePointsDATA
        {
            public RPGTreePoint treePoint;
            public int amount;
            public float chance;
        }

        public List<treePointsDATA> treePointsData = new List<treePointsDATA>();

        [Serializable]
        public class skillsDATA
        {
            public RPGSkill skill;
            public float chance;
        }

        public List<skillsDATA> skillsData = new List<skillsDATA>();

        [Serializable]
        public class classExpDATA
        {
            public int expAmount;
            public float chance;
        }

        public classExpDATA classExpData;

        [Serializable]
        public class skillExpDATA
        {
            public RPGSkill skill;
            public int expAmount;
            public float chance;
        }

        public List<skillExpDATA> skillExpData = new List<skillExpDATA>();

        [Serializable]
        public class taskDATA
        {
            public RPGTask task;
            public float chance;
        }

        public List<taskDATA> taskData = new List<taskDATA>();

        public RPGResourceNode resourceNodeData;


        public enum InteractiveNodeState
        {
            ready,
            cooldown,
            disabled
        }

        public InteractiveNodeState nodeState;

        public int UseCount;
        public float Cooldown, nextUse, interactionTime, useDistanceMax = 2;

        public GameObject readyVisual, onCooldownVisual, disabledVisual;
        public GameObject currentVisualGO;
        public Transform lootBagSpawnPoint;
        public bool spawnLootBagOnPlayer, isTrigger, isClick = true;

        public List<RequirementsManager.RequirementDATA> useRequirement =
            new List<RequirementsManager.RequirementDATA>();

        public string animationName;


        private void Start()
        {
            SetNodeState(InteractiveNodeState.ready);
        }

        private void HideAllVisuals()
        {
            if (readyVisual != null) readyVisual.SetActive(false);

            if (onCooldownVisual != null) onCooldownVisual.SetActive(false);

            if (disabledVisual != null) disabledVisual.SetActive(false);
        }

        private void ShowVisual(GameObject go)
        {
            if (go != null) go.SetActive(true);
        }

        private IEnumerator resetNode(float delay)
        {
            yield return new WaitForSeconds(delay);

            SetNodeState(InteractiveNodeState.ready);
        }

        private void SetNodeState(InteractiveNodeState newState)
        {
            nodeState = newState;
            HideAllVisuals();
            switch (nodeState)
            {
                case InteractiveNodeState.ready:
                    ShowVisual(readyVisual);
                    gameObject.layer = 0;
                    break;
                case InteractiveNodeState.cooldown:
                    ShowVisual(onCooldownVisual);
                    gameObject.layer = 2;
                    break;
                case InteractiveNodeState.disabled:
                    ShowVisual(disabledVisual);
                    gameObject.layer = 2;
                    break;
            }
        }

        public void UseNode()
        {
            if (!(Time.time >= nextUse)) return;
            if (nodeType != InteractiveNodeType.resourceNode)
            {
                nextUse = Time.time + Cooldown;
                StartCoroutine(resetNode(Cooldown));
            }
            else
            {
                var curRank = RPGBuilderUtilities.getNodeCurrentRank(resourceNodeData);
                var rankREF = RPGBuilderUtilities.GetResourceNodeRankFromID(resourceNodeData.ranks[curRank].rankID);
                nextUse = Time.time + rankREF.respawnTime;
                StartCoroutine(resetNode(rankREF.respawnTime));
            }

            SetNodeState(InteractiveNodeState.cooldown);

            if (animationName != "") CombatManager.playerCombatInfo.GetComponent<Animator>().SetTrigger(animationName);

            var totalItemDropped = 0;
            var lootData = new List<LootBagHolder.Loot_Data>();
            switch (nodeType)
            {
                case InteractiveNodeType.container:
                    float LOOTCHANCEMOD = CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                        RPGStat.STAT_TYPE.LOOT_CHANCE_MODIFIER);
                    foreach (var t in containerTablesData)
                    {
                        var chance = Random.Range(0, t.chance);
                        if (t.chance != 0 && !(chance <= t.chance)) continue;
                        foreach (var t1 in t.lootTable.lootItems)
                        {
                            var itemDropAmount = Random.Range(0f, 100f);
                            if (LOOTCHANCEMOD > 0) itemDropAmount += itemDropAmount * (LOOTCHANCEMOD / 100);
                            if (!(itemDropAmount <= t1.dropRate)) continue;
                            var stack = 0;
                            if (t1.min ==
                                t1.max)
                                stack = t1.min;
                            else
                                stack = Random.Range(t1.min,
                                    t1.max + 1);

                            RPGItem itemREF = RPGBuilderUtilities.GetItemFromID(t1.itemID);
                            if (itemREF.dropInWorld)
                            {
                                var newLoot = new InventoryManager.WorldLootItems_DATA();
                                newLoot.item = itemREF;
                                newLoot.count = stack;
                                GameObject newLootGO = Instantiate(itemREF.itemWorldModel, new Vector3(transform.position.x,
                                    transform.position.y + 1, transform.position.z), Quaternion.identity);
                                newLoot.worldDroppedItemREF = newLootGO.AddComponent<WorldDroppedItem>();
                                newLoot.worldDroppedItemREF.curLifetime = 0;
                                newLoot.worldDroppedItemREF.maxDuration = itemREF.durationInWorld;
                                newLoot.worldDroppedItemREF.item = itemREF;
                                if (itemREF.randomStats.Count > 0)
                                {
                                    newLoot.randomItemID = RPGBuilderUtilities.GenerateRandomItemStats(itemREF.ID, false, false);
                                }
                                newLoot.worldDroppedItemREF.InitPhysics(transform.position);
                                    InventoryManager.Instance.allWorldDroppedItems.Add(newLoot);
                            }
                            else
                            {
                                var newLoot = new LootBagHolder.Loot_Data();
                                newLoot.item = itemREF;
                                newLoot.count = stack;
                        
                                if (itemREF.randomStats.Count > 0)
                                {
                                    newLoot.randomItemID = RPGBuilderUtilities.GenerateRandomItemStats(itemREF.ID, false, false);
                                }
                                lootData.Add(newLoot);
                            }
                            totalItemDropped++;
                        }
                    }

                    break;

                case InteractiveNodeType.resourceNode:
                    if (RPGBuilderUtilities.isResourceNodeKnown(resourceNodeData.ID))
                    {
                        var curRank = RPGBuilderUtilities.getNodeCurrentRank(resourceNodeData);
                        var rankREF =
                            RPGBuilderUtilities.GetResourceNodeRankFromID(resourceNodeData.ranks[curRank].rankID);
                        var lootTableREF = RPGBuilderUtilities.GetLootTableFromID(rankREF.lootTableID);
                        foreach (var t in lootTableREF.lootItems)
                        {
                            var chance = Random.Range(0f, 100f);
                            if (!(chance <= t.dropRate)) continue;
                            var stack = 0;
                            if (t.min == t.max)
                                stack = t.min;
                            else
                                stack = Random.Range(t.min,
                                    t.max + 1);

                            RPGItem itemREF = RPGBuilderUtilities.GetItemFromID(t.itemID);
                            if (itemREF.dropInWorld)
                            {
                                var newLoot = new InventoryManager.WorldLootItems_DATA();
                                newLoot.item = itemREF;
                                newLoot.count = stack;
                                GameObject newLootGO = Instantiate(itemREF.itemWorldModel, new Vector3(transform.position.x,
                                    transform.position.y + 1, transform.position.z), Quaternion.identity);
                                newLoot.worldDroppedItemREF = newLootGO.AddComponent<WorldDroppedItem>();
                                newLoot.worldDroppedItemREF.curLifetime = 0;
                                newLoot.worldDroppedItemREF.maxDuration = itemREF.durationInWorld;
                                newLoot.worldDroppedItemREF.item = itemREF;
                                if (itemREF.randomStats.Count > 0)
                                {
                                    newLoot.randomItemID = RPGBuilderUtilities.GenerateRandomItemStats(itemREF.ID, false, false);
                                }
                                newLoot.worldDroppedItemREF.InitPhysics(transform.position);
                                    InventoryManager.Instance.allWorldDroppedItems.Add(newLoot);
                            }
                            else
                            {
                                var newLoot = new LootBagHolder.Loot_Data();
                                newLoot.item = itemREF;
                                newLoot.count = stack;
                        
                                if (itemREF.randomStats.Count > 0)
                                {
                                    newLoot.randomItemID = RPGBuilderUtilities.GenerateRandomItemStats(itemREF.ID, false, false);
                                }
                                lootData.Add(newLoot);
                            }
                            totalItemDropped++;
                        }

                        LevelingManager.Instance.GenerateSkillEXP(resourceNodeData.skillRequiredID, rankREF.Experience);
                    }

                    break;

                case InteractiveNodeType.effectNode:
                    foreach (var t in effectsData)
                    {
                        var chance = Random.Range(0, t.chance);
                        if (t.chance == 0 || chance <= t.chance)
                            CombatManager.Instance.ExecuteEffect(CombatManager.playerCombatInfo,
                                CombatManager.playerCombatInfo, t.effect, null);
                    }

                    break;

                case InteractiveNodeType.questNode:
                    foreach (var t in questsData)
                    {
                        var chance = Random.Range(0, t.chance);
                        if (t.chance == 0 || chance <= t.chance)
                            QuestJournalDisplayManager.Instance.DisplayQuestContent(t.quest);
                    }

                    break;

                case InteractiveNodeType.giveTreePoint:
                    foreach (var t in treePointsData)
                    {
                        var chance = Random.Range(0, t.chance);
                        if (t.chance == 0 || chance <= t.chance)
                            TreePointsManager.Instance.AddTreePoint(t.treePoint.ID,
                                t.amount);
                    }

                    break;

                case InteractiveNodeType.giveClassEXP:
                {
                    var chance = Random.Range(0, classExpData.chance);
                    if (classExpData.chance == 0 || chance <= classExpData.chance)
                        LevelingManager.Instance.AddClassXP(classExpData.expAmount);
                }
                    break;
            }

            if (lootData.Count <= 0) return;
            if (totalItemDropped <= 0) return;
            var spawnPos = Vector3.zero;
            spawnPos = spawnLootBagOnPlayer
                ? CombatManager.playerCombatInfo.transform.position
                : lootBagSpawnPoint.position;
            var lootbag = Instantiate(InventoryManager.Instance.lootBagPrefab, spawnPos,
                InventoryManager.Instance.lootBagPrefab.transform.rotation);

            var lootBagRef = lootbag.GetComponent<LootBagHolder>();
            lootBagRef.lootData = lootData;
        }

        private void InitInteractionTime(float interactionTime)
        {
            CombatManager.playerCombatInfo.InitInteracting(this, interactionTime);
            PlayerInfoDisplayManager.Instance.InitInteractionBar();
        }

        private bool UseRequirementsMet()
        {
            switch (nodeType)
            {
                case InteractiveNodeType.resourceNode:
                    if (!RPGBuilderUtilities.isResourceNodeKnown(resourceNodeData.ID)) return false;
                    break;
            }

            foreach (var t in useRequirement)
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
                    case RequirementsManager.RequirementType.abilityKnown:
                        //intValue1 = RPGBuilderUtilities.getSkillLevel(useRequirement[i].skillRequiredID);
                        break;
                    case RequirementsManager.RequirementType._class:
                        intValue1 = t.classRequiredID;
                        break;
                }

                return RequirementsManager.Instance.HandleRequirementType(t, intValue1, true);
            }

            return true;
        }


        private void OnMouseOver()
        {
            if (!isClick) return;
            if (RPGBuilderUtilities.IsPointerOverUIObject()) return;
            if (Input.GetMouseButtonUp(1) &&
                !CombatManager.playerCombatInfo.isInteractiveNodeCasting)
                if (Vector3.Distance(transform.position, CombatManager.playerCombatInfo.transform.position) <=
                    useDistanceMax)
                {
                    if (UseRequirementsMet())
                    {
                        if (nodeType == InteractiveNodeType.resourceNode)
                        {
                            var curRank = RPGBuilderUtilities.getNodeCurrentRank(resourceNodeData);
                            var rankREF =
                                RPGBuilderUtilities.GetResourceNodeRankFromID(resourceNodeData.ranks[curRank].rankID);
                            if (rankREF.gatherTime == 0)
                                UseNode();
                            else
                                InitInteractionTime(rankREF.gatherTime);
                        }
                        else
                        {
                            if (interactionTime == 0)
                                UseNode();
                            else
                                InitInteractionTime(interactionTime);
                        }
                    }
                }
                else
                {
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent("This is too far", 3);
                }

            if (nodeState == InteractiveNodeState.ready)
                CursorManager.Instance.SetCursor(CursorManager.cursorType.interactiveObject);
        }

        private void OnMouseExit()
        {
            CursorManager.Instance.ResetCursor();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isTrigger || other.gameObject != CombatManager.playerCombatInfo.gameObject) return;
            if (CombatManager.playerCombatInfo.isInteractiveNodeCasting || !UseRequirementsMet()) return;
            if (nodeType == InteractiveNodeType.resourceNode)
            {
                var curRank = RPGBuilderUtilities.getNodeCurrentRank(resourceNodeData);
                var rankREF =
                    RPGBuilderUtilities.GetResourceNodeRankFromID(resourceNodeData.ranks[curRank].rankID);
                if (rankREF.gatherTime == 0)
                    UseNode();
                else
                    InitInteractionTime(rankREF.gatherTime);
            }
            else
            {
                if (interactionTime == 0)
                    UseNode();
                else
                    InitInteractionTime(interactionTime);
            }
        }
    }
}