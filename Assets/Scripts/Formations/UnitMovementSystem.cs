using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct UnitMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float speed = 5f;

        foreach (var (transform, target, entity) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<TargetPosition>>().WithEntityAccess())
        {
            float3 current = transform.ValueRO.Position;
            float3 goal = target.ValueRO.Value;
            float3 dir = math.normalize(goal - current);
            float dist = math.distance(goal, current);

            if (dist > 0.1f)
            {
                transform.ValueRW.Position += dir * speed * deltaTime;
            }
        }
    }
}
