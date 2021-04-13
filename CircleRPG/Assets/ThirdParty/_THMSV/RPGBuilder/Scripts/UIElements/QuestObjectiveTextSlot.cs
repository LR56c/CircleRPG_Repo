using TMPro;
using UnityEngine;

namespace THMSV.RPGBuilder.UIElements
{
    public class QuestObjectiveTextSlot : MonoBehaviour
    {
        public TextMeshProUGUI objectiveText;

        public void InitSlot(string text)
        {
            objectiveText.text = text;
        
        }
    }
}
