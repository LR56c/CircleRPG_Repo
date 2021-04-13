using System;
using System.Collections.Generic;
using System.Linq;
using THMSV.RPGBuilder.Character;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class CustomInputManager : MonoBehaviour
    {
        [Serializable]
        public class ActionKeys
        {
            public string keyName;
            public KeyCode defaultKey;
            public KeyCode currentKey;
        }

        public ActionKeys[] abilityKeys;
        public ActionKeys[] itemKeys;
        public ActionKeys[] UIKeys;
        public ActionKeys[] controlsKeys;

        public bool InitializationCompleted;

        private string currentlyModifiedKeybindName;
        private bool isKeyChecking;
        
        public List<CanvasGroup> allOpenedPanels = new List<CanvasGroup>();

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public void AddOpenedPanel(CanvasGroup cg)
        {
            allOpenedPanels.Insert(0, cg);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (allOpenedPanels.Count > 0)
                {
                    allOpenedPanels[0].gameObject.GetComponent<IDisplayPanel>().Hide();
                    return;
                }
                if (CombatManager.Instance.currentTarget != null)
                {
                    CombatManager.Instance.ResetPlayerTarget();
                    return;
                }
            }

            if (CombatManager.playerCombatInfo != null && InitializationCompleted)
            {
                CheckAbilityKeys();
                CheckItemKeys();
                CheckUIKeys();
                CheckControlsKeys();
            }

            if (!isKeyChecking) return;
            HandleKeyChange();
        }

        private void HandleKeyChange()
        {
            foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
                if (Input.GetKey(vKey))
                {
                    ModifyKeybind(currentlyModifiedKeybindName, vKey);
                    isKeyChecking = false;
                    currentlyModifiedKeybindName = "";
                }
        }

        private void CheckAbilityKeys()
        {
            foreach (var t in abilityKeys)
                if (Input.GetKeyDown(t.currentKey))
                {
                    if (RPGBuilderUtilities.IsPointerOverUIObject()) return;
                    // WE USED AN ABILITY KEY
                    var abilityIDString = t.keyName.Replace("ABILITY_ACTION_", "");
                    var abilityID = int.Parse(abilityIDString);
                    if (ActionBarDisplayManager.Instance.abilitySlots[abilityID - 1].curAb != null)
                        CombatManager.Instance.TRIGGER_PLAYER_ABILITY(CombatManager.playerCombatInfo
                            .abilitiesData[abilityID - 1].currentAbility);

                }
        }

        private void CheckItemKeys()
        {
            foreach (var t in itemKeys)
                if (Input.GetKeyDown(t.currentKey))
                {
                    if (RPGBuilderUtilities.IsPointerOverUIObject()) return;
                    // WE USED AN ITEM KEY
                    var itemIDString = t.keyName.Replace("ITEM_ACTION_", "");
                    var itemID = int.Parse(itemIDString);
                    if (ActionBarDisplayManager.Instance.itemSlots[itemID - 1].curItem != null)
                        InventoryManager.Instance.UseItemFromBar(ActionBarDisplayManager.Instance.itemSlots[itemID - 1].curItem);
                }
        }

        private void CheckUIKeys()
        {
            foreach (var t in UIKeys)
                if (Input.GetKeyDown(t.currentKey))
                {
                    switch (t.keyName)
                    {
                        case "CHARACTERPANEL":
                            CharacterPanelDisplayManager.Instance.Toggle();
                            break;
                        case "INVENTORYPANEL":
                            InventoryDisplayManager.Instance.Toggle();
                            break;
                        case "SKILLSPANEL":
                            SkillBookDisplayManager.Instance.Toggle();
                            break;
                        case "QUESTPANEL":
                            QuestJournalDisplayManager.Instance.Toggle();
                            break;
                        case "OPTIONSPANEL":
                            GameOptionsDisplayManager.Instance.Toggle();
                            if (CombatManager.playerCombatInfo.playerControllerREF.CurrentController ==
                                RPGBCharacterController.ControllerType.ThirdPerson &&
                                !CombatManager.playerCombatInfo.playerControllerREF.ClickToRotate)
                                CombatManager.playerCombatInfo.playerControllerREF.ClickToRotate = true;
                            break;
                        case "LOOTALL":
                            if (LootPanelDisplayManager.Instance.thisCG.alpha == 1)
                            {
                                LootPanelDisplayManager.Instance.LootAll();
                            }
                            break;
                    }
                }
        }

        private void CheckControlsKeys()
        {
            foreach (var t in controlsKeys)
                if (Input.GetKeyDown(t.currentKey))
                {
                    switch (t.keyName)
                    {
                        case "TOGGLECURSOR":
                            if (CombatManager.playerCombatInfo.playerControllerREF.CurrentController !=
                                RPGBCharacterController.ControllerType.ThirdPerson) return;
                            CombatManager.playerCombatInfo.playerControllerREF.ClickToRotate =
                                !CombatManager.playerCombatInfo.playerControllerREF.ClickToRotate;
                            break;

                    }
                }
        }

        public void InitKeyChecking(string keybindName)
        {
            isKeyChecking = true;
            currentlyModifiedKeybindName = keybindName;
        }

        public KeyCode getCurrentKeyByName(string keybindName)
        {
            foreach (var t in abilityKeys)
                if (t.keyName == keybindName)
                    return t.currentKey;
            
            foreach (var t in itemKeys)
                if (t.keyName == keybindName)
                    return t.currentKey;
            
            foreach (var t in UIKeys)
                if (t.keyName == keybindName)
                    return t.currentKey;

            return (from t in controlsKeys where t.keyName == keybindName select t.currentKey).FirstOrDefault();
        }

        private void ModifyKeybind(string keybindName, KeyCode newKey)
        {
            for (var i = 0; i < abilityKeys.Length; i++)
                if (abilityKeys[i].keyName == keybindName)
                {
                    abilityKeys[i].currentKey = newKey;
                    var keyExisted = false;
                    foreach (var t in CharacterData.Instance.abilityKeybindsDATA.Where(t => t.keybindName == keybindName))
                    {
                        t.key = newKey;
                        keyExisted = true;
                    }
                    ActionBarDisplayManager.Instance.HandleKeybindText(newKey, ActionBarDisplayManager.Instance.abilitySlots[i].slotREF.keybindText);

                    if (keyExisted) continue;
                    CharacterData.Instance.abilityKeybindsDATA.Add(getNewKeybindData(keybindName, newKey));

                }

            for (var i = 0; i < itemKeys.Length; i++)
                if (itemKeys[i].keyName == keybindName)
                {
                    itemKeys[i].currentKey = newKey;
                    var keyExisted = false;
                    foreach (var t in CharacterData.Instance.itemKeybindsDATA.Where(t => t.keybindName == keybindName))
                    {
                        t.key = newKey;
                        keyExisted = true;
                    }
                    ActionBarDisplayManager.Instance.HandleKeybindText(newKey, ActionBarDisplayManager.Instance.itemSlots[i].slotREF.keybindText);

                    if (keyExisted) continue;
                    CharacterData.Instance.itemKeybindsDATA.Add(getNewKeybindData(keybindName, newKey));

                }

            foreach (var t1 in UIKeys)
                if (t1.keyName == keybindName)
                {
                    t1.currentKey = newKey;
                    var keyExisted = false;
                    foreach (var t in CharacterData.Instance.UIKeybindsDATA.Where(t => t.keybindName == keybindName))
                    {
                        t.key = newKey;
                        keyExisted = true;
                    }

                    if (keyExisted) continue;
                    CharacterData.Instance.UIKeybindsDATA.Add(getNewKeybindData(keybindName, newKey));
                }

            foreach (var t1 in controlsKeys)
                if (t1.keyName == keybindName)
                {
                    t1.currentKey = newKey;
                    var keyExisted = false;
                    foreach (var t in CharacterData.Instance.controlsKeybindsDATA.Where(t => t.keybindName == keybindName))
                    {
                        t.key = newKey;
                        keyExisted = true;
                    }

                    if (keyExisted) continue;
                    CharacterData.Instance.controlsKeybindsDATA.Add(getNewKeybindData(keybindName, newKey));
                }

            SettingsPanelDisplayManager.Instance.InitializeKeybindSlots();
        }

        CharacterData.KeybindDATA getNewKeybindData(string keybindName, KeyCode newKey)
        {
            var newKeyData = new CharacterData.KeybindDATA();
            newKeyData.keybindName = keybindName;
            newKeyData.key = newKey;
            return newKeyData;
        }

        public void InitKEYS()
        {
            foreach (var t in abilityKeys)
            foreach (var t1 in CharacterData.Instance.abilityKeybindsDATA)
                if (t1.keybindName == t.keyName)
                    t.currentKey = t1.key;
            
            foreach (var t in itemKeys)
            foreach (var t1 in CharacterData.Instance.itemKeybindsDATA)
                if (t1.keybindName == t.keyName)
                    t.currentKey = t1.key;
            
            foreach (var t in UIKeys)
            foreach (var t1 in CharacterData.Instance.UIKeybindsDATA)
                if (t1.keybindName == t.keyName)
                    t.currentKey = t1.key;
            
            foreach (var t in controlsKeys)
            foreach (var t1 in CharacterData.Instance.controlsKeybindsDATA)
                if (t1.keybindName == t.keyName)
                    t.currentKey = t1.key;

            InitializationCompleted = true;
        }


        public static CustomInputManager Instance { get; private set; }
    }
}