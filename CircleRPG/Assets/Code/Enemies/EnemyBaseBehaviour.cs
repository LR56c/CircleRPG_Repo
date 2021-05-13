using System;
using System.Collections.Generic;
using Code.Domain.Interfaces;
using Code.Enemies.SMB;
using Code.Installers;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Code.Enemies.Types
{
    public abstract class EnemyBaseBehaviour : MonoBehaviour, IAttack
    {
        [SerializeField] protected Rigidbody    _rb;
        [SerializeField] protected Collider     _myCollider;
        [SerializeField] protected Animator     _animator;
        
        public Action            OnAttackComplete;
        public event Action      OnDied;
        public event Action<int, Color> OnDamaged;

        public int GetCurrentHealth() => _currentHealth;

        public int GetMaxHealth() => _initialMaxHealth;
        
        private int              _dieParam = Animator.StringToHash("Died");

        [SerializeField] protected int                _currentHealth    = 100;
        protected                  int                _initialMaxHealth = 0;
        [SerializeField] protected float              _tweenTimeRotate  = 1.0f;
        [SerializeField] private   List<Collider>     _inAreaHeros;
        [SerializeField] private GameObject _fx;
        
        public                     float              TweenTimeRotate => _tweenTimeRotate;
        private                    KilledEnemyService _killedEnemyService;
        private                    bool               _isDead;

        protected virtual void Awake()
        {
            _initialMaxHealth = _currentHealth;
        }

        protected virtual void OnEnable()
        {
            MySceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);

            var tempIdleSMB = _animator.GetBehaviour<EnemyIdleSMB>();

            if (!tempIdleSMB)
            {
                Debug.Log($"No se ha podido establecer FX en {gameObject.name}");
                return;
            }

            tempIdleSMB.SetFX(_fx);
        }
        
        protected virtual void OnDisable() {}

        protected virtual void Start()
        {
            _killedEnemyService =
                ServiceLocator.Instance.GetService<KilledEnemyService>();
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
            OnDied?.Invoke();
            gameObject.SetActive(false);
        }

        protected abstract void DoAttack();
        protected abstract void DamageReceivedNotify(bool isDead);

        private bool ApplyDamage(int damage)
        {
            _currentHealth -= damage;

            if(_currentHealth > 0)
                return false;
            
            _killedEnemyService.AddOne();
            _animator.SetTrigger(_dieParam);
            _currentHealth = 0;
            return true;
        }

        public void DamageReceived(int damage, Color colorIndex)
        {
            if(_isDead) return;
            _isDead = ApplyDamage(damage);
            OnDamaged?.Invoke(damage, colorIndex);
            DamageReceivedNotify(_isDead);
        }

    }
}