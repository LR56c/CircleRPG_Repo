using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;
using UnityEngine.AI;

namespace THMSV.RPGBuilder.AI
{
    public class RPGBAIAgent : MonoBehaviour
    {
        public enum AGENT_STATES
        {
            Idle = 0,
            Walk = 1,
            Run = 2,
            Combat = 3
        }
        public AGENT_STATES curState;

        public enum AGENT_MOVEMENT_STATES
        {
            idle,
            roam,
            chaseTarget,
            followOwner
        }
        public AGENT_MOVEMENT_STATES curMovementState;

        public NavMeshAgent thisAgent;
        public Animator thisAnim;

        public float StateUpdateRate = 0.2f, nextStateUpdate, rotationSpeed;

        public CombatNode thisCombatInfo;
        public CombatNode target;

        private bool isWaitingNextRoam;
        private float nextRoam;

        private float moveSpeed;

        private float nextAbilityCast;

        [System.Serializable]
        public class THREAT_TABLE_DATA
        {
            public CombatNode combatNode;
            public int curThreat;
        }
        public List<THREAT_TABLE_DATA> threatTable = new List<THREAT_TABLE_DATA>();

        public void AlterThreatTable (CombatNode cbtNode, int Amount)
        {
            var cbtNodeIndex = getCombatNodeIndexInThreatTable(cbtNode);
            if(cbtNodeIndex != -1)
            {
                threatTable[cbtNodeIndex].curThreat += Amount;
            } else
            {
                var newThreatTableData = new THREAT_TABLE_DATA();
                newThreatTableData.combatNode = cbtNode;
                newThreatTableData.curThreat = Amount;
                threatTable.Add(newThreatTableData);
            }

            AssignHighestThreatTarget();
        }

        private THREAT_TABLE_DATA getThreatTableDATA (CombatNode cbtNode)
        {
            foreach (var t in threatTable)
                if (t.combatNode == cbtNode) return t;

            return null;
        }

        public void RemoveCombatNodeFromThreatTabble (CombatNode cbtNode)
        {
            var thisThreatTableDARA = getThreatTableDATA(cbtNode);
            if (thisThreatTableDARA != null) threatTable.Remove(thisThreatTableDARA);
            if (target == cbtNode)
            {
                target = null;
                nextRoam = 0;
                curMovementState = AGENT_MOVEMENT_STATES.idle;
                curState = AGENT_STATES.Idle;
                thisAgent.isStopped = false;
            }

            if (threatTable.Count != 0) return;
            if (thisCombatInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.pet) return;
            curMovementState = AGENT_MOVEMENT_STATES.followOwner;
            curState = AGENT_STATES.Idle;
        }

        public void ClearThreatTable()
        {
            foreach (var t in threatTable)
            {
                if (target != t.combatNode) continue;
                target = null;
                nextRoam = 0;
                curMovementState = AGENT_MOVEMENT_STATES.idle;
                curState = AGENT_STATES.Idle;
                thisAgent.isStopped = false;

            }
            threatTable.Clear();
            if (thisCombatInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.pet) return;
            curMovementState = AGENT_MOVEMENT_STATES.followOwner;
            curState = AGENT_STATES.Idle;
        }


        private void AssignHighestThreatTarget ()
        {
            if (!thisCombatInfo.dead) SetTarget(getHighestThreatTarget());
        }

        private CombatNode getHighestThreatTarget ()
        {
            var highestThreat = 0;
            CombatNode cbtNode = null;

            for (var i = 0; i < threatTable.Count; i++)
            {
                if(threatTable[i].combatNode == null)
                {
                    threatTable.Remove(threatTable[i]);
                    continue;
                }

                if (threatTable[i].curThreat <= highestThreat) continue;
                highestThreat = threatTable[i].curThreat;
                cbtNode = threatTable[i].combatNode;
            }
            return cbtNode;
        }

        private int getCombatNodeIndexInThreatTable (CombatNode cbtNode)
        {
            for (var i = 0; i < threatTable.Count; i++)
                if(threatTable[i].combatNode == cbtNode) return i;
            return -1;
        }

