using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyMoveMidPointSMB : EnemyMoveBaseSMB
    {
        //TODO: colocar a los enemigos coll que ataque si estan muy cerca
        
        protected override void Move()
        {
            var myPosition = m_MonoBehaviour.transform.position;
            var heroPosition = m_MonoBehaviour.GetHero();
            
            var destination = myPosition;

            if(heroPosition)
            {
                destination = (myPosition + heroPosition.transform.position) / 2;
            }
            
            _navMeshAgent.SetDestination(destination);
        }
    }
}