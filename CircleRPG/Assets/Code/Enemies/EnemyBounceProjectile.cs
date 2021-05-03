using System;
using Code.Player.Heroes;
using DG.Tweening;
using UnityConstants;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyBounceProjectile : MonoBehaviour
    {
        [SerializeField] private int       _damage;
        [SerializeField] private float     _speed = 5.0f;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float     _secondsToDestroy = 5.0f;
        private                  Vector3   _oldVelocity = Vector3.zero;

        private void OnEnable()
        {
            _rb.velocity = transform.forward * _speed;
        }

        private void FixedUpdate()
        {
            _oldVelocity = _rb.velocity;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            var player = other.collider.GetComponent<HeroBaseBehaviour>();

            var wall = other.collider.CompareTag(Tags.Wall);

            if(wall)
            {
                var normal = other.GetContact(0).normal;
                var reflect = Vector3.Reflect(_oldVelocity, normal);
                _rb.velocity = reflect;
                transform.rotation = Quaternion.LookRotation(reflect);
                Destroy(gameObject, _secondsToDestroy);
            }
            
            if(player)
            {
                player.DamageReceived(_damage);
                Destroy(gameObject);
            }
        }
    }
}