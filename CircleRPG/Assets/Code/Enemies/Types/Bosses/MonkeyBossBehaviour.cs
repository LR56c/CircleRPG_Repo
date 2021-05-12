using System;
using DG.Tweening;
using FredericRP.ObjectPooling;
using UnityConstants;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

namespace Code.Enemies.Types
{
    public class MonkeyBossBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private MonkeyBossBehaviour[] _monkey = new MonkeyBossBehaviour[2];
        [SerializeField] private float      _speed       = 5.0f;
        private                  Vector3    _oldVelocity = Vector3.zero;
       
        [Header("Gen")] 
        [SerializeField] private float _randomDirRange             = 1.0f;
        
            //transform.rotation = Quaternion.LookRotation(GetRandomPointXZ());

        protected override void DoAttack()
        {
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
            if(!isDead) return;

            if(_monkey.Length == 0) return;

            for(int i = 0; i < _monkey.Length; i++)
            {
                _monkey[i].transform.position = transform.position;
                _monkey[i].gameObject.SetActive(true);
            }
        }

        private void FixedUpdate()
        {
            _rb.velocity = (_speed * Time.fixedDeltaTime) * transform.forward;
            _oldVelocity = _rb.velocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.layer == Layers.PlayerProjectile) return;

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