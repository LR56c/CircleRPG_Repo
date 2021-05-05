using Code.Player.Heroes;
using UnityConstants;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private int       _damage;
        [SerializeField] private float     _speed = 5.0f;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private bool      bContact = false;


        private void OnEnable()
        {
            if(bContact) return;
            _rb.velocity = transform.forward * _speed;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.layer == Layers.Wall)
            {
                Destroy(gameObject);
            }

            if(other.gameObject.CompareTag(Tags.Enemy)) return;
            
            var player = other.collider.GetComponent<HeroBaseBehaviour>();

            if(player)
            {
                player.DamageReceived(_damage);
                Destroy(gameObject);
            }
        }
    }
}