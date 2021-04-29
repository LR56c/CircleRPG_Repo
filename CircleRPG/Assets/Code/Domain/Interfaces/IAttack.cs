using System;

namespace Code.Enemies
{
    public interface IAttack
    {
        void Attack(Action onComplete);
    }
}