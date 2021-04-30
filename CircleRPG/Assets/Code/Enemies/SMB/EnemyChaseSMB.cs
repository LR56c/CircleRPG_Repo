﻿using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyChaseSMB : EnemyMoveSMB
    {
        protected override void Move()
        {
            var heroPosition = m_MonoBehaviour.GetHero();
            
            var destination = m_MonoBehaviour.transform.position;

            if(heroPosition)
            {
                destination = heroPosition.transform.position;
                Debug.Log("chase smb");
            }
            
            m_MonoBehaviour.Move(destination);
        }
    }
}