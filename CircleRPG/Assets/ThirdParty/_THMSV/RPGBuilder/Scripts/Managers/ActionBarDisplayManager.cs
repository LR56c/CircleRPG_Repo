using System.Collections.Generic;
using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace THMSV.RPGBuilder.Managers
{
    public class ActionBarDisplayManager : MonoBehaviour
    {
        [SerializeField] private GameObject actionBarSlotPrefab;
        [SerializeField] private Transform abilitiesSlotsParent;
        
        
        [System.Serializable]
        public class AbilitiesSlots
        {
            public AbilitySlotHolder slotREF;
            public RPGAbility curAb;
        }
        public List<AbilitiesSlots> abilitySlots = new List<AbilitiesSlots>();
    
        [System.Serializable]
        public class ItemsSlots
        {
            public ItemActionSlotHolder slotREF;
            public RPGItem curItem;
        }
        public ItemsSlots[] itemSlots;
        public void SetItemToSlot (RPGItem item, int index)
        {
            itemSlots[index].curItem = item;
            InitializeItemSlots();

            CharacterData.Instance.actionBarItems[index].curItemID = item.ID;
        }
        
        public void SetAbilityToSlot (RPGAbility ab, int index)
        {
            if (index > abilitySlots.Count-1) return;
            abilitySlots[index].curAb = ab;
            InitializeAbilitySlots();

            CombatManager.playerCombatInfo.abilitiesData[index].currentAbility = ab;
            CharacterData.Instance.actionBarAbilities[index].curAbID = ab.ID;
        }

        public void ResetAbilitySlot(int index)
        {
            abilitySlots[index].curAb = null;
            InitializeAbilitySlots();

            CombatManager.playerCombatInfo.abilitiesData[index].currentAbility = null;
            CombatManager.playerCombatInfo.abilitiesData[index].curAbilityID = -1;

            abilitySlots[index].slotREF.icon.enabled = false;
            CharacterData.Instance.actionBarAbilities[index].curAbID = -1;
        }

        public void ResetItemSlot(int index)
        {
            itemSlots[index].curItem = null;
            InitializeItemSlots();

            itemSlots[index].slotREF.icon.enabled = false;
            CharacterData.Instance.actionBarItems[index].curItemID = -1;
        }
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public void InitAbilityActionBar()
        {
            foreach (var t in abilitySlots)
            {
                Destroy(t.slotREF.gameObject);
            }
            abilitySlots.Clear();
            for (int i = 0; i < RPGBuilderEssentials.Instance.combatSettings.actionBarSlots; i++)
            {
                AbilitiesSlots newSlot = new AbilitiesSlots();

                GameObject newSlotGO = Instantiate(actionBarSlotPrefab, abilitiesSlotsParent);
                newSlot.slotREF = newSlotGO.GetComponent<AbilitySlotHolder>();
                
                abilitySlots.Add(newSlot);
            }
            
            CombatManager.playerCombatInfo.abilitiesData.Clear();
            for (int i = 0; i < abilitySlots.Count; i++)
            {
                CombatNode.AbilitiesDATA newAbData = new CombatNode.AbilitiesDATA();
                CombatManager.playerCombatInfo.abilitiesData.Add(newAbData);
            }
        }

        private void Update()
        {
            foreach (var t in abilitySlots)
                if (t.curAb != null)
                {
                    var cdDATA = CharacterData.Instance.getAbilityCD(t.curAb);
                    if (cdDATA != null)
                    {
                        if (CombatManager.playerCombatInfo != null)
                        {
                            if (cdDATA[1] > 0)
                            {
                                t.slotREF.CDOverlay.fillAmount = cdDATA[1] / cdDATA[0];
                                t.slotREF.abilityCDText.text = cdDATA[1].ToString("F0");
                            }
                            else
                            {
                                t.slotREF.CDOverlay.fillAmount = 0;
                                t.slotREF.abilityCDText.text = "";
                            }
                        }
                        else
                        {
                            t.slotREF.CDOverlay.fillAmount = 0;
                            t.slotREF.abilityCDText.text = "";
                        }
                    }
                    else
                    {
                        t.slotREF.CDOverlay.fillAmount = 0;
                        t.slotREF.abilityCDText.text = "";
                    }
                }
                else
                {
                    t.slotREF.CDOverlay.fillAmount = 0;
                    t.slotREF.abilityCDText.text = "";
                }
        }

        public void InitializeAbilitySlots()
        {
            for (var i = 0; i < abilitySlots.Count; i++)
                if (abilitySlots[i].curAb != null)
                {
                    abilitySlots[i].slotREF.icon.enabled = true;
                    abilitySlots[i].slotREF.Init(abilitySlots[i].curAb, i);
                    if (i <= CustomInputManager.Instance.abilityKeys.Length - 1)
                        HandleKeybindText(CustomInputManager.Instance.abilityKeys[i].currentKey,
                            abilitySlots[i].slotREF.keybindText);
                }
                else
                {
                    abilitySlots[i].slotREF.icon.enabled = false;
                    abilitySlots[i].slotREF.Reset();
                    if (i <= CustomInputManager.Instance.abilityKeys.Length - 1)
                        HandleKeybindText(CustomInputManager.Instance.abilityKeys[i].currentKey,
                            abilitySlots[i].slotREF.keybindText);
                }
        }


        public void InitializeItemSlots ()
        {
            for (var i = 0; i < itemSlots.Length; i++)
                if (itemSlots[i].curItem != null)
                {
                    itemSlots[i].slotREF.icon.enabled = true;
                    itemSlots[i].slotREF.Init(itemSlots[i].curItem, i);
                    HandleKeybindText(CustomInputManager.Instance.itemKeys[i].currentKey, itemSlots[i].slotREF.keybindText);
                } else
                {
                    itemSlots[i].slotREF.icon.enabled = false;
                    itemSlots[i].slotREF.Reset();
                    HandleKeybindText(CustomInputManager.Instance.itemKeys[i].currentKey, itemSlots[i].slotREF.keybindText);
                }
        }

        public void HandleKeybindText(KeyCode key, TextMeshProUGUI textRef)
        {
            var KeyBindString = key.ToString();
            if (KeyBindString.Contains("Alpha"))
            {
                var alphakey = KeyBindString.Remove(0, 5);
                textRef.text = alphakey;
            }
            else if (KeyBindString.Contains("Mouse"))
            {
                var alphakey = KeyBindString.Remove(0, 5);
                textRef.text = "M" + alphakey;
            }
            else
            {
                textRef.text = KeyBindString;
            }
        }

        public void CheckItemBarState()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].curItem == null) continue;
                int ttlCount = InventoryManager.Instance.getTotalCountOfItem(itemSlots[i].curItem);
                if (ttlCount <= 0)
                {
                    ResetItemSlot(i);
                }
                else
                {
                    itemSlots[i].slotREF.UpdateSlot(ttlCount);
                }
            }
        }

        public static ActionBarDisplayManager Instance { get; private set; }
    }
}
