using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class JobBasedEnemy : MonoBehaviour
{
    [SerializeField] private bool turnOffDirectionGizmo;

    [SerializeField] private bool useJobs;
    [SerializeField] public int totalUnitsToTest;

    [SerializeField] private int spawnWidth;
    [SerializeField] private int spawnLength;

    //create a group of bugs
    [SerializeField] private Transform pfBug;
    private List<Bug> BugList;

    public class Bug
    {
        public Transform transform;
    }

    private void Start()
    {
        Vector3 spawnCenter = transform.position;

        //create our bug list with random spawn location (modified with spawn width/length)
        BugList = new List<Bug>();
        for (int i = 0; i < totalUnitsToTest; i++)
        {
            Transform bugTransform = Instantiate(pfBug,
                new Vector3(UnityEngine.Random.Range(spawnCenter.x - spawnWidth, spawnCenter.x + spawnWidth),
                    transform.position.y,
                    UnityEngine.Random.Range(spawnCenter.z - spawnLength, spawnCenter.z + spawnLength)),
                quaternion.identity);

            //add new instance to list
            BugList.Add(new Bug
            {
                transform = bugTransform,
            });
        }
    }

    private void FixedUpdate()
    {
        //create our arrays to use
        TransformAccessArray transformAccessArray = new TransformAccessArray(BugList.Count);

        //fill up our arrays with the current data
        for (int i = 0; i < BugList.Count; i++)
        {
            transformAccessArray.Add(BugList[i].transform);
        }

        //create transform job - Pass in the data to the job
        BugParallelJobTransform bugParallelJobTransform = new BugParallelJobTransform
        {
            deltaTime = Time.deltaTime,
            playerPosition = GameObject.FindGameObjectWithTag("ThisPlayer").transform.position,
            speed = UnityEngine.Random.Range(0.01f, 0.13f)
        };

        //schedule the transform job
        JobHandle transformHandle = bugParallelJobTransform.Schedule(transformAccessArray);

        //run the job
        transformHandle.Complete();

        //dispose of arrays
        transformAccessArray.Dispose();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth * 2, 2, spawnLength * 2));
    }

    private void ReallyToughTask()
    {
        //this simulates something tough like pathfinding
        float value = 0.0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}

[BurstCompile]
public struct BugParallelJobTransform : IJobParallelForTransform
{
    //Code Monkey 
    public float deltaTime;
    public Vector3 playerPosition;

    //random generation
    public float speed;

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 direction = playerPosition - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        //show direction
        //Debug.DrawRay(transform.position,direction,Color.green);
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, deltaTime * 5f);

        Vector3 adjBugPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 adjPlayerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);

        //Move towards player :):):):):)
        transform.position = Vector3.Lerp(adjBugPosition, adjPlayerPosition, speed * deltaTime);
    }
}