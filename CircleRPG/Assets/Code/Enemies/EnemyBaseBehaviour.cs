using System;
using System.Collections.Generic;
using Code.Domain.Enemies;
using Code.Domain.Interfaces;
using Code.Player;
using Code.Utility;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Code.Enemies
{
    public abstract class EnemyBaseBehaviour : MonoBehaviour, IDamageable, IAttack, IMove
    {
        //preguntar
        [SerializeField] protected EnemyBaseData _baseData;

        protected Rigidbody    _rb;
        protected NavMeshAgent _navMeshAgent;
        protected Animator     _animator;
        private int _dieParam        = Animator.StringToHash("Die");
        
        private int _movingParam     = Animator.StringToHash("Moving");
        private int _attackParam     = Animator.StringToHash("Attack");
        private int _moveRandomParam = Animator.StringToHash("MoveRandom");

        [SerializeField] private LayerMask _groundLayer = UnityConstants.Layers.FloorMask;
        [SerializeField] private LayerMask _playerLayer = UnityConstants.Layers.ChampionMask;
        
        [SerializeField] private bool _playerInAttackRange = false;
        [SerializeField] private float _attackRange = 3f;
        

        [SerializeField] private bool _playerInSightRange  = false;
        [SerializeField] private float _sightRange  = 5f;


        [SerializeField] protected float _slerpSpeed = 5f;
        [SerializeField] private bool bUpdate = true;
        [SerializeField] protected float _currentHealth;

        [SerializeField] private List<HeroBaseBehaviour> _heros;
        
        public void SetUpdate(bool value) => bUpdate = value;
        public bool PlayerInAttackRange   => _playerInAttackRange;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            EnemyLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);
            SceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);

            //TODO: pool projectiles
            //TODO: ver forma de guardar
            //TODO: ver colision enemy
            //TODO: esto deberia tomarse de teamconfig, dando una clase con un array de los elegidos
            _heros.Add(ServiceLocator.Instance.GetService<ArcherHeroBehaviour>());
            _heros.Add(ServiceLocator.Instance.GetService<ShieldHeroBehaviour>());
            _heros.Add(ServiceLocator.Instance.GetService<HammerHeroBehaviour>());
        }

        protected HeroBaseBehaviour GetHeroPosition()
        {
            //TODO: almacenando el heroe elegido, cada vez que ataque ver si sigue con vida, para elegir otro
            int randomIndex = Random.Range(0, _heros.Count);
            return _heros[randomIndex];
        }
        
        /* PARA EDITOR
         if(Input.GetKeyDown(KeyCode.F1))
        {
            MyDrawSphere sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere)
                                            .AddComponent<MyDrawSphere>();
            //agregar posicion de creacion
            sphere.Config(Vector3.zero, 5f, _playerLayer, 10f);
        }*/
        
        public void Move()
        {
            DoMove();
        }

        protected abstract void DoMove();

        public void Attack(Action onComplete)
        {
            //TODO: ver aqui si el heroe elegido sigue con vida?
            
            /*if(CanAttack())
            {
                DoAttack();
            }*/
            
            DoAttack(onComplete);
        }

        //protected abstract bool CanAttack();
        protected abstract void DoAttack(Action onComplete);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _sightRange);
        }
    }
}