
using UnityEngine;

namespace com.SolClovser.StateTree
{
    public class TransitionEdge : ScriptableObject
    {
        [HideInInspector]
        public string guid;

        [Space]
        public BaseStateTreeCondition condition;
        public ConditionsEquality conditionsEquality;
        [Space]
        
        [HideInInspector]
        public BaseNode from;
        [HideInInspector]
        public BaseNode to;
        
        [Space]
        public AnimationTransitionSettings animationTransitionSettings;
    }
}

