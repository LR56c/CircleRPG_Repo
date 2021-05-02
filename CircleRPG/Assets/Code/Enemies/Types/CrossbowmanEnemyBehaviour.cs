using UnityEngine;

namespace Code.Enemies.Types
{
    public class CrossbowmanEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform       _spawnPosition;
        [SerializeField] private EnemyProjectile _enemyProjectile;
        private                  Vector3         _spawnPos;
        private                  Quaternion      _rotationDir;

        protected override void DoAttack()
        {
            
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
            
        }

        protected void CalculateEnterDir()
        {
            var hero = GetHero();
            if(!hero) return;
            _spawnPos = _spawnPosition.position;
            var dir = (hero.bounds.center - _spawnPos).normalized;
            _rotationDir = Quaternion.LookRotation(dir);
        }

        protected void ThrowArrow()
        {
            EnemyProjectile go = Instantiate(_enemyProjectile, _spawnPos, _rotationDir);
        }
    }
}