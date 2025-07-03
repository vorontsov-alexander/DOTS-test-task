using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PlayerDamageSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerHealthComponent>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            EntityQuery damageQuery = GetEntityQuery(ComponentType.ReadWrite<DealDamageTag>());
            var dmgEntities = damageQuery.ToEntityArray(Allocator.TempJob);
            var dmgValues = damageQuery.ToComponentDataArray<DealDamageTag>(Allocator.TempJob);

            if (SystemAPI.TryGetSingletonRW<PlayerHealthComponent>(out var playerHealth))
            {
                for (int i = 0; i < dmgEntities.Length; i++)
                {
                    playerHealth.ValueRW.Health -= dmgValues[i].Amount;
                    commandBuffer.DestroyEntity(dmgEntities[i]);
                }
            }
            dmgEntities.Dispose();
            dmgValues.Dispose();

            var bullets = GetEntityQuery(ComponentType.ReadOnly<EnemyBulletTag>(),
                ComponentType.ReadOnly<LocalTransform>());
            var bulletEntities = bullets.ToEntityArray(Allocator.TempJob);
            var bulletTransforms = bullets.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

            var bulletDatas = GetComponentLookup<BulletData>(true);
            var playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerHealthComponent>(),
                ComponentType.ReadOnly<LocalTransform>());
            var player = playerQuery.GetSingletonEntity();
            var playerTransform = GetComponent<LocalTransform>(player);
            var playerHealthComponent = GetComponent<PlayerHealthComponent>(player);

            float bulletRadius = 0.3f;
            float2 playerPos = playerTransform.Position.xz;

            for (int i = 0; i < bulletEntities.Length; i++)
            {
                float2 bulletPos = bulletTransforms[i].Position.xz;
                if (math.distance(bulletPos, playerPos) < bulletRadius)
                {
                    int damage = bulletDatas[bulletEntities[i]].Damage;
                    playerHealthComponent.Health -= damage;
                    commandBuffer.DestroyEntity(bulletEntities[i]);
                    Debug.Log("Damaga 1");
                }
            }
            SetComponent(player, playerHealthComponent);

            bulletEntities.Dispose();
            bulletTransforms.Dispose();

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}