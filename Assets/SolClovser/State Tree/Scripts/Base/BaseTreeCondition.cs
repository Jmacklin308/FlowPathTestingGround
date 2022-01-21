
using UnityEngine;

namespace com.SolClovser.StateTree
{
    public abstract class BaseStateTreeCondition : ScriptableObject
    {
        public abstract bool Decide(StateTreeRunner stateTreeRunner);

        public virtual void ResetCondition()
        {
        
        }
    }
}

