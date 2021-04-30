using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMidPoint : MonoBehaviour
{
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    private void Start()
    {
        var aa = (point1.position + point2.position) / 2;
        Debug.DrawRay(aa, Vector3.up, Color.magenta,10f);
    }
}
