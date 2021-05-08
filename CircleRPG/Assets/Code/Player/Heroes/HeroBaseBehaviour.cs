using System;
using Code.Domain.Interfaces;
using Code.Enemies;
using Code.Installers;
using Code.UI;
using Code.Utility;
using DG.Tweening;
using FredericRP.ObjectPooling;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Player.Heroes
{
    public abstract class HeroBaseBehaviour : MonoBehaviour, IAttack
    {
        [SerializeField] private NavMeshAgent _agent;

        [SerializeField] private Animator _animator;
        //[SerializeField] private Collider     _collider;

        private int              _dieParam = Animator.StringToHash("Die");
        public  Action           OnAttackComplete;
        public event Action      OnDied;
        public event Action<int> OnDamaged;
        public int               GetCurrentHealth() => _currentHealth;

        public int GetMaxHealth() => _maxHealth;

        [SerializeField] private Transform      _pointToMove;
        [SerializeField] private HeroProjectile _projectilePrefab;
        [SerializeField] private Transform      _spawnProjectilePoint;
        [SerializeField] private int            _currentHealth   = 100;
        [SerializeField] private int            _maxHealth       = 120;
        [SerializeField] private float          _tweenTimeRotate = 0.2f;

        private                  ObjectPool _pool;
        [SerializeField] private string     _prefabPoolName;

        private                  UIHeroAbility      _uiHeroAbility;
        private                  KilledEnemyService _killedEnemyService;
        private                  int                _killCount         = 0;
        [SerializeField] private int                _killsToGetAbility = 1;
        public                   Collider           FocusEnemy{get;set;}

        protected virtual void Start()
        {
            MySceneLinkedSMB<HeroBaseBehaviour>.Initialise(_animator, this);

            _uiHeroAbility = ServiceLocator.Instance.GetService<UIHeroAbility>();
            _pool = ObjectPool.GetObjectPool("pool");

            _killedEnemyService =
                ServiceLocator.Instance.GetService<KilledEnemyService>();
            _killedEnemyService.OnEnemyKilled += OnEnemyKilled;
        }

        [ContextMenu("Active UI")]
        private void ForceActiveHeroAbility()
        {
            _uiHeroAbility.ActiveHeroAbility(GetHeroEType());
        }

        private void OnEnemyKilled(int oneDeath)
        {
            _killCount += oneDeath;

            if(_killCount >= _killsToGetAbility)
            {
                _uiHeroAbility.ActiveHeroAbility(GetHeroEType());
                _killCount = 0;
            }
        }

        protected abstract int GetHeroEType();

        protected virtual void Update()
        {
            _agent.SetDestination(_pointToMove.position);
        }

        public void Attack(Action onComplete)
        {
            if(!CanAttack()) return;

            //TurnToTarget();
            OnAttackComplete = onComplete;
            //DoAttack(FocusEnemy);
        }

        public void TurnToTarget()
        {
            transform.DOLookAt(FocusEnemy.transform.position, _tweenTimeRotate,
                               AxisConstraint.Y, Vector3.up);
        }

        public bool CanAttack()
        {
            return FocusEnemy;
        }

        //protected abstract void DoAttack(Collider objetive);

        public void DamageReceived(int damage)
        {
            bool isDead = ApplyDamage(damage);
            DamageReceivedNotify(isDead);
        }

        //TODO: falta ver como se veeria cura en healthBar hero
        public void Heal(int value)
        {
            _currentHealth += value;

            if(_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        private void AnimationDiedComplete()
        {
            _pointToMove.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        protected abstract void DamageReceivedNotify(bool isDead);

        private bool ApplyDamage(int damage)
        {
            _currentHealth -= damage;
            OnDamaged?.Invoke(damage);

            if(_currentHealth > 0)
                return false;

            OnDied?.Invoke();
            _animator.SetTrigger(_dieParam);
            _currentHealth = 0;
            return true;
        }

        protected virtual void ThrowProjectile()
        {
            if(!FocusEnemy) return;

            Vector3 enemyPos = FocusEnemy.bounds.center;

            Vector3 spawnPos = _spawnProjectilePoint.position;
            Vector3 dir = (enemyPos - spawnPos).normalized;
            Quaternion rotationDir = Quaternion.LookRotation(dir);

            var my = _pool.GetFromPool(_prefabPoolName);
            my.transform.position = spawnPos;
            my.transform.rotation = rotationDir;

            /*HeroProjectile go = Instantiate(_projectilePrefab,
                                            spawnPos, rotationDir);*/
        }

        public void DoWarp(Vector3 pos)
        {
            _agent.Warp(pos);
        }
    }
}