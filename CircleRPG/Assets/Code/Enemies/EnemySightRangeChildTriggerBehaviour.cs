using UnityEngine;

namespace Code.Enemies
{
    public class EnemySightRangeChildTriggerBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator           _enemyAnimator;
        [SerializeField] private EnemyBaseBehaviour _enemyBaseBehaviour;

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(UnityConstants.Tags.Player)) return;
            _enemyAnimator.SetBool("ToSight", true);
            _enemyBaseBehaviour.AddHeroToList(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.CompareTag(UnityConstants.Tags.Player)) return;
            _enemyAnimator.SetBool("ToSight", false);
            _enemyBaseBehaviour.RemoveHeroToList(other);
        }
    }
}