using FredericRP.ObjectPooling;
using UnityEngine;

namespace Code.Enemies.Types
{
    public class PlantEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform       _ballPosition;
        [SerializeField] private EnemyProjectile _ballProjectilePrefab;
        private                  ObjectPool      _pool;
        [SerializeField] private string          _prefabPoolName = "Plant";

        protected override void Start()
        {
            base.Start();
            _pool = ObjectPool.GetObjectPool("pool");
        }

        protected override void DoAttack()
        {
            
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }

        protected void ThroBall()
        {
            var hero = GetHero();
            if(!hero) return;
            var location = _ballPosition.position;
            var dir = (hero.bounds.center - location).normalized;
            var rotationDir = Quaternion.LookRotation(dir);

            /*EnemyProjectile go = Instantiate(_ballProjectilePrefab,
                                             location, rotationDir);*/
            
            GameObject go = _pool.GetFromPool(_prefabPoolName);
            go.transform.position = location;
            go.transform.rotation = rotationDir;
        }

    }
}