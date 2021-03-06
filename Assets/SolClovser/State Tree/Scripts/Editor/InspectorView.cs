
using com.SolClovser.StateTree;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory: UxmlFactory<InspectorView, VisualElement.UxmlTraits>{}

    private Editor _editor;
    
    public InspectorView()
    {
        
    }

    public void UpdateSelection(BaseNodeView nodeView)  
    {
       Clear(); 
       
       UnityEngine.Object.DestroyImmediate(_editor);
       _editor = Editor.CreateEditor(nodeView.nodeThisViewRepresents);

       IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
       
       Add(container);
    }
    
    public void UpdateEdgeSelection(EdgeView edgeView)  
    {
        Clear(); 
       
        UnityEngine.Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(edgeView.transitionEdgeThisViewRepresents);
        IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
        Add(container);
    }
}
