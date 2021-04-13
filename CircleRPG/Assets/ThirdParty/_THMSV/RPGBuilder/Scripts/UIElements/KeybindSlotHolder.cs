using THMSV.RPGBuilder.Managers;
using TMPro;
using UnityEngine;

namespace THMSV.RPGBuilder.UIElements
{
    public class KeybindSlotHolder : MonoBehaviour
    {
        public TextMeshProUGUI keybindNameText, keybindValueText;
        public string KEYBINDNAME;
        public string displayName;

        public void ClickKeybindSlot()
        {
            CustomInputManager.Instance.InitKeyChecking(KEYBINDNAME);
        }

        public void InitializeSlot()
        {
            ActionBarDisplayManager.Instance.HandleKeybindText(CustomInputManager.Instance.getCurrentKeyByName(KEYBINDNAME), keybindValueText);
            keybindNameText.text = displayName + ":";
        }
    }
}