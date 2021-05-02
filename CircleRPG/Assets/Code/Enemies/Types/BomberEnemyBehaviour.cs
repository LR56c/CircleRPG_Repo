using DG.Tweening;
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
            
            EnemyProjectile go = Instantiate(_enemyProjectile, _spawnPosition.position, Quaternion.identity);
            go.transform.DOJump(hero.transform.position, _jumpPower, _numJumps, _duration)
              .SetEase(Ease.InCubic)
              .OnComplete(() =>
              {
                  _dangerZone.position = transform.position;
                  _dangerZone.gameObject.SetActive(false);
              });
            
            AttackFinish();
        }
    }
}