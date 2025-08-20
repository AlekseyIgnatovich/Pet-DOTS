using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ProjectileMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Projectile>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (projectile, transform) in SystemAPI.Query<RefRO<Projectile>, RefRW<LocalTransform>>())
        {
            transform.ValueRW.Position += projectile.ValueRO.Direction * projectile.ValueRO.Speed * deltaTime;
        }
    }
}