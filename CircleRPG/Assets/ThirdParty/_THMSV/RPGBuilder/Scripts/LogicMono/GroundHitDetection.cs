using System.Collections;
using System.Collections.Generic;
using THMSV.RPGBuilder.CombatVisuals;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

namespace THMSV.RPGBuilder.LogicMono
{
    public class GroundHitDetection : MonoBehaviour
    {
        public CombatVisualEffect cbtVisualREF;
        public RPGAbility ability;
        public float curTime;
        public float radius;
        public int hitCount;
        public float intervalBetweenHits;
        public float activationDelay;
    

        private CombatNode ownerNode;

        private int cachedMaxUnitHit;
        private RPGAbilityRankData rankREF;
        
        void InitValues()
        {
            cachedMaxUnitHit = rankREF.MaxUnitHit +
                               (int)CombatManager.Instance.GetTotalOfStatType(cbtVisualREF.nodeREF, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
        }
        
        public void InitGroundAbility(CombatVisualEffect cbtref, float destroyDuration, RPGAbility ABILITY)
        {
            cbtVisualREF = cbtref;
            Destroy(gameObject, destroyDuration);

            ability = ABILITY;
            var curRank = RPGBuilderUtilities.getNodeCurrentRank(ability);
            rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);

            radius = rankREF.groundRadius;
            hitCount = rankREF.groundHitCount;
            intervalBetweenHits = rankREF.groundHitInterval;
            activationDelay = rankREF.groundHitTime;
            InitValues();
            StartCoroutine(StartGroundHit(activationDelay));
        }

        private IEnumerator StartGroundHit (float activationDelay)
        {
            var curRank = RPGBuilderUtilities.getNodeCurrentRank(ability);
            rankREF = RPGBuilderUtilities.GetAbilityRankFromID(ability.ranks[curRank].rankID);
            yield return new WaitForSeconds(activationDelay);

            for (var i = 0; i < hitCount; i++)
            {
                var hitColliders = new Collider[0];
                hitColliders = Physics.OverlapSphere(transform.position, radius, rankREF.hitLayers);

                var closestUnits = getClosestUnits(hitColliders, cachedMaxUnitHit);
                foreach (var t in closestUnits) CombatManager.Instance.EXECUTE_GROUND_ABILITY_HIT(cbtVisualREF.nodeREF, t, ability, rankREF);
                if (hitCount > 1)
                    yield return new WaitForSeconds(intervalBetweenHits);
                else
                    yield break;
            }
        }

        private List<CombatNode> getClosestUnits(Collider[] hitColliders, int maxUnitHit)
        {
            var closestUnits = new List<CombatNode>();
            var allDistances = new List<float>();

            foreach (var t in hitColliders)
                if (allDistances.Count >= maxUnitHit)
                {
                    CombatNode cbtNode = t.GetComponent<CombatNode>();
                    if(cbtNode == null) continue;
                    if (cbtNode.dead) continue;
                    var dist = Vector3.Distance(transform.position, t.transform.position);
                    var CurBiggestDistanceInArray = Mathf.Max(allDistances.ToArray());
                    var IndexOfBiggest = allDistances.IndexOf(CurBiggestDistanceInArray);
                    if (!(dist < CurBiggestDistanceInArray)) continue;
                    allDistances[IndexOfBiggest] = dist;
                    closestUnits[IndexOfBiggest] = cbtNode;
                }
                else
                {
                    CombatNode cbtNode = t.GetComponent<CombatNode>();
                    if(cbtNode == null) continue;
                    if (cbtNode.dead) continue;
                    allDistances.Add(Vector3.Distance(transform.position, t.transform.position));
                    closestUnits.Add(cbtNode);
                }

            return closestUnits;
        }
    
    }
}
