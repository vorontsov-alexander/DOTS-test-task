using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Authoring
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public GameObject EnemyPrefab;
        public float SpawnRate = 3f;
        public Vector3 SpawnPoint;
        public float2 ArenaSize = new (12f, 13.5f);
    }
    
    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EnemySpawnerDataComponent()
            {
                EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnRate = authoring.SpawnRate,
                SpawnPoint = authoring.SpawnPoint,
                HalfArenaSize = authoring.ArenaSize 
            });
        }
    }
}