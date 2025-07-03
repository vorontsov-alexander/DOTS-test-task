using System;
using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class WallsAuthoring : MonoBehaviour
    {
        public Vector3 Scale;

        public void Awake()
        {
            Scale = transform.localScale;
        }
    }

    public class WallBaker : Baker<WallsAuthoring>
    {
        public override void Bake(WallsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<WallTag>(entity);
            AddComponent(entity, new WallScale
            {
                Value = authoring.Scale,
            });
        }
    }

}