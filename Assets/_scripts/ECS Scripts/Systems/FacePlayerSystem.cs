using _scripts.ECS_Scripts.ComponentTags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace _scripts.ECS_Scripts.Systems
{
    public class FacePlayerSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float3 playerPos;
            playerPos = (float3)GameManager.GetPlayerPosition();

            var jobHandle = Entities.WithAll<EnemyTag>().ForEach((Entity entity, ref Translation trans, ref Rotation rot) =>
            {
                // 5
                float3 direction = playerPos - trans.Value;
                direction.y = 0f;

                // 6
                rot.Value = quaternion.LookRotation(direction, math.up());
            }).Schedule(inputDeps);
            return jobHandle;
        }
    }
}