#if UNITY_EDITOR
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

public partial class SphereGizmoDrawSystem : SystemBase
{
    private EntityQuery _query;

    protected override void OnCreate()
    {
        // Создаем запрос один раз при инициализации системы
        _query = GetEntityQuery(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<DebugSphere>());
        
        // Отключаем автоматическое выполнение системы
        Enabled = false;
    }

    protected override void OnUpdate() { }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NonSelected)]
    private static void DrawGizmosForEntities(Transform transform, GizmoType gizmoType)
    {
        var system = World.DefaultGameObjectInjectionWorld?
            .GetExistingSystemManaged<SphereGizmoDrawSystem>();
        
        if (system == null || !Application.isPlaying) return;

        using (var entities = system._query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var localToWorld = system.EntityManager.GetComponentData<LocalToWorld>(entity);
                var sphere = system.EntityManager.GetComponentData<DebugSphere>(entity);
                
                Gizmos.color = sphere.Color;
                Gizmos.DrawWireSphere(localToWorld.Position, sphere.Radius);
            }
        }
    }
}
#endif