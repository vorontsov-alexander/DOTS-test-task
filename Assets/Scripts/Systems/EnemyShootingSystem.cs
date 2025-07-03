using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class EnemyShootingSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _commandBuffer;

        protected override void OnCreate()
        {
            _commandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            RequireForUpdate<EnemyShootingData>();
        }

        protected override void OnUpdate()
        {
            var playerQuery = GetEntityQuery(
                ComponentType.ReadOnly<PlayerTag>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadWrite<PlayerHealthComponent>()
            );
            
            var playerEntities = playerQuery.ToEntityArray(Allocator.TempJob);
            var playerTransforms = playerQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var playerHealths = playerQuery.ToComponentDataArray<PlayerHealthComponent>(Allocator.TempJob);

            if (playerTransforms.Length == 0)
            {
                playerEntities.Dispose();
                playerTransforms.Dispose();
                playerHealths.Dispose();
                return;
            }

            float deltaTime = SystemAPI.Time.DeltaTime;
            EntityCommandBuffer.ParallelWriter commandBuffer = _commandBuffer.CreateCommandBuffer().AsParallelWriter();
            Entity bulletPrefab = SystemAPI.GetSingleton<ShootingData>().BulletPrefab;
            BulletData prefabData = EntityManager.GetComponentData<BulletData>(bulletPrefab);

            Entities
                .WithAll<EnemyTag>()
                .WithReadOnly(playerTransforms)
                .WithReadOnly(playerEntities)
                .WithReadOnly(playerHealths)
                .ForEach((Entity enemy, int entityInQueryIndex,
                    ref EnemyShootingData shootData,
                    in LocalTransform localTransform,
                    in EnemyEnterData enter,
                    in EnemyAttackRange attackRange,
                    in EnemyAttackDamage md,
                    in EnemyHealth enemyHealth) =>
                {
                    if (enemyHealth.Value <= 0 || !enter.HasEntered)
                        return;

                    int closestPlayerIndex = 0;
                    float3 closestPlayerPos = playerTransforms[0].Position;
                    float minDistance = math.distance(localTransform.Position, closestPlayerPos);

                    for (int i = 1; i < playerTransforms.Length; i++)
                    {
                        float distance = math.distance(localTransform.Position, playerTransforms[i].Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestPlayerIndex = i;
                            closestPlayerPos = playerTransforms[i].Position;
                        }
                    }

                    if (minDistance <= attackRange.Value)
                    {
                        PlayerHealthComponent health = playerHealths[closestPlayerIndex];
                        health.Health -= 2;
                        commandBuffer.SetComponent(entityInQueryIndex, playerEntities[closestPlayerIndex], health);
                        commandBuffer.DestroyEntity(entityInQueryIndex, enemy);
                        return;
                    }

                    shootData.Timer += deltaTime;
                    if (shootData.Timer < shootData.FireRate)
                        return;

                    shootData.Timer = 0f;
                    Entity bullet = commandBuffer.Instantiate(entityInQueryIndex, shootData.BulletPrefab);
                    float3 direction = math.normalize(closestPlayerPos - localTransform.Position);

                    commandBuffer.SetComponent(entityInQueryIndex, bullet, new LocalTransform
                    {
                        Position = localTransform.Position,
                        Rotation = quaternion.LookRotationSafe(direction, math.up()),
                        Scale = 1f
                    });

                    commandBuffer.SetComponent(entityInQueryIndex, bullet, new BulletData
                    {
                        Direction = direction,
                        Speed = prefabData.Speed,
                        Lifetime = prefabData.Lifetime,
                        Damage = 1,
                        Timer = 0f
                    });
                    commandBuffer.AddComponent<BulletTag>(entityInQueryIndex, bullet);
                    commandBuffer.AddComponent<EnemyBulletTag>(entityInQueryIndex, bullet);
                })
                .Schedule();
            
            playerEntities.Dispose(Dependency);
            playerTransforms.Dispose(Dependency);
            playerHealths.Dispose(Dependency);
            
            _commandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}