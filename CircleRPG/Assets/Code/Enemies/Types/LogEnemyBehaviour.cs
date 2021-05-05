using Code.Enemies.SMB;
using Unity.Profiling;
using UnityEngine;

namespace Code.Enemies.Types
{
    public class LogEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform _dangerZone;
        
        [SerializeField] private EnemyProjectile _projectile;
        [SerializeField] private float           _initialAngle = 0.0f;
        [SerializeField] private int             _numbers      = 4;
        private float           _sumAngle     = 0f;

        protected override void Awake()
        {
            base.Awake();
            _sumAngle = 360f / _numbers;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            var tempJumpMoveSmb = _animator.GetBehaviour<EnemyJumpMoveSMB>();
            
            if(!tempJumpMoveSmb)
            {
                Debug.Log("No se ha podido establecer DangerZone en Log Enemy");
                return;
            }
            tempJumpMoveSmb.SetSpecificDangerZone(_dangerZone);
        }

        protected override void DoAttack()
        {
            
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }

        protected void ThrowX()
        {
            float angle = _initialAngle;
            for(int i = 0; i < _numbers; i++)
            {
                angle += _sumAngle;
                Instantiate(_projectile, _myCollider.bounds.center,
                            Quaternion.Euler(0f, angle, 0f));
            }
        }
    }
}