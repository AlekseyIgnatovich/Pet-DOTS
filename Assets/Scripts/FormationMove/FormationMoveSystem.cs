using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct FormationMoveSystem : ISystem
{
    EntityQuery commandQuery;

    public void OnCreate(ref SystemState state)
    {
        commandQuery = state.GetEntityQuery(ComponentType.ReadOnly<FormationMoveCommand>());
    }

    public void OnUpdate(ref SystemState state)
    {
        if (commandQuery.IsEmptyIgnoreFilter) return;
        
        var entities = commandQuery.ToEntityArray(Allocator.Temp);
        var commandEntity = entities[0];
        var command = state.EntityManager.GetComponentData<FormationMoveCommand>(commandEntity);

        int rowCount = command.RowCount;
        float spacing = 2f;
        float3 origin = command.Target;
        
        // Здесь нужно использовать foreach, но в виде partial system с [foreach]-стилем
        int index = 0;
        foreach (var (unit, target) in SystemAPI.Query<RefRO<FormationUnit>, RefRW<TargetPosition>>())
        {
            int row = index / rowCount;
            int col = index % rowCount;

            float3 offset = new float3((col - rowCount / 2f) * spacing, 0, -row * spacing);
            target.ValueRW.Value = origin + offset;
            index++;
        }

        // Удаляем компонент команды
        state.EntityManager.DestroyEntity(commandEntity);
    }
}