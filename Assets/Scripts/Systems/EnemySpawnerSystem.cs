using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class EnemySpawnerSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _commandBuffer;
        
        protected override void OnCreate()
        {
            RequireForUpdate<EnemySpawnerDataComponent>();
            _commandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = _commandBuffer.CreateCommandBuffer();
            float deltaTime = SystemAPI.Time.DeltaTime;
            Entities.ForEach((ref EnemySpawnerDataComponent spawnerData) =>
            {
                spawnerData.Timer += deltaTime;
                if (spawnerData.Timer < spawnerData.SpawnRate)
                    return;

                spawnerData.Timer = 0f;

                Entity enemy = commandBuffer.Instantiate(spawnerData.EnemyPrefab);
                commandBuffer.SetComponent(enemy, new LocalTransform
                {
                    Position = spawnerData.SpawnPoint,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                commandBuffer.AddComponent(enemy, new EnemyEnterData
                {
                    HalfArenaSize = spawnerData.HalfArenaSize,
                    HasEntered = false
                });
            }).Schedule();

            _commandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}