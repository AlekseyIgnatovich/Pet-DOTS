using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct CameraSystem : ISystem
{
    
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CameraTag,RenderCamera>())
        {
            // Управление через WASD
            float3 moveDir = float3.zero;
            if (Input.GetKey(KeyCode.W))
                moveDir += math.forward();

            if (Input.GetKey(KeyCode.S))
                moveDir += math.back();

            if (Input.GetKey(KeyCode.A))
                moveDir += math.left();

            if (Input.GetKey(KeyCode.D))
                moveDir += math.right();
            
            if (Input.GetKey(KeyCode.Q))
                moveDir += math.up();
            
            if (Input.GetKey(KeyCode.E))
                moveDir += math.down();

            float speed = 5.0f;
            transform.ValueRW.Position += moveDir * speed * deltaTime;
        }
    }
    
    public class CameraAuthoring : MonoBehaviour
    {
    }

    class CameraBaker : Baker<CameraAuthoring>
    {
        public override void Bake(CameraAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<CameraTag>(entity);
        }
    }

    public struct CameraTag : IComponentData {}
}