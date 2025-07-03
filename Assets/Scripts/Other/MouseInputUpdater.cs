using Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;

public class MouseInputUpdater : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity singleton;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = entityManager.CreateEntityQuery(typeof(MouseInputData));
        if (!query.IsEmpty)
        {
            singleton = query.GetSingletonEntity();
        }
        else
        {
            singleton = entityManager.CreateEntity();
            entityManager.AddComponentData(singleton, new MouseInputData { IsLeftMouseDown = false });
        }
    }

    void Update()
    {
        if (singleton == Entity.Null || !entityManager.Exists(singleton))
            return;

        if (!entityManager.HasComponent<MouseInputData>(singleton))
        {
            entityManager.AddComponentData(singleton, new MouseInputData { IsLeftMouseDown = false });
        }

        bool isPressed = Mouse.current != null && Mouse.current.leftButton.isPressed;

        entityManager.SetComponentData(singleton, new MouseInputData { IsLeftMouseDown = isPressed });
    }
}