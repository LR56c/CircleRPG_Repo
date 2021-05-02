using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Code.Enemies.Types
{
    public class CubeImpulseEnemyBehaviour : EnemyBaseBehaviour
    {
        private                  Rigidbody _rb;
        [SerializeField] private float     _distanceInMeters = 8.0f;
        [SerializeField] private float     _seconds          = 5.0f;
        [SerializeField] private float     _finalVelocity    = 0.0f;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            _finalVelocity = (_distanceInMeters / _seconds) * 10;
        }

        protected override void DoAttack()
        {
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }

        protected void Impulse()
        {
            _rb.velocity += transform.forward * _finalVelocity;
            DOVirtual.DelayedCall(_seconds, () =>
            {
                AttackFinish();
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            });
        }
    }
}