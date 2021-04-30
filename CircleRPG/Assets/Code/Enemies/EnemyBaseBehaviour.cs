using System;
using System.Collections.Generic;
using Code.Domain.Interfaces;
using Code.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies
{
    public abstract class EnemyBaseBehaviour : MonoBehaviour, IDamageable, IAttack,
                                               IMove
    {
        protected Rigidbody    _rb;
        protected NavMeshAgent _navMeshAgent;
        protected Animator     _animator;
        protected                Action         OnAttackComplete;
        private   int          _dieParam = Animator.StringToHash("Died");

        [SerializeField] private int            _currentHealth = 100;
        [SerializeField] private List<Collider> _inAreaHeros;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            MySceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);
            //TODO: pool projectiles
            //TODO: ver forma de guardar
        }

        public Collider GetHero()
        {
            if(_inAreaHeros.Count == 0) return null;

            if(_inAreaHeros[0].gameObject.activeInHierarchy)
            {
                return _inAreaHeros[0];
            }

            _inAreaHeros.Remove(_inAreaHeros[0]);
            return GetHero();
        }

        public void AddHeroToList(Collider hero)
        {
            _inAreaHeros.Add(hero);
        }

        public void RemoveHeroToList(Collider hero)
        {
            _inAreaHeros.Remove(hero);
        }

        public void Move(Vector3 destination)
        {
            DoMove(destination);
        }

        protected abstract void DoMove(Vector3 destination);

        public void Attack(Action onComplete)
        {
            if(!CanAttack()) return;

            OnAttackComplete = onComplete;
            DoAttack();
        }

        public bool CanAttack()
        {
            return GetHero();
        }
        
        public abstract    void AttackFinish();
        protected abstract void DoAttack();
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

        public void DamageReceived(int damage)
        {
            bool isDead = ApplyDamage(damage);
            DamageReceivedNotify(isDead);
        }
    }
}