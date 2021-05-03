using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.Types
{
    public class BatEnemyBehaviour : EnemyBaseBehaviour
    {
        private                  NavMeshAgent _navMeshAgent;
        private                  float        _cachedSpeed      = 0f;
        [SerializeField] private float        _distanceInMeters = 8.0f;
        [SerializeField] private float        _seconds          = 5.0f;
        [SerializeField] private float        _finalVelocity    = 0.0f;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _finalVelocity = (_distanceInMeters / _seconds) * 10;
        }

        protected override void DoAttack()
        {
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }

        protected void BatImpulse()
        {
            _cachedSpeed = _navMeshAgent.speed;
            _navMeshAgent.speed = 0f;
            _rb.velocity += transform.forward * _finalVelocity;
            DOVirtual.DelayedCall(_seconds, () =>
            {
                AttackFinish();
                _navMeshAgent.speed = _cachedSpeed;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            });
        }
    }
}