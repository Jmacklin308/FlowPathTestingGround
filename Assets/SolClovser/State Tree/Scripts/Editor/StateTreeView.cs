
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using com.SolClovser.StateTree;
using UnityEngine;
using Edge = UnityEditor.Experimental.GraphView.Edge;

public class StateTreeView : GraphView
{
    public new class UxmlFactory: UxmlFactory<StateTreeView, GraphView.UxmlTraits>{}

    private InspectorView _inspectorView;
    private List<BaseNodeView> _listOfNodeViews = new List<BaseNodeView>();

    private List<Edge> _emptyEdgeList = new List<Edge>();
    private Vector2 _mousePositionTrack;
    private bool _abort;
    
    public Action<BaseNodeView> OnNodeSelected;
    public Action<EdgeView> OnEdgeSelected;
    public StateTreeAsset _stateTreeAsset;

    public StateTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/StateTreeEditor.uss");
        styleSheets.Add(styleSheet);
        
        this.RegisterCallback<MouseMoveEvent>(OnMouseMove);
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        _mousePositionTrack = contentViewContainer.WorldToLocal(evt.mousePosition);
    }

    /// <summary>
    /// Recreate the whole tree
    /// </summary>
    /// <param name="tree"></param>
    public void PopulateView(StateTreeAsset tree)
    {
        _stateTreeAsset = tree;
        
        SetAllStatesToInactive(true);
    
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChanged;

        _stateTreeAsset.nodes.ForEach(n=> CreateNodeView(n));
        
        // loop transitions and connect edges
        for (int i = 0; i < _stateTreeAsset.nodes.Count; i++)
        {
            for (int j = 0; j < _stateTreeAsset.nodes[i].transitions.Count; j++)
            {
                BaseNodeView fromNode = FindNodeView(_stateTreeAsset.nodes[i].transitions[j].from);
                BaseNodeView toNode = FindNodeView(_stateTreeAsset.nodes[i].transitions[j].to);
                
                CreateEdgeView(_stateTreeAsset.nodes[i].transitions[j], fromNode.outputPort, toNode.inputPort);
            }
        }
    }

    /// <summary>
    /// Deletes all elements
    /// </summary>
    public void ClearGraph()
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
    }
    
    /// <summary>   
    /// Find the node view from its guid
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private BaseNodeView FindNodeView(BaseNode node)
    {
        return GetNodeByGuid(node.guid) as BaseNodeView;
    }

    #region Overrides to Unity Stuff

    /// <summary>
    /// Visualize the compatible ports when you try to drag edges
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="nodeAdapter"></param>
    /// <returns></returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => 
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    /// <summary>
    /// Called everytime something in graph has changed
    /// </summary>
    /// <param name="graphViewChange"></param>
    /// <returns></returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        _abort = false;
        
        // Iterate through elements that will get deleted by unity
        if (graphViewChange.elementsToRemove != null)
        {
            // Scan if root node is in to be removed list. if so abort
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                BaseNodeView nodeView = elem as BaseNodeView;
                if (nodeView != null)
                {
                    RootNode rootNode = nodeView.nodeThisViewRepresents as RootNode;
                    
                    if (rootNode != null)
                    {
                        _abort = true;
                    }
                }
            });

            if (_abort)
            {
                Debug.LogWarning("Can't remove root node!");
                graphViewChange.elementsToRemove.Clear();
                return graphViewChange;
            }

            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                BaseNodeView nodeView = elem as BaseNodeView;
                if (nodeView != null)
                {
                    _stateTreeAsset.DeleteNode(nodeView.nodeThisViewRepresents);
                }
                
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    BaseNodeView parentView = edge.output.node as BaseNodeView;
                    BaseNode parentNode = parentView.nodeThisViewRepresents;
                    
                    BaseNodeView childView = edge.input.node as BaseNodeView;
                    BaseNode childNode = childView.nodeThisViewRepresents;

                    _stateTreeAsset.RemoveTransition(parentNode, childNode);
                }
            });
        }
    
        // Iterate through edges that will get created by unity
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                CreateEdge(edge.output, edge.input);
            });
        }
    
        // Pass an empty list of edges because we already created them manually
        graphViewChange.edgesToCreate = _emptyEdgeList;
        
        return graphViewChange;
    }
    
    /// <summary>
    /// Build the menu when you right click
    /// </summary>
    /// <param name="evt"></param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction($"Create New State Node", (a) => CreateNode(typeof(StateNode)));
        evt.menu.AppendAction($"Create Return Node", (a) => CreateNode(typeof(ReturnNode)));

        // {
        //     var types = TypeCache.GetTypesDerivedFrom<BaseTreeCondition>();
        //     foreach (var type in types)
        //     {
        //         evt.menu.AppendAction($"Condition Nodes / {type.Name}", (a) => CreateNode(type));
        //     }
        // }
    }
    
    #endregion

    /// <summary>
    /// Create new node with its asset and view
    /// </summary>
    /// <param name="type"></param>
    private void CreateNode(Type type)
    {
        if (_stateTreeAsset.rootNode == null)
        {
            CreateRootNode();
        }
        
        BaseNode node = _stateTreeAsset.CreateNode(type);

        node.lastEditorPosition = _mousePositionTrack;

        CreateNodeView(node);
    }

    /// <summary>
    /// Create node view for given node
    /// </summary>
    /// <param name="nodeToRepresent"></param>
    private void CreateNodeView(BaseNode nodeToRepresent)
    {
        BaseNodeView nodeView = new BaseNodeView(nodeToRepresent);
        
        nodeView.OnNodeSelected = OnNodeSelected;
        
        AddElement(nodeView);
        _listOfNodeViews.Add(nodeView);
    }

    /// <summary>
    /// Create the root node
    /// </summary>
    private void CreateRootNode()
    {
        BaseNode node = _stateTreeAsset.CreateNode(typeof(RootNode));
        CreateNodeView(node);
        _stateTreeAsset.rootNode = node;
    }

    private void CreateEdge(Port output, Port input)
    {
        BaseNodeView fromNodeView = output.node as BaseNodeView;
        BaseNode fromNode = fromNodeView.nodeThisViewRepresents;
        
        BaseNodeView toNodeView = input.node as BaseNodeView;
        BaseNode toNode = toNodeView.nodeThisViewRepresents;
        
        TransitionEdge transitionEdge = _stateTreeAsset.CreateEdge(fromNode, toNode);

        fromNode.transitions.Add(transitionEdge);
        
        CreateEdgeView(transitionEdge, output, input);
    }
    
    private void CreateEdgeView(TransitionEdge transitionEdge, Port outputPort, Port inputPort)
    {
        EdgeView edgeView = new EdgeView(transitionEdge, outputPort, inputPort);
        edgeView.OnEdgeSelected = OnEdgeSelected;

        AddElement(edgeView);
    }

    /// <summary>
    /// Update the node styles
    /// </summary>
    public void UpdateEditor()
    {
        for (int i = 0; i < _listOfNodeViews.Count; i++)
        {
            if (_listOfNodeViews[i].nodeThisViewRepresents.isActive)
            {
                _listOfNodeViews[i].AddToClassList("active-node");
            }
            else
            {
                _listOfNodeViews[i].RemoveFromClassList("active-node");
            }
        }
    }
    
    /// <summary>
    /// Reset nodes to inactive
    /// </summary>
    public void SetAllStatesToInactive(bool skipDefaultState)
    {
        if (_stateTreeAsset == null) return;
        if (_stateTreeAsset.rootNode == null) return;
        
        for (int i = 0; i < _stateTreeAsset.nodes.Count; i++)
        {
            if (skipDefaultState)
            {
                if ( _stateTreeAsset.rootNode.transitions.Count > 0 && _stateTreeAsset.nodes[i] == _stateTreeAsset.rootNode.transitions[0].to)
                {
                    continue;
                }
            }
            _stateTreeAsset.nodes[i].isActive = false;
        }
    }
}
