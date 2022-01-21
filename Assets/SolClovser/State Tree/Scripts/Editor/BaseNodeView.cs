
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.SolClovser.StateTree
{
    [System.Serializable]
    public class BaseNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public readonly BaseNode nodeThisViewRepresents;
        public Port inputPort;
        public Port outputPort;
        public Action<BaseNodeView> OnNodeSelected;

        /// <summary>
        /// Constructor, takes in a base node and represents it visually
        /// </summary>
        /// <param name="nodeToRepresent"></param>
        public BaseNodeView(BaseNode nodeToRepresent)
        {
            this.nodeThisViewRepresents = nodeToRepresent;
            this.title = nodeToRepresent.nodeTitle;
            this.viewDataKey = nodeToRepresent.guid;

            switch (nodeToRepresent)
            {
                case RootNode _:
                    SetupRootNode();
                    break;
                case StateNode _:
                    SetupStateNode();
                    break;
                case ReturnNode _:
                    SetupReturnNode();
                    break;
            }
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/GlobalNodeStyle.uss");
            styleSheets.Add(styleSheet); 
            
            style.left = nodeToRepresent.lastEditorPosition.x;
            style.top = nodeToRepresent.lastEditorPosition.y;
        }

        private void SetupRootNode()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/RootNodeStyleOverride.uss");
            styleSheets.Add(styleSheet); 
            
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);
        }
        
        private void SetupStateNode()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/StateNodeStyleOverride.uss");
            styleSheets.Add(styleSheet); 
            
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            outputContainer.Add(inputPort);
            
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);
        }
        
        private void SetupConditionNode()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/ConditionNodeStyleOverride.uss");
            styleSheets.Add(styleSheet); 
            
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            outputContainer.Add(inputPort);
            
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Condition Met";
            outputContainer.Add(outputPort);
        }
        
        private void SetupReturnNode()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SolClovser/State Tree/Scripts/Editor/Styles/ReturnNodeStyleOverride.uss");
            styleSheets.Add(styleSheet); 
            
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            outputContainer.Add(inputPort);
        }

        /// <summary>
        /// Records last position in editor to basenode fields
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            nodeThisViewRepresents.lastEditorPosition.x = newPos.xMin;
            nodeThisViewRepresents.lastEditorPosition.y = newPos.yMin;
            EditorUtility.SetDirty(nodeThisViewRepresents);
        } 
        
        /// <summary>
        /// On node selected
        /// </summary>
        public override void OnSelected() 
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        /// <summary>
        /// On node deselected
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();
            this.title = nodeThisViewRepresents.nodeTitle;
            nodeThisViewRepresents.SetAssetName();
        }
    }
}

