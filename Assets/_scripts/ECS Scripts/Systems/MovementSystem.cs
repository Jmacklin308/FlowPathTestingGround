using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _scripts.ECS_Scripts.Systems
{
    public class MovementSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            //move all entities with move forward component
            Entities.WithAll<EnemyMoveECS>().ForEach(
                (ref Translation trans, ref Rotation rot, ref EnemyMoveECS enemyMoveEcs) =>
                {
                    trans.Value += enemyMoveEcs.speed * Time.DeltaTime * math.forward(rot.Value);
                });
        }
    }
}