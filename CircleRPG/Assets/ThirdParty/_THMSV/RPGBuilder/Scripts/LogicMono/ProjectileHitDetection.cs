using System.Collections.Generic;
using THMSV.RPGBuilder.CombatVisuals;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

namespace THMSV.RPGBuilder.LogicMono
{
    public class ProjectileHitDetection : MonoBehaviour
    {
        public CombatVisualEffect cbtVisualREF;
        public Rigidbody RB;
        public bool comesBackToCaster, goToNearbyUnits;
        public Transform casterTransform;
        public float curTime;
        public float ComeBackAfterTime;
        public float ComeBackSpeed;

        public int curNearbyUnitsHit;
        public float distMaxToNearbyUnit;

        public bool didHitFirstUnit, isReflectingProjectiles;
        public int curReflectedProjectileAmount, maxRelfectedProjectileAmount;
        public GameObject curNearbyTargetGO;

        public List<CombatNode> alreadyHitNodes = new List<CombatNode>();

        private Vector3 initPos;
        private RPGAbilityRankData rankREF;

        private Vector3 previousPos;
        private float curVelocity;
        private bool isReady;

        private CombatNode ownerNode;
        private float cachedExtraSpeed = 0;
        private float cachedMaxDistance = 0;
        
        public void InitProjectile(CombatVisualEffect cbtref, float destroyDuration)
        {
            cbtVisualREF = cbtref;
            initPos = transform.position;
            previousPos = initPos;
            if (!goToNearbyUnits && !comesBackToCaster) Destroy(gameObject, destroyDuration);
            InitRank();
            isReady = true;
            InitProjectileValues();
        }

        public void InitProjectile(CombatVisualEffect cbtref, float destroyDuration, bool comeback, Transform castertr, float time, float speed)
        {
            cbtVisualREF = cbtref;
            if (!goToNearbyUnits && !comesBackToCaster) Destroy(gameObject, destroyDuration);

            comesBackToCaster = comeback;
            casterTransform = castertr;
            ComeBackAfterTime = time;
            ComeBackSpeed = speed;
            initPos = transform.position;
            previousPos = initPos;
            InitRank();
            isReady = true;
            InitProjectileValues();
        }

        void InitProjectileValues()
        {
            cachedMaxDistance = rankREF.projectileDistance + (rankREF.projectileDistance * (CombatManager.Instance.GetTotalOfStatType(cbtVisualREF.nodeREF, RPGStat.STAT_TYPE.PROJECTILE_RANGE)/100));
            if (RB == null)
            {
                cachedExtraSpeed =
                    CombatManager.Instance.GetTotalOfStatType(cbtVisualREF.nodeREF, RPGStat.STAT_TYPE.PROJECTILE_SPEED);
            }
        }