        private void FixedUpdate()
        {
            UpdateState();
            if (thisCombatInfo.npcDATA.isMovementEnabled)
            {
                switch (curMovementState)
                {
                    case AGENT_MOVEMENT_STATES.chaseTarget:
                        moveSpeed = thisCombatInfo.getCurrentValue("MOVE_SPEED");
                        break;
                    case AGENT_MOVEMENT_STATES.followOwner:
                        moveSpeed = thisCombatInfo.ownerCombatInfo.getCurrentValue("MOVE_SPEED");
                        break;
                    default:
                        moveSpeed = thisCombatInfo.getCurrentValue("MOVE_SPEED") / 2;
                        break;
                }
            }
            
            if (!thisCombatInfo.isStunned() && !thisCombatInfo.isSleeping())
            {
                OutsideStateLoopUpdate();
            } else
            {
                moveSpeed = 0;
                thisAgent.acceleration = 0;
            }

            thisAgent.speed = moveSpeed;
            thisAgent.acceleration = moveSpeed*3;

            HandleStandTime();
        }

        private void HandleStandTime ()
        {
            if (!standTimeActive) return;
            currentStandTimeDur += Time.deltaTime;

            if (currentStandTimeDur >= maxStandTimeDur) resetStandTime();
        }

        private void OutsideStateLoopUpdate ()
        {
            if (target == null) return;
            if (standTimeActive && !canRotateInStandTime) return;
            transform.LookAt(
                new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        }


        private void UpdateState()
        {
            if (!(Time.time >= nextStateUpdate)) return;
            nextStateUpdate = Time.time + StateUpdateRate;
            
            if(thisCombatInfo.npcDATA.isMovementEnabled) HandleMovement();

            if (target == null)
            {
                if (thisCombatInfo.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                {
                    if(thisCombatInfo.ownerCombatInfo.currentPetsCombatActionType == CombatNode.PET_COMBAT_ACTION_TYPES.aggro) LookForTarget();
                }
                else
                {
                    LookForTarget();
                }
            }
            else
            {
                CheckTargetDistance();
                if(thisCombatInfo.npcDATA.isCombatEnabled)HandleCombat();
            }
            if (!standTimeActive && thisCombatInfo.npcDATA.isMovementEnabled) SetAnimation();
        }
    

        private float currentStandTimeDur, maxStandTimeDur;
        public bool standTimeActive;
        public bool canRotateInStandTime;
        public void InitStandTime(float max, bool canRotate)
        {
            standTimeActive = true;
            canRotateInStandTime = canRotate;
            currentStandTimeDur = 0;
            maxStandTimeDur = max;
            thisAgent.ResetPath();
            thisAgent.isStopped = true;
            thisAgent.velocity = Vector3.zero;

            thisAnim.SetInteger("MovementState", 3);
        }

        public void resetStandTime()
        {
            standTimeActive = false;
            currentStandTimeDur = 0;
            maxStandTimeDur = 0;
            thisAgent.isStopped = false;
        }

        public void InitStun()
        {
            thisAgent.ResetPath();
            thisAgent.isStopped = true;
            thisAgent.velocity = Vector3.zero;

            curMovementState = AGENT_MOVEMENT_STATES.idle;
            curState = AGENT_STATES.Idle;
            thisAnim.SetInteger("MovementState", 0);
            thisAnim.Rebind();
        }

        public void ResetStun()
        {
            thisAgent.isStopped = false;

            if(target == null)
            {
                switch (thisCombatInfo.nodeType)
                {
                    case CombatNode.COMBAT_NODE_TYPE.mob:
                        curMovementState = AGENT_MOVEMENT_STATES.idle;
                        break;
                    case CombatNode.COMBAT_NODE_TYPE.pet:
                        curMovementState = AGENT_MOVEMENT_STATES.followOwner;
                        break;
                }

                curState = AGENT_STATES.Idle;
            } else
            {
                curMovementState = AGENT_MOVEMENT_STATES.chaseTarget;
                curState = AGENT_STATES.Run;
            }
        }

        bool canBeAggroed(CombatNode t)
        {
            if (t.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
            {
                return true;
            }

            return t.npcDATA != null && t.npcDATA.isCombatEnabled;
        }

        private CombatNode getClosestNode ()
        {
            var potentialNodes = new List<CombatNode>();
            foreach (var t in CombatManager.Instance.allCombatNodes)
                if (!t.dead && shouldAggro(t) && canBeAggroed(t)){ potentialNodes.Add(t);}

            CombatNode closestNode = null;
            float smallestDist = 0;

            foreach (var t in potentialNodes)
            {
                var dist = Vector3.Distance(transform.position, t.transform.position);
                if (smallestDist != 0 && !(dist < smallestDist)) continue;
                closestNode = t;
                smallestDist = dist;
            }
            return closestNode;
        }

        private bool shouldAggro (CombatNode potentialNode)
        {
            switch (thisCombatInfo.npcDATA.alignmentType)
            {
                case RPGNpc.ALIGNMENT_TYPE.ALLY when potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.player:
                    return false;
                case RPGNpc.ALIGNMENT_TYPE.ALLY:
                {
                    if (potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.mob || potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                        switch (potentialNode.npcDATA.alignmentType)
                        {
                            case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                                return true;
                            case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                                return false;
                        }

                    break;
                }
                case RPGNpc.ALIGNMENT_TYPE.ENEMY when potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.player:
                    return true;
                case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                {
                    if (potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.mob || potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                        switch (potentialNode.npcDATA.alignmentType)
                        {
                            case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                                return false;
                            case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                            case RPGNpc.ALIGNMENT_TYPE.ALLY:
                                return true;
                        }

                    break;
                }
                case RPGNpc.ALIGNMENT_TYPE.NEUTRAL when potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.player:
                    return false;
                case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                {
                    if (potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.mob || potentialNode.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                        switch (potentialNode.npcDATA.alignmentType)
                        {
                            case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                            case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                            case RPGNpc.ALIGNMENT_TYPE.ALLY:
                                return false;
                        }

                    break;
                }
            }

            return false;
        }

        private void LookForTarget ()
        {
            if (threatTable.Count != 0) return;
            var closestNode = getClosestNode();
            if (closestNode == null)
            {
                if (thisCombatInfo.nodeType != CombatNode.COMBAT_NODE_TYPE.pet) return;
                curMovementState = AGENT_MOVEMENT_STATES.followOwner;
                curState = AGENT_STATES.Idle;
                return;
            }
            var dist = Vector3.Distance(transform.position, closestNode.transform.position);
            if (!(dist < thisCombatInfo.npcDATA.AggroRange)) return;
            target = closestNode;
            curMovementState = AGENT_MOVEMENT_STATES.chaseTarget;
            curState = AGENT_STATES.Run;
            thisAgent.isStopped = false;

            if (thisCombatInfo.npcDATA.npcType != RPGNpc.NPC_TYPE.BOSS) return;
            if (BossUISlotHolder.Instance.thisNode == null || BossUISlotHolder.Instance.thisNode == thisCombatInfo) BossUISlotHolder.Instance.Init(thisCombatInfo);
        }

        public void SetTarget (CombatNode newTarget)
        {
            target = newTarget;
            curMovementState = AGENT_MOVEMENT_STATES.chaseTarget;
            curState = AGENT_STATES.Run;
            thisAgent.isStopped = false;
        }

        private void CheckTargetDistance()
        {
            float dist = 0;
            switch (thisCombatInfo.nodeType)
            {
                case CombatNode.COMBAT_NODE_TYPE.mob:
                    dist = Vector3.Distance(thisCombatInfo.spawnerREF.transform.position, target.transform.position);
                    break;
                case CombatNode.COMBAT_NODE_TYPE.pet:
                    dist = Vector3.Distance(transform.position, target.transform.position);
                    break;
            }

            if (!(dist > thisCombatInfo.npcDATA.DistanceToTargetReset)) return;
            target = null;
            nextRoam = 0;
            curMovementState = AGENT_MOVEMENT_STATES.idle;
            curState = AGENT_STATES.Idle;
            thisAgent.isStopped = false;
        }

        private void HandleMovement()
        {
            if (standTimeActive || thisCombatInfo.isStunned() || thisCombatInfo.isSleeping() ||
                thisCombatInfo.isRooted()) return;
            if (target == null)
                switch (thisCombatInfo.nodeType)
                {
                    case CombatNode.COMBAT_NODE_TYPE.mob:
                        HandleMobRoaming();
                        break;
                    case CombatNode.COMBAT_NODE_TYPE.pet:
                        switch (thisCombatInfo.ownerCombatInfo.currentPetsMovementActionType)
                        {
                            case CombatNode.PET_MOVEMENT_ACTION_TYPES.stay:
                                // do nothing, stand there
                                curMovementState = AGENT_MOVEMENT_STATES.idle;
                                curState = AGENT_STATES.Idle;
                                thisAgent.isStopped = true;
                                thisAgent.stoppingDistance = 0;
                                break;
                            case CombatNode.PET_MOVEMENT_ACTION_TYPES.follow:
                                HandlePetFollowOwner();
                                break;
                        }

                        break;
                }
            else
                HandleTargetChasing();
        }

        private void HandleTargetChasing ()
        {
            if (curMovementState != AGENT_MOVEMENT_STATES.chaseTarget &&
                curMovementState != AGENT_MOVEMENT_STATES.idle) return;
            var dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist >= thisCombatInfo.npcDATA.distanceFromTarget)
            {
                curMovementState = AGENT_MOVEMENT_STATES.chaseTarget;
                curState = AGENT_STATES.Run;
                thisAgent.isStopped = false;
                thisAgent.stoppingDistance = thisCombatInfo.npcDATA.distanceFromTarget;
                thisAgent.SetDestination(target.transform.position);
            }
            else
            {
                curMovementState = AGENT_MOVEMENT_STATES.idle;
                curState = AGENT_STATES.Combat;
                thisAgent.ResetPath();
                thisAgent.isStopped = true;
            }
        }

        private Vector3 getPointDistanceFromTarget(float distance)
        {
            var directionOfTravel = transform.position - target.transform.position;
            var finalDirection = (directionOfTravel + directionOfTravel.normalized) * distance;
            var targetPosition = target.transform.position - finalDirection;

            return targetPosition;
        }

        private void HandleMobRoaming ()
        {
            if (thisCombatInfo.npcDATA.RoamRange == 0) return;
            if (Time.time >= nextRoam && curMovementState == AGENT_MOVEMENT_STATES.idle)
            {
                nextRoam = Time.time + thisCombatInfo.npcDATA.RoamDelay;

                var newRoamDestination = new Vector3(thisCombatInfo.spawnerREF.transform.position.x + Random.Range(-thisCombatInfo.npcDATA.RoamRange, thisCombatInfo.npcDATA.RoamRange), transform.position.y, thisCombatInfo.spawnerREF.transform.position.z + Random.Range(-thisCombatInfo.npcDATA.RoamRange, thisCombatInfo.npcDATA.RoamRange));

                curMovementState = AGENT_MOVEMENT_STATES.roam;
                curState = AGENT_STATES.Walk;
                thisAgent.isStopped = false;
                thisAgent.stoppingDistance = 0;
                thisAgent.SetDestination(newRoamDestination);

            }
            else if (curMovementState == AGENT_MOVEMENT_STATES.roam)
            {
                var dist = Vector3.Distance(transform.position, thisAgent.destination);
                if (!(dist < 1)) return;
                curMovementState = AGENT_MOVEMENT_STATES.idle;
                curState = AGENT_STATES.Idle;
                thisAgent.isStopped = true;
                thisAgent.stoppingDistance = 0;
            }
        }

        private void HandlePetFollowOwner ()
        {
            if (curMovementState != AGENT_MOVEMENT_STATES.followOwner &&
                curMovementState != AGENT_MOVEMENT_STATES.idle) return;
            var dist = Vector3.Distance(transform.position, thisCombatInfo.ownerCombatInfo.transform.position);
            if (dist <= thisCombatInfo.npcDATA.distanceFromOwner)
            {
                curMovementState = AGENT_MOVEMENT_STATES.followOwner;
                curState = AGENT_STATES.Idle;
                thisAgent.isStopped = true;
                return;
            }
            var newFollowDestination = thisCombatInfo.ownerCombatInfo.transform.position;
            newFollowDestination += thisCombatInfo.ownerCombatInfo.transform.forward * 1;

            curMovementState = AGENT_MOVEMENT_STATES.followOwner;
            curState = AGENT_STATES.Walk;
            thisAgent.isStopped = false;
            thisAgent.stoppingDistance = 0;
            thisAgent.SetDestination(newFollowDestination);
        }

        private void HandleCombat()
        {
            for (var i = 0; i < thisCombatInfo.abilitiesData.Count; i++)
                if (thisCombatInfo.abilitiesData[i].CDLeft == 0 && Time.time >= nextAbilityCast && target != null)
                {
                    var dist = Vector3.Distance(transform.position, target.transform.position);
                    if (!(dist <= thisCombatInfo.npcDATA.distanceFromTarget)) continue;
                    nextAbilityCast = Time.time + CombatManager.Instance.NPC_GCD_DURATION;
                    UseNPCAbility(i);
                }
        }

        private void UseNPCAbility (int abilityIndex)
        {
            CombatManager.Instance.TRIGGER_NPC_ABILITY(thisCombatInfo, thisCombatInfo.abilitiesData[abilityIndex].currentAbility);
        }


        private void SetAnimation()
        {
            thisAnim.SetInteger("MovementState", (int)curState);
        }
    }
}
