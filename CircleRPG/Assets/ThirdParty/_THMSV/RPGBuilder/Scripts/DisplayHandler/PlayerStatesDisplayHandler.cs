using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;

namespace THMSV.RPGBuilder.DisplayHandler
{
    public class PlayerStatesDisplayHandler : MonoBehaviour
    {
        private static PlayerStatesDisplayHandler instance;

        public Transform buffParent, debuffParent;
        public GameObject stateSlotPrefab;

        [System.Serializable]
        public class currentPlayerStatesSlots
        {
            public string stateName;
            public RPGEffect stateEffect;
            public GameObject slotGO;
            public NodeStateSlot slotREF;
            public CombatNode.NodeStatesDATA stateDATA;
        }
        public List<currentPlayerStatesSlots> curStatesSlots;

        private void Start()
        {
            if (instance != null) return;
            instance = this;
        }

        public void UpdateStackText (int index)
        {
            curStatesSlots[index].slotREF.UpdateStackText();
        }

        public void DisplayState (CombatNode.NodeStatesDATA newState)
        {
            var newStateSlot = (GameObject)Instantiate(stateSlotPrefab);
            var newSlotREF = newStateSlot.GetComponent<NodeStateSlot>();
            if (newState.stateEffect.isBuffOnSelf)
            {
                newStateSlot.transform.SetParent(buffParent);
                newSlotREF.InitStateSlot(true, newState.stateEffect, newState.stateIcon, newState.stateMaxDuration, curStatesSlots.Count);
            } else
            {
                newStateSlot.transform.SetParent(debuffParent);
                newSlotREF.InitStateSlot(false, newState.stateEffect, newState.stateIcon, newState.stateMaxDuration, curStatesSlots.Count);
            }
            var newStateSlotData = new currentPlayerStatesSlots
            {
                stateName = newState.stateName,
                stateEffect = newState.stateEffect,
                slotGO = newStateSlot,
                slotREF = newSlotREF,
                stateDATA = newState
            };

            curStatesSlots.Add(newStateSlotData);
        }

        public void UpdateState(int index)
        {
            curStatesSlots[index].slotREF.InitStateSlot(curStatesSlots[index].stateDATA.stateEffect.isBuffOnSelf, curStatesSlots[index].stateDATA.stateEffect, curStatesSlots[index].stateDATA.stateIcon, curStatesSlots[index].stateDATA.stateMaxDuration, curStatesSlots[index].slotREF.thisIndex);
        }

        public void RemoveState (int index)
        {
            int cachedIndex = curStatesSlots[index].slotREF.thisIndex;
            Destroy(curStatesSlots[index].slotGO);
            curStatesSlots.RemoveAt(index);
            foreach (var t in curStatesSlots.Where(t => t.slotREF.thisIndex > cachedIndex))
            {
                t.slotREF.thisIndex--;
            }
        }


        public static PlayerStatesDisplayHandler Instance => instance;
    }
}
