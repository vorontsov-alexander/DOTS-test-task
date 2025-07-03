using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public int Health = 10; // "Здоровье: выдерживает 10 ударов" ударов пуль или удар который ближний с двойным уроном?
        public float FireRate = 1f;
        public float Speed;
        public float AreaX;
        public float AreaZ;
    }
    
    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerDataComponent
            {
                Speed = authoring.Speed,
                AreaX = authoring.AreaX,
                AreaZ = authoring.AreaZ
            });
            AddComponent(entity, new ShootingData
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                FireRate = authoring.FireRate,
                Timer = 0f
            });
            AddComponent(entity, new PlayerHealthComponent
            {
                Health = authoring.Health
            });
            AddComponent<PlayerTag>(entity);
        }
    }
}
