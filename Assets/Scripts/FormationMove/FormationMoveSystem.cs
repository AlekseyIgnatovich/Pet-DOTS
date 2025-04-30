using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct FormationMoveSystem : ISystem
{
    EntityQuery squadQuery;

    public void OnCreate(ref SystemState state)
    {
        squadQuery = state.GetEntityQuery(ComponentType.ReadOnly<SquadData>(), ComponentType.ReadOnly<LocalTransform>());
    }

    public void OnUpdate(ref SystemState state)
    {
        if (squadQuery.IsEmptyIgnoreFilter) return;
        
        var squad = squadQuery.ToEntityArray(Allocator.Temp)[0];

        var squadTransform = state.EntityManager.GetComponentData<LocalTransform>(squad);
        var squadData = state.EntityManager.GetComponentData<SquadData>(squad);

        int rowCount = squadData.RowCount;
        float spacing = 2f;
        
        // Здесь нужно использовать foreach, но в виде partial system с [foreach]-стилем
        int index = 0;
        foreach (var (unit, target) in SystemAPI.Query<RefRO<FormationUnit>, RefRW<TargetPosition>>())
        {
            int row = index / rowCount;
            int col = index % rowCount;

            float3 offset = new float3((col - rowCount / 2f) * spacing, 0, -row * spacing);
            target.ValueRW.Value = squadTransform.Position + offset;
            index++;
        }
    }
}