using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PlayerLookSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!SystemAPI.HasSingleton<MouseWorldPosition>()) return;

            float3 cursorPos = SystemAPI.GetSingleton<MouseWorldPosition>().Value;
            
            Entities
                .WithAll<PlayerTag>()
                .ForEach((ref LocalTransform transform) =>
                {
                    float3 direction = cursorPos - transform.Position;
                    direction.y = 0f;

                    if (math.lengthsq(direction) > 0.001f)
                    {
                        transform.Rotation = quaternion.LookRotationSafe(direction, math.up());
                    }
                }).Schedule();
        }
    }
}