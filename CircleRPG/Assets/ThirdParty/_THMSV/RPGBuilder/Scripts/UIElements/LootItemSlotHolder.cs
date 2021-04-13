using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class LootItemSlotHolder : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler,
        IDropHandler
    {
        public Image itemIcon;
        public TextMeshProUGUI itemStackText, itemNameText;

        private int thisLootIndex;
        private LootBagHolder holder;

        private GameObject curDraggedItem;

        public void Init(int lootIndex, LootBagHolder bagHolder)
        {
            thisLootIndex = lootIndex;
            holder = bagHolder;
            itemIcon.sprite = holder.lootData[thisLootIndex].item.icon;
            itemStackText.text = holder.lootData[thisLootIndex].count.ToString();
            itemNameText.text = holder.lootData[thisLootIndex].item.displayName;
        }

        public void ShowTooltip()
        {
            ItemTooltip.Instance.Show(holder.lootData[thisLootIndex].item.ID, holder.lootData[thisLootIndex].randomItemID, true);
        }

        public void HideTooltip()
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            InventoryManager.Instance.AddItem(holder.lootData[thisLootIndex].item.ID,
                holder.lootData[thisLootIndex].count,false, holder.lootData[thisLootIndex].randomItemID);
            holder.lootData[thisLootIndex].looted = true;
            LootPanelDisplayManager.Instance.RemoveItemSlot(gameObject);
            holder.CheckLootState();
            Destroy(gameObject);
            ItemTooltip.Instance.Hide();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            curDraggedItem = Instantiate(InventoryManager.Instance.draggedItemImage, transform.position,
                Quaternion.identity);
            curDraggedItem.transform.SetParent(InventoryManager.Instance.draggedItemParent);
            curDraggedItem.GetComponent<Image>().sprite = holder.lootData[thisLootIndex].item.icon;
        }

        public void OnDrag(PointerEventData eventData)
        {
            curDraggedItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (InventoryDisplayManager.Instance.thisCG.alpha == 1)
            {
                foreach (var t in InventoryManager.Instance.bags[0].slots)
                    if (RectTransformUtility.RectangleContainsScreenPoint(t.rect,
                        Input.mousePosition))
                    {
                        InventoryManager.Instance.AddItem(holder.lootData[thisLootIndex].item.ID,
                            holder.lootData[thisLootIndex].count,false, holder.lootData[thisLootIndex].randomItemID);
                        Destroy(curDraggedItem);
                        holder.lootData[thisLootIndex].looted = true;
                        LootPanelDisplayManager.Instance.RemoveItemSlot(gameObject);
                        holder.CheckLootState();
                        Destroy(gameObject);
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