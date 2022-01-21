
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.SolClovser.StateTree
{
    [CreateAssetMenu(menuName = "Sol Clovser/State Tree/New State Tree", order = -1)]
    public class StateTreeAsset : ScriptableObject
    {
        public BaseNode rootNode;
        public List<BaseNode> nodes = new List<BaseNode>();
        
        #if UNITY_EDITOR
        
        /// <summary>
        /// Create scriptable object asset for the node
        /// </summary>
        /// <returns></returns>
        public BaseNode CreateNode(Type type)
        {
            BaseNode newNodeAsset = CreateInstance(type) as BaseNode;

            string givenNodeName = GiveNodeAName(newNodeAsset, type);

            newNodeAsset.treeAssetTitle = this.name;
            newNodeAsset.nodeTitle = givenNodeName;
            newNodeAsset.name = this.name + "." + givenNodeName;

            newNodeAsset.guid = GUID.Generate().ToString();

            nodes.Add(newNodeAsset);
        
            AssetDatabase.AddObjectToAsset(newNodeAsset, this);
            AssetDatabase.SaveAssets();

            return newNodeAsset;
        }

        /// <summary>
        /// Create a transition edge
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <returns></returns>
        public TransitionEdge CreateEdge(BaseNode fromNode, BaseNode toNode)
        {
            TransitionEdge transitionEdge = CreateInstance<TransitionEdge>();

            transitionEdge.name = "transition";
            transitionEdge.guid = GUID.Generate().ToString();

            transitionEdge.from = fromNode;
            transitionEdge.to = toNode;

            AssetDatabase.AddObjectToAsset(transitionEdge, this);
            AssetDatabase.SaveAssets();

            return transitionEdge;
        }
        
        /// <summary>
        /// Delete scriptable object asset of this node
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteNode(BaseNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            AssetDatabase.RemoveObjectFromAsset(nodeToDelete);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Remove child from parents connected nodes list
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveTransition(BaseNode parent, BaseNode child)
        {
            for (int i = 0; i < parent.transitions.Count; i++)
            {
                if (parent.transitions[i].to == child)
                {
                    AssetDatabase.RemoveObjectFromAsset(parent.transitions[i]);
                    parent.transitions.Remove(parent.transitions[i]);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        /// <summary>
        /// Set node's initial name
        /// </summary>
        /// <param name="nodeToEvaluate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GiveNodeAName(BaseNode nodeToEvaluate, System.Type type)
        {
            RootNode tempRootNode = nodeToEvaluate as RootNode;
            StateNode tempStateNode = nodeToEvaluate as StateNode;
            ReturnNode tempReturnNode = nodeToEvaluate as ReturnNode;

            if (tempStateNode != null)
            {
                return "New State Node";
            }
            
            if (tempRootNode != null)
            {
                return "Root Node";
            }

            if (tempReturnNode != null)
            {
                return "Return Node";
            }

            return "New Empty Node";
        }
        #endif
    }
}
