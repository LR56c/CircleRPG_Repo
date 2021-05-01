namespace Code.Enemies.SMB
{
    public class EnemyNotMoveSMB : EnemyMoveBaseSMB
    {
        protected override void Move()
        {
            NavMeshCancel();
        }
    }
}