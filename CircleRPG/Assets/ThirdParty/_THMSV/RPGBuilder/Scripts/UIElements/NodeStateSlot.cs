using THMSV.RPGBuilder.DisplayHandler;
using THMSV.RPGBuilder.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class NodeStateSlot : MonoBehaviour
    {
        public Image stateBorder, stateIcon;
        public TextMeshProUGUI stackText;
        public Color buffColor, debuffColor;
        private RPGEffect curEffect;
        public int thisIndex;

        private float curDuration, maxDuration;

        private bool isUpdating;

        public void InitStateSlot(bool buff, RPGEffect effect, Sprite icon, float MaxDur, int index)
        {
            stateBorder.color = buff ? buffColor : debuffColor;

            stateIcon.sprite = icon;
            stateBorder.fillAmount = 1;
            maxDuration = MaxDur;
            curDuration = MaxDur;
            thisIndex = index;

            UpdateStackText();

            isUpdating = true;
        }

        public void UpdateStackText()
        {
            stackText.text = "" + CombatManager.playerCombatInfo.nodeStateData[thisIndex].curStack;
        }

        private void FixedUpdate()
        {
            if (isUpdating) curDuration -= Time.deltaTime;

            if (curDuration <= 0) PlayerStatesDisplayHandler.Instance.RemoveState(thisIndex);
        }

        private void Update()
        {
            if (isUpdating) stateBorder.fillAmount = curDuration / maxDuration;
        }
    }
}