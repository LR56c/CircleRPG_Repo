using UnityEngine;

namespace Code.Enemies.Types
{
    public class GiantPlantBossBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private Transform[]       _ballPositions;
        [SerializeField] private EnemyProjectile _ballProjectilePrefab;

        [SerializeField] private float _minRandomDestroyTime = 0.5f;
        [SerializeField] private float _maxRandomDestroyTime = 2.5f;
        
        
        [Header("Numbers solo puede ser Impar")]
        [SerializeField] private int _numbers = 5;
        [SerializeField] private float _angleStep = 45f;
        private                  int   _offsetMultiplier;

        protected override void Start()
        {
            base.Start();
            PreCalculateOffsetMultiplier();
        }

        protected override void DoAttack()
        {
            
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }

        public void ThrowMultipleBall()
        {
            var hero = GetHero();
            if(!hero) return;

            foreach(var ballPosition in _ballPositions)
            {
                var location = ballPosition.position;
                
                //TODO: ver despues si se tiene que ajustar location.y a hero.bounds.center.y
                var offsetAngle = transform.eulerAngles.y - (_angleStep * _offsetMultiplier);

                for(int i = 0; i < _numbers; i++)
                {
                    EnemyProjectile go = Instantiate(_ballProjectilePrefab, location, 
                                Quaternion.Euler(0f, offsetAngle, 0f));
                    float randomDestroy =
                        Random.Range(_minRandomDestroyTime, _maxRandomDestroyTime);
                    //TODO: aqui se deberia volver a la pool
                    Debug.Log($"rand: {randomDestroy.ToString()}");
                    Destroy(go, randomDestroy);
                    offsetAngle += _angleStep;
                }
            }
        }
        
        private void PreCalculateOffsetMultiplier()
        {
            if((_numbers % 2) == 1)
            {
                _offsetMultiplier = Mathf.FloorToInt(_numbers / 2);
            }
            else
            {
                Debug.LogError($"Variable Numbers en {gameObject.name}, debe ser impar");
            }
        }
    }
}