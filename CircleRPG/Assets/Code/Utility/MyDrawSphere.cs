using System;
using DG.Tweening;
using UnityEngine;

namespace Code.Utility
{
    public class MyDrawSphere : MonoBehaviour
    {
        private Collider _collider;
        private float    _radius = 1f;
        
        public void Config(Vector3 position,
                           float radius,
                           LayerMask layer,
                           float duration)
        {
            transform.position = position;
            transform.DOScale(transform.localScale * radius, 0.1f);
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _radius = radius;
            
            var coll = Physics.CheckSphere(position, 5f, layer);
            var msg = coll ? "correct" : "invalid";
            Debug.Log(msg);
            
            Destroy(gameObject, duration);
        }
        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius/2);
        }
    }
}