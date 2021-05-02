using Code.Player.Heroes;
using UnityConstants;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField]             private int       _damage;
        [SerializeField]             private float     _speed = 5.0f;
        [SerializeField]             private Rigidbody _rb;
        [SerializeField] private bool         bContact = false;
        

        private void OnEnable()
        {
            if(bContact) return;
            _rb.velocity = transform.forward * _speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<HeroBaseBehaviour>();
            
            var wall = other.CompareTag(Tags.Wall);

            if(wall)
            {
                Destroy(gameObject);
            }

            if(player)
            {
                player.DamageReceived(_damage);
                Destroy(gameObject);
            }
        }
    }
}