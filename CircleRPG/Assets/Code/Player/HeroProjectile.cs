using Code.Enemies.Types;
using Code.Player.Heroes;
using UnityConstants;
using UnityEngine;

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

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<EnemyBaseBehaviour>();
            var wall = other.CompareTag(Tags.Wall);

            if(wall)
            {
                Destroy(gameObject);
            }

            if(enemy)
            {
                enemy.DamageReceived(_damage);
                Destroy(gameObject);
            }
        }
    }
}