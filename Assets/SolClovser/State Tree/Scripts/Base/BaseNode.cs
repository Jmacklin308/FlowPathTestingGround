
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.SolClovser.StateTree
{
    public abstract class BaseNode : ScriptableObject
    {
        [HideInInspector]
        public string guid;
        public string nodeTitle;
        [HideInInspector] 
        public string treeAssetTitle;
        [HideInInspector]
        public bool isActive;
        [HideInInspector]
        public Vector2 lastEditorPosition;
        [HideInInspector]
        public List<TransitionEdge> transitions = new List<TransitionEdge>();
        
        #if UNITY_EDITOR
        public void SetAssetName()
        {
            this.name = treeAssetTitle + "_" + nodeTitle;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        #endif
        
        public abstract void StateStart(StateTreeRunner stateTreeRunner, AnimationTransitionSettings animationTransitionSettings);
        public abstract void StateUpdate(StateTreeRunner stateTreeRunner);
        public abstract void StateExit(StateTreeRunner stateTreeRunner);
    }
}

