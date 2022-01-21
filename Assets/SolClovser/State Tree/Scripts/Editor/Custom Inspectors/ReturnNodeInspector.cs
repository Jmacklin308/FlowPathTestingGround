using System.Collections;
using System.Collections.Generic;
using com.SolClovser.StateTree;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReturnNode))]
public class ReturnNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // EditorGUILayout.HelpBox("Returns to the state you select.", MessageType.Info, true);
        DrawDefaultInspector();
       
    }
}
