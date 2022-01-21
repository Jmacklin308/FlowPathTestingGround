
using System;
using com.SolClovser.StateTree;
using UnityEngine;

public class StateTreeRunner : MonoBehaviour
{
    [Tooltip("The State Tree Asset that will run.")]
    public StateTreeAsset stateTreeAsset;
    [Tooltip("The animator component of the object you want to animate.")]
    public Animator stateTreeAnimator;
    public BaseNode CurrentState { get; private set; }
    public Action<BaseNode> OnStateStart;
    public Action<BaseNode> OnStateExit;
    
    [Tooltip("This can be any MonoBehaviour script you want.")]
    public MonoBehaviour entityController;

    private void Start()
    {
        // stateTree = stateTree.Clone();
        TransitionToState(stateTreeAsset.rootNode.transitions[0].to, new AnimationTransitionSettings());
    }

    private void Update()
    {
        CurrentState.StateUpdate(this);
    }
    
    public void TransitionToState(BaseNode nextState, AnimationTransitionSettings animationTransitionSettings)
    {
        if (CurrentState != null)
        {
            // Invoke the state exit event
            if(OnStateExit != null) OnStateExit.Invoke(CurrentState);
            
            // Actually exit the state
            CurrentState.StateExit(this);
        }

        // Set current state
        CurrentState = nextState;

        // Invoke state start event
        if(OnStateStart != null) OnStateStart.Invoke(CurrentState);
        
        // Start the new state
        CurrentState.StateStart(this, animationTransitionSettings);
    }
}
