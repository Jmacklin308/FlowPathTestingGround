
using UnityEngine;

namespace com.SolClovser.StateTree
{
    public class ReturnNode : BaseNode
    {
        [Space]
        public StateNode stateToReturn;

        public override void StateStart(StateTreeRunner stateTreeRunner, AnimationTransitionSettings animationTransitionSettings)
        {
            stateTreeRunner.TransitionToState(stateToReturn, animationTransitionSettings);
        }

        public override void StateUpdate(StateTreeRunner stateTreeRunner)
        {
            
        }

        public override void StateExit(StateTreeRunner stateTreeRunner)
        {

        }
    }
}

