using System;
using System.Collections.Generic;
using Code.Domain.Interfaces;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.Types
{
    public abstract class EnemyBaseBehaviour : MonoBehaviour, IDamageable, IAttack
    {
        protected Collider _myCollider;
        protected Animator _animator;
        public    Action   OnAttackComplete;
        private   int      _dieParam = Animator.StringToHash("Died");

        [SerializeField] private int            _currentHealth   = 100;
        [SerializeField] private float          _tweenTimeRotate = 1.0f;
        [SerializeField] private List<Collider> _inAreaHeros;
        public                   float          TweenTimeRotate => _tweenTimeRotate;

        protected virtual void Awake()
        {
            /*_rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();*/
            _animator = GetComponent<Animator>();
            _myCollider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            MySceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);
            //TODO: pool projectiles
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

        public void Attack(Action onComplete)
        {
            if(!CanAttack()) return;

            var targetPos = _inAreaHeros[0].transform.position;
            TurnToTarget(targetPos);
            OnAttackComplete = onComplete;
        }

        //podria estar en una interfaz?
        public void TurnToTarget(Vector3 target)
        {
            transform.DOLookAt(target, _tweenTimeRotate, AxisConstraint.Y, Vector3.up);
        }

        public bool CanAttack()
        {
            return GetHero();
        }

        protected virtual void AttackFinish()
        {
            OnAttackComplete?.Invoke();
        }

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