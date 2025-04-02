using Unity.Burst;
using Unity.Entities;

using Unity. Mathematics;
using Unity. Transforms;
using UnityEngine;

partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI. Time.DeltaTime;
        foreach (var (transform, playerSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerSpeedData>>())
        {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");
            
            
            float speed = playerSpeed.ValueRO.Value;
            Debug.LogError($"hInput: {hInput}, vInput: {vInput} speed: {speed}");

            transform.ValueRW.Position += new float3(hInput, 0f, vInput) * deltaTime * speed;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
