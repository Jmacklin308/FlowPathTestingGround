using System;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyUnit> listOfEnemies;
    public List<Player> listOfPlayers;
    public Transform player;

    void Awake()
    {
        player = listOfPlayers[0].GetComponent<Transform>();
    }

    private void Start()
    {
        player = listOfPlayers[0].GetComponent<Transform>();
        
        //set target to player
        foreach (var t in listOfEnemies)
        {
                //assign each enemy their target
                t.target = player; 
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Runs the job
        EnemyJob();
        
    }

    private void EnemyJob()
    {
        //store and array of each enemies transform to pass onto the job
        TransformAccessArray transformAccessArray = new TransformAccessArray(listOfEnemies.Count);
        
        //fill up the arrays with the data
        foreach (EnemyUnit t in listOfEnemies)
        {
            transformAccessArray.Add(t.transform);
        }
        
        
        //create our job. Pass in the data inside the native arrays above to the job
        EnemyParallelJobTransform enemyParallelJobTransform = new EnemyParallelJobTransform
        {
            //this is where we supply info from unity to the job (because it cant use main thread unity methods/fields)
            deltaTime = Time.deltaTime,
            speed = Random.Range(0.01f,0.13f),
            
            //point to the player
            playerPosition = player.position,

        };
        
        //schedule the job to be run
        JobHandle jobHandle = enemyParallelJobTransform.Schedule(transformAccessArray);
        
        
        //complete the job
        jobHandle.Complete();
        
        //dispose array
        transformAccessArray.Dispose();
    }


    
    [BurstCompile]
   public struct EnemyParallelJobTransform : IJobParallelForTransform
   {
       // public NativeArray<float> moveXArray;
       // public NativeArray<float> moveZArray;
       public float deltaTime;
       public Vector3 playerPosition;
       public float speed;
       
       public void Execute(int index, TransformAccess transform)
       {
           Vector3 position = transform.position;
           
           Vector3 direction = playerPosition - position;

           //rotate to player
           float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
           Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
           transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, deltaTime * 5.0f);

           
           Vector3 adjBugPosition = new Vector3(position.x, position.y, position.z);
           Vector3 adjPlayerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);

           //Move towards player 
           position = Vector3.Lerp(adjBugPosition,adjPlayerPosition,speed * deltaTime);
           transform.position = position;
       }
   }
}
