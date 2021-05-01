using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyChaseSMB : EnemyMoveBaseSMB
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
    }
}