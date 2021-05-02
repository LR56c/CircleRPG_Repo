using UnityEngine;

namespace Code.Enemies.Triggers
{
    public class CubeEnemyStayTriggerAttackBehaviour : EnemyStayTriggerAttackBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        
        protected override void DoStay()
        {
            _rb.angularVelocity = Vector3.zero;
            _rb.ResetInertiaTensor();
        }
    }
}