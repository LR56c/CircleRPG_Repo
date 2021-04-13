using THMSV.RPGBuilder.LogicMono;
using THMSV.RPGBuilder.Managers;
using UnityEngine;
using TMPro;

namespace THMSV.RPGBuilder.UIElements
{
    public class StatDataHolder : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI statText;
        private RPGStat statREF;

        public void InitStatText(CombatNode.NODE_STATS statDataREF)
        {
            statREF = statDataREF.stat;
            if (statREF.statType == RPGStat.STAT_TYPE.VITALITY)
                statText.text = statREF.displayName + ": " + statDataREF.curMaxValue;
            else
                statText.text = statREF.displayName + ": " + statDataREF.curValue;
        }

        public void ShowStatTooltip()
        {
            if(statREF.description != "") CharacterPanelDisplayManager.Instance.ShowStatTooltipPanel(statREF);
        }

        public void HideStatTooltip()
        {
            CharacterPanelDisplayManager.Instance.HideStatTooltipPanel();
        }
        
    }
}
