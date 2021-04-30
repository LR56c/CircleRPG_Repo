using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyMidPointSMB : EnemyMoveSMB
    {
        protected override void Move()
        {
            var myPosition = m_MonoBehaviour.transform.position;
            var heroPosition = m_MonoBehaviour.GetHero();
            
            var destination = myPosition;

            if(heroPosition)
            {
                destination = (myPosition + heroPosition.transform.position) / 2;
                Debug.Log("midPoint");
            }
            
            m_MonoBehaviour.Move(destination);
        }
    }
}