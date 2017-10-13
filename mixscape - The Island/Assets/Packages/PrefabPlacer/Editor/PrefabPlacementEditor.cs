using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabPlacement), true)]
public class PrefabPlacementEditor : Editor
{
    public PrefabPlacement PrefabPlacer { get { return target as PrefabPlacement; } }

    private UnityEditorInternal.ReorderableList _prefabList;

    public void OnEnable()
    {
        _prefabList = null;
    }

    public void OnDisable()
    {
        PrefabPlacer.PlacementMode = false;
    }

    public void OnSceneGUI()
    {
        if (PrefabPlacer.PlacementMode)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                PrefabPlacer.PlaceAtScreenPosition(Event.current.mousePosition);
                EditorUtility.SetDirty(target);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (_prefabList == null)
        {
            SetupPrefabList();
        }

        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Placement mode:", EditorStyles.boldLabel);
        GUILayout.Label(PrefabPlacer.PlacementMode ? "ACTIVE" : "Inactive", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        if (PrefabPlacer.PlacementMode)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(string.Format("Last Hit: {0}, {1}, {2}", PrefabPlacer.LastHit.x, PrefabPlacer.LastHit.y, PrefabPlacer.LastHit.z));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Separator();

        if (GUILayout.Button("Toggle Placement Mode"))
        {
            PrefabPlacer.TogglePlacementMode();
        }

        EditorGUILayout.Separator();
        EditorGUI.BeginChangeCheck();
        _prefabList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    void SetupPrefabList()
    {
        float vSpace = 2;
        float hSpace = 3;
        float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2.5f;

        _prefabList = new UnityEditorInternal.ReorderableList(serializedObject,
            serializedObject.FindProperty("Prefabs"),
            true, true, true, true);

        _prefabList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Prefabs");
            GUIContent offsetText = new GUIContent("Offset Y");
            GUIContent placeText = new GUIContent("Place?");

            var offsetTextDimensions = GUI.skin.label.CalcSize(offsetText);
            var placeTextDimensions = GUI.skin.label.CalcSize(placeText);

            rect.x += rect.width - (offsetTextDimensions.x + placeTextDimensions.x);
            rect.width = offsetTextDimensions.x;
            EditorGUI.LabelField(rect, offsetText);

            rect.x += offsetTextDimensions.x;
            rect.width = placeTextDimensions.x;
            EditorGUI.LabelField(rect, placeText);
        };
        _prefabList.drawElementCallback
            = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = _prefabList.serializedProperty.GetArrayElementAtIndex(index);

                SerializedProperty propertyPrefab = element.FindPropertyRelative("Prefab");
                SerializedProperty propertyPlace = element.FindPropertyRelative("Place");
                SerializedProperty propertyOffsetY = element.FindPropertyRelative("OffsetY");

                rect.y += vSpace;
                Vector2 pos = rect.position;
                rect.width -= floatFieldWidth + hSpace;
                rect.width -= floatFieldWidth + hSpace;
                rect.height = EditorGUIUtility.singleLineHeight;
                propertyPrefab.objectReferenceValue = EditorGUI.ObjectField(rect, GUIContent.none, propertyPrefab.objectReferenceValue, typeof(GameObject), false);

                pos.x += rect.width + hSpace; rect.position = pos;
                rect.width = floatFieldWidth;
                EditorGUI.PropertyField(rect, propertyOffsetY, GUIContent.none);

                pos.x += rect.width + hSpace; rect.position = pos;
                rect.width = floatFieldWidth;
                EditorGUI.PropertyField(rect, propertyPlace, GUIContent.none);
            };
        _prefabList.onAddCallback = (UnityEditorInternal.ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;

            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Prefab").objectReferenceValue = null;
            element.FindPropertyRelative("Place").boolValue = false;
            element.FindPropertyRelative("OffsetY").floatValue = -0.15f;
        };
    }
}
