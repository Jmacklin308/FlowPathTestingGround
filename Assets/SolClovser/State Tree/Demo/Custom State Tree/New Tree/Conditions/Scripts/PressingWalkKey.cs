using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.SolClovser.StateTree;

[CreateAssetMenu(menuName = "Sol Clovser/State Tree/Conditions/PressingWalkKey")]
public class PressingWalkKey : BaseStateTreeCondition
{
    public override bool Decide(StateTreeRunner stateTreeRunner)
    {
        if (Input.GetKey(KeyCode.W))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ResetCondition()
    { 
        base.ResetCondition();
    }
}