        private void InitRank()
        {
            var curRank = 0;
            if (cbtVisualREF.nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.mob || cbtVisualREF.nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction || cbtVisualREF.nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(cbtVisualREF.ThisAB);

            rankREF = RPGBuilderUtilities.GetAbilityRankFromID(cbtVisualREF.ThisAB.ranks[curRank].rankID);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (CombatManager.Instance.LayerContains(CombatManager.Instance.ProjectileDestroyLayer,
                other.gameObject.layer))
            {
                EnvironmentHit();
            }

            var projREF = other.gameObject.GetComponent<ProjectileHitDetection>();
            if (projREF != null && projREF.isReflectingProjectiles &&
                projREF.curReflectedProjectileAmount < projREF.maxRelfectedProjectileAmount)
            {
                //reflect
                projREF.curReflectedProjectileAmount++;
                RB.velocity = -RB.velocity;
                Debug.LogError("REFLECTED " + gameObject.name);
                if (projREF.curReflectedProjectileAmount >= projREF.maxRelfectedProjectileAmount)
                    Destroy(projREF.gameObject);
                return;
            }

            if (!CombatManager.Instance.LayerContains(
                cbtVisualREF.ThisCurAbRank.hitLayers, other.gameObject.layer)) return;
            CombatNode nodeREF = other.gameObject.GetComponent<CombatNode>();
            if (nodeREF == null) return;
            CombatNodeHit(nodeREF);
        }

        private void lookForNextUnit ()
        {
            var curRank = 0;
            if (cbtVisualREF.nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.mob || cbtVisualREF.nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.objectAction || cbtVisualREF.nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.pet)
                curRank = 0;
            else
                curRank = RPGBuilderUtilities.getNodeCurrentRank(cbtVisualREF.ThisAB);
            if (curNearbyUnitsHit < RPGBuilderUtilities.GetAbilityRankFromID(cbtVisualREF.ThisAB.ranks[curRank].rankID).projectileNearbyUnitMaxHit)
            {
                curNearbyUnitsHit++;
                CombatManager.Instance.FIND_NEARBY_UNITS(cbtVisualREF.nodeREF, gameObject, cbtVisualREF.ThisAB, this);
            } else
            {
                Destroy(gameObject);
            }
        }

        void EnvironmentHit()
        {
            if (cbtVisualREF.HitGO != null) SpawnHitEffect();
            Destroy(gameObject);
        }

        void SpawnHitEffect()
        {
            if (cbtVisualREF.HitGO == null) return;
            var hitEffectGO = (GameObject) Instantiate(cbtVisualREF.HitGO,
                transform.position + cbtVisualREF.hitEffectPOS, Quaternion.identity);
            Destroy(hitEffectGO, cbtVisualREF.hitEffectDuration);
        }

        void CombatNodeHit(CombatNode hitNodeRef)
        {
            if (cbtVisualREF != null)
            {
                if (hitNodeRef.dead) return;
                cbtVisualREF.ProjectileHit(gameObject, cbtVisualREF.nodeREF, hitNodeRef);
                SpawnHitEffect();
                if (!goToNearbyUnits) return;
                if (!alreadyHitNodes.Contains(hitNodeRef)) alreadyHitNodes.Add(hitNodeRef);
                if (!didHitFirstUnit)
                {
                    didHitFirstUnit = true;
                }

                lookForNextUnit();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void CheckCollisions()
        {
            curVelocity = Vector3.Distance(previousPos, transform.position);
            
            var YROT = transform.eulerAngles.y;
            if (YROT < 0)
                YROT = YROT + 365;
            else if(YROT > 360) YROT = YROT - 360;
            Quaternion Rot = Quaternion.AngleAxis(YROT, transform.forward);
            Vector3 Dir = Rot * (transform.forward * curVelocity);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Dir, out hit, curVelocity, CombatManager.Instance.ProjectileDestroyLayer))
            {
                EnvironmentHit();
            } else if (Physics.Raycast(transform.position, Dir, out hit, curVelocity, rankREF.hitLayers))
            {
                CombatNode nodeREF = hit.collider.gameObject.GetComponent<CombatNode>();
                if (nodeREF == null) return;
                CombatNodeHit(nodeREF);
            }

            previousPos = transform.position;
        }

        void HandleMovement()
        {
            var transform1 = transform;
            float totalProjectileSpeed = rankREF.projectileSpeed + (rankREF.projectileSpeed * (cachedExtraSpeed / 100));
            transform1.position += transform1.forward * totalProjectileSpeed * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (!isReady) return;
            if (cachedMaxDistance != 0)
            {
                if (Vector3.Distance(initPos, transform.position) >= cachedMaxDistance)
                {
                    Destroy(gameObject);
                }
            }

            if (RB == null)
            {
                HandleMovement();
                CheckCollisions();
            }

            if (comesBackToCaster)
            {
                if (curTime < ComeBackAfterTime)
                {
                    curTime += Time.deltaTime;
                } else
                {
                    RB.velocity = Vector3.zero;
                    transform.position = Vector3.MoveTowards(transform.position, casterTransform.position, ComeBackSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, casterTransform.position) < 0.5f) Destroy(gameObject);
                }
            }

            if (!goToNearbyUnits || !didHitFirstUnit || curNearbyTargetGO == null) return;
            
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(curNearbyTargetGO.transform.position.x, curNearbyTargetGO.transform.position.y+1f, curNearbyTargetGO.transform.position.z), rankREF.projectileSpeed * Time.deltaTime);
        }

    }
}
