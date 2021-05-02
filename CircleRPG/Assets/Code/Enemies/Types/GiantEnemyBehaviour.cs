using UnityEngine;

namespace Code.Enemies.Types
{
    public class GiantEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField]             private Transform       _ballPosition;
        [SerializeField]             private EnemyProjectile _ballProjectilePrefab;
        [SerializeField] private float               _angle = 45f;
        
        protected override void DoAttack()
        {
         
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }
        
        public void ThrowBalls()
        {
            var hero = GetHero();
            if(!hero) return;
            var location = _ballPosition.position;
            var dir = (hero.bounds.center - location).normalized;
            var rotationDir = Quaternion.LookRotation(dir);

            EnemyProjectile goCentral = Instantiate(_ballProjectilePrefab,
                                                    location, rotationDir);

            Quaternion angleRight = Quaternion.AngleAxis(_angle, Vector3.up);
            Vector3 rightDir = angleRight * dir;
            Quaternion rotRight = Quaternion.LookRotation(rightDir);
            EnemyProjectile goRight = Instantiate(_ballProjectilePrefab,
                                                  location, rotRight);

            Quaternion angleLeft = Quaternion.AngleAxis(-_angle, Vector3.up);
            Vector3 leftDir = angleLeft * dir;
            Quaternion rotLeft = Quaternion.LookRotation(leftDir);
            EnemyProjectile goLeft = Instantiate(_ballProjectilePrefab,
                                                 location, rotLeft);
        }
    }
}