using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct SpawnerSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SquadSpawnTag>();
        state.RequireForUpdate<SpawnerData>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity unitPrefab = default;
        foreach (var prefab in SystemAPI.Query<RefRO<SpawnerData>>())
        {
            unitPrefab = prefab.ValueRO.Prefab;
        }

        foreach (var squad in SystemAPI.Query<RefRO<SquadSpawnTag>, RefRO<SquadData>>().WithEntityAccess())
        {
            Debug.LogError($"Start spawn squad");

            for (int i = 0; i < squad.Item2.ValueRO.StartUnitsCount; i++)
            {
                var instance = ecb.Instantiate(unitPrefab);
                ecb.AddComponent<LocalTransform>(instance);
                ecb.SetComponent(instance, LocalTransform.FromPosition(float3.zero));
                ecb.AddComponent(instance, new FormationUnit() { Index = i });
                ecb.AddComponent<UnitTargetPosition>(instance);
                Debug.LogError($"Instatiated!!!");
            }
            
            ecb.RemoveComponent<SquadSpawnTag>(squad.Item3);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        
    }
    
    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
       
    }

    public void OnStopRunning(ref SystemState state)
    {
        // Debug.LogError($"OnStopRunning");
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
