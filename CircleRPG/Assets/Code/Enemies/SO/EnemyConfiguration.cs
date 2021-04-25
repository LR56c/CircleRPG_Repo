using UnityEngine;

namespace Code
{
    public abstract class EnemyConfiguration : ScriptableObject
    {
        public float SightRange  = 5f;
        public float AttackRange = 5f;

        public virtual void DoAttack()
        {
        }
    }
}