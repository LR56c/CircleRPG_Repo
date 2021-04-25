namespace Code
{
    public class GiantEnemyBehaviour : EnemyBaseBehaviour
    {
        protected override bool CanAttack()
        {
            return false;
        }

        protected override void DoAttack()
        {
        }

        protected override void DamageReceived(bool isDead)
        {
            
        }
    }
}