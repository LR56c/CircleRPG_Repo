using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using THMSV.RPGBuilder.AI;
using THMSV.RPGBuilder.Managers;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(NPCSpawner))]
public class NPCSpawnerEditor : Editor
{
    private NPCSpawner REF;
    private int curTab;

    private SerializedProperty spawnerType;
    private SerializedProperty spawningEnabled;
    private SerializedProperty spawnCount;

    private readonly string[] toolBarTabs = {"ENDLESS", "COUNT", "MANUAL"};

    private void OnEnable()
    {
        REF = (NPCSpawner) target;
        spawningEnabled = serializedObject.FindProperty("spawningEnabled");
        spawnerType = serializedObject.FindProperty("spawnerType");
        spawnCount = serializedObject.FindProperty("spawnCount");

        curTab = spawnerType.enumValueIndex;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        curTab = GUILayout.Toolbar(curTab, toolBarTabs);
        spawnerType.enumValueIndex = curTab;
        switch (curTab)
        {
            case 1:
                spawnCount.intValue = EditorGUILayout.IntField("Count", spawnCount.intValue);
                break;
            case 2:
            {
                if (Application.isPlaying)
                    if (GUILayout.Button("Spawn"))
                        REF.ManualSpawnNPC();
                break;
            }
        }
        if (GUILayout.Button("+ Add NPC")) REF.spawnData.Add(new NPCSpawner.NPC_SPAWN_DATA());

        var ThisList = serializedObject.FindProperty("spawnData");
        REF.spawnData = GetTargetObjectOfProperty(ThisList) as List<NPCSpawner.NPC_SPAWN_DATA>;

        float spawnRateTaken = 0;
        for (var a = 0; a < REF.spawnData.Count; a++)
        {
            GUILayout.Space(10);
            var requirementNumber = a + 1;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("" + requirementNumber + ":", GUILayout.Width(25));
            
            EditorGUILayout.BeginVertical();
            REF.spawnData[a].npc = (RPGNpc) EditorGUILayout.ObjectField("NPC", REF.spawnData[a].npc, typeof(RPGNpc), false);
            REF.spawnData[a].spawnChance = EditorGUILayout.Slider("Spawn Rate", REF.spawnData[a].spawnChance, 0f, 100f-spawnRateTaken);
            spawnRateTaken += REF.spawnData[a].spawnChance;
            EditorGUILayout.EndVertical();

            GUIStyle xButtonStyle = new GUIStyle();
            xButtonStyle.normal.textColor = Color.red;
            xButtonStyle.fontSize = 25;
            
            if (GUILayout.Button("X", xButtonStyle,GUILayout.Width(25), GUILayout.Height(25)))
            {
                REF.spawnData.RemoveAt(a);
                return;
            }
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        /*var tps2 = serializedObject.FindProperty("spawnData");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(tps2, true);
        */
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(REF);
    }

    public object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }

        return obj;
    }
    private object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (var i = 0; i <= index; i++)
            if (!enm.MoveNext()) return null;
        return enm.Current;
    }
    
    
}