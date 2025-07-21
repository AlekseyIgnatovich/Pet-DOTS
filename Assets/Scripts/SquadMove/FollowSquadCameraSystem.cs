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

        foreach (var (transform, squadData) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<SquadData>>()
                     .WithAll<SquadCameraTarget>())
        {
            targetPosition = transform.ValueRO.Position;
            targetForward = transform.ValueRO.Forward();

        }

        //ToDo: поправить так чтобы проверка была не нужна
        var broken = float.IsNaN(targetForward.x) || float.IsNaN(targetForward.y) || float.IsNaN(targetForward.z);
        if (broken)
        {
            return;
        }

        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CameraFollowTag>())
        {
            var camOffset = -targetForward * 20f + new float3(0, 10, 0);
            var desiredPosition = targetPosition + camOffset;
            transform.ValueRW.Position = math.lerp(transform.ValueRW.Position, desiredPosition, SystemAPI.Time.DeltaTime * 5f);
            transform.ValueRW.Rotation = quaternion.LookRotationSafe(targetPosition - transform.ValueRW.Position, math.up());
        }
    }
}