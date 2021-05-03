using System;
using Code.Domain.Interfaces;
using Code.Enemies;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Player.Heroes
{
    public abstract class HeroBaseBehaviour : MonoBehaviour, IDamageable, IAttack
    {
        private                  NavMeshAgent _agent;
        private                  Animator     _animator;
        private                  int          _dieParam = Animator.StringToHash("Die");
        public                   Action       OnAttackComplete;
        
        [SerializeField] private Transform _pointToMove;
        [SerializeField] private HeroProjectile _projectilePrefab;
        [SerializeField] private Transform _spawnProjectilePoint;
        [SerializeField] private int        _currentHealth   = 100;
        [SerializeField] private float      _tweenTimeRotate = 0.2f;
        public                   Collider   FocusEnemy{get;set;}
        
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Start()
        {
            MySceneLinkedSMB<HeroBaseBehaviour>.Initialise(_animator, this);
        }

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
            transform.DOLookAt(FocusEnemy.transform.position, _tweenTimeRotate, AxisConstraint.Y, Vector3.up);
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

        protected abstract void DamageReceivedNotify(bool isDead);

        private bool ApplyDamage(int damage)
        {
            _currentHealth -= damage;

            if(_currentHealth > 0)
                return false;

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

            //TODO: una pool con dontDestroyOnLoad, le pediria la creacion a una factory en la escena boot
            //TODO: aqui se pediria el objeto de la pool, la pool lo activaria y aqui se le pasaria la posicion y rotacion
            HeroProjectile go = Instantiate(_projectilePrefab,
                                            spawnPos, rotationDir);
        }
    }
}