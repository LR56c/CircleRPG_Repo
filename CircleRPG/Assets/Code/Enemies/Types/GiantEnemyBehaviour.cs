using FredericRP.ObjectPooling;
using UnityEngine;

namespace Code.Enemies.Types
{
    public class GiantEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField]             private Transform       _ballPosition;
        [SerializeField]             private EnemyProjectile _ballProjectilePrefab;
        
        [Header("Numbers solo puede ser Impar")]
        [SerializeField] private int _numbers = 3;
        [SerializeField] private float      _angleStep = 45f;
        private                  int        _offsetMultiplier;
        private                  ObjectPool _pool;
        [SerializeField] private string     _prefabPoolName = "Giant";

        protected override void Start()
        {
            base.Start();
            PreCalculateOffsetMultiplier();
            _pool = ObjectPool.GetObjectPool("pool");
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


        protected override void DoAttack()
        {
         
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }
        
        public void ThrowBalls()
        {
            var hero = GetHero();
            if(!hero) return;
            var location = _ballPosition.position;
            location.y = hero.bounds.center.y;
            
            var offsetAngle = transform.eulerAngles.y - (_angleStep * _offsetMultiplier);

            for(int i = 0; i < _numbers; i++)
            {
                var go = _pool.GetFromPool(_prefabPoolName);
                go.transform.position = location;
                go.transform.rotation = Quaternion.Euler(0f, offsetAngle,0f);
                //Instantiate(_ballProjectilePrefab, location, Quaternion.Euler(0f, offsetAngle, 0f));
                offsetAngle += _angleStep;
            }
        }
    }
}