using System;
using System.Collections.Generic;
using Code.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Code
{
    public abstract class EnemyBaseBehaviour : MonoBehaviour
    {
        private Rigidbody _rb;
        private NavMeshAgent _navMeshAgent;
        private Animator  _animator;
        
        [SerializeField] private LayerMask _groundLayer = UnityConstants.Layers.FloorMask;
        [SerializeField] private LayerMask _playerLayer = UnityConstants.Layers.ChampionMask;
        
        [SerializeField] private bool       _playerInAttackRange = false;
        [SerializeField] private bool       _playerInSightRange  = false;
        
        [SerializeField] private float      _sightRange          = 5f;
        [SerializeField] private float      _attackRange         = 3f;
        
        private                  bool       _walkPointSet;
        private                  Vector3    _walkPoint;
        [SerializeField] private float      _walkPointRange;
        private                  bool       _alreadyAttacked;
        
        [SerializeField] protected float        _currentHealth;
        [SerializeField] private float              _slerpSpeed = 5f;

        [SerializeField] private List<HeroBaseBehaviour> _heros;
        
        //pool projectiles
        [SerializeField] private GameObject _projectile;

        public event Action OnDamageReceived;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            //TODO: esto deberia tomarse de teamconfig
            _heros.Add(ServiceLocator.Instance.GetService<ArcherHeroBehaviour>());
            _heros.Add(ServiceLocator.Instance.GetService<ShieldHeroBehaviour>());
            _heros.Add(ServiceLocator.Instance.GetService<HammerHeroBehaviour>());        }

        /*
        #region Fix

        void Start()
        {
            SceneLinkedSMB<EnemyBaseBehaviour>.Initialise(_animator, this);
        }

        private void Update()
        {
            /*_playerInAttackRange = Physics.CheckSphere(transform.position,
                                                      _attackRange,
                                                      _playerLayer);#1#
            
            /*_playerInSightRange = Physics.CheckSphere(transform.position,
                                                     _sightRange,
                                                     _playerLayer);

            var playerInRangeAttack = Physics.OverlapSphere(transform.position, 
                                                                _attackRange,
                                                                _playerLayer);
            
            _championController = playerInRangeAttack?[0].GetComponent<ChampionController>();
            
            _playerInAttackRange = _championController;
            
            Debug.Log($"player detect: {_championController?.gameObject.name}", 
                                                _championController?.gameObject);
            
            if (!_playerInSightRange && !_playerInAttackRange) Patroling();
            if (_playerInSightRange  && !_playerInAttackRange) ChasePlayer();#1#
            
            //if (_playerInAttackRange && _playerInSightRange) AttackPlayer();
        }
        
        private void Patroling()
        {
            if (!_walkPointSet) SearchWalkPoint();

            if (_walkPointSet)
                _navMeshAgent.SetDestination(_walkPoint);

            Vector3 distanceToWalkPoint = transform.position - _walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                _walkPointSet = false;
        }
        private void SearchWalkPoint()
        {
            //Calculate random point in range
            float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
            float randomX = Random.Range(-_walkPointRange, _walkPointRange);

            _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(_walkPoint, -transform.up, 2f, _groundLayer))
                _walkPointSet = true;
        }

        private void ChasePlayer()
        {
            Assert.IsFalse(_heroBaseBehaviour, "ChasePlayer fail");
            _navMeshAgent.SetDestination(_heroBaseBehaviour.transform.position);
        }

        private void AttackPlayer()
        {
            //Make sure enemy doesn't move
            _navMeshAgent.SetDestination(transform.position);

            Assert.IsFalse(_heroBaseBehaviour, "AttackPlayer fail");
            Vector3 direction =
                (_heroBaseBehaviour.transform.position - transform.position).normalized;
            
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _slerpSpeed);
            //transform.LookAt(_championController.transform.position);

            if (!_alreadyAttacked)
            {
                ///Attack code here
                Rigidbody rb = Instantiate(_projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up      * 8f,  ForceMode.Impulse);
                ///End of attack code

                _alreadyAttacked = true;
                Invoke(nameof(ResetAttack), 1f /*timeBetweenAttacks#1#);
            }
        }
        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
        }
        
        private void DestroyEnemy()
        {
            Destroy(gameObject);
        }

        /*private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _sightRange);
        }#1#

        #endregion
        */
        
        
        public void ChildTriggerStay(Collider other)
        {
            var player = other.GetComponent<HeroBaseBehaviour>();
            if(player)
            {
                Debug.Log($"{gameObject.name}, chield trigger detect: {other.gameObject.name}", other.gameObject);
            }
        }

        public void Attack()
        {
            if(CanAttack())
            {
                DoAttack();
            }
        }

        protected abstract bool CanAttack();
        protected abstract void DoAttack();
        protected abstract void DamageReceived(bool isDead);

        public void ReceiveDamage(int damage)
        {
            bool isDead = ApplyDamage(damage);
            DamageReceived(isDead);
            NotifyDamageReceived();
        }
        
        private bool ApplyDamage(int damage)
        {
            _currentHealth -= damage;
            
            if(_currentHealth > 0) 
                return false;

            _currentHealth = 0;
            
            return true;
        }
        
        private void NotifyDamageReceived()
        {
            OnDamageReceived?.Invoke();
        }
    }
}