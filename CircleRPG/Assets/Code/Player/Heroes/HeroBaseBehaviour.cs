using System;
using Code.Domain.Interfaces;
using Code.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Player.Heroes
{
    public abstract class HeroBaseBehaviour : MonoBehaviour, IDamageable, IAttack
    {
        private                  NavMeshAgent _agent;
        private                  Animator     _animator;
        public                  Action       OnAttackComplete;
        [SerializeField] private GameObject   _point;
        [SerializeField] private int          _currentHealth;
        private int          _dieParam = Animator.StringToHash("Die");
        public                   Collider            FocusEnemy{get;set;}
        
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
            _agent.SetDestination(_point.transform.position);
        }

        public void Attack(Action onComplete)
        {
            if(!CanAttack()) return;

            OnAttackComplete = onComplete;
            DoAttack(FocusEnemy);
        }
        
        public bool CanAttack()
        {
            return FocusEnemy;
        }
        
        protected abstract void DoAttack(Collider objetive);

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
        
        public virtual void AttackFinish()
        {
            OnAttackComplete?.Invoke();
        }

        public void TestChangeColorMagenta()
        {
            //ataco
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.magenta);
        }
        
        public void TestChangeColorYellow()
        {
            //idle
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        }
        
        public void TestChangeColorGreen()
        {
            //moving
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        }
    }
}