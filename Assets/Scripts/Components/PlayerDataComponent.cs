using Unity.Entities;

namespace Components
{
    public struct PlayerDataComponent : IComponentData
    {
        public float Speed;
        public float AreaX;
        public float AreaZ;
    }

    public struct PlayerHealthComponent : IComponentData
    {
        public int Health;
    }

    public struct UIHealthData : IComponentData
    {
        public int CurrentHealth;
        public int MaxHealth;
    }
    
    public struct PlayerTag : IComponentData {}
}