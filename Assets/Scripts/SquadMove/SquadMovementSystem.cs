using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SquadMovementSystem : ISystem
{
    const float rotationSpeed = 1.5f;
    const float moveSpeed = 1.5f;
    
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (input, transform) in SystemAPI.Query<RefRO<SquadMoveInput>, RefRW<LocalTransform>>())
        {
            var moveDir = new float3(input.ValueRO.MoveInput.x, 0, input.ValueRO.MoveInput.y);
            if (math.lengthsq(moveDir) > 0.01f)
            {
                // Поворот к направлению
                var targetRot = quaternion.LookRotationSafe(math.normalizesafe(moveDir), math.up());
                transform.ValueRW.Rotation = math.slerp(
                    transform.ValueRW.Rotation,
                    targetRot,
                    rotationSpeed * dt
                );

                // Движение
                var forward = transform.ValueRW.Forward();
                transform.ValueRW.Position += forward * moveSpeed * dt;
            }
        }
    }
}