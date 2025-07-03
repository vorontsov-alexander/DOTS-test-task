using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PlayerMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            float2 input = float2.zero;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) input.y += 1;
                if (Keyboard.current.sKey.isPressed) input.y -= 1;
                if (Keyboard.current.aKey.isPressed) input.x -= 1;
                if (Keyboard.current.dKey.isPressed) input.x += 1;
            }

            if (math.lengthsq(input) > 0)
                input = math.normalize(input);
            
            new PlayerMovementJob
            {
                DeltaTime = deltaTime,
                Input = input
            }.ScheduleParallel(); 
        }
    }
    
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        public float DeltaTime;
        public float2 Input;

        private void Execute(ref LocalTransform transform, in PlayerDataComponent playerData, in PlayerTag tag)
        {
            float3 moveDir = new float3(Input.x, 0f, Input.y);
            float3 newPos = transform.Position + moveDir * playerData.Speed * DeltaTime;

            newPos.x = math.clamp(newPos.x, -playerData.AreaX, playerData.AreaX);
            newPos.z = math.clamp(newPos.z, -playerData.AreaZ, playerData.AreaZ);
        
            transform.Position = newPos;
        }
    }
}