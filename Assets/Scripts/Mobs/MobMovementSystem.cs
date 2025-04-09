using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct MobMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RandomDirectionComponent>();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var mob in SystemAPI.Query<RefRO<RandomDirectionComponent>, RefRW<LocalTransform>>())
        {
            mob.Item2.ValueRW.Position += mob.Item1.ValueRO.direction * deltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
