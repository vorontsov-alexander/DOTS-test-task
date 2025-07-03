using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public float FireRate;
        public float MoveSpeed = 3f;
        public int Health = 1;
        public float AttackRange = 1.5f;
        public int AttackDamage = 1;
    }

    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring a)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new EnemyMoveSpeed { Value = a.MoveSpeed });
            AddComponent(entity, new EnemyHealth { Value = a.Health });
            AddComponent(entity, new EnemyAttackRange { Value = a.AttackRange });
            AddComponent(entity, new EnemyAttackDamage { Value = a.AttackDamage });
            AddComponent(entity, new EnemyShootingData
            {
                BulletPrefab = GetEntity(a.BulletPrefab, TransformUsageFlags.Dynamic),
                FireRate     = a.FireRate,
                Timer        = 0f
            });
        }
    }
}