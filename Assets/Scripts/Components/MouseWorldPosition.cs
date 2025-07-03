using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MouseWorldPosition : IComponentData
    {
        public float3 Value;
    }
    
    public struct MouseInputData : IComponentData
    {
        public bool IsLeftMouseDown;
    }
}