using DG.Tweening;
using FredericRP.ObjectPooling;
using UnityEngine;

namespace Code.Enemies.Types
{
    public class BomberEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform       _dangerZone;
        [SerializeField] private Transform       _spawnPosition;
        [SerializeField] private EnemyProjectile _enemyProjectile;
        [SerializeField] private float           _jumpPower = 3.0f;
        [SerializeField] private int             _numJumps  = 1;
        [SerializeField] private float           _duration  = 4.0f;
        private                  ObjectPool      _pool;
        [SerializeField] private string          _prefabPoolName = "Bomber";


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

        protected void ThrowBomb()
        {
            var hero = GetHero();
            if(!hero) return;
            
            _dangerZone.gameObject.SetActive(true);
            _dangerZone.position = transform.position;
            _dangerZone.DOMove(hero.transform.position, (_duration / 2));
            
            //EnemyProjectile go = Instantiate(_enemyProjectile, _spawnPosition.position, Quaternion.identity);
            var go = _pool.GetFromPool(_prefabPoolName);
            go.transform.position = _spawnPosition.position;
            go.transform.DOJump(hero.transform.position, _jumpPower, _numJumps, _duration)
              .SetEase(Ease.OutQuint)
              .OnComplete(() =>
              {
                  _dangerZone.position = transform.position;
                  _dangerZone.gameObject.SetActive(false);
              });
            
            AttackFinish();
        }
    }
}