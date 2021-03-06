using Code.Enemies.SMB;
using FredericRP.ObjectPooling;
using UnityEngine;

namespace Code.Enemies.Types
{
    public class CrossbowmanEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform       _spawnPosition;
        [SerializeField] private EnemyProjectile _enemyProjectile;
        private                  Vector3         _spawnPos;
        private                  Quaternion      _rotationDir;
        [SerializeField] private GameObject      _aimRay;
        private                  ObjectPool      _pool;
        [SerializeField] private string          _prefabPoolName = "Crossbow";


        protected override void OnEnable()
        {
            base.OnEnable();
            
            var tempAimSMB = _animator.GetBehaviour<EnemyAimSMB>();

            if(!tempAimSMB)
            {
                Debug.Log("No se ha podido establecer AimRay en Crossbowman");
                return;
            }
            tempAimSMB.SetSpecificAimRay(_aimRay);
        }

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
            var go = _pool.GetFromPool(_prefabPoolName);
            go.transform.position = _spawnPos;
            go.transform.rotation = _rotationDir;
            //EnemyProjectile go = Instantiate(_enemyProjectile, _spawnPos, _rotationDir);
        }
    }
}