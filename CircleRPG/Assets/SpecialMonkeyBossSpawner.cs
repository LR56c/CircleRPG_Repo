using System;
using System.Collections;
using System.Collections.Generic;
using Code.Enemies.Types;
using FredericRP.ObjectPooling;
using UnityEngine;

public class SpecialMonkeyBossSpawner : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private MonkeyBossBehaviour _monkeyBossBehaviour;
    private                  ObjectPool          _pool;
    [SerializeField] private string              _prefabPoolName = "Monkey";
    private                  bool                bFirst          = false;

    private void OnEnable()
    {
        if(bFirst)
        {
            _pool = ObjectPool.GetObjectPool("pool");

            GameObject go = _pool.GetFromPool(_prefabPoolName);
            go.transform.position = _spawnPoint.position;
            go.transform.localScale *= 0.75f;
        }
        else
        {
            bFirst = true;
        }
    }
}