﻿using UnityEngine;

public class RPGWorldPosition : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;

    public Vector3 position;

    public void updateThis(RPGWorldPosition newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        position = newData.position;
        displayName = newData.displayName;
    }
}