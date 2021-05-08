using System;
using Code.Player.Heroes;
using DG.Tweening;
using FredericRP.ObjectPooling;
using UnityConstants;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private int        _damage;
        [SerializeField] private float      _speed = 5.0f;
        [SerializeField] private Rigidbody  _rb;
        [SerializeField] private bool       bContact = false;
        private                  ObjectPool _pool;
        private                  float      _ultimateSeconds = 15.0f;
        private                  Tween      _ultimateDestroy;
        
        private void OnEnable()
        {
            _pool = ObjectPool.GetObjectPool("pool");
            _ultimateDestroy = DOVirtual.DelayedCall(_ultimateSeconds, () => _pool.Pool(gameObject));
        }

        private void OnDisable()
        {
            _ultimateDestroy.Kill();
        }

        private void FixedUpdate()
        {
            if(bContact) return;
            _rb.velocity = transform.forward * (_speed * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.CompareTag(Tags.BreakBullet))
            {
                _pool.Pool(gameObject);
                //Destroy(gameObject);
            }

            //if(other.gameObject.CompareTag(Tags.Enemy)) return;
            
            var player = other.collider.GetComponent<HeroBaseBehaviour>();

            if(player)
            {
                player.DamageReceived(_damage);
                _pool.Pool(gameObject);
                //Destroy(gameObject);
            }
        }
    }
}