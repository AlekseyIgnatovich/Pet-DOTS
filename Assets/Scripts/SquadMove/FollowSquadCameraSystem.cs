using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SquadCameraTarget : IComponentData {} // добавляется на сущность-отряд
public struct CameraFollowTag : IComponentData {} // добавляется на камеру (запечённую!)

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct FollowSquadCameraSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float3 targetPosition = float3.zero;
        float3 targetForward = math.forward();
        bool hasTarget = false;

        foreach (var (transform, input) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<SquadMoveInput>>()
                     .WithAll<SquadCameraTarget>())
        {
            targetPosition = transform.ValueRO.Position;
            float2 move = input.ValueRO.MoveInput;
            if (!move.Equals(float2.zero))
            {
                targetForward = math.normalize(new float3(move.x, 0, move.y));
                hasTarget = true;
            }

            break;
        }

        //ToDo: поправить так чтобы проверка была не нужна
        var broken = float.IsNaN(targetForward.x) || float.IsNaN(targetForward.y) || float.IsNaN(targetForward.z);
        if (broken)
        {
            return;
        }

        if (!hasTarget) return;

        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CameraFollowTag>())
        {
            var camOffset = -targetForward * 10f + new float3(0, 5, 0);
            var desiredPosition = targetPosition + camOffset;
            transform.ValueRW.Position = math.lerp(transform.ValueRW.Position, desiredPosition, SystemAPI.Time.DeltaTime * 5f);
            transform.ValueRW.Rotation = quaternion.LookRotationSafe(targetPosition - transform.ValueRW.Position, math.up());
        }
    }
}