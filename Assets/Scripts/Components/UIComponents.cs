using Unity.Entities;

namespace Components
{
    public struct LoadGameSceneTag : IComponentData {}
    public struct LoadMenuSceneTag : IComponentData {}
    public struct RestartGameTag : IComponentData {}
    public struct QuitGameTag : IComponentData {}
    public struct SetTimeScaleRequest : IComponentData { public float Scale; }
    public struct GlobalTimeScale : IComponentData
    {
        public float Value;
    }
}