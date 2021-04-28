using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Test.OrderTest
{
    public class ControllerExample : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _positionsList = new List<Vector3>();
        [SerializeField] private GameObject    _spawnPoint;

        [SerializeField] private List<GameObject> _units             = new List<GameObject>();
        [SerializeField] private float[]          _ringDistance      = {5f,10f,20f};
        private                  int[]            _ringPositionCount = {5, 10, 20};
    
        private void Awake()
        {
            _positionsList =
                GetPositionListAround(_spawnPoint.transform.position,
                                      _ringDistance, _ringPositionCount);

            for(int i = 0; i < _units.Count; i++)
            {
                _units[i].transform.position = _positionsList[i];
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                int randIndex = Random.Range(0, _units.Count);
                GameObject go = _units[randIndex];
                _units.Remove(go);
                Debug.Log($"Eliminado: {go.name}");
                Destroy(go);
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Recargar");
                for(int i = 0; i < _units.Count; i++)
                {
                    _units[i].transform.position = _positionsList[i];
                }
            }
        }

        private List<Vector3> GetPositionListAround(Vector3 startPosition,
                                                    float[] ringDistance,
                                                    int[]   ringPositionCount) 
        {
            List<Vector3> positionList = new List<Vector3>();
        
            positionList.Add(startPosition);
        
            for (int i = 0; i < ringDistance.Length; i++) {
                positionList.AddRange(GetPositionListAround(startPosition,
                                                                ringDistance[i], 
                                                                ringPositionCount[i]));
            }
        
            return positionList;
        }
    
        private IEnumerable<Vector3> GetPositionListAround(Vector3 startPosition,
                                                           float   distance, 
                                                           int     positionCount) 
        {
            List<Vector3> positionList = new List<Vector3>();
        
            for (int i = 0; i < positionCount; i++) {
                float angle = i * (360f / positionCount);
                Vector3 dir = ApplyRotationToVector(angle);
                Vector3 position = startPosition + dir * distance;
                positionList.Add(position);
            }
        
            return positionList;
        }
    
        private Vector3 ApplyRotationToVector(float angle) 
        {
            return Quaternion.Euler(0, angle, 0 ) * Vector3.forward;
        }
    }
}
