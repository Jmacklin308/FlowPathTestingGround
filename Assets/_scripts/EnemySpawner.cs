using System;
using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    [Range(0,4000)]
    public int totalEnemiesToSpawn;
    [Range(0,400)]
    public int spawnWidth;
    [Range(0,400)]
    public int spawnLength;
    public GameObject enemy;

    private Entity enemyEntityPrefab;

    private void SpawnEnemies()
    {
        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            float randx = transform.position.x + Random.Range(-spawnWidth, spawnWidth);
            float randz = transform.position.z + Random.Range(-spawnWidth, spawnWidth);
            Vector3 randPos = new Vector3(randx, 1, randz);
            
            Instantiate(enemy, randPos, quaternion.identity);
        }
    }


    private void Awake()
    {
        SpawnEnemies();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position,new Vector3(spawnWidth * 2,2, spawnLength * 2));
    }
}
