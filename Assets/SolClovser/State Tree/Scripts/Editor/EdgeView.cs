using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace com.SolClovser.StateTree
{
    public class EdgeView : UnityEditor.Experimental.GraphView.Edge
    {
        public readonly TransitionEdge transitionEdgeThisViewRepresents;

        // public Port inputPort;
        // public Port outputPort;
    
        public Action<EdgeView> OnEdgeSelected;

        public EdgeView(TransitionEdge transitionEdge, Port outPort, Port inPort)
        {
            transitionEdgeThisViewRepresents = transitionEdge;
            this.viewDataKey = transitionEdgeThisViewRepresents.guid;

            // outputPort = outPort;
            // inputPort = inPort;
            
            Edge actualEdge = outPort.ConnectTo(inPort);
            // actualEdge.viewDataKey = transitionEdgeThisViewRepresents.guid;
            Add(actualEdge);

            actualEdge.RegisterCallback<MouseUpEvent>(HandleMouseClick);
        }
    
        private void HandleMouseClick(MouseUpEvent evt)
        {
            if (OnEdgeSelected != null)
            {
                OnEdgeSelected.Invoke(this);
            }
        }
    }
}

