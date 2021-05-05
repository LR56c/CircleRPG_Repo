using System;

namespace Code.Installers
{
    public class KilledEnemyService
    {
        public event Action<int> OnEnemyKilled;

        //Suma contador kills de los heroes
        public void AddOne()
        {
            OnEnemyKilled?.Invoke(1);
        }
    }
}