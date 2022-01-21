
using System.IO;
using com.SolClovser.StateTree;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateNode))]
public class StateNodeInspector : Editor
{
    private static StateNode _stateNode;

    private bool _showTextField;
    private string _className;

    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        
        _stateNode = (StateNode)target;

        if (_stateNode.Behaviour)
        {
            if (GUILayout.Button("Select Behaviour"))
            {
                AssetDatabase.OpenAsset(_stateNode.Behaviour);
            }
        }
        else
        {
            if (GUILayout.Button("Add Behaviour"))
            {
                _showTextField = true;
            }
        }
        
        if (_showTextField)
        { 
            EditorGUILayout.HelpBox("Enter the new class name", MessageType.None);
            StateTreeEditor.EditorStatus = "Awaiting class name...";
            GUI.SetNextControlName("classNameTextField"); 
            EditorGUI.FocusTextInControl("classNameTextField");

            _className = EditorGUILayout.TextField(_className); 

            if (Event.current.isKey)
            {
                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    _showTextField = false;
                    CreateScriptFile(_className);
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                {
                    _showTextField = false;
                    StateTreeEditor.EditorStatus = "Creation aborted.";
                }
            }
        }
    }

    #region Script And Asset Creation
    
    private void CreateScriptFile(string className)
    {
        char firstLetter = className[0];
        if (char.IsLetter(firstLetter) == false)
        {
            Debug.LogWarning("Class name needs to start with a letter");
            return;
        }
        
        className = className.Replace(" ", "");
        className = className.Replace(".", "_");
        
        EditorPrefs.SetString("HasNew", className);

        if (!Directory.Exists("Assets/Custom State Tree/" + _stateNode.treeAssetTitle + "/Custom Behaviours/Scripts"))
        {
            // Directory.CreateDirectory("Assets/Custom State Tree/" + _stateNode.treeAssetTitle);
            // Directory.CreateDirectory("Assets/Custom State Tree/" + _stateNode.treeAssetTitle + "/Behaviours");
            Directory.CreateDirectory("Assets/Custom State Tree/" + _stateNode.treeAssetTitle + "/Behaviours/Scripts");
        }
        
        string copyPath = "Assets/Custom State Tree/" + _stateNode.treeAssetTitle + "/Behaviours/Scripts/" + className + ".cs";
        
        if(File.Exists(copyPath) == false)
        {
            StreamWriter streamWriter;
            using (streamWriter = new StreamWriter(copyPath))
            {
                streamWriter.WriteLine("using UnityEngine;");
                streamWriter.WriteLine("using System.Collections;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using com.SolClovser.StateTree;");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("[CreateAssetMenu(menuName = \"Sol Clovser/State Tree/Behaviours/" + className + "\")]");
                streamWriter.WriteLine("public class " + className + " : BaseStateTreeBehaviour");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("    public override void StateStart(StateTreeRunner stateTreeRunner)");
                streamWriter.WriteLine("    { ");
                streamWriter.WriteLine("        throw new System.NotImplementedException();"); 
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("    public override void StateUpdate(StateTreeRunner stateTreeRunner)");
                streamWriter.WriteLine("    { ");
                streamWriter.WriteLine("        throw new System.NotImplementedException();"); 
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("    public override void StateExit(StateTreeRunner stateTreeRunner)");
                streamWriter.WriteLine("    { ");
                streamWriter.WriteLine("        throw new System.NotImplementedException();"); 
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("}");
            }
        }
        AssetDatabase.Refresh();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void CreateAssetWhenReady()
    {
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || _stateNode == null)
        {
            EditorApplication.delayCall += CreateAssetWhenReady;
            return;
        }
        
        EditorApplication.delayCall += CreateAssetNow;
    }

    private static void CreateAssetNow()
    {
        string s = EditorPrefs.GetString("HasNew");  

        if (s.Length > 0)
        {
            EditorPrefs.DeleteKey("HasNew");

            var action = ScriptableObject.CreateInstance(s);

            AssetDatabase.CreateAsset(action, "Assets/Custom State Tree/" + _stateNode.treeAssetTitle + "/Behaviours/" + s + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _stateNode.Behaviour = action as BaseStateTreeBehaviour;
        }
    }
    
    #endregion
}
