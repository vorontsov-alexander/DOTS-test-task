using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct EnemyTag : IComponentData {} 
    
    public struct WallTag : IComponentData {}

    public struct EnemyShootingData : IComponentData
    {
        public Entity BulletPrefab;
        public float FireRate;
        public float Timer;
    
        public float BulletSpeed;
        public float BulletLifetime;
        public int BulletDamage;
    }
    
    public struct WallScale : IComponentData
    {
        public float3 Value;
    }
    
    public struct EnemyEnterData : IComponentData
    {
        public float2 HalfArenaSize;
        public bool  HasEntered;
    }
    
    public struct EnemyMoveSpeed : IComponentData
    {
        public float Value;
    }

    public struct EnemyHealth : IComponentData
    {
        public int Value;
    }

    public struct EnemyAttackRange : IComponentData
    {
        public float Value;
    }

    public struct EnemyAttackDamage : IComponentData
    {
        public int Value;
    }
}