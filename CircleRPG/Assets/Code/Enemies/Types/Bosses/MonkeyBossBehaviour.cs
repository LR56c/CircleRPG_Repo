using System;
using DG.Tweening;
using UnityConstants;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

namespace Code.Enemies.Types
{
    public class MonkeyBossBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private MonkeyBossBehaviour _monkey;
        [SerializeField] private float               _speed      = 5.0f;
        private                  Vector3             _oldVelocity = Vector3.zero;

        [Header("Gen")] 
        private static int   _currentGen                 = 0;
        [SerializeField] private int   _numbersSucessors           = 2;
        [SerializeField] private float _randomDirRange             = 1.0f;
        [SerializeField] private int   _numbersOfCopiesInstantiate = 2;

        protected override void OnEnable()
        {
            base.OnEnable();
            transform.rotation = Quaternion.LookRotation(GetRandomPointXZ());
            _rb.velocity = transform.forward * _speed;
        }

        protected override void DoAttack()
        {
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
            if(isDead)
            {
                if(_currentGen <= _numbersSucessors)
                {
                    for(int i = 0; i < _numbersOfCopiesInstantiate; i++)
                    {
                        MonkeyBossBehaviour copy =
                            Instantiate(_monkey, transform.position, Quaternion.identity);
                    }

                    _currentGen++;
                }
                
                gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            //_rb.velocity = (_speed * Time.fixedDeltaTime) * transform.forward;
            _oldVelocity = _rb.velocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            /*var obj = other.gameObject;
            if(obj.CompareTag(Tags.Wall) || obj.CompareTag(Tags.Player) ||
               obj.CompareTag(Tags.Enemy))
            {
                
            }*/
            
            var normal = other.GetContact(0).normal;
            var reflect = Vector3.Reflect(_oldVelocity, normal);
            _rb.velocity = reflect;
            transform.rotation = Quaternion.LookRotation(reflect);
        }

        private Vector3 GetRandomPointXZ()
        {
            float randomZ = Random.Range(-_randomDirRange, _randomDirRange);
            float randomX = Random.Range(-_randomDirRange, _randomDirRange);
            var myPos = transform.position;

            return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
        }
    }
}