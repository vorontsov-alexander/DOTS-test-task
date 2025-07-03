using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Components
{
    public struct GameDataComponent : IComponentData
    {
        public Entity PlayerPrefab;
        public LocalTransform PlayerSpawnTransform;
        
        public GameDataComponent(Entity playerPrefab, Vector3 position, Vector3 rotation, float scale)
        {
            PlayerPrefab = playerPrefab;
            PlayerSpawnTransform = new LocalTransform
            {
                Position = position,
                Rotation = quaternion.EulerXYZ(rotation),
                Scale = scale
            };
        }
    }
}