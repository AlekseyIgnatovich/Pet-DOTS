using Unity.Collections;
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
        
        LocalTransform squadTransform = default;
        foreach (var (squadData, localTransform) in SystemAPI.Query<RefRO<SquadData>, RefRO<LocalTransform>>())
        {
            squadTransform = localTransform.ValueRO;
        }

        foreach (var (transform, target, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitTargetPosition>>().WithEntityAccess())
        {
            float3 current = transform.ValueRO.Position;
            float3 goal = target.ValueRO.Value;
            float3 dir = math.normalize(goal - current);
            float dist = math.distance(goal, current);

            
            // translation.Value = math.rotate(Rotation, translation.Value);
            if (dist > 0.1f)
            {
                transform.ValueRW.Rotation = squadTransform.Rotation;
                transform.ValueRW.Position += dir * speed * deltaTime;
            }
        }
    }
}
