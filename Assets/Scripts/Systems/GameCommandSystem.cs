using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class GameCommandSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();

            // Обработка запросов на смену сцены
            foreach (var _ in SystemAPI.Query<LoadGameSceneTag>())
            {
                SceneManager.LoadScene("GameScene");
                ecb.DestroyEntity(SystemAPI.GetSingletonEntity<LoadGameSceneTag>());
            }

            foreach (var _ in SystemAPI.Query<LoadMenuSceneTag>())
            {
                SceneManager.LoadScene("MainMenu");
                ecb.DestroyEntity(SystemAPI.GetSingletonEntity<LoadMenuSceneTag>());
            }

            foreach (var _ in SystemAPI.Query<RestartGameTag>())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                ecb.DestroyEntity(SystemAPI.GetSingletonEntity<RestartGameTag>());
            }

            foreach (var _ in SystemAPI.Query<QuitGameTag>())
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                ecb.DestroyEntity(SystemAPI.GetSingletonEntity<QuitGameTag>());
            }
            foreach (var request in SystemAPI.Query<RefRO<SetTimeScaleRequest>>())
            {
                SystemAPI.SetSingleton(new GlobalTimeScale { Value = request.ValueRO.Scale });
                ecb.DestroyEntity(SystemAPI.GetSingletonEntity<SetTimeScaleRequest>());
            }
        }
    }
}