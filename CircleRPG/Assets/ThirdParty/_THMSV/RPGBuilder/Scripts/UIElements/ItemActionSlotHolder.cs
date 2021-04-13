using System;
using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class ItemActionSlotHolder : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
    {
        public Image icon;
        public TextMeshProUGUI keybindText, stackText;

        private RPGItem thisItem;
        private GameObject curDraggedItem;
        private int curSlot;

        public void Init(RPGItem item, int slot)
        {
            thisItem = item;
            icon.sprite = thisItem.icon;
            curSlot = slot;
            int ttlCount = InventoryManager.Instance.getTotalCountOfItem(item);
            UpdateSlot(ttlCount);
        }

        public void UpdateSlot(int ttlStack)
        {
            stackText.text = ttlStack.ToString();
        }

        public void ClickUseItem()
        {
            if (ActionBarDisplayManager.Instance.itemSlots[curSlot].curItem != null)
                InventoryManager.Instance.UseItemFromBar(ActionBarDisplayManager.Instance.itemSlots[curSlot].curItem);
        }

        public void Reset()
        {
            thisItem = null;
            stackText.text = "";
        }

        public void ShowTooltip()
        {
            if(thisItem!=null)ItemTooltip.Instance.Show(thisItem.ID, -1, false);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (thisItem == null) return;
            curDraggedItem = Instantiate(InventoryManager.Instance.draggedItemImage, transform.position,
                Quaternion.identity);
            curDraggedItem.transform.SetParent(InventoryManager.Instance.draggedItemParent);
            curDraggedItem.GetComponent<Image>().sprite = thisItem.icon;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (thisItem == null) return;
            if (curDraggedItem != null)
                curDraggedItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (thisItem == null) return;
            if (curDraggedItem == null) return;
            
            for (var i = 0; i < ActionBarDisplayManager.Instance.itemSlots.Length; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    ActionBarDisplayManager.Instance.itemSlots[i].slotREF.GetComponent<RectTransform>(),
                    Input.mousePosition))
                {
                    ActionBarDisplayManager.Instance.SetItemToSlot(thisItem, i);
                    Destroy(curDraggedItem);
                }

            ActionBarDisplayManager.Instance.ResetItemSlot(curSlot);
            Destroy(curDraggedItem);
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    }
}
