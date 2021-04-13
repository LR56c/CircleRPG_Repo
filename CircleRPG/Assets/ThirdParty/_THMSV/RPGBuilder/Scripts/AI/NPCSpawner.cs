using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.Logic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace THMSV.RPGBuilder.AI
{
    public class NPCSpawner : MonoBehaviour
    {
    
        [SerializeField] private AILogic.SpawnerType spawnerType;
        [SerializeField] private int spawnCount;
        //[SerializeField] private List<RPGNpc> NPCList = new List<RPGNpc>();

        [System.Serializable]
        public class NPC_SPAWN_DATA
        {
            public RPGNpc npc;
            public float spawnChance;
        }
        public List<NPC_SPAWN_DATA> spawnData = new List<NPC_SPAWN_DATA>();
        
        private CombatNode curNPC;

        private bool hasSpawnedOnce;
        private int curSpawnedCount;
    

        private void Start()
        {
            StartCoroutine(ExecuteSpawner(0));
        }

        public IEnumerator ExecuteSpawner (float delay)
        {
            switch (spawnerType)
            {

                case AILogic.SpawnerType.Count:
                    if (curSpawnedCount >= spawnCount) yield break;
                
                    yield return new WaitForSeconds(delay);
                    SpawnNPC();
                    curSpawnedCount++;
                    break;

                case AILogic.SpawnerType.Endless:
                    yield return new WaitForSeconds(delay);
                    SpawnNPC();
                    break;
                case AILogic.SpawnerType.Manual:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        RPGNpc PickRandomNPC()
        {
            float rdmNPC = Random.Range(0f, 100f);
            float offset = 0;
            foreach (var t in spawnData)
            {
                if (rdmNPC >= 0 + offset && rdmNPC <= t.spawnChance + offset)
                {
                    RPGNpc npc = t.npc;
                    return npc;
                }
                offset += t.spawnChance;
            }

            return null;
        }

        private void SpawnNPC ()
        {
            RPGNpc pickedNPC = PickRandomNPC();
            if (pickedNPC == null) return;
            
            var npc = Instantiate(pickedNPC.NPCPrefab, transform.position, transform.rotation);
            curNPC = npc.GetComponent<CombatNode>();
            curNPC.spawnerREF = this;
            curNPC.InitializeCombatNode();

            CombatManager.Instance.allCombatNodes.Add(curNPC);
        }
    
        public void ManualSpawnNPC()
        {
            if(curNPC!=null) Destroy(curNPC.gameObject);

            RPGNpc pickedNPC = PickRandomNPC();
            if (pickedNPC == null) return;
            var npc = Instantiate(pickedNPC.NPCPrefab, transform.position, transform.rotation);
            curNPC = npc.GetComponent<CombatNode>();
            curNPC.spawnerREF = this;
            curNPC.InitializeCombatNode();

            CombatManager.Instance.allCombatNodes.Add(curNPC);
        }

        private void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
    }
}
