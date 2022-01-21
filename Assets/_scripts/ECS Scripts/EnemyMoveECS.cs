using UnityEngine;
using Unity.Entities;


namespace _scripts.ECS_Scripts
{
    [GenerateAuthoringComponent]
    public struct EnemyMoveECS : IComponentData
    {
        //There is only data here related to enemy moving
        public float speed;

    }
}