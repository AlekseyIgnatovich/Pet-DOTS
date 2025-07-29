using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public struct SquadMoveInput : IComponentData
{
    public float Move;
    public float Rotation;
    public float Distance;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SquadInputSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SquadMoveInput>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var move = 0f;
        var rotation = 0f;
        var distance = 0f;
        
        if (Input.GetKey(KeyCode.W)) move += 1f;
        if (Input.GetKey(KeyCode.S)) move -= 1f;
        if (Input.GetKey(KeyCode.D)) rotation += 1f;
        if (Input.GetKey(KeyCode.A)) rotation -= 1f;
        if (Input.GetKey(KeyCode.Q)) distance -= 1f;
        if (Input.GetKey(KeyCode.Z)) distance += 1f;

        foreach (var input in SystemAPI.Query<RefRW<SquadMoveInput>>())
        {
            input.ValueRW.Move = move;
            input.ValueRW.Rotation = rotation;
            input.ValueRW.Distance = distance;
        }
    }
}