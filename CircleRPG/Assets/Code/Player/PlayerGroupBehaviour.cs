using System.Collections.Generic;
using Code.Player.Heroes;
using Code.Utility;
using Lean.Touch;
using Rewired.ComponentControls;
using UnityEngine;

namespace Code.Player
{
    public class PlayerGroupBehaviour : MonoBehaviour
    {
        private TouchJoystick _touchJoystick;
        private Rigidbody     _rb;
        private Animator      _myAnimator;
        private Vector3       _joystickValue;
        private int           _attackParam = Animator.StringToHash("Attack");
        private int           _movingParam = Animator.StringToHash("MoveVector");
        private bool          bWaitAttack  = false;

        [SerializeField] private float _speed = 20f;

        [SerializeField] private HeroBaseBehaviour[] _heroes = new HeroBaseBehaviour[3];
        [SerializeField] private Animator[]          _heroesAnimator = new Animator[3];

        [SerializeField] private List<Collider> _enemyList = new List<Collider>();
        [SerializeField] private GameObject     _focusEnemyCircle;
        [SerializeField] private Collider       _focusEnemy;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _myAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            for(int i = 0; i < _heroes.Length; i++)
            {
                _heroesAnimator[i] = _heroes[i].GetComponent<Animator>();
            }


            _touchJoystick = ServiceLocator.Instance.GetService<TouchJoystick>();
            _touchJoystick.ValueChangedEvent += OnValueChangedEvent;
        }

        private void OnDisable()
        {
            _touchJoystick.ValueChangedEvent -= OnValueChangedEvent;
        }

        private void OnValueChangedEvent(Vector2 position)
        {
            _joystickValue = new Vector3(position.x, 0f, position.y);
        }

        private void Update()
        {
            CheckInputs();

            UpdateAnimator();

            CheckEnemy();
            CheckFocusEnemy();
        }

        private void CheckInputs()
        {
            Vector3 vector = _speed * Time.deltaTime * _joystickValue;
            _rb.velocity += vector;
            _touchJoystick.gameObject.SetActive(LeanTouch.Fingers.Count != 3);
        }

        private void CheckFocusEnemy()
        {
            if(_focusEnemy)
            {
                _focusEnemyCircle.SetActive(true);
                _focusEnemyCircle.transform.position = _focusEnemy.transform.position;
            }
            else
            {
                _focusEnemyCircle.SetActive(false);
            }
        }

        private void CheckEnemy()
        {
            if(!GetFocusEnemy()) return;

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
                return null;
            }

            if(_enemyList[0].gameObject.activeInHierarchy)
            {
                _focusEnemy = _enemyList[0];
                return _enemyList[0];
            }

            _enemyList.Remove(_enemyList[0]);
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
        }

        public void AddCollider(Collider collider)
        {
            _enemyList.Add(collider);
        }
    }
}