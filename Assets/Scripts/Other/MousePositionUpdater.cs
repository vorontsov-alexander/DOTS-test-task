using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class MousePositionUpdater : MonoBehaviour
{
    public Camera MainCamera;
    private EntityManager _entityManager;
    private Entity _singleton;

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = _entityManager.CreateEntityQuery(typeof(MouseWorldPosition));
        if (!query.IsEmpty)
        {
            _singleton = query.GetSingletonEntity();
        }
        else
        {
            _singleton = _entityManager.CreateEntity(typeof(MouseWorldPosition));
            _entityManager.SetComponentData(_singleton, new MouseWorldPosition { Value = 0 });
        }
    }

    void Update()
    {
        if (MainCamera == null || Mouse.current == null) 
            return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = MainCamera.ScreenPointToRay(mouseScreen);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (!plane.Raycast(ray, out float enter)) 
            return;
        Vector3 worldPoint = ray.GetPoint(enter);
        _entityManager.SetComponentData(_singleton, new MouseWorldPosition
        {
            Value = new float3(worldPoint.x, 0f, worldPoint.z)
        });
    }
}