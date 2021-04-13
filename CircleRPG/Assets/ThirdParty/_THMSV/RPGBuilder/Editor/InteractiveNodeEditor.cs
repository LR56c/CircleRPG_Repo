using THMSV.RPGBuilder;
using THMSV.RPGBuilder.World;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(InteractiveNode))]
public class InteractiveNodeEditor : Editor
{
    private InteractiveNode nodeREF;

    private Color TitleColor = new Color(0, 255, 140, 255), SubTitleColor = new Color(255, 145, 0, 255);

    private int curCategory;
    private int curTab;

    private void OnEnable()
    {
        nodeREF = (InteractiveNode) target;
        switch (nodeREF.nodeCategory)
        {
            case InteractiveNode.InteractiveNodeCategory.combat:
            {
                curCategory = 0;

                switch (nodeREF.nodeType)
                {
                    case InteractiveNode.InteractiveNodeType.effectNode:
                        curTab = 0;
                        break;
                    case InteractiveNode.InteractiveNodeType.giveTreePoint:
                        curTab = 1;
                        break;
                    case InteractiveNode.InteractiveNodeType.giveClassEXP:
                        curTab = 2;
                        break;
                }
                break;
            }
            case InteractiveNode.InteractiveNodeCategory.general:
            {
                curCategory = 1;

                switch (nodeREF.nodeType)
                {
                    case InteractiveNode.InteractiveNodeType.container:
                        curTab = 0;
                        break;
                    case InteractiveNode.InteractiveNodeType.teachSkill:
                        curTab = 1;
                        break;
                    case InteractiveNode.InteractiveNodeType.giveSkillEXP:
                        curTab = 2;
                        break;
                }
                break;
            }
            case InteractiveNode.InteractiveNodeCategory.world:
            {
                curCategory = 2;

                switch (nodeREF.nodeType)
                {
                    case InteractiveNode.InteractiveNodeType.questNode:
                        curTab = 0;
                        break;
                    case InteractiveNode.InteractiveNodeType.completeTask:
                        curTab = 1;
                        break;
                    case InteractiveNode.InteractiveNodeType.resourceNode:
                        curTab = 2;
                        break;
                }
                break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((InteractiveNode) target),
            typeof(InteractiveNode), false);
        GUI.enabled = true;

        EditorGUI.BeginChangeCheck();
        var SubTitleStyle = new GUIStyle();
        SubTitleStyle.alignment = TextAnchor.UpperLeft;
        SubTitleStyle.fontSize = 17;
        SubTitleStyle.fontStyle = FontStyle.Bold;
        SubTitleStyle.normal.textColor = Color.white;

        curCategory = GUILayout.Toolbar(curCategory, new[] {"COMBAT", "GENERAL", "WORLD"});
        switch (curCategory)
        {
            case 0:
            {
                nodeREF.nodeCategory = InteractiveNode.InteractiveNodeCategory.combat;

                curTab = GUILayout.Toolbar(curTab, new[] {"EFFECTS", "TREE POINTS", "CLASS EXP"});
                if (curTab > 3) curTab = 0;
                switch (curTab)
                {
                    case 0:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.effectNode;

                        GUILayout.Space(5);
                        GUILayout.Label("Effects Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("effectsData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    case 1:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.giveTreePoint;

                        GUILayout.Space(5);
                        GUILayout.Label("Tree Points Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("treePointsData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    case 2:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.giveClassEXP;

                        GUILayout.Space(5);
                        GUILayout.Label("Class EXP Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("classExpData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                }

                break;
            }
            case 1:
            {
                nodeREF.nodeCategory = InteractiveNode.InteractiveNodeCategory.general;

                curTab = GUILayout.Toolbar(curTab, new[] {"CONTAINER", "SKILLS", "SKILLS EXP"});
                if (curTab > 2) curTab = 0;
                switch (curTab)
                {
                    case 0:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.container;

                        GUILayout.Space(5);
                        GUILayout.Label("Container Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("containerTablesData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();

                        nodeREF.spawnLootBagOnPlayer =
                            EditorGUILayout.Toggle("Spawn Loot Bag on Player?", nodeREF.spawnLootBagOnPlayer);
                        if (!nodeREF.spawnLootBagOnPlayer)
                            nodeREF.lootBagSpawnPoint = (Transform) EditorGUILayout.ObjectField("Loot Bag Position",
                                nodeREF.lootBagSpawnPoint, typeof(Transform), true);
                        break;
                    }
                    case 1:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.teachSkill;

                        GUILayout.Space(5);
                        GUILayout.Label("Skills Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("skillsData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    case 2:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.giveSkillEXP;

                        GUILayout.Space(5);
                        GUILayout.Label("Skills EXP Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("skillExpData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                }

                break;
            }
            case 2:
            {
                nodeREF.nodeCategory = InteractiveNode.InteractiveNodeCategory.world;

                curTab = GUILayout.Toolbar(curTab, new[] {"QUESTS", "TASKS", "RESOURCES"});
                if (curTab > 2) curTab = 0;
                switch (curTab)
                {
                    case 0:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.questNode;

                        GUILayout.Space(5);
                        GUILayout.Label("Quest Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("questsData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    case 1:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.completeTask;

                        GUILayout.Space(5);
                        GUILayout.Label("Tasks Specific", SubTitleStyle);
                        GUILayout.Space(5);
                        var tps2 = serializedObject.FindProperty("taskData");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(tps2, true);
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    case 2:
                    {
                        nodeREF.nodeType = InteractiveNode.InteractiveNodeType.resourceNode;

                        GUILayout.Space(5);
                        GUILayout.Label("Resource Node Specific", SubTitleStyle);
                        GUILayout.Space(5);

                        nodeREF.resourceNodeData = (RPGResourceNode) EditorGUILayout.ObjectField("Resource Node",
                            nodeREF.resourceNodeData, typeof(RPGResourceNode), false);

                        nodeREF.spawnLootBagOnPlayer =
                            EditorGUILayout.Toggle("Spawn Loot Bag on Player?", nodeREF.spawnLootBagOnPlayer);
                        if (!nodeREF.spawnLootBagOnPlayer)
                            nodeREF.lootBagSpawnPoint = (Transform) EditorGUILayout.ObjectField("Loot Bag Position",
                                nodeREF.lootBagSpawnPoint, typeof(Transform), true);
                        break;
                    }
                }

                break;
            }
        }

        // REQUIREMENT FIELDS
        GUILayout.Space(5);
        GUILayout.Label("Requirements", SubTitleStyle);
        GUILayout.Space(5);
        nodeREF.isClick =
            EditorGUILayout.Toggle("Is Click?", nodeREF.isClick);
        nodeREF.isTrigger =
            EditorGUILayout.Toggle("Is Trigger?", nodeREF.isTrigger);
        nodeREF.useDistanceMax = EditorGUILayout.FloatField("Use Distance", nodeREF.useDistanceMax);
        
        var tps5 = serializedObject.FindProperty("useRequirement");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(tps5, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        // GENERAL FIELDS
        GUILayout.Space(5);
        GUILayout.Label("State", SubTitleStyle);
        GUILayout.Space(5);

        nodeREF.nodeState = (InteractiveNode.InteractiveNodeState) EditorGUILayout.EnumPopup("State", nodeREF.nodeState);
        if (nodeREF.nodeType != InteractiveNode.InteractiveNodeType.resourceNode)
        {
            nodeREF.UseCount = EditorGUILayout.IntField("Use Count", nodeREF.UseCount);
            nodeREF.Cooldown = EditorGUILayout.FloatField("Cooldown", nodeREF.Cooldown);
            nodeREF.interactionTime = EditorGUILayout.FloatField("Interaction Time", nodeREF.interactionTime);
            nodeREF.useDistanceMax = EditorGUILayout.FloatField("Use Distance Max", nodeREF.useDistanceMax);
        }

        GUILayout.Space(5);
        GUILayout.Label("Animation", SubTitleStyle);
        GUILayout.Space(5);

        nodeREF.animationName = EditorGUILayout.TextField("Player Animation", nodeREF.animationName);
        
        GUILayout.Space(5);
        GUILayout.Label("VISUALS", SubTitleStyle);
        GUILayout.Space(5);
        nodeREF.readyVisual =
            (GameObject) EditorGUILayout.ObjectField("Ready Visual", nodeREF.readyVisual, typeof(GameObject), true);
        nodeREF.onCooldownVisual = (GameObject) EditorGUILayout.ObjectField("On Cooldown Visual",
            nodeREF.onCooldownVisual, typeof(GameObject), true);
        nodeREF.disabledVisual =
            (GameObject) EditorGUILayout.ObjectField("Disabled Visual", nodeREF.disabledVisual, typeof(GameObject),
                true);

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(nodeREF);
    }
}