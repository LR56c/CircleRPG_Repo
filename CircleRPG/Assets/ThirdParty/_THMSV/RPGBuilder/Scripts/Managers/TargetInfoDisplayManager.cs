using THMSV.RPGBuilder.LogicMono;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.Managers
{
    public class TargetInfoDisplayManager : MonoBehaviour
    {
        public CanvasGroup thisCG;
        public TextMeshProUGUI targetNameText, targetHPText;
        public Image targetHealthbar;

        public Sprite allyHB, neutralHB, enemyHB;

        private CombatNode curTarget;

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static TargetInfoDisplayManager Instance { get; private set; }

        public void InitTargetUI(CombatNode cbtNode)
        {
            RPGBuilderUtilities.EnableCG(thisCG);
            curTarget = cbtNode;
            if (curTarget.nodeType == CombatNode.COMBAT_NODE_TYPE.player)
            {
                targetNameText.text = CharacterData.Instance.CharacterName;
                targetHealthbar.sprite = allyHB;
            }
            else
            {
                targetNameText.text = cbtNode.npcDATA.displayName;
                switch (cbtNode.npcDATA.alignmentType)
                {
                    case RPGNpc.ALIGNMENT_TYPE.ALLY:
                        targetHealthbar.sprite = allyHB;
                        break;
                    case RPGNpc.ALIGNMENT_TYPE.NEUTRAL:
                        targetHealthbar.sprite = neutralHB;
                        break;
                    case RPGNpc.ALIGNMENT_TYPE.ENEMY:
                        targetHealthbar.sprite = enemyHB;
                        break;
                }
            }

            UpdateTargetHealthBar();
        }

        public void ResetTarget()
        {
            curTarget = null;
            RPGBuilderUtilities.DisableCG(thisCG);
        }

        public void UpdateTargetHealthBar()
        {
            if (curTarget != null)
            {
                targetHealthbar.fillAmount = curTarget.getCurrentValue("HEALTH") / curTarget.getCurrentMaxValue("HEALTH");
                targetHPText.text = (int)curTarget.getCurrentValue("HEALTH") + " / " + (int)curTarget.getCurrentMaxValue("HEALTH");
            }
            else
            {
                CombatManager.Instance.ResetPlayerTarget();
            }
        }
    }
}