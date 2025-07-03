using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PlayerShootingSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _commandBuffer;
        private bool _wasHeldLast;

        protected override void OnCreate()
        {
            RequireForUpdate<ShootingData>();
            _commandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.HasSingleton<MouseWorldPosition>() || !SystemAPI.HasSingleton<MouseInputData>())
                return;

            float3 target = SystemAPI.GetSingleton<MouseWorldPosition>().Value;
            target.y = 0.5f;
            float deltaTime = SystemAPI.Time.DeltaTime;

            EntityCommandBuffer commandBuffer = _commandBuffer.CreateCommandBuffer();
            Entity bulletPrefab = GetSingleton<ShootingData>().BulletPrefab;
            BulletData prefabData = EntityManager.GetComponentData<BulletData>(bulletPrefab);

            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref ShootingData shootingData, in LocalTransform transform) =>
                {
                    shootingData.Timer += deltaTime;

                    if (shootingData.Timer < shootingData.FireRate)
                        return;

                    shootingData.Timer = 0f;

                    Entity bullet = commandBuffer.Instantiate(shootingData.BulletPrefab);
                    float3 dir = math.normalize(target - transform.Position);

                    commandBuffer.SetComponent(bullet, new LocalTransform
                    {
                        Position = transform.Position,
                        Rotation = quaternion.LookRotationSafe(dir, math.up()),
                        Scale = 1f
                    });
                    commandBuffer.SetComponent(bullet, new BulletData
                    {
                        Speed = prefabData.Speed,
                        Lifetime = prefabData.Lifetime,
                        Damage = prefabData.Damage,
                        Timer = 0f,
                        Direction = dir
                    });
                    commandBuffer.AddComponent<PlayerBulletTag>(bullet);
                })
                .Run();

            _commandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}