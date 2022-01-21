using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public EnemyManager manager;
    
    //current states
    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Retreat,
        Die
    }

    public State startingState = State.Idle;
    public State currentState;

    //for moving
    public Transform target;
    public float distanceToTarget;
    
    //enemy info
    public float totalHealth = 100;
    public bool isDead = false;
    
    //for Job system
    // public float moveX;
    // public float moveZ;
    
    void Awake()
    {
        //Phone home
        manager = GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>();
        
        //call home and add yourself to the list
        manager.listOfEnemies.Add(this);
        
        //default to idle if no input made
        currentState = startingState;
    }
}
