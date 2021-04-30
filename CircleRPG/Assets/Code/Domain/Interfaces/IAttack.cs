using System;

namespace Code.Domain.Interfaces
{
    public interface IAttack
    {
        void Attack(Action onComplete);
    }
}