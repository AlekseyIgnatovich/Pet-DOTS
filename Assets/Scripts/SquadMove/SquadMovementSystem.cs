using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SquadMovementSystem : ISystem
{
    const float rotationSpeed = 30f;
    const float moveSpeed = 1f;
    
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (input, squadTransform) in SystemAPI.Query<RefRO<SquadMoveInput>, RefRW<LocalTransform>>())
        {
            if (input.ValueRO.Move == 0 && input.ValueRO.Rotation == 0)
                continue;

            var moveDir = squadTransform.ValueRO.Forward() * input.ValueRO.Move;
            squadTransform.ValueRW.Position += moveDir * moveSpeed * dt;
            
            float angle = math.radians(rotationSpeed * input.ValueRO.Rotation * dt);
            quaternion deltaRotation = quaternion.AxisAngle(math.up(), angle);

            var source = squadTransform.ValueRO.Rotation.value;
            if (source.x == 0 && source.y == 0 && source.z == 0 && source.w == 0)
            {
                squadTransform.ValueRW.Rotation = quaternion.identity;
            }
            
            squadTransform.ValueRW.Rotation = math.mul(deltaRotation, squadTransform.ValueRO.Rotation);
        }
    }
}