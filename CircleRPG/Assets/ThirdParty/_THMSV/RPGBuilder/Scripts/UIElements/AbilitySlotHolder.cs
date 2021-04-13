using THMSV.RPGBuilder.Managers;
using THMSV.RPGBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class AbilitySlotHolder : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
    {
        public Image icon, CDOverlay;
        public TextMeshProUGUI abilityCDText, keybindText;

        private RPGAbility thisAb;
        private GameObject curDraggedAbility;
        private int curSlot;
        [SerializeField] private bool dragAllowed = true;
        

        public void Init(RPGAbility ab, int slot)
        {
            thisAb = ab;
            icon.sprite = thisAb.icon;
            curSlot = slot;
        }

        public void Reset()
        {
            thisAb = null;
        }
        
        
        public void ClickUseAbility()
        {
            if (ActionBarDisplayManager.Instance.abilitySlots[curSlot].curAb != null)
                CombatManager.Instance.TRIGGER_PLAYER_ABILITY(CombatManager.playerCombatInfo
                    .abilitiesData[curSlot].currentAbility);
        }

        public void ShowTooltip()
        {
            if(thisAb!=null)AbilityTooltip.Instance.Show(thisAb);
        }

        public void HideTooltip()
        {
            AbilityTooltip.Instance.Hide();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!dragAllowed) return;
            if (thisAb == null) return;
            curDraggedAbility = Instantiate(TreesDisplayManager.Instance.draggedNodeImage, transform.position,
                Quaternion.identity);
            curDraggedAbility.transform.SetParent(TreesDisplayManager.Instance.draggedNodeParent);
            curDraggedAbility.GetComponent<Image>().sprite = thisAb.icon;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (thisAb == null) return;
            if (curDraggedAbility != null)
                curDraggedAbility.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (thisAb == null) return;
            if (curDraggedAbility == null) return;
            for (var i = 0; i < ActionBarDisplayManager.Instance.abilitySlots.Count; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    ActionBarDisplayManager.Instance.abilitySlots[i].slotREF.GetComponent<RectTransform>(),
                    Input.mousePosition))
                {
                    ActionBarDisplayManager.Instance.SetAbilityToSlot(thisAb, i);
                    Destroy(curDraggedAbility);
                }

            ActionBarDisplayManager.Instance.ResetAbilitySlot(curSlot);
            Destroy(curDraggedAbility);
        }

        public void OnDrop(PointerEventData eventData)
        {
        }
    }
}