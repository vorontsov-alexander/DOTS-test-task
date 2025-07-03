using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class BulletAuthoring : MonoBehaviour
    {
        public float Speed = 15f;
        public float Lifetime = 5f;
        public int Damage = 1;
    }

    public class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletData
            {
                Speed = authoring.Speed,
                Lifetime = authoring.Lifetime,
                Damage = 1,
                Direction = float3.zero
            });
            AddComponent(entity, new BulletTag());
        }
    }
}