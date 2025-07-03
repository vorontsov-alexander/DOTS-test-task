using Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    /*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        var world = World.DefaultGameObjectInjectionWorld; ;
        
        var mainMenuUI = new MenuUISystem();
        world.AddSystemManaged(mainMenuUI);
        
        var gameUI = new GameUISystem();
        world.AddSystemManaged(gameUI);

        // Добавляем системы в соответствующие группы
        
        world.GetExistingSystemManaged<PresentationSystemGroup>()
            .AddSystemToUpdateList(mainMenuUI);
        
        world.GetExistingSystemManaged<PresentationSystemGroup>()
            .AddSystemToUpdateList(gameUI);

        // Загружаем стартовую сцену
        SceneManager.LoadScene("MainMenu");
    }*/
}