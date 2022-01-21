using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace _scripts.ECS_Scripts.Jobs
{
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {

        public float moveSpeed;
        public float deltaTime;
        public float3 playerPos;
        
        
        public void Execute(int index, TransformAccess transform)
        {
            throw new System.NotImplementedException();
        }
    }
}