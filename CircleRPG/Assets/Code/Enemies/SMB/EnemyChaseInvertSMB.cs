using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyChaseInvertSMB : EnemyMoveBaseSMB
    {
        protected override void Move()
        {
            var heroPosition = m_MonoBehaviour.GetHero();
            
            var destination = m_MonoBehaviour.transform.position;

            if(heroPosition)
            {
                destination = heroPosition.transform.position;
            }

            _navMeshAgent.SetDestination(destination);
        }

        protected override void MoveBase()
        {
            if(_animator.GetBool(_cameFromAttackParam))
            {
                WaitDelay();
                if(bWait) Move();
                return;
            }

            NavMeshCancel();
        }
    }
}