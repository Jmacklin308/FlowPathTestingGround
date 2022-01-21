using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameObject player; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public static Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }
    
    
}
