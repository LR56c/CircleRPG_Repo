using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;

namespace THMSV.RPGBuilder.World
{
    [RequireComponent(typeof(CombatNode))]
    public class ObjectActionTrigger : MonoBehaviour
    {
        public enum ActionType
        {
            ability,
            effect
        }

        public ActionType actionType;

        public RPGAbility abilityTriggered;
        public RPGEffect effectTriggered;

        public float cooldown, nextHit;

        public string hitTag;
        private CombatNode thisNode;

        private void Start()
        {
            thisNode = GetComponent<CombatNode>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != hitTag) return;
            if (!(Time.time >= nextHit)) return;
            nextHit = Time.time + cooldown;

            if (actionType == ActionType.ability)
                TriggerAbility();
            else
                TriggerEffect(other.gameObject.GetComponent<CombatNode>());
        }

        private void TriggerEffect(CombatNode nodeHit)
        {
            CombatManager.Instance.ExecuteEffect(thisNode, nodeHit, effectTriggered, null);
        }

        private void TriggerAbility()
        {
            CombatManager.Instance.InitExtraAbility(thisNode, abilityTriggered);
        }
    }
}