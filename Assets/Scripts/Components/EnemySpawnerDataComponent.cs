using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    public struct EnemySpawnerDataComponent : IComponentData
    {
        public Entity EnemyPrefab;
        public float SpawnRate;
        public float3 SpawnPoint;
        public float2  HalfArenaSize;
        public float Timer;
    }
}