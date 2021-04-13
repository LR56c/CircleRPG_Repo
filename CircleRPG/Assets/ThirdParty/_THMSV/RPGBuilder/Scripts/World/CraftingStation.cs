using THMSV.RPGBuilder.Managers;
using UnityEngine;

namespace THMSV.RPGBuilder.World
{
    public class CraftingStation : MonoBehaviour
    {
        public RPGCraftingStation station;
        [SerializeField] private float useDistanceMax;

        private void OnMouseOver()
        {
            if (RPGBuilderUtilities.IsPointerOverUIObject()) return;
            if (Input.GetMouseButtonUp(1))
                if (Vector3.Distance(transform.position, CombatManager.playerCombatInfo.transform.position) <=
                    useDistanceMax)
                {
                    if (CraftingPanelDisplayManager.Instance.thisCG.alpha == 0)
                        InitCraftingStation();
                }
                else
                    ErrorEventsDisplayManager.Instance.ShowErrorEvent("This is too far", 3);

            CursorManager.Instance.SetCursor(CursorManager.cursorType.craftingStation);
        }

        private void OnMouseExit()
        {
            CursorManager.Instance.ResetCursor();
        }

        private void InitCraftingStation()
        {
            CraftingPanelDisplayManager.Instance.Show(station);
        }
    }
}