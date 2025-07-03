using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TimeManagementSystem : SystemBase
    {
        private EntityQuery _timeScaleQuery;
    
        protected override void OnCreate()
        {
            if (!SystemAPI.HasSingleton<GlobalTimeScale>())
            {
                EntityManager.CreateEntity(typeof(GlobalTimeScale));
                SystemAPI.SetSingleton(new GlobalTimeScale { Value = 1f });
            }
        
            _timeScaleQuery = GetEntityQuery(typeof(GlobalTimeScale));
        }

        public void SetTimeScale(float scale)
        {
            var timeScale = SystemAPI.GetSingleton<GlobalTimeScale>();
            timeScale.Value = math.clamp(scale, 0f, 1f);
            SystemAPI.SetSingleton(timeScale);
        }

        protected override void OnUpdate() { }
    }
}