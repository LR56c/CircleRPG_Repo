using System;

namespace Code.Domain.Interfaces
{
    public interface IDamageable
    {
        void   DamageReceived(int damage);
    }
}