using Code.Utility;
using UnityEngine;

namespace Code.Enemies.FMS
{
    public class EnemyChaseSMB : EnemyMovingBaseSMB
    {
        protected override void Move()
        {
            m_MonoBehaviour.Move();
            bAction = true;
        }
    }
}