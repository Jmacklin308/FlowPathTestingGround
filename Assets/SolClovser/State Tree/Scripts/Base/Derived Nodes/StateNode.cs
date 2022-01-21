
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.SolClovser.StateTree
{
    public class StateNode : BaseNode
    {
        [Space]
        public BaseStateTreeBehaviour Behaviour;
        [Space]
        
        [Tooltip("You might want to reset some conditions.")]
        public List<BaseStateTreeCondition> resets = new List<BaseStateTreeCondition>();
        
        [Tooltip("Animation to transition")]
        public AnimationState animationState;

        public override void StateStart(StateTreeRunner stateTreeRunner, AnimationTransitionSettings animationTransitionSettings)
        {
            if (resets.Count > 0)
            {
                ResetConditions();
            }
            
            if(Behaviour) Behaviour.StateStart(stateTreeRunner);
            
            if(animationState.animationStateName.Length > 0)
            {
                Animator stateTreeAnimator = stateTreeRunner.stateTreeAnimator;
                SetAnimatorState(stateTreeAnimator, animationTransitionSettings);
            }
            
            base.isActive = true;
        }

        public override void StateUpdate(StateTreeRunner stateTreeRunner)
        {
            if(Behaviour) Behaviour.StateUpdate(stateTreeRunner);
            CheckTransitions(stateTreeRunner);
        }
        
        private void CheckTransitions(StateTreeRunner stateTreeRunner)
        {
            for (int i = 0; i < base.transitions.Count; i++)
            {
                bool decisionResult = base.transitions[i].condition.Decide(stateTreeRunner);
                    
                if (decisionResult == (base.transitions[i].conditionsEquality == ConditionsEquality.True))
                {
                    if (stateTreeRunner.CurrentState == base.transitions[i].to) return;

                    stateTreeRunner.TransitionToState(base.transitions[i].to, base.transitions[i].animationTransitionSettings);
                }
            }
        }

        private void ResetConditions()
        {
            for (int i = 0; i < resets.Count; i++)
            {
                resets[i].ResetCondition();
            }
        }
        
        private void SetAnimatorState(Animator stateTreeAnimator, AnimationTransitionSettings animationTransitionSettings)
        {
            stateTreeAnimator.CrossFadeInFixedTime(animationState.animationStateName, 
                animationTransitionSettings.fixedTransitionDuration,
                animationState.animationStateLayer, 
                animationTransitionSettings.fixedTimeOffset);
        }
        
        public override void StateExit(StateTreeRunner stateTreeRunner)
        {
            base.isActive = false;
            if(Behaviour) Behaviour.StateExit(stateTreeRunner);
        }
    } 
}

