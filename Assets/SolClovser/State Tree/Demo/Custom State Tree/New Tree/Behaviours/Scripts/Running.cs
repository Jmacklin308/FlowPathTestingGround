using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.SolClovser.StateTree;

[CreateAssetMenu(menuName = "Sol Clovser/State Tree/Behaviours/Running")]
public class Running : BaseStateTreeBehaviour
{
    private PlayerController _playerController;
    
    // This will work when the state starts
    public override void StateStart(StateTreeRunner stateTreeRunner)
    { 
        _playerController = stateTreeRunner.entityController as PlayerController;
    }

    // This will work every frame while state is active
    public override void StateUpdate(StateTreeRunner stateTreeRunner)
    {
        Vector3 oldPosition = _playerController.transform.position;
        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y,
            oldPosition.z + _playerController.movementSpeed * Time.deltaTime);

        _playerController.transform.position = newPosition;
    }

    // This will work on state exit
    public override void StateExit(StateTreeRunner stateTreeRunner)
    { 
        
    }
}
