using System;
using System.Collections.Generic;
using Code.Player.Heroes;
using Code.UI;
using Code.Utility;
using Lean.Touch;
using Rewired.ComponentControls;
using UnityEngine;

namespace Code.Player
{
    public class PlayerGroupBehaviour : MonoBehaviour
    {
        private Rigidbody     _rb;
        private Animator      _myAnimator;
        private Vector3       _joystickValue;
        private int           _attackParam = Animator.StringToHash("Attack");
        private int           _movingParam = Animator.StringToHash("MoveVector");
        [SerializeField] private bool          bWaitAttack  = false;
        private int _heroDiedCounter = 0;
        
        [Header("External")] 
        
        [SerializeField] private TouchJoystick _touchJoystick;
        
        [Header("Config")]
        
        [SerializeField] private float _speed = 20f;

        [SerializeField] private UILevel     _uiLevel;
        [SerializeField] private Transform[] _circleHelper = new Transform[3];

        [SerializeField] private HeroBaseBehaviour[] _heroes         = new HeroBaseBehaviour[3];
        [SerializeField] private Animator[]          _heroesAnimator = new Animator[3];
        
        [SerializeField] private List<Collider> _enemyList = new List<Collider>();
        
        [SerializeField] private GameObject _focusEnemyCircle;
        [SerializeField] private Collider   _focusEnemy;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _myAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            _touchJoystick.ValueChangedEvent += OnValueChangedEvent;

            foreach(var hero in _heroes)
            {
                hero.OnDied += OnHeroDied;
            }
        }
        
        private void OnHeroDied()
        {
            _heroDiedCounter++;

            if(_heroDiedCounter >= 3)
            {
                _uiLevel.OnLose();
                _heroDiedCounter = 3;
            }
        }

        private void OnDisable()
        {
            _touchJoystick.ValueChangedEvent -= OnValueChangedEvent;
            
            foreach(var hero in _heroes)
            {
                hero.OnDied -= OnHeroDied;
            }
        }

        private void OnValueChangedEvent(Vector2 position)
        {
            _joystickValue = new Vector3(position.x, 0f, position.y);
        }

        private void Update()
        {
            CheckInputs();

            UpdateAnimator();

            if(!GetFocusEnemy()) return;
            CheckNear();
            CheckEnemyToAnimators();
            CheckFocusEnemy();
        }

        private void CheckNear()
        {
            // se podria revisar cada 5s
            if(_enemyList.Count <= 1) return;
            
            float actualEnemyDistance =
                Vector3.Distance(transform.position, _focusEnemy.transform.position);

            foreach(Collider t in _enemyList)
            {
                if(t != _focusEnemy)
                {
                    float otherEnemyDistance =
                        Vector3.Distance(transform.position, t.transform.position);

                    if(otherEnemyDistance < actualEnemyDistance)
                    {
                        _focusEnemy = t;
                    }
                }
            }
        }

        private void CheckInputs()
        {
            Vector3 vector = _speed * Time.deltaTime * _joystickValue;
            _rb.velocity += vector;
            _touchJoystick.gameObject.SetActive(LeanTouch.Fingers.Count <= 2);
        }

        private void CheckFocusEnemy()
        {
            if(_focusEnemy.gameObject.activeInHierarchy && _focusEnemy)
            {
                _focusEnemyCircle.SetActive(true);
                _focusEnemyCircle.transform.position = _focusEnemy.transform.position;
            }
            else
            {
                _focusEnemyCircle.SetActive(false);
            }
        }

        private void CheckEnemyToAnimators()
        {
            if(!bWaitAttack)
            {
                bWaitAttack = true;
                
                foreach(var heroAnimator in _heroesAnimator)
                {
                    heroAnimator.SetTrigger(_attackParam);
                }

                foreach(var heroBehaviour in _heroes)
                {
                    heroBehaviour.FocusEnemy = _focusEnemy;
                    heroBehaviour.Attack(() => {bWaitAttack = false;});
                }
               
            }
        }

        private Collider GetFocusEnemy()
        {
            if(_enemyList.Count == 0)
            {
                _focusEnemy = null;
                _focusEnemyCircle.SetActive(false);
                return null;
            }

            if(_enemyList[0].gameObject.activeInHierarchy && _enemyList[0])
            {
                _focusEnemy = _enemyList[0];
                return _enemyList[0];
            }
           
            _enemyList.Remove(_enemyList[0]);
            _focusEnemy = null;
            _focusEnemyCircle.SetActive(false);

            return GetFocusEnemy();
        }

        private void UpdateAnimator()
        {
            foreach(Animator animator in _heroesAnimator)
            {
                animator.SetFloat(_movingParam, _rb.velocity.magnitude);
            }

            _myAnimator.SetFloat(_movingParam, _rb.velocity.magnitude);
        }

        public void RemoveCollider(Collider collider)
        {
            _enemyList.Remove(collider);

            if(GetFocusEnemy()) return;
            _focusEnemyCircle.SetActive(false);
            bWaitAttack = false;

            foreach(var heroAnimator in _heroesAnimator)
            {
                heroAnimator.ResetTrigger(_attackParam);
            }
        }

        public void AddCollider(Collider collider)
        {
            _enemyList.Add(collider);
            GetFocusEnemy();
        }

        public void MoveHeroes(Vector3 newPos)
        {
            transform.position = newPos;

            for(int i = 0; i < _heroes.Length; i++)
            {
                var circlePos = _circleHelper[i].position;
                _heroes[i].DoWarp(circlePos);
            }
        }
    }
}