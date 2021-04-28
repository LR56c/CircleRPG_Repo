using UnityEngine;
using UnityEngine.AI;

namespace Code.Player
{
    public abstract class HeroBaseBehaviour : MonoBehaviour
    {
        private                  NavMeshAgent _agent;
        [SerializeField] private GameObject   _point;
    
        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            RegisterHero();
        }

        protected abstract void RegisterHero();

        protected virtual void Update()
        {
            _agent.SetDestination(_point.transform.position);
        }
    }
}
