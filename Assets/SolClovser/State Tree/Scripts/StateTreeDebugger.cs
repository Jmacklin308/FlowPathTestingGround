using System.Collections;
using System.Collections.Generic;
using com.SolClovser.StateTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateTreeDebugger : MonoBehaviour
{
    public RectTransform lastActionsPanel;
    public GameObject lastActionTextPrefab;

    [Tooltip("Fill this if you want to see states on screen")]
    public StateTreeRunner stateTreeRunner;

    private void Start()
    {
        if (stateTreeRunner == null)
        {
            Debug.LogWarning("Please drag and drop State Tree Runner to field if you want to see states on screen.");
            return;
        }

        stateTreeRunner.OnStateStart += OnStateReceived;
    }
    
    private void OnStateReceived(BaseNode state)
    {
        if (state is ReturnNode) return;
        
        GameObject tmP = Instantiate(lastActionTextPrefab, Vector3.zero, Quaternion.identity);
        tmP.transform.SetParent(lastActionsPanel);
        
        tmP.GetComponent<Text>().text = state.nodeTitle;
        // tmP.GetComponent<TextMeshProUGUI>().color = CurrentState.sceneGizmoColor;
        tmP.SetActive(true);
    }
}
