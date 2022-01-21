
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using com.SolClovser.StateTree;

public class StateTreeEditor : EditorWindow
{
    private StateTreeView _stateTreeView;
    private InspectorView _inspectorView;
    private StateTreeAsset _stateTreeAsset;
    private Label _treeNameLabel;
    private Button _startNewTreeButton;
    private TextField _enterNewTreeNameField;

    private static Label _statusLabel;
    public static string EditorStatus
    {
        get => _statusLabel.text;
        set => _statusLabel.text = value;
    }

    [MenuItem("Tools/Sol Clovser/State Tree Editor/Editor Window")]
    public static void ShowExample()
    {
        StateTreeEditor wnd = GetWindow<StateTreeEditor>();
        wnd.titleContent = new GUIContent("State Tree Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/SolClovser/State Tree/Scripts/Editor/StateTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/StateTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        _stateTreeView = root.Q<StateTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _treeNameLabel = root.Query<Label>("treeNameLabel");
        _startNewTreeButton = root.Query<Button>("startNewTreeButton");
        _enterNewTreeNameField = root.Query<TextField>("enterNewTreeNameField");
        
        _statusLabel = root.Query<Label>("statusLabel");
        

        _stateTreeView.OnNodeSelected = OnNodeSelectionChanged;
        _stateTreeView.OnEdgeSelected = OnEdgeSelectionChanged;
        
        _startNewTreeButton.clicked += StartNewTreeButtonOnClicked;
        _treeNameLabel.RegisterCallback<PointerDownEvent>(TreeNameLabelClicked);
        _enterNewTreeNameField.RegisterCallback<KeyDownEvent>(EnterNewTreeNameFieldKeyDown);
      

        EditorApplication.playModeStateChanged += PlayButtonPressed;

        OnSelectionChange();
    }
    
    private void OnSelectionChange()
    {
        if (Selection.activeGameObject)
        {
            StateTreeRunner stateTreeRunner = Selection.activeGameObject.GetComponent<StateTreeRunner>();
            if (stateTreeRunner != null && stateTreeRunner.stateTreeAsset != null)
            {
                StateTreeAsset treeAsset = stateTreeRunner.stateTreeAsset;
                _startNewTreeButton.visible = false;
                
                // In further Unity versions use:
                // AssetDatabase.CanOpenAssetInEditor(_stateTreeAsset.GetInstanceID()
                // Instead of AssetDatabase.CanOpenForEdit(_stateTreeAsset)
                
                if(AssetDatabase.CanOpenForEdit(treeAsset))
                {
                    _stateTreeView.PopulateView(treeAsset);
                    _stateTreeAsset = treeAsset;
                    _treeNameLabel.text = treeAsset.name + " State Tree";
                }
            }
            else
            {
                _stateTreeView.ClearGraph();
                _treeNameLabel.text = "";
                _startNewTreeButton.visible = true;
            }
        }
        else
        {
            StateTreeAsset treeAsset = Selection.activeObject as StateTreeAsset;
            
            // In further Unity versions use:
            // AssetDatabase.CanOpenAssetInEditor(_stateTreeAsset.GetInstanceID()
            // Instead of AssetDatabase.CanOpenForEdit(_stateTreeAsset)
            
            if (treeAsset && AssetDatabase.CanOpenForEdit(treeAsset))
            {
                _stateTreeView.PopulateView(treeAsset);
                _stateTreeAsset = treeAsset;
                _treeNameLabel.text = treeAsset.name + " State Tree";
            }
        }
    }
    
    private void OnNodeSelectionChanged(BaseNodeView node)
    {
        _inspectorView.UpdateSelection(node);
    }

    private void OnEdgeSelectionChanged(EdgeView edge)
    {
        _inspectorView.UpdateEdgeSelection(edge);
    }
    
    private void Update()
    {
        // In further Unity versions use:
        // AssetDatabase.CanOpenAssetInEditor(_stateTreeAsset.GetInstanceID()
        // Instead of AssetDatabase.CanOpenForEdit(_stateTreeAsset)
        
        if (_stateTreeAsset && _stateTreeView != null && AssetDatabase.CanOpenForEdit(_stateTreeAsset))
        {
            _stateTreeView.UpdateEditor();
        }
    }

    private void PlayButtonPressed(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            _stateTreeView.SetAllStatesToInactive(false);
        }
    }
    
    #region Public Methods

    public static void ResetStatus()
    {
        EditorStatus = "All clear. Enjoy your day.";
    }
    #endregion
    
    #region Element Events
    
    private void StartNewTreeButtonOnClicked()
    {
        if (Selection.activeGameObject == null)
        {
            EditorStatus = "Nothing is selected. Please select a game object to start.";
            return;
        }
        
        if (Selection.activeGameObject)
        {
            StateTreeRunner stateTreeRunner = Selection.activeGameObject.AddComponent<StateTreeRunner>();

            if (!Directory.Exists("Assets/Custom State Tree"))
            {
                Directory.CreateDirectory("Assets/Custom State Tree");
            }
            
            StateTreeAsset asset = ScriptableObject.CreateInstance<StateTreeAsset>();
                
            AssetDatabase.CreateAsset(asset, "Assets/Custom State Tree/New Tree.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            stateTreeRunner.stateTreeAsset = asset;
            _stateTreeAsset = asset;
            _stateTreeView._stateTreeAsset = asset;

            _startNewTreeButton.visible = false;

            _treeNameLabel.text = asset.name + " State Tree";
        }
    }

    private void TreeNameLabelClicked(PointerDownEvent evt)
    {
        if (_stateTreeAsset == null) return;
        
        _enterNewTreeNameField.visible = true;
        _enterNewTreeNameField.value = _stateTreeAsset.name;
        _enterNewTreeNameField.Focus();
    }
    
    private void EnterNewTreeNameFieldKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Escape)
        {
            _enterNewTreeNameField.visible = false;
        }
        else if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            string path = AssetDatabase.GetAssetPath(_stateTreeAsset);
            AssetDatabase.RenameAsset(path, _enterNewTreeNameField.value);

            AssetDatabase.SaveAssets();
            _treeNameLabel.text = _enterNewTreeNameField.value + " State Tree";

            _enterNewTreeNameField.visible = false;
        }
    }
    #endregion
}