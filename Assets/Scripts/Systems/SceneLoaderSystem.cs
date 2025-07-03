using Components;
using Unity.Entities;
using UnityEngine.SceneManagement;

public partial struct SceneLoaderSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (_, entity) in SystemAPI.Query<LoadGameSceneTag>()
            .WithEntityAccess())
        {
            SceneManager.LoadScene("GameScene");
            state.EntityManager.DestroyEntity(entity);
        }
        
        foreach (var (_, entity) in SystemAPI.Query<LoadMenuSceneTag>()
            .WithEntityAccess())
        {
            SceneManager.LoadScene("MainMenu");
            state.EntityManager.DestroyEntity(entity);
        }
    }
}