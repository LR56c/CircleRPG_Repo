using UnityEngine;

namespace THMSV.RPGBuilder.Character
{
    public class PlayerAnimatorLayerHandler : MonoBehaviour
    {
        private Animator thisAnim;
        private RPGBCharacterController thisController;

        private void Start()
        {
            thisAnim = GetComponent<Animator>();
            thisController = GetComponent<RPGBCharacterController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (thisController.GetDesiredSpeed() > 0)
            {
                thisAnim.SetLayerWeight(1, 0);
                thisAnim.SetLayerWeight(2, 1);
            }
            else
            {
                thisAnim.SetLayerWeight(1, 1);
                thisAnim.SetLayerWeight(2, 0);
            }
        }
    }
}
