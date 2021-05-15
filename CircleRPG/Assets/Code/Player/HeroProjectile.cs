using System;
using Code.Enemies.Types;
using Code.Player.Heroes;
using DG.Tweening;
using FredericRP.ObjectPooling;
using UnityConstants;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies
{
    public class HeroProjectile : MonoBehaviour
    {
        [SerializeField] private             int        _damage;
        [SerializeField] private             float      _speed = 5.0f;
        [SerializeField] private             Rigidbody  _rb;
        [SerializeField] private             string     _prefabPoolName;
        private                              ObjectPool _pool;
        private                              float      _ultimateSeconds = 15.0f;
        private                              Tween      _ultimateDestroy;
        [SerializeField] private Color          _enemyColor;
        

        private void OnEnable()
        {
            _pool = ObjectPool.GetObjectPool("pool");
            _ultimateDestroy = DOVirtual.DelayedCall(_ultimateSeconds, () => _pool.Pool(gameObject));
            //_rb.velocity = transform.forward * _speed;
        }

        private void OnDisable()
        {
            _ultimateDestroy.Kill();
        }

        private void FixedUpdate()
        {
            _rb.velocity = transform.forward * (_speed * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.CompareTag(Tags.BreakBullet))
            {
                _pool.Pool(gameObject);
                //Destroy(gameObject);
            }

            var enemy = other.gameObject.GetComponent<EnemyBaseBehaviour>();

            if(enemy)
            {
                enemy.DamageReceived(_damage, _enemyColor);
                _pool.Pool(gameObject);
                //Destroy(gameObject);
            }
        }
    }
}