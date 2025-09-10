using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public struct ProjectileTag : IComponentData {}
public struct TargetTag : IComponentData {}
public struct Health : IComponentData
{
    public int Value;
}

[BurstCompile]
public partial struct ProjectileHitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var sim = SystemAPI.GetSingleton<SimulationSingleton>();
        
        // Ждём завершения симуляции физики прежде чем читать TriggerEvents
        state.Dependency.Complete(); 

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // Перебираем все триггер-события за этот кадр
        foreach (var triggerEvent in sim.AsSimulation().TriggerEvents)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            bool aIsProjectile = state.EntityManager.HasComponent<ProjectileTag>(entityA);
            bool bIsProjectile = state.EntityManager.HasComponent<ProjectileTag>(entityB);

            // Projectile -> Target
            if (aIsProjectile && state.EntityManager.HasComponent<TargetTag>(entityB))
            {
                ApplyHit(ref state, ref ecb, entityA, entityB);
            }
            else if (bIsProjectile && state.EntityManager.HasComponent<TargetTag>(entityA))
            {
                ApplyHit(ref state, ref ecb, entityB, entityA);
            }
        }

        ecb.Playback(state.EntityManager);
    }

    private void ApplyHit(ref SystemState state, ref EntityCommandBuffer ecb, Entity projectile, Entity target)
    {
        // Удаляем прожектайл
        ecb.DestroyEntity(projectile);

        // Наносим урон (если есть Health)
        if (state.EntityManager.HasComponent<Health>(target))
        {
            var health = state.EntityManager.GetComponentData<Health>(target);
            health.Value -= 10; // например, -10 HP
            ecb.SetComponent(target, health);
        }
    }
}