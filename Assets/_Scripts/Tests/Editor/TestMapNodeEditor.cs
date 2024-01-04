using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(TestMapNode))]
[CanEditMultipleObjects]
public class TestMapNodeEditor : Editor
{
    SerializedProperty up, right, down, left;

    void OnEnable()
    {
        up = serializedObject.FindProperty("up");
        right = serializedObject.FindProperty("right");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TestMapNode className = (TestMapNode)target;

        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        EditorGUILayout.PrefixLabel("Directions");
        EditorGUI.indentLevel--;
        //EditorGUILayout.PropertyField(right);
        className.up = EditorGUILayout.ToggleLeft("↑", className.up, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        className.right =  EditorGUILayout.ToggleLeft("→", className.right, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        className.down = EditorGUILayout.ToggleLeft("↓", className.down, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        className.left = EditorGUILayout.ToggleLeft("←", className.left, GUILayout.Width(50), GUILayout.ExpandWidth(false));
        //serializedObject.positionZ = EditorGUILayout.ToggleLeft("Down", serializedObject.positionZ, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        //serializedObject.positionZ = EditorGUILayout.ToggleLeft("Left", serializedObject.positionZ, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
