using System;
using System.Collections;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace THMSV.RPGBuilder.World
{
    public class WorldDroppedItem : MonoBehaviour
    {
        public float curLifetime, maxDuration;
        public RPGItem item;
        
        private void FixedUpdate()
        {
            curLifetime += Time.deltaTime;
            if (curLifetime >= maxDuration)
            {
                InventoryManager.Instance.DestroyWorldDroppedItem(this);
            }
        }


        public void InitPhysics(Vector3 exploPos)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
            foreach (var t in CombatManager.Instance.allCombatNodes)
            {
                Physics.IgnoreCollision(collider,
                    t.nodeType == CombatNode.COMBAT_NODE_TYPE.player
                        ? t.gameObject.GetComponent<CharacterController>()
                        : t.gameObject.GetComponent<Collider>());
            }

            rb.drag = 3;
            rb.AddRelativeForce(new Vector3(Random.Range(-5, 5), 5, Random.Range(-5, 5)), ForceMode.Impulse);
            rb.AddRelativeTorque(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), ForceMode.Impulse);
            ScreenSpaceWorldDroppedItems.Instance.RegisterNewNameplate(GetComponent<Renderer>(), gameObject, item);
        }
    }
}
