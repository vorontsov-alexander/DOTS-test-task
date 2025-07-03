using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct BulletTag : IComponentData {}

    public struct PlayerBulletTag : IComponentData {}
    
    public struct EnemyBulletTag : IComponentData {}
    
    public struct DealDamageTag : IComponentData
    {
        public int Amount;
    }
    
    public struct BulletData : IComponentData
    {
        public float Speed;
        public float Lifetime;
        public float Timer;
        public int Damage;
        public float3 Direction;
    }

    public struct ShootingData : IComponentData
    {
        public Entity BulletPrefab;
        public float FireRate;
        public float Timer;
    }
}