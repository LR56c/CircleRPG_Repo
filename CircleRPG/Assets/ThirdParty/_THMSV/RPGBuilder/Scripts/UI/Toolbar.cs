using UnityEngine;

namespace THMSV.RPGBuilder.UI
{
    public class Toolbar : MonoBehaviour
    {

        [SerializeField] private Animator characterButtonAnimator;
        [SerializeField] private Animator skillbookButtonAnimator;

        public void InitToolbar()
        {
            characterButtonAnimator.SetBool("glowing", RPGBuilderUtilities.hasPointsToSpendInClassTrees());
            skillbookButtonAnimator.SetBool("glowing", RPGBuilderUtilities.hasPointsToSpendInSkillTrees());
        }
    
    
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }
    
        public static Toolbar Instance { get; private set; }
    }
}
