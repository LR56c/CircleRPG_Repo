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
        
        private int _movingParam     = Animator.StringToHash("Moving");
        private int _dieParam        = Animator.StringToHash("Die");
        private int _attackParam     = Animator.StringToHash("Attack");
        private int _moveRandomParam = Animator.StringToHash("MoveRandom");

        [SerializeField] private LayerMask _groundLayer = UnityConstants.Layers.FloorMask;
        [SerializeField] private LayerMask _playerLayer = UnityConstants.Layers.ChampionMask;
        
        [Space(5f)]
        
        [SerializeField] private bool _playerInAttackRange = false;
        [SerializeField] private float _attackRange = 3f;
        
        [Space(5f)]

        [SerializeField] private bool _playerInSightRange  = false;
        [SerializeField] private float _sightRange  = 5f;

        [Space(5f)]

        [SerializeField] protected float _currentHealth;
        [SerializeField] protected float _slerpSpeed = 5f;
        [SerializeField] private bool bUpdate = true;

        [SerializeField] private List<HeroBaseBehaviour> _heros;
        
        public void SetUpdate(bool value) => bUpdate = value;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            SceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);

            //TODO: pool projectiles
            //TODO: ver forma de guardar
            //TODO: ver colision enemy
            //TODO: esto deberia tomarse de teamconfig, dando una clase con un array de los elegidos
            _heros.Add(ServiceLocator.Instance.GetService<ArcherHeroBehaviour>());
            _heros.Add(ServiceLocator.Instance.GetService<ShieldHeroBehaviour>());
            _heros.Add(ServiceLocator.Instance.GetService<HammerHeroBehaviour>());
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.F1))
            {
                MyDrawSphere sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere)
                                                .AddComponent<MyDrawSphere>();
                //agregar posicion de creacion
                sphere.Config(Vector3.zero, 5f, _playerLayer, 10f);
            }
#endif
            
            _playerInAttackRange = Physics.CheckSphere(transform.position,
                                                       _attackRange, _playerLayer);

            _playerInSightRange = Physics.CheckSphere(transform.position,
                                                      _sightRange, _playerLayer);

            if(!bUpdate) return;
            /*
             *  Move Random
             *  Si el jugador no esta en vision y
             *  no esta en rango de ataque
             */
            if(!_playerInSightRange && !_playerInAttackRange)
            {
                Debug.Log("Move Random");
                _animator.SetBool(_moveRandomParam, true);
                _animator.SetBool(_movingParam,     true);
            }
            /*
             *  Move Player
             *  Si el jugador esta en vision y
             *  no esta en rango de ataque
             */
            else if(_playerInSightRange && !_playerInAttackRange)
            {
                Debug.Log("Move Player");
                _animator.SetBool(_moveRandomParam, false);
                _animator.SetBool(_movingParam,     true);
            }
            /*
             *  Attack Player
             *  Si el jugador esta en vision y
             *  esta en rango de ataque
             */
            else if(_playerInAttackRange && _playerInSightRange)
            {
                Debug.Log("Attack Player");
                _animator.SetTrigger(_attackParam);
            }
        }
        
        public void Move()
        {
            DoMove();
        }

        protected abstract void DoMove();

        public void Attack()
        {
            /*if(CanAttack())
            {
                DoAttack();
            }*/
            
            DoAttack();
        }

        //protected abstract bool CanAttack();
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _sightRange);
        }
    }
}