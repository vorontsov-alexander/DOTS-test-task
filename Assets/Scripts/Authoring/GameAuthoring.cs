using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class GameAuthoring : MonoBehaviour
    {
        [Header("Player")]
        public GameObject PlayerPrefab;
        public Vector3 PlayerSpawnPosition;
        public Vector3 Rotation;
        public float Scale = 1f;
    }

    public class GameBaker : Baker<GameAuthoring>
    {
        public override void Bake(GameAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameDataComponent(GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic), authoring.PlayerSpawnPosition, authoring.Rotation, authoring.Scale));
        }
    }
}