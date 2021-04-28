using Code.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.FMS
{
    public class MovingSMB : SceneLinkedSMB<EnemyBaseBehaviour>
    {
        private                  int          _moveRandomParam = Animator.StringToHash("MoveRandom");
        private                  int          _movingParam     = Animator.StringToHash("Moving");
        private                  NavMeshAgent _navMeshAgent;
        private                  int          _areaMask = 0;
        [SerializeField]             private bool         bMoveRandom      = false;
        [SerializeField]             private bool         bHavePoint      = false;
        [SerializeField]             private float          _walkPointRange = 5.0f;
        [SerializeField] private Vector3 _validPoint;
        

        public override void OnStart(Animator animator)
        {
            _navMeshAgent = m_MonoBehaviour.GetComponent<NavMeshAgent>();
            _areaMask = _navMeshAgent.areaMask;
        }

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetUpdate(false);
            bMoveRandom = animator.GetBool(_moveRandomParam);
        }

        public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                         int      layerIndex)
        {
            //TODO: arreglar esto y ver porque no va el flujo que pensaba, meter action si no funciona

            if(!bHavePoint)
            {
                if(bMoveRandom)
                {
                    GetRandomPoint();
                }
                else
                {
                    bHavePoint = true;
                }
            }
            else
            {
                if(bMoveRandom)
                {
                    _navMeshAgent.SetDestination(_validPoint);
                }
                else
                {
                    m_MonoBehaviour.Move();

                }

                if(NavMeshShouldCancel())
                {
                    Debug.Log("cancel nav");
                    NavMeshCancel();
                }
            }
        }
    
        public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetUpdate(true);
        }
    
        public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                           int      layerIndex)
        {
            
        }
    
        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bHavePoint = false;
        }

        private void GetRandomPoint()
        {
            var point = GetRandomPointInXZ();

            if(NavMesh.SamplePosition(point, out NavMeshHit hit, _walkPointRange, _areaMask))
            {
                _validPoint = hit.position;
                Debug.DrawRay(_validPoint, Vector3.up, Color.cyan, 10f);
                bHavePoint = true;
            }
        }

        private Vector3 GetRandomPointInCircle()
        {
            var value = Random.Range(-_walkPointRange, _walkPointRange) *
                        Random.insideUnitCircle;
            
            return new Vector3(value.x, 0, value.y);
        }
        
        private Vector3 GetRandomPointInXZ()
        {
            float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
            float randomX = Random.Range(-_walkPointRange, _walkPointRange);
            var myPos = m_MonoBehaviour.transform.position;
            
            return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
        }
        
        private bool NavMeshShouldCancel()
        {
            return _navMeshAgent.pathPending || _navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance;
        }
        
        private void NavMeshCancel()
        {
            _thisAnimator.SetBool(_movingParam, false);
            _thisAnimator.SetBool(_moveRandomParam, false);
            _navMeshAgent.ResetPath();
        }
    }
}
