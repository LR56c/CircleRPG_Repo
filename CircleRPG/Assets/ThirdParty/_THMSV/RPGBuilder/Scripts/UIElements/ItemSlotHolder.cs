using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class ItemSlotHolder : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler,
        IDropHandler
    {
        public Image icon, quality;
        public TextMeshProUGUI stackText;

        public RPGItem thisItem;
        public int bagIndex;
        public int slotIndex;

        private GameObject curDraggedItem;

        public void InitSlot(RPGItem item, int bag_index, int slot_index)
        {
            thisItem = item;
            bagIndex = bag_index;
            slotIndex = slot_index;
            icon.sprite = item.icon;
            Sprite itemQualitySprite = RPGBuilderUtilities.getItemQualitySprite(item.quality);
            if (itemQualitySprite != null)
            {
                quality.enabled = true;
                quality.sprite = RPGBuilderUtilities.getItemQualitySprite(item.quality);
            }
            else
            {
                quality.enabled = false;
            }
            var curstack = InventoryManager.Instance.bags[bag_index].slots[slotIndex].curStack;
            stackText.text = curstack > 1 ? curstack.ToString() : "";
        }

        public void ShowTooltip()
        {
            ItemTooltip.Instance.Show(thisItem.ID, InventoryManager.Instance.bags[bagIndex].slots[slotIndex].itemRandomID, true);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            if (MerchantPanelDisplayManager.Instance.thisCG.alpha == 1)
            {
                ConfirmationPopupManager.Instance.InitPopup(ConfirmationPopupManager.ConfirmationPopupType.sellItem,
                    thisItem, InventoryManager.Instance.bags[bagIndex].slots[slotIndex].curStack, bagIndex, slotIndex);
                return;
            }

            InventoryManager.Instance.UseItem(thisItem, bagIndex, slotIndex);
            ItemTooltip.Instance.Hide();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            curDraggedItem = Instantiate(InventoryManager.Instance.draggedItemImage, transform.position,
                Quaternion.identity);
            curDraggedItem.transform.SetParent(InventoryManager.Instance.draggedItemParent);
            curDraggedItem.GetComponent<Image>().sprite = thisItem.icon;
        }

        public void OnDrag(PointerEventData eventData)
        {
            curDraggedItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
            if (InventoryDisplayManager.Instance.thisCG.alpha == 1)
            {
                for (var i = 0; i < InventoryManager.Instance.bags[0].slots.Count; i++)
                    if (RectTransformUtility.RectangleContainsScreenPoint(
                        InventoryManager.Instance.bags[0].slots[i].rect,
                        Input.mousePosition))
                    {
                        InventoryManager.Instance.MoveItem(bagIndex, slotIndex, 0, i);
                        Destroy(curDraggedItem);
                        return;
                    }
            }

            if (CharacterPanelDisplayManager.Instance.thisCG.alpha == 1)
            {
                switch (thisItem.itemType)
                {
                    case "ARMOR":
                    {
                        foreach (var t in CharacterPanelDisplayManager.Instance.armorSlotData)
                            if (RectTransformUtility.RectangleContainsScreenPoint(
                                t.itemUIRef.GetComponent<RectTransform>(),
                                Input.mousePosition))
                            {
                                InventoryManager.Instance.EquipItem(
                                    InventoryManager.Instance.bags[bagIndex].slots[slotIndex].itemStored, bagIndex,
                                    slotIndex, InventoryManager.Instance.bags[bagIndex].slots[slotIndex].itemRandomID);
                                Destroy(curDraggedItem);
                                return;
                            }

                        break;
                    }
                    case "WEAPON":
                    {
                        foreach (var t in CharacterPanelDisplayManager.Instance.weaponSlotData)
                            if (RectTransformUtility.RectangleContainsScreenPoint(
                                t.itemUIRef.GetComponent<RectTransform>(),
                                Input.mousePosition))
                            {
                                InventoryManager.Instance.EquipItem(
                                    InventoryManager.Instance.bags[bagIndex].slots[slotIndex].itemStored, bagIndex,
                                    slotIndex, InventoryManager.Instance.bags[bagIndex].slots[slotIndex].itemRandomID);
                                Destroy(curDraggedItem);
                                return;
                            }

                        break;
                    }
                }
            }

            for (var i = 0; i < ActionBarDisplayManager.Instance.itemSlots.Length; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    ActionBarDisplayManager.Instance.itemSlots[i].slotREF.GetComponent<RectTransform>(),
                    Input.mousePosition))
                {
                    ActionBarDisplayManager.Instance.SetItemToSlot(InventoryManager.Instance.bags[bagIndex].slots[slotIndex].itemStored, i);
                    Destroy(curDraggedItem);
                    return;
                }

            ConfirmationPopupManager.Instance.InitPopup(ConfirmationPopupManager.ConfirmationPopupType.deleteItem, thisItem,
                InventoryManager.Instance.bags[bagIndex].slots[slotIndex].curStack, bagIndex, slotIndex);
            Destroy(curDraggedItem);
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    }
}