using System;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.Types
{
    public class PigBossBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private EnemyBounceProjectile _projectile;

        [Header("Numbers solo puede ser Impar")]
        [SerializeField] private int _numbers = 5;
        [SerializeField] private float _angleStep = 45f;
        private                  int   _offsetMultiplier;

        protected override void DoAttack()                        {}
        protected override void DamageReceivedNotify(bool isDead) {}

        protected override void Start()
        {
            base.Start();
            PreCalculateOffsetMultiplier();
        }

        private void PreCalculateOffsetMultiplier()
        {
            if((_numbers % 2) == 1)
            {
                _offsetMultiplier = Mathf.FloorToInt(_numbers / 2);
            }
            else
            {
                Debug.LogError($"Variable Numbers en {gameObject.name}, debe ser impar");
            }
        }

        protected void ThrowBouncingBalls()
        {
            var hero = GetHero();
            if(!hero) return;
            var location = transform.position;
            var heroPos = hero.bounds.center;
            location.y = heroPos.y;

            var offsetAngle = transform.eulerAngles.y - (_angleStep * _offsetMultiplier);

            for(int i = 0; i < _numbers; i++)
            {
                Instantiate(_projectile, location, Quaternion.Euler(0f, offsetAngle, 0f));
                offsetAngle += _angleStep;
            }
        }
    }
}