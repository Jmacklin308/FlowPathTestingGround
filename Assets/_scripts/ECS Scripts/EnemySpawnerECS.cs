using System;
using _scripts.ECS_Scripts.ComponentTags;
using NaughtyAttributes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Random = UnityEngine.Random;


namespace _scripts.ECS_Scripts
{
    public class EnemySpawnerECS : MonoBehaviour
    {
        [Header("Enemy ")]
        public GameObject prefab;

        private EntityManager entityManager;
        private EntityArchetype archetype;
        private Entity enemyEntityPrefab;

        [Header("Enemy Spawing: ")]
        [Range(0, 4000)] public int totalEnemiesToSpawn;
        [Range(0, 400)] public int spawnWidth;
        [Range(0, 400)] public int spawnLength;


        
        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [Button("Spawn wave")]
        private void SpawnWave()
        {
            
            NativeArray<Entity> enemyArray = new NativeArray<Entity>(totalEnemiesToSpawn, Allocator.Temp);
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);


            archetype = entityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(RenderMesh),
                typeof(RenderBounds),
                typeof(LocalToWorld),
                typeof(EnemyMoveECS),
                typeof(EnemyTag));
            
            
            //spawn our enemies
            for (int i = 0; i < enemyArray.Length; i++)
            {
                
                //convert to entity
                enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);

                enemyArray[i] = entityManager.Instantiate(enemyEntityPrefab);
                
                //Entity setup
                var position = transform.position;
                float randx = position.x + Random.Range(-spawnWidth, spawnWidth);
                float randz = position.z + Random.Range(-spawnWidth, spawnWidth);

                entityManager.SetArchetype(enemyEntityPrefab,archetype);
                //add entity data
                entityManager.AddComponentData(enemyEntityPrefab, new Translation { Value = new float3(randx, 0.0f, randz) });
                entityManager.AddComponentData(enemyEntityPrefab,
                    new Rotation { Value = quaternion.EulerXYZ(new float3(0f, Random.Range(-45f,45f), 0f)) });
                entityManager.AddComponentData(enemyEntityPrefab, new EnemyMoveECS { speed = Random.Range(0.78f,1.60f)});
                entityManager.AddComponentData(enemyEntityPrefab, new EnemyTag());
            }
            
            //clean up
            enemyArray.Dispose();
        }
        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var position = transform.position;
            float x = spawnWidth * 2;
            float z = spawnLength * 2;
            
            Gizmos.DrawWireCube(position, new Vector3(x , 2, z));
        }
    }
}