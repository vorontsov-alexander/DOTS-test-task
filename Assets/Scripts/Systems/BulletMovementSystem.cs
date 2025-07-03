using Components;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BulletMovementSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _commandBuffer;

        protected override void OnCreate()
        {
            _commandBuffer = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            EntityCommandBuffer.ParallelWriter commandBuffer = _commandBuffer.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithAll<BulletTag>()
                .ForEach((Entity entity, int entityInQueryIndex, ref LocalTransform transform, ref BulletData bullet) =>
                {
                    transform.Position += bullet.Direction * bullet.Speed * deltaTime;
                    bullet.Timer += deltaTime;

                    if (bullet.Timer > bullet.Lifetime)
                    {
                        commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            _commandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}