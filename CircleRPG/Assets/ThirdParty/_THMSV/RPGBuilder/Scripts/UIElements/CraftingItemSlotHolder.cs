using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class CraftingItemSlotHolder : MonoBehaviour
    {
        public Image itemIcon, bg;
        public TextMeshProUGUI countText;

        public Color ownedColor, notOwnedColor;
        private RPGItem thisItem;

        public void InitSlot(Sprite icon, bool owned, int count, RPGItem item)
        {
            itemIcon.sprite = icon;
            countText.text = count.ToString();

            if (owned)
                bg.color = ownedColor;
            else
                bg.color = notOwnedColor;
            thisItem = item;
        }

        public void ShowTooltip()
        {
            ItemTooltip.Instance.Show(thisItem.ID, -1, true);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }
    }
}