using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;


[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class FormationClickSystem : SystemBase
{
    private Camera _mainCamera;

    protected override void OnCreate()
    {
        RequireForUpdate<FormationUnit>(); // чтобы система работала, только если есть юниты
    }

    protected override void OnStartRunning()
    {
        _mainCamera = Camera.main;
    }

    protected override void OnUpdate()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit, 1000f))
        {
            var pos = hit.point;
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

            var entityManager = EntityManager;

            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FormationMoveCommand
            {
                Target = pos,
                RowCount = 4
            });
        }
    }
}

