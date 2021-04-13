using THMSV.RPGBuilder.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class RaceSlotHolder : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI raceName;
        public Image selectedBorder;
        public int raceIndex;

        public void Init(RPGRace thisRace, int index)
        {
            icon.sprite = thisRace.icon;
            raceName.text = thisRace.displayName;
            raceIndex = index;
        }

        public void ClickSelect()
        {
            MainMenuManager.Instance.SelectRace(raceIndex);
        }
    }
}