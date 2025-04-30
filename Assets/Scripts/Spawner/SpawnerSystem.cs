using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct SpawnerSystem : ISystem, ISystemStartStop
{
    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerData>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var squad = ecb.CreateEntity();
        ecb.AddComponent<SquadData>(squad);
        ecb.SetComponent(squad, new SquadData(){MoveSpeed = 2, RotationSpeed = 4, RowCount = 4 });
        ecb.AddComponent<SquadMoveInput>(squad);
        ecb.AddComponent<LocalTransform>(squad);
        ecb.AddComponent<LocalToWorld>(squad);
        ecb.AddComponent<SquadCameraTarget>(squad);
        
        ecb.AddComponent(squad, new DebugSphere()
        {
            Radius = 1,
            Color = Color.red,
        });
        
        foreach (var prefab in SystemAPI.Query<RefRW<SpawnerData>>())
        {
            for (int i = 0; i < prefab.ValueRW.SpawnCount; i++)
            {
                var instance = ecb.Instantiate(prefab.ValueRO.Prefab);
                ecb.AddComponent<LocalTransform>(instance);
                ecb.SetComponent(instance, LocalTransform.FromPosition(float3.zero));
                ecb.AddComponent<MobComponent>(instance);
                ecb.AddComponent<FormationUnit>(instance, new FormationUnit() { Index = prefab.ValueRW.SpawnCount });
                ecb.AddComponent<TargetPosition>(instance);
            }
            
            prefab.ValueRW.SpawnCount--;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    public void OnStopRunning(ref SystemState state)
    {
        Debug.Log($"OnStopRunning");
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
