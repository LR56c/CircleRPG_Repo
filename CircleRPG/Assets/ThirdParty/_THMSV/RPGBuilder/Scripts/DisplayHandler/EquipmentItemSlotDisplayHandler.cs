using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.DisplayHandler
{
    public class EquipmentItemSlotDisplayHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
    {

        public CanvasGroup itemCG;
        public Image icon;
        public RPGItem curItem;

        public int weaponID;

        private GameObject curDraggedItem;
        private int rdmItemID = -1;
        
        public void InitItem(RPGItem item, int randomItemID)
        {
            curItem = item;
            rdmItemID = randomItemID;
            RPGBuilderUtilities.EnableCG(itemCG);
            icon.sprite = item.icon;
        }

        public void ResetItem()
        {
            RPGBuilderUtilities.DisableCG(itemCG);
            icon.sprite = null;
            curItem = null;
            rdmItemID = -1;
        }

        public void ShowTooltip()
        {
            if (curItem != null)
                ItemTooltip.Instance.Show(curItem.ID, rdmItemID, false);
            else
                HideTooltip();
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            if (curItem == null) return;
            InventoryManager.Instance.UnequipItem(curItem, weaponID);
            curItem = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (curItem == null) return;
            curDraggedItem = (GameObject)Instantiate(InventoryManager.Instance.draggedItemImage, transform.position, Quaternion.identity);
            curDraggedItem.transform.SetParent(InventoryManager.Instance.draggedItemParent);
            curDraggedItem.GetComponent<Image>().sprite = curItem.icon;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (curDraggedItem == null) return;
            curDraggedItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (InventoryDisplayManager.Instance.thisCG.alpha == 1)
            {
                for (var i = 0; i < InventoryManager.Instance.bags[0].slots.Count; i++)
                    if (RectTransformUtility.RectangleContainsScreenPoint(
                        InventoryManager.Instance.bags[0].slots[i].rect, Input.mousePosition))
                        if (!InventoryManager.Instance.bags[0].slots[i].inUse)
                        {
                            InventoryManager.Instance.MoveItemFromCharToBag(curItem, 0, i, weaponID, rdmItemID);
                            Destroy(curDraggedItem);
                            return;
                        }
                        else
                        {
                            Destroy(curDraggedItem);
                            return;
                        }
            }

            Destroy(curDraggedItem);
        }

        public void OnDrop(PointerEventData eventData)
        {

        }
    }
}
