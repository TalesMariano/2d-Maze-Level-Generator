using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeNode))]
[CanEditMultipleObjects]
public class MazeNodeEditor : Editor
{
    SerializedProperty up, right, down, left;

    void OnEnable()
    {
        up = serializedObject.FindProperty("up");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(up);
        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        EditorGUILayout.PrefixLabel("Directions");
        EditorGUI.indentLevel--;
        EditorGUILayout.PropertyField(up);
        //up = EditorGUILayout.ToggleLeft("Up", serializedObject.up, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        // serializedObject.positionY = EditorGUILayout.ToggleLeft("Right", serializedObject.positionY, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        //serializedObject.positionZ = EditorGUILayout.ToggleLeft("Down", serializedObject.positionZ, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        //serializedObject.positionZ = EditorGUILayout.ToggleLeft("Left", serializedObject.positionZ, GUILayout.Width(28), GUILayout.ExpandWidth(false));
        EditorGUILayout.EndHorizontal();
    }
}
