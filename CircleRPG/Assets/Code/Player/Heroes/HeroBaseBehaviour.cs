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

        private int _skillDataPref = 0;
        [SerializeField]private string _skillPref;
        [SerializeField]private string _dataPref;
        private int              _dieParam = Animator.StringToHash("Die");
        public  Action           OnAttackComplete;
        public event Action      OnDied;
        public event Action<int> OnDamaged;
        public event Action<int> OnHeal;
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

        private                  KilledEnemyService _killedEnemyService;
        private                  int                _killCount         = 0;
        [SerializeField] private int                _killsToGetAbility = 1;
        public                   Collider           FocusEnemy{get;set;}
        
        [Header("External")]
        [SerializeField] private UIHeroAbility _uiHeroAbility;

        

        protected virtual void Start()
        {
            MySceneLinkedSMB<HeroBaseBehaviour>.Initialise(_animator, this);

            _pool = ObjectPool.GetObjectPool("pool");

            _killedEnemyService =
                ServiceLocator.Instance.GetService<KilledEnemyService>();
            _killedEnemyService.OnEnemyKilled += OnEnemyKilled;
            
            //start check kill data
            if (PlayerPrefs.HasKey(_dataPref))
            {
                var kills = PlayerPrefs.GetInt(_dataPref);
                _killCount = kills;
            }

            //start check has skill
            if (PlayerPrefs.HasKey(_skillPref))
            {
                var skillData = PlayerPrefs.GetInt(_skillPref);

                if (skillData > 0)
                {
                    _uiHeroAbility.ActiveHeroAbility(GetHeroEType());
                }
            }
        }

        private void OnEnemyKilled(int oneDeath)
        {
            _killCount += oneDeath;
            AddKillPlayerPref();

            if(_killCount >= _killsToGetAbility)
            {
                _uiHeroAbility.ActiveHeroAbility(GetHeroEType());
                PlayerPrefs.SetInt(_skillPref, 1);
                ResetKillPlayerPref();
                _killCount = 0;
            }
        }

        private void AddKillPlayerPref()
        {
            if (PlayerPrefs.HasKey(_dataPref))
            {
                var kills = PlayerPrefs.GetInt(_dataPref);
                kills++;
                PlayerPrefs.SetInt(_dataPref, kills);
            }
            else
            {
                PlayerPrefs.SetInt(_dataPref, 1);
            }
        }

        private void ResetKillPlayerPref()
        {
            PlayerPrefs.DeleteKey(_dataPref);
        }

        protected abstract int GetHeroEType();

        protected virtual void Update()
        {
            _agent.SetDestination(_pointToMove.position);
        }

        public void Attack(Action onComplete)
        {
            if(!CanAttack()) return;

            OnAttackComplete = onComplete;
        }

        public void TurnToTarget()
        {
            transform.DOLookAt(FocusEnemy.transform.position, _tweenTimeRotate,
                               AxisConstraint.Y, Vector3.up);
        }

        public bool CanAttack()
        {
            return FocusEnemy;
            //return FocusEnemy.gameObject.activeInHierarchy;
        }
        
        public void DamageReceived(int damage)
        {
            bool isDead = ApplyDamage(damage);
            DamageReceivedNotify(isDead);
        }

        public void Heal(int value)
        {
            _currentHealth += value;
            OnHeal?.Invoke(value);

            if(_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        private void AnimationDiedComplete()
        {
            OnDied?.Invoke();
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

            
            _animator.SetTrigger(_dieParam);
            _currentHealth = 0;
            return true;
        }

        protected virtual void ThrowProjectile()
        {
            if(!CanAttack()) return;

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