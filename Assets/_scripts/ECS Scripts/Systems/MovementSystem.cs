using _scripts.ECS_Scripts.ComponentTags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace _scripts.ECS_Scripts.Systems
{
    public class MovementSystem : SystemBase
    {
        private EntityCommandBufferSystem commandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }


        protected override void OnUpdate()
        {
            //move all entities with move forward component
            Entities.ForEach(
            (ref Translation trans, ref Rotation rot, in EnemyMoveECS emove) =>
            {
                trans.Value += emove.speed * Time.DeltaTime * math.forward(rot.Value);
            }).ScheduleParallel();
        }
    }
}