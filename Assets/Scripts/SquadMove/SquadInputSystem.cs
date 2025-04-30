using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public struct SquadMoveInput : IComponentData
{
    public float2 MoveInput;
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
        var move = float2.zero;

        if (Input.GetKey(KeyCode.W)) move.y += 1f;
        if (Input.GetKey(KeyCode.S)) move.y -= 1f;
        if (Input.GetKey(KeyCode.D)) move.x -= 1f;
        if (Input.GetKey(KeyCode.A)) move.x += 1f;

        move = math.normalize(move) * math.clamp(math.length(move), 0f, 1f);

        foreach (var input in SystemAPI.Query<RefRW<SquadMoveInput>>())
        {
            input.ValueRW.MoveInput = move;
        }
    }
}