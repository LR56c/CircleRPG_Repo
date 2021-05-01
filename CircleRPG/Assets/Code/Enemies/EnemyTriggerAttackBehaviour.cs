using Code.Player.Heroes;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyTriggerAttackBehaviour : MonoBehaviour
    {
        [SerializeField] private int _damage = 5;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<HeroBaseBehaviour>();
            if(!player) return;
            player.DamageReceived(_damage);
        }
    }
}