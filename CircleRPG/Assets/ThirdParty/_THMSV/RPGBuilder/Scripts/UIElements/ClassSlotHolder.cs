using THMSV.RPGBuilder.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class ClassSlotHolder : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI className;
        public Image selectedBorder;
        public int classIndex;

        public void Init(RPGClass thisClass, int index)
        {
            icon.sprite = thisClass.icon;
            className.text = thisClass.displayName;
            classIndex = index;
        }

        public void ClickSelect()
        {
            MainMenuManager.Instance.SelectClass(classIndex);
        }
    }
}