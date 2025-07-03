using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class BulletCollisionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        RequireForUpdate<WallScale>();
        RequireForUpdate<PlayerHealthComponent>();
        _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float bulletRadius = 0.3f;
        float entityRadius = 0.5f;

        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

        // Игрок
        var playerQuery = SystemAPI.QueryBuilder()
            .WithAll<PlayerTag, LocalTransform, PlayerHealthComponent>()
            .Build();
        var playerEntities = playerQuery.ToEntityArray(Allocator.TempJob);
        var playerTransforms = playerQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var playerHealths = playerQuery.ToComponentDataArray<PlayerHealthComponent>(Allocator.TempJob);

        if (playerEntities.Length == 0)
        {
            playerEntities.Dispose();
            playerTransforms.Dispose();
            playerHealths.Dispose();
            return;
        }

        // Враги
        var enemyQuery = SystemAPI.QueryBuilder()
            .WithAll<EnemyTag, LocalTransform>()
            .Build();
        var enemyEntities = enemyQuery.ToEntityArray(Allocator.TempJob);
        var enemyTransforms = enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        // Стены (общие данные, копируем дважды)
        var wallQuery = SystemAPI.QueryBuilder()
            .WithAll<WallTag, LocalTransform, WallScale>()
            .Build();
        var wallTransformsShared = wallQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var wallScalesShared = wallQuery.ToComponentDataArray<WallScale>(Allocator.TempJob);

        var wallTransformsForEnemyBullets = new NativeArray<LocalTransform>(wallTransformsShared, Allocator.TempJob);
        var wallScalesForEnemyBullets = new NativeArray<WallScale>(wallScalesShared, Allocator.TempJob);
        var wallTransformsForPlayerBullets = new NativeArray<LocalTransform>(wallTransformsShared, Allocator.TempJob);
        var wallScalesForPlayerBullets = new NativeArray<WallScale>(wallScalesShared, Allocator.TempJob);

        wallTransformsShared.Dispose();
        wallScalesShared.Dispose();

        // Вражеские пули → игрок
        Entities
            .WithAll<BulletTag, EnemyBulletTag>()
            .WithReadOnly(playerTransforms)
            .WithReadOnly(playerHealths)
            .WithReadOnly(playerEntities)
            .WithDisposeOnCompletion(playerTransforms)
            .WithDisposeOnCompletion(playerHealths)
            .WithDisposeOnCompletion(playerEntities)
            .WithDisposeOnCompletion(wallTransformsForEnemyBullets)
            .WithDisposeOnCompletion(wallScalesForEnemyBullets)
            .ForEach((Entity bullet, int entityInQueryIndex, in LocalTransform bulletTransform) =>
            {
                float2 bulletPos = bulletTransform.Position.xz;

                for (int i = 0; i < playerTransforms.Length; i++)
                {
                    float2 playerPos = playerTransforms[i].Position.xz;

                    if (math.distance(bulletPos, playerPos) < bulletRadius + entityRadius)
                    {
                        var health = playerHealths[i];
                        health.Health -= 1;
                        ecb.SetComponent(entityInQueryIndex, playerEntities[i], health);
                        ecb.DestroyEntity(entityInQueryIndex, bullet);
                        return;
                    }
                }

                for (int i = 0; i < wallTransformsForEnemyBullets.Length; i++)
                {
                    float2 center = wallTransformsForEnemyBullets[i].Position.xz;
                    float2 halfSize = new float2(
                        wallScalesForEnemyBullets[i].Value.z * 0.5f,
                        wallScalesForEnemyBullets[i].Value.x * 0.5f
                    );

                    float2 min = center - halfSize;
                    float2 max = center + halfSize;

                    float2 closest = math.clamp(bulletPos, min, max);

                    if (math.distancesq(bulletPos, closest) < bulletRadius * bulletRadius)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, bullet);
                        return;
                    }
                }
            }).Schedule();

        // Пули игрока → враги
        Entities
            .WithAll<BulletTag, PlayerBulletTag>()
            .WithReadOnly(enemyTransforms)
            .WithReadOnly(enemyEntities)
            .WithDisposeOnCompletion(enemyTransforms)
            .WithDisposeOnCompletion(enemyEntities)
            .WithDisposeOnCompletion(wallTransformsForPlayerBullets)
            .WithDisposeOnCompletion(wallScalesForPlayerBullets)
            .ForEach((Entity bullet, int entityInQueryIndex, in LocalTransform bulletTransform) =>
            {
                float2 bulletPos = bulletTransform.Position.xz;

                for (int i = 0; i < enemyTransforms.Length; i++)
                {
                    float2 enemyPos = enemyTransforms[i].Position.xz;

                    if (math.distance(bulletPos, enemyPos) < bulletRadius + entityRadius)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, bullet);
                        ecb.DestroyEntity(entityInQueryIndex, enemyEntities[i]);
                        return;
                    }
                }

                for (int i = 0; i < wallTransformsForPlayerBullets.Length; i++)
                {
                    float2 center = wallTransformsForPlayerBullets[i].Position.xz;
                    float2 halfSize = new float2(
                        wallScalesForPlayerBullets[i].Value.z * 0.5f,
                        wallScalesForPlayerBullets[i].Value.x * 0.5f
                    );

                    float2 min = center - halfSize;
                    float2 max = center + halfSize;

                    float2 closest = math.clamp(bulletPos, min, max);

                    if (math.distancesq(bulletPos, closest) < bulletRadius * bulletRadius)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, bullet);
                        return;
                    }
                }
            }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
