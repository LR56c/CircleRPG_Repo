using System;
using UnityEngine;

namespace Code
{
    public class CircleController : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;

        private void Update()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            Vector3 v = _speed * Time.deltaTime * new Vector3(x, 0f, z);
            
            transform.Translate(v);
        }
    }
}