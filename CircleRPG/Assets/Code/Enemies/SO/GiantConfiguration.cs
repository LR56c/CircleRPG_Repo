using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "Giant", menuName = "EnemyConfiguration/Giant", order = 0)]
    public class GiantConfiguration : EnemyConfiguration
    {
        public override void DoAttack()
        {
            base.DoAttack();
        }
    }
}