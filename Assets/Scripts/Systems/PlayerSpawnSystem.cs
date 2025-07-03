using Components;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PlayerSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<GameDataComponent>();
    }

    protected override void OnStartRunning()
    {
        GameDataComponent gameDataComponent = SystemAPI.GetSingleton<GameDataComponent>();
        Entity player = EntityManager.Instantiate(gameDataComponent.PlayerPrefab);
        EntityManager.SetComponentData(player, gameDataComponent.PlayerSpawnTransform);
    }
    
    protected override void OnUpdate()
    {
       
    }
}
