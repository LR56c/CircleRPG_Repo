using System.Collections.Generic;
using THMSV.RPGBuilder.Character;
using THMSV.RPGBuilder.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.Managers
{
    public class SettingsPanelDisplayManager : MonoBehaviour
    {
        public CanvasGroup thisCG;

        public List<KeybindSlotHolder> keybindSlots = new List<KeybindSlotHolder>();

        [SerializeField] private Image clickToMoveButton, WASDButton, ThirdPersonButton;
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        
        }

        public void InitControllerButtons()
        {
            switch (CombatManager.playerCombatInfo.playerControllerREF.CurrentController)
            {
                case RPGBCharacterController.ControllerType.TopDown:
                    WASDButton.color = Color.green;
                    break;
                case RPGBCharacterController.ControllerType.ClickMove:
                    clickToMoveButton.color = Color.green;
                    break;
                case RPGBCharacterController.ControllerType.ThirdPerson:
                    ThirdPersonButton.color = Color.green;
                    break;
            }
        }

        void disableAllControllerButtons()
        {
            clickToMoveButton.color = Color.white;
            WASDButton.color = Color.white;
            ThirdPersonButton.color = Color.white;
        }

        public void SelectController(string controllerName)
        {
            disableAllControllerButtons();
            switch (controllerName)
            {
                case "click":
                    clickToMoveButton.color = Color.green;
                    CombatManager.playerCombatInfo.playerControllerREF.CurrentController = RPGBCharacterController.ControllerType.ClickMove;
                    break;

                case "wasd":
                    WASDButton.color = Color.green;
                    CombatManager.playerCombatInfo.playerControllerREF.CurrentController = RPGBCharacterController.ControllerType.TopDown;
                    break;

                case "thirdPerson":
                    ThirdPersonButton.color = Color.green;
                    CombatManager.playerCombatInfo.playerControllerREF.CurrentController = RPGBCharacterController.ControllerType.ThirdPerson;
                    break;
            }
        }

        public void InitializeKeybindSlots()
        {
            foreach (var t in keybindSlots)
                t.InitializeSlot();
        }

        private void Show()
        {
            RPGBuilderUtilities.EnableCG(thisCG);
            transform.SetAsLastSibling();
            InitializeKeybindSlots();
        
        }

        public void Hide()
        {
            gameObject.transform.SetAsFirstSibling();
            RPGBuilderUtilities.DisableCG(thisCG);
        }

        private void Awake()
        {
            Hide();
        }

        public void Toggle()
        {
            if (thisCG.alpha == 1)
                Hide();
            else
                Show();
        }

        public static SettingsPanelDisplayManager Instance { get; private set; }
    }
}