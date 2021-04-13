using THMSV.RPGBuilder.LogicMono;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class BossUISlotHolder : MonoBehaviour
    {
        public CanvasGroup thisCG;
        public Image HPBar;
        public TextMeshProUGUI HPText, BossNameText;
        public CombatNode thisNode;


        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static BossUISlotHolder Instance { get; private set; }

        public void Init(CombatNode nodeRef)
        {
            RPGBuilderUtilities.EnableCG(thisCG);
            thisNode = nodeRef;
            if (HPBar != null) HPBar.fillAmount = nodeRef.getCurrentValue("HEALTH") / nodeRef.getCurrentMaxValue("HEALTH");
            if (HPText != null)
                HPText.text = nodeRef.getCurrentValue("HEALTH") + " / " + nodeRef.getCurrentMaxValue("HEALTH");
            if (BossNameText != null) BossNameText.text = nodeRef.npcDATA.displayName + " | LvL. " + nodeRef.NPCLevel;
        }

        public void UpdateHealth()
        {
            if (thisNode == null) ResetBossUI();
            var curHp = thisNode.getCurrentValue("HEALTH");
            var maxHp = thisNode.getCurrentMaxValue("HEALTH");
            if (HPBar != null) HPBar.fillAmount = curHp / maxHp;
            if (HPText != null) HPText.text = (int) curHp + " / " + (int) maxHp;
            if (curHp <= 0) ResetBossUI();
        }

        private void ResetBossUI()
        {
            RPGBuilderUtilities.DisableCG(thisCG);
            thisNode = null;
        }
    }
}