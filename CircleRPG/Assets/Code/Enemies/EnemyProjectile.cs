using Code.Player.Heroes;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyProjectile : MonoBehaviour
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
            var player = other.collider.GetComponent<HeroBaseBehaviour>();
            if(player)
            {
                player.DamageReceived(_damage);
                Destroy(gameObject);
            }
        }
    }
}