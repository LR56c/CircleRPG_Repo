using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class QuestItemSlotHolder : MonoBehaviour
    {
        public enum QuestRewardType
        {
            itemGiven,
            rewardGiven,
            rewardToPick
        }

        public QuestRewardType thisType;

        public Image icon;
        public TextMeshProUGUI stackText;

        private RPGItem thisItem;
        private RPGCurrency thisCurrency;
        private RPGTreePoint thisTreePoint;
        private RPGQuest.QuestRewardDATA thisRewardDATA;

        public Image selectedBorder;

        public void InitItemGivenSlot(RPGItem item, int count)
        {
            selectedBorder.enabled = false;
            thisItem = item;
            icon.sprite = item.icon;
            var curstack = count;
            stackText.text = curstack.ToString();
        }

        public void InitSlot(RPGItem item, int count, QuestRewardType type, RPGQuest.QuestRewardDATA rewardDATA)
        {
            thisRewardDATA = rewardDATA;
            selectedBorder.enabled = false;
            thisType = type;
            thisItem = item;
            icon.sprite = item.icon;
            var curstack = count;
            stackText.text = curstack.ToString();
        }

        public void InitSlot(RPGCurrency currency, int count, QuestRewardType type, RPGQuest.QuestRewardDATA rewardDATA)
        {
            thisRewardDATA = rewardDATA;
            selectedBorder.enabled = false;
            thisType = type;
            thisCurrency = currency;
            icon.sprite = currency.icon;
            var curstack = count;
            stackText.text = curstack.ToString();
        }

        public void InitSlot(RPGTreePoint treePoint, int count, QuestRewardType type, RPGQuest.QuestRewardDATA rewardDATA)
        {
            thisRewardDATA = rewardDATA;
            selectedBorder.enabled = false;
            thisType = type;
            thisTreePoint = treePoint;
            icon.sprite = treePoint.icon;
            var curstack = count;
            stackText.text = curstack.ToString();
        }

        public void InitSlotEXP(int count, QuestRewardType type, RPGQuest.QuestRewardDATA rewardDATA)
        {
            thisRewardDATA = rewardDATA;
            selectedBorder.enabled = false;
            thisType = type;
            icon.sprite = QuestInteractionDisplayManager.Instance.experienceICON;
            var curstack = count;
            stackText.text = curstack.ToString();
        }

        public void SelectRewardToPick()
        {
            if (thisType == QuestRewardType.rewardToPick)
                QuestInteractionDisplayManager.Instance.SelectAReward(this, thisRewardDATA);
        }

        public void ShowTooltip()
        {
            if (thisItem != null) ItemTooltip.Instance.Show(thisItem.ID, -1, true);
            if (thisCurrency != null) ItemTooltip.Instance.ShowCurrencyTooltip(thisCurrency.ID);
            if (thisTreePoint != null) ItemTooltip.Instance.ShowTreePointTooltip(thisTreePoint.ID);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }
    }
}