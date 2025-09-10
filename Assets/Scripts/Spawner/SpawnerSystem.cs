using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct SpawnerSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SquadSpawnTag>();
        state.RequireForUpdate<SpawnerData>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        SpawnerData spawnerData = default;
        foreach (var item in SystemAPI.Query<RefRO<SpawnerData>>())
        {
            spawnerData = item.ValueRO;
        }
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        int spawnerIndex = 0;
        foreach (var squad in SystemAPI.Query<RefRO<SquadSpawnTag>, RefRO<SquadData>, RefRO<TeamComp>>().WithEntityAccess())
        {
            for (int i = 0; i < squad.Item2.ValueRO.StartUnitsCount; i++)
            {
                var instance = ecb.Instantiate(spawnerData.UnitPrefab);
                ecb.AddComponent<LocalTransform>(instance);
                ecb.SetComponent(instance, LocalTransform.FromPosition(float3.zero));
                spawnerIndex++;
                ecb.AddComponent(instance, new FormationUnit() { Index = i });
                ecb.AddComponent<UnitTargetPosition>(instance);
                ecb.AddComponent<TeamComp>(instance);
                ecb.SetComponent(instance, new TeamComp() { PlayerId = squad.Item3.ValueRO.PlayerId });
            }
            
            ecb.RemoveComponent<SquadSpawnTag>(squad.Item4);
        }

        for (int i = 0; i < 10; i++)
        {
            var instance = ecb.Instantiate(spawnerData.ShootTargetPrefab);

            ecb.AddComponent<LocalTransform>(instance);
            ecb.AddComponent<TargetTag>(instance);

            float3 pos = new float3(
                UnityEngine.Random.Range(-5f, 5f),
                0f,
                UnityEngine.Random.Range(-5f, 5f)
            );
            ecb.SetComponent(instance, LocalTransform.FromPosition(pos));
            
            ecb.AddComponent<Health>(instance);
            ecb.SetComponent<Health>(instance, new Health(){Value = 30});
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
