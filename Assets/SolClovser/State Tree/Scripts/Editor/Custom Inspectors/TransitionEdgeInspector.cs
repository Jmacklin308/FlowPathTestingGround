
using System.IO;
using UnityEditor;
using UnityEngine;
using com.SolClovser.StateTree;

[CustomEditor(typeof(TransitionEdge))]
public class TransitionEdgeInspector : Editor
{
    private GUIStyle _style;

    private static TransitionEdge _transitionEdge;

    private bool _showTextField;
    private string _className;

    public override void OnInspectorGUI()
    {
        _transitionEdge = (TransitionEdge) target;
        
        _style = new GUIStyle(EditorStyles.label);
        _style.fontSize = 14; 

        EditorGUILayout.LabelField(_transitionEdge.from.nodeTitle + " >> " + _transitionEdge.to.nodeTitle, _style);
        EditorGUILayout.Space();
        
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        if (_transitionEdge.condition)
        {
            if (GUILayout.Button("Select Condition"))
            {
                AssetDatabase.OpenAsset(_transitionEdge.condition);
            }
        }
        else
        {
            if (GUILayout.Button("Add Condition"))
            {
                _showTextField = true;
            }
        }

        if (_showTextField)
        { 
            EditorGUILayout.HelpBox("Enter the new class name", MessageType.None);
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

        if (!Directory.Exists("Assets/Custom State Tree/" + _transitionEdge.to.treeAssetTitle + "/Conditions/Scripts"))
        {
            // Directory.CreateDirectory("Assets/Custom State Tree/" + _transitionEdge.to.treeAssetTitle);
            // Directory.CreateDirectory("Assets/Custom State Tree/" + _transitionEdge.to.treeAssetTitle + "/Conditions");
            Directory.CreateDirectory("Assets/Custom State Tree/" + _transitionEdge.to.treeAssetTitle + "/Conditions/Scripts");
        }
        
        string copyPath = "Assets/Custom State Tree/" + _transitionEdge.to.treeAssetTitle + "/Conditions/Scripts/" + className + ".cs";
        
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
                streamWriter.WriteLine("[CreateAssetMenu(menuName = \"Sol Clovser/State Tree/Conditions/" + className + "\")]");
                streamWriter.WriteLine("public class " + className + " : BaseStateTreeCondition");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("    public override bool Decide(StateTreeRunner stateTreeRunner)");
                streamWriter.WriteLine("    { ");
                streamWriter.WriteLine("        throw new System.NotImplementedException();"); 
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("    public override void ResetCondition()");
                streamWriter.WriteLine("    { ");
                streamWriter.WriteLine("        base.ResetCondition();"); 
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("}");
            }
        }
        AssetDatabase.Refresh();
    }
        
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void CreateAssetWhenReady()
    {
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || _transitionEdge == null)
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

            var condition = ScriptableObject.CreateInstance(s);

            if (condition == null)
            {
                return;
            }
            
            AssetDatabase.CreateAsset(condition, "Assets/Custom State Tree/" + _transitionEdge.to.treeAssetTitle + "/Conditions/" + s + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _transitionEdge.condition = condition as BaseStateTreeCondition;
        }
    }
    
    #endregion
}
