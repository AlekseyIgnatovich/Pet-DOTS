using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct UnitSpawnerSystem : ISystem
{
    private static bool inited = false;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitPrefab>();
    }

    public void OnUpdate(ref SystemState state)
    {
        //if (inited)
         //   return;
        
        inited = true;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var unitPrefab in SystemAPI.Query<UnitPrefab>())
        {
            for (int i = 0; i < 10; i++)
            {
                var entity = ecb.Instantiate(unitPrefab.Value);
                ecb.SetComponent(entity, new LocalTransform
                {
                    Position = new Unity.Mathematics.float3(i * 2, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                ecb.AddComponent<MyMarkTag>(entity);
            }

            // Ставим тег, чтобы не повторять спавн каждый кадр (или удаляем сущность с этим компонентом)
            state.Enabled = false;
        }
    }
}

public struct MyMarkTag : IComponentData {}