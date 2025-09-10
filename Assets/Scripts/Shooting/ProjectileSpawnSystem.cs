using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ProjectileSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShootComp>();
    }

    public void OnUpdate(ref SystemState state)
    {
        SpawnerData spawnerData = default;
        foreach (var item in SystemAPI.Query<RefRO<SpawnerData>>())
        {
            spawnerData = item.ValueRO;
        }
        
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var squad in SystemAPI.Query<RefRO<TeamComp>, RefRO<ShootComp>>().WithEntityAccess())
        {
            foreach (var unit in SystemAPI.Query<RefRO<TeamComp>, RefRO<LocalTransform>, RefRO<UnitTargetPosition>>().WithEntityAccess())
            {
                if (squad.Item1.ValueRO.PlayerId != unit.Item1.ValueRO.PlayerId)
                {
                    continue;
                }
                
                var projectile = ecb.Instantiate(spawnerData.ProjectilePrefab);
                float3 startPos = unit.Item2.ValueRO.Position;
                ecb.SetComponent(projectile, new LocalTransform
                {
                    Position = startPos,
                    Rotation = unit.Item2.ValueRO.Rotation,
                    Scale = 1
                });

                ecb.AddComponent<Projectile>(projectile);
                ecb.AddComponent<ProjectileTag>(projectile);
                ecb.SetComponent(projectile, new Projectile
                {
                    Speed = 20f,
                    Direction = math.mul(unit.Item2.ValueRO.Rotation, math.forward())
                });
                
                ecb.AddComponent<Lifetime>(projectile);
                ecb.SetComponent(projectile, new Lifetime { Value = 2f });
            }

            ecb.RemoveComponent<ShootComp>(squad.Item3);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
