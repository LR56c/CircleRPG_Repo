using System;
using System.Collections;
using System.Collections.Generic;
using THMSV.RPGBuilder.Character;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

namespace THMSV.RPGBuilder.CombatVisuals
{
    public class CombatVisualEffect : MonoBehaviour
    {
        
        private CombatNode ownerNode;

        public CombatNode OwnerNode
        {
            get => ownerNode;
            set => ownerNode = value;
        }
        public List<GameObject> allGOs = new List<GameObject>();

        private enum CombatVisualType
        {
            EffectOnNode,
            Projectile,
            Ground,
            ReflectingShield
        }

        [SerializeField] private CombatVisualType combatVisualType;

        [SerializeField] private UnitTransformSockets.SOCKETS socket;
        private GameObject nodeGO;
        [SerializeField] private GameObject EffectGO;
        [SerializeField] private AudioClip EffectSound;
        [SerializeField] private bool attachSoundToEffect;
        public GameObject HitGO;
        [SerializeField] private Vector3 effectPOS;
        public Vector3 hitEffectPOS;

        [NonSerialized]public CombatNode nodeREF;

        [SerializeField] private float activationDelay;
        [SerializeField] private float thisDuration;
        [SerializeField] private float mainEffectDuration;
        public float hitEffectDuration;
        [SerializeField] private float YPosOffset;
        [SerializeField] private bool comesBackToCaster;
        [SerializeField] private bool parentedToNode;
        [SerializeField] private bool isGroundCasted;
        private RPGAbility thisAb;
        public RPGAbility ThisAB 
        {
            get => thisAb;
            set => thisAb = value;
        }
    
        private RPGEffect thisEffect;
        private RPGAbilityRankData curAbilityRank;
        public RPGAbilityRankData ThisCurAbRank 
        {
            get => curAbilityRank;
            set => curAbilityRank = value;
        }

        [Serializable]
        public class CURRENT_PROJECTILES
        {
            public GameObject projGO;
            public int unitHit;

        }
        public List<CURRENT_PROJECTILES> currentProjectiles;

        private PlayerAppearanceHandler unitAppearanceREF;

        private int cachedMaxUnitHit;

        public void InitCombatVisual (CombatNode node, RPGAbility ab, PlayerAppearanceHandler appearREF, RPGAbilityRankData rankREF)
        {
            Destroy(gameObject, thisDuration);

            nodeGO = node.gameObject;
            thisAb = ab;
            curAbilityRank = rankREF;
            unitAppearanceREF = appearREF;
            nodeREF = node;
            InitValues();
            StartCoroutine(ExecuteCombatVisual(rankREF));
        }
        public void InitCombatVisual(CombatNode node, RPGAbility ab, PlayerAppearanceHandler appearREF, RPGEffect effect, RPGAbilityRankData rankREF)
        {
            Destroy(gameObject, thisDuration);

            nodeGO = node.gameObject;
            thisAb = ab;
            curAbilityRank = rankREF;
            thisEffect = effect;
            unitAppearanceREF = appearREF;
            nodeREF = node;
            InitValues();
            StartCoroutine(ExecuteCombatVisual(rankREF));
        }

        public void InitCombatVisual(CombatNode node, RPGAbility ab, Vector3 POS, PlayerAppearanceHandler appearREF, RPGAbilityRankData rankREF)
        {
            Destroy(gameObject, thisDuration);

            nodeGO = node.gameObject;
            thisAb = ab;
            curAbilityRank = rankREF;
            effectPOS = POS;
            isGroundCasted = true;
            unitAppearanceREF = appearREF;
            nodeREF = node;
            InitValues();
            StartCoroutine(ExecuteCombatVisual(rankREF));
        }

        void InitValues()
        {
            if (curAbilityRank == null) return;
            cachedMaxUnitHit = curAbilityRank.MaxUnitHit +
                               (int)CombatManager.Instance.GetTotalOfStatType(nodeREF, RPGStat.STAT_TYPE.ABILITY_MAX_HIT);
        }

        private void PlaySound (GameObject go)
        {
            if (EffectSound == null) return;
            GameObject goTarget = null;
            goTarget = attachSoundToEffect ? go : gameObject;
            var ASource = goTarget.AddComponent<AudioSource>();
            ASource.volume = 0.2f;
            ASource.PlayOneShot(EffectSound);
        }

        private void HandleProjectile(RPGBCharacterController.ControllerType controllerType, RPGAbilityRankData rankREF, float OFFSET)
        {
            switch (controllerType)
            {
                case RPGBCharacterController.ControllerType.ClickMove:
                case RPGBCharacterController.ControllerType.TopDown:
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var plane = new Plane(Vector3.up, new Vector3(0, transform.position.y + YPosOffset, 0));
                    float distance;
                    if (plane.Raycast(ray, out distance))
                    {
                        var target = ray.GetPoint(distance);
                        var direction = target - transform.position;
                        var rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                        var cbtVisualGO = Instantiate(EffectGO, new Vector3(nodeGO.transform.position.x, nodeGO.transform.position.y + YPosOffset, nodeGO.transform.position.z), Quaternion.identity);
                        var newProj = new CURRENT_PROJECTILES();
                        newProj.projGO = cbtVisualGO;
                        newProj.unitHit = 0;
                        currentProjectiles.Add(newProj);

                        var projHitRef = cbtVisualGO.GetComponent<ProjectileHitDetection>();
                        if (!comesBackToCaster)
                            projHitRef.InitProjectile(this, mainEffectDuration);
                        else
                            projHitRef.InitProjectile(this, mainEffectDuration, true, nodeGO.transform, rankREF.projectileComeBackAfterTime, rankREF.projectileComeBackSpeed);
                        if (parentedToNode)
                        {
                            cbtVisualGO.transform.SetParent(nodeGO.transform);
                            cbtVisualGO.transform.localPosition = Vector3.zero;
                        }

                        projHitRef.transform.rotation = Quaternion.Euler(0, rotation + OFFSET, 0);
                        if (projHitRef.RB != null)
                        {
                            projHitRef.RB.constraints = RigidbodyConstraints.FreezePositionY |
                                                        RigidbodyConstraints.FreezeRotationX |
                                                        RigidbodyConstraints.FreezeRotationY |
                                                        RigidbodyConstraints.FreezeRotationZ;
                            float totalProjectileSpeed = rankREF.projectileSpeed + (rankREF.projectileSpeed *
                                (CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                                    RPGStat.STAT_TYPE.PROJECTILE_SPEED) / 100));
                            projHitRef.RB.velocity = projHitRef.transform.forward * totalProjectileSpeed;
                        }
                        

                        if (EffectSound != null) PlaySound(cbtVisualGO);
                    }
                    break;
                }
                case RPGBCharacterController.ControllerType.ThirdPerson:
                {
                    var cbtVisualGO = Instantiate(EffectGO, nodeREF.appearanceREF.ProjectilePoint.transform.position, Quaternion.identity);
                    var newProj = new CURRENT_PROJECTILES();
                    newProj.projGO = cbtVisualGO;
                    newProj.unitHit = 0;
                    currentProjectiles.Add(newProj);

                    var projHitRef = cbtVisualGO.GetComponent<ProjectileHitDetection>();
                    if (!comesBackToCaster)
                        projHitRef.InitProjectile(this, mainEffectDuration);
                    else
                        projHitRef.InitProjectile(this, mainEffectDuration, true, nodeGO.transform, rankREF.projectileComeBackAfterTime, rankREF.projectileComeBackSpeed);
                    if (parentedToNode)
                    {
                        cbtVisualGO.transform.SetParent(nodeGO.transform);
                        cbtVisualGO.transform.localPosition = Vector3.zero;
                    }

                    if (nodeREF.playerControllerREF.ClickToRotate)
                    {
                        projHitRef.transform.rotation = Quaternion.Euler(nodeGO.transform.eulerAngles.x, nodeGO.transform.eulerAngles.y + OFFSET, nodeGO.transform.eulerAngles.z);
                        if (projHitRef.RB != null)
                        {
                            projHitRef.RB.constraints = RigidbodyConstraints.FreezePositionY |
                                                        RigidbodyConstraints.FreezeRotationX |
                                                        RigidbodyConstraints.FreezeRotationY |
                                                        RigidbodyConstraints.FreezeRotationZ;
                            float totalProjectileSpeed = rankREF.projectileSpeed + (rankREF.projectileSpeed *
                                (CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                                    RPGStat.STAT_TYPE.PROJECTILE_SPEED) / 100));
                            projHitRef.RB.velocity = projHitRef.transform.forward * totalProjectileSpeed;
                        }
                    }
                    else
                    {
                        Vector3 v3LookPoint;
                        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                        RaycastHit hit;

                        v3LookPoint = Physics.Raycast(ray, out hit, 300, CombatManager.Instance.ProjectileCheckLayer)
                            ? hit.point
                            : ray.GetPoint(300);

                        projHitRef.transform.LookAt(v3LookPoint);
                        projHitRef.transform.rotation = Quaternion.Euler(projHitRef.transform.eulerAngles.x,
                            projHitRef.transform.eulerAngles.y + OFFSET, projHitRef.transform.eulerAngles.z);
                        if (projHitRef.RB != null)
                        {
                            
                            float totalProjectileSpeed = rankREF.projectileSpeed + (rankREF.projectileSpeed *
                                (CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                                    RPGStat.STAT_TYPE.PROJECTILE_SPEED) / 100));
                            projHitRef.RB.AddRelativeForce(
                                projHitRef.transform.forward * (totalProjectileSpeed  * 50));
                            projHitRef.RB.constraints = RigidbodyConstraints.FreezeRotationX |
                                                        RigidbodyConstraints.FreezeRotationY |
                                                        RigidbodyConstraints.FreezeRotationZ;
                        }
                    }

                    if (EffectSound != null) PlaySound(cbtVisualGO);

                    break;
                }
            }
        }

        private IEnumerator ExecuteCombatVisual(RPGAbilityRankData rankREF)
        {
            yield return new WaitForSeconds(activationDelay);

            if (EffectGO == null && EffectSound != null)
            { 
                PlaySound(CombatManager.playerCombatInfo.gameObject);
                yield break;
            }

            switch (combatVisualType)
            {
                case CombatVisualType.EffectOnNode:
                {
                    if (nodeGO == null)
                    {
                        Destroy(gameObject);
                        yield break;
                    }
                    var trsf = nodeGO.transform;
                    if (unitAppearanceREF != null)
                        if (socket != UnitTransformSockets.SOCKETS.none)
                            switch (socket)
                            {
                                case UnitTransformSockets.SOCKETS.head:
                                    if (unitAppearanceREF.HeadTransformSocket != null) trsf = unitAppearanceREF.HeadTransformSocket;
                                    break;
                            }

                    var cbtVisualGO = Instantiate(EffectGO, trsf.position, EffectGO.transform.rotation);
                    allGOs.Add(cbtVisualGO);
                    if (parentedToNode)
                    { // PARENT THE EFFECT IF NEEDED
                        cbtVisualGO.transform.SetParent(trsf);
                        cbtVisualGO.transform.localPosition = new Vector3(0, YPosOffset, 0);
                        cbtVisualGO.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        //cbtVisualGO.transform.localRotation = nodeGO.transform.localRotation;
                    }


                    if(EffectSound != null) PlaySound(cbtVisualGO);
                    // DESTROY THE EFFECT
                    Destroy(cbtVisualGO, mainEffectDuration);
                    break;
                }
                case CombatVisualType.ReflectingShield:
                {
                    var trsf = nodeGO.transform;
                    if (unitAppearanceREF != null)
                        if (socket != UnitTransformSockets.SOCKETS.none)
                            switch (socket)
                            {
                                case UnitTransformSockets.SOCKETS.head:
                                    if (unitAppearanceREF.HeadTransformSocket != null) trsf = unitAppearanceREF.HeadTransformSocket;
                                    break;
                            }

                    var cbtVisualGO = (GameObject)Instantiate(EffectGO, trsf.position, EffectGO.transform.rotation);
                    allGOs.Add(cbtVisualGO);
                    if (parentedToNode)
                    { // PARENT THE EFFECT IF NEEDED
                        cbtVisualGO.transform.SetParent(trsf);
                        cbtVisualGO.transform.localPosition = new Vector3(0, YPosOffset, 0);
                        cbtVisualGO.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        //cbtVisualGO.transform.localRotation = nodeGO.transform.localRotation;
                    }

                    var projRef = cbtVisualGO.GetComponent<ProjectileHitDetection>();
                    projRef.maxRelfectedProjectileAmount = thisEffect.projectilesReflectedCount;
                    if (!comesBackToCaster) projRef.InitProjectile(this, mainEffectDuration);

                    // DESTROY THE EFFECT
                    Destroy(cbtVisualGO, mainEffectDuration);
                    break;
                }
                case CombatVisualType.Ground:
                {
                    effectPOS.y += YPosOffset;
                    var cbtVisualGO = (GameObject)Instantiate(EffectGO, effectPOS, EffectGO.transform.rotation);
                    allGOs.Add(cbtVisualGO);
                    var groundHitRef = cbtVisualGO.GetComponent<GroundHitDetection>();
                    groundHitRef.InitGroundAbility(this, mainEffectDuration, thisAb);

                    if (EffectSound != null) PlaySound(cbtVisualGO);
                    // DESTROY THE EFFECT
                    Destroy(cbtVisualGO, mainEffectDuration);
                    break;
                }
                case CombatVisualType.Projectile:
                {

                    if (rankREF.projectileAngleSpread > 0)
                    {
                        float totalSpread = rankREF.projectileAngleSpread + (rankREF.projectileAngleSpread * (CombatManager.Instance.GetTotalOfStatType(nodeREF,
                            RPGStat.STAT_TYPE.PROJECTILE_ANGLE_SPREAD)/100));
                        float totalCount = rankREF.projectileCount + CombatManager.Instance.GetTotalOfStatType(nodeREF, RPGStat.STAT_TYPE.PROJECTILE_COUNT);
                        var OFFSET = -(totalSpread / 2);
                        var projCountOnEachSide = (totalCount - 1) / 2;
                        var intervalOffset = totalSpread / 2 / projCountOnEachSide;

                        for (var i = 0; i < totalCount; i++)
                            if (nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
                            {
                                HandleProjectile(nodeREF.playerControllerREF.CurrentController, rankREF, OFFSET);
                                OFFSET += intervalOffset;

                                if (rankREF.projectileDelay > 0) yield return new WaitForSeconds(rankREF.projectileDelay);
                            }
                            else
                            {
                                var cbtVisualGO = Instantiate(EffectGO, new Vector3(nodeGO.transform.position.x, nodeGO.transform.position.y + YPosOffset, nodeGO.transform.position.z), Quaternion.identity);
                                var newProj = new CURRENT_PROJECTILES();
                                newProj.projGO = cbtVisualGO;
                                newProj.unitHit = 0;
                                currentProjectiles.Add(newProj);

                                var projHitRef = cbtVisualGO.GetComponent<ProjectileHitDetection>();
                                if (!comesBackToCaster)
                                    projHitRef.InitProjectile(this, mainEffectDuration);
                                else
                                    projHitRef.InitProjectile(this, mainEffectDuration, true, nodeGO.transform, rankREF.projectileComeBackAfterTime, rankREF.projectileComeBackSpeed);
                                if (parentedToNode)
                                {
                                    cbtVisualGO.transform.SetParent(nodeGO.transform);
                                    cbtVisualGO.transform.localPosition = Vector3.zero;
                                }

                                projHitRef.transform.rotation = Quaternion.Euler(0, nodeGO.transform.rotation.eulerAngles.y + OFFSET, 0);
                                
                                float totalProjectileSpeed = rankREF.projectileSpeed + (rankREF.projectileSpeed *
                                    (CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                                        RPGStat.STAT_TYPE.PROJECTILE_SPEED) / 100));
                                projHitRef.RB.velocity = projHitRef.transform.forward * totalProjectileSpeed;
                                OFFSET += intervalOffset;

                                if (EffectSound != null) PlaySound(cbtVisualGO);

                                if (rankREF.projectileDelay > 0) yield return new WaitForSeconds(rankREF.projectileDelay);
                            }
                    }
                    else
                    {
                        float totalCount = rankREF.projectileCount + CombatManager.Instance.GetTotalOfStatType(nodeREF, RPGStat.STAT_TYPE.PROJECTILE_COUNT);
                        for (var i = 0; i < totalCount; i++)
                            if (nodeREF.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
                            {
                                HandleProjectile(nodeREF.playerControllerREF.CurrentController, rankREF, 0);

                                if (rankREF.projectileDelay > 0) yield return new WaitForSeconds(rankREF.projectileDelay);
                            }
                            else
                            {
                                var cbtVisualGO = (GameObject)Instantiate(EffectGO, new Vector3(nodeGO.transform.position.x, nodeGO.transform.position.y + YPosOffset, nodeGO.transform.position.z), Quaternion.identity);
                                var newProj = new CURRENT_PROJECTILES();
                                newProj.projGO = cbtVisualGO;
                                newProj.unitHit = 0;
                                currentProjectiles.Add(newProj);

                                var projHitRef = cbtVisualGO.GetComponent<ProjectileHitDetection>();
                                if (!comesBackToCaster)
                                    projHitRef.InitProjectile(this, mainEffectDuration);
                                else
                                    projHitRef.InitProjectile(this, mainEffectDuration, true, nodeGO.transform, rankREF.projectileComeBackAfterTime, rankREF.projectileComeBackSpeed);
                                if (parentedToNode)
                                {
                                    cbtVisualGO.transform.SetParent(nodeGO.transform);
                                    cbtVisualGO.transform.localPosition = Vector3.zero;
                                }

                                projHitRef.transform.rotation = Quaternion.Euler(0, nodeGO.transform.rotation.eulerAngles.y, 0);
                                
                                float totalProjectileSpeed = rankREF.projectileSpeed + (rankREF.projectileSpeed *
                                    (CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                                        RPGStat.STAT_TYPE.PROJECTILE_SPEED) / 100));
                                projHitRef.RB.velocity = projHitRef.transform.forward * totalProjectileSpeed;

                                if (EffectSound != null) PlaySound(cbtVisualGO);

                                if (rankREF.projectileDelay > 0) yield return new WaitForSeconds(rankREF.projectileDelay);
                            }
                    }

                    break;
                }
            }
        }

        public void ProjectileHit (GameObject projGO, CombatNode casterInfo, CombatNode targetInfo)
        {
            for (var i = 0; i < currentProjectiles.Count; i++)
                if(currentProjectiles[i].projGO == projGO)
                {
                    currentProjectiles[i].unitHit++;

                    CombatManager.Instance.EXECUTE_PROJECTILE_ABILITY_HIT(casterInfo, targetInfo, thisAb, curAbilityRank);

                    
                    if (currentProjectiles[i].unitHit < cachedMaxUnitHit ||
                        curAbilityRank.projectileNearbyUnitMaxHit != 0) continue;
                    //DESTROY PROJECTILE
                    Destroy(currentProjectiles[i].projGO);
                    currentProjectiles.RemoveAt(i);
                }
        }

    }
}
