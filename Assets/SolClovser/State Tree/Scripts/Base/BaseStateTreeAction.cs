using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.SolClovser.StateTree
{
    public abstract class BaseStateTreeBehaviour : ScriptableObject
    {
        public abstract void StateStart(StateTreeRunner stateTreeRunner);
        public abstract void StateUpdate(StateTreeRunner stateTreeRunner);
        public abstract void StateExit(StateTreeRunner stateTreeRunner);
    } 
}

