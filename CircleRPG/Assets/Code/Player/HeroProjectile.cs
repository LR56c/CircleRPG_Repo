using System;
using Code.Enemies.Types;
using Code.Player.Heroes;
using UnityConstants;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies
{
    public class HeroProjectile : MonoBehaviour
    {
        [SerializeField] private int       _damage;
        [SerializeField] private float     _speed = 5.0f;
        [SerializeField] private Rigidbody _rb;

        private void OnEnable()
        {
            _rb.velocity = transform.forward * _speed;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.layer == UnityConstants.Layers.Wall)
            {
                Destroy(gameObject);
            }

            var enemy = other.gameObject.GetComponent<EnemyBaseBehaviour>();

            if(enemy)
            {
                enemy.DamageReceived(_damage);
                Destroy(gameObject);
            }
        }
    }
}