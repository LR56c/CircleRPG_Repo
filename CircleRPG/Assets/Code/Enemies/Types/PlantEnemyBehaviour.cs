using UnityEngine;

namespace Code.Enemies.Types
{
    public class PlantEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform       _ballPosition;
        [SerializeField] private EnemyProjectile _ballProjectilePrefab;
        
        protected override void DoAttack()
        {
            
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }

        public void ThrowBall()
        {
            var hero = GetHero();
            if(!hero) return;
            var location = _ballPosition.position;
            var dir = (hero.bounds.center - location).normalized;
            var rotationDir = Quaternion.LookRotation(dir);

            EnemyProjectile go = Instantiate(_ballProjectilePrefab,
                                             location, rotationDir);
        }
    }
}