using Code.Enemies.Types;
using UnityEngine;

namespace Code.Enemies.Triggers
{
    public class EnemySightRangeChildTriggerBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator           _enemyAnimator;
        [SerializeField] private EnemyBaseBehaviour _enemyBaseBehaviour;
        //public                   int testParam = Animator.StringToHash("ToAttack");

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
            _enemyAnimator.SetBool("ToAttack", false);
            _enemyBaseBehaviour.RemoveHeroToList(other);
        }
    }
}