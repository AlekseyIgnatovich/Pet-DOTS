using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var prefab in SystemAPI.Query<RefRW<SpawnerData>>())
        {
            if(prefab.ValueRW.SpawnCount <= 0)
                continue;
            
            var instance = ecb.Instantiate(prefab.ValueRO.Prefab);
            var random = Random.CreateFromIndex((uint) state.GlobalSystemVersion);
            var offset = math.float3(random.NextFloat(-5,5), 0.5f, random.NextFloat(-5,5));
            ecb.AddComponent<LocalTransform>(instance);
            ecb.SetComponent(instance, LocalTransform.FromPosition(offset));
            ecb.AddComponent<MobComponent>(instance);

            ecb.AddComponent<RandomDirectionComponent>(instance,
                new RandomDirectionComponent()
                    { direction = math.float3(random.NextFloat(-1, 1), 0, random.NextFloat(-1, 1)) });
            
            prefab.ValueRW.SpawnCount--;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
