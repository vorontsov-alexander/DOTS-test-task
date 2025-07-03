using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class EnemyMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float3 playerPosition = float3.zero;
            bool found = false;
            Entities
                .WithAll<PlayerTag>()
                .ForEach((in LocalTransform playerTransform) =>
                {
                    playerPosition = playerTransform.Position;
                    found = true;
                })
                .Run();

            if (!found) return;

            EnemyMovementJob job = new EnemyMovementJob
            {
                PlayerPosition = playerPosition,
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            job.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct EnemyMovementJob : IJobEntity
    {
        public float3 PlayerPosition;
        public float  DeltaTime;

        private void Execute(
            ref LocalTransform    enemyTransform,
            ref EnemyEnterData    enterData,
            in  EnemyMoveSpeed    moveSpeed,
            in  EnemyAttackRange  attackRange,
            ref EnemyHealth       enemyHealth
        )
        {
            float3 pos = enemyTransform.Position;

            if (!enterData.HasEntered)
            {
                bool insideX = math.abs(pos.x) <= enterData.HalfArenaSize.x;
                bool insideZ = math.abs(pos.z) <= enterData.HalfArenaSize.y;

                if (!insideX || !insideZ)
                {
                    float3 target = new float3(
                        math.clamp(pos.x, -enterData.HalfArenaSize.x, enterData.HalfArenaSize.x),
                        0.5f, //offset 😭
                        math.clamp(pos.z, -enterData.HalfArenaSize.y, enterData.HalfArenaSize.y)
                    );
                    float3 direction = math.normalize(target - pos);
                    enemyTransform.Position += direction * moveSpeed.Value * DeltaTime;
                    enemyTransform.Rotation = quaternion.LookRotationSafe(direction, math.up());
                    return;
                }
                enterData.HasEntered = true;
            }

            float3 toPlayerDirection = PlayerPosition - pos;
            float  distance = math.length(toPlayerDirection);

            if (distance > attackRange.Value)
            {
                float3 direction = math.normalize(toPlayerDirection);
                enemyTransform.Position += direction * moveSpeed.Value * DeltaTime;
                enemyTransform.Rotation = quaternion.LookRotationSafe(direction, math.up());
                return;
            }
            enemyHealth.Value = 0;
        }
    }
}
