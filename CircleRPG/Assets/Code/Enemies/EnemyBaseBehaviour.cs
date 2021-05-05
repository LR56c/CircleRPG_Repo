using System;
using System.Collections.Generic;
using Code.Domain.Interfaces;
using Code.Installers;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.Types
{
    public abstract class EnemyBaseBehaviour : MonoBehaviour, IDamageable, IAttack
    {
        [SerializeField] protected Rigidbody    _rb;
        [SerializeField] protected Collider     _myCollider;
        [SerializeField] protected Animator     _animator;
        
        public  Action                  OnAttackComplete;
        public event Action                  OnDied;
        private int                          _dieParam = Animator.StringToHash("Died");

        [SerializeField] private   int                _currentHealth   = 100;
        [SerializeField] protected float              _tweenTimeRotate = 1.0f;
        [SerializeField] private   List<Collider>     _inAreaHeros;
        public                     float              TweenTimeRotate => _tweenTimeRotate;
        private                    KilledEnemyService _killedEnemyService;
        private                    bool               _isDead;

        protected virtual void Awake()
        {
        }

        protected virtual void OnEnable()  {}
        protected virtual void OnDisable() {}

        protected virtual void Start()
        {
            MySceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);
            _killedEnemyService =
                ServiceLocator.Instance.GetService<KilledEnemyService>();
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

        protected virtual void EndAnimDead()
        {
            gameObject.SetActive(false);
        }
        
        protected virtual void StartAnimDead()
        {
            //TODO: revisar cofre
            _myCollider.enabled = false; 
        }

        protected abstract void DoAttack();
        protected abstract void DamageReceivedNotify(bool isDead);

        private bool ApplyDamage(int damage)
        {
            _currentHealth -= damage;

            if(_currentHealth > 0)
                return false;

            OnDied?.Invoke();
            _killedEnemyService.AddOne();
            _animator.SetTrigger(_dieParam);
            _currentHealth = 0;
            return true;
        }

        public void DamageReceived(int damage)
        {
            if(_isDead) return;
            _isDead = ApplyDamage(damage);
            DamageReceivedNotify(_isDead);
        }
    }
}