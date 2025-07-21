using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct CalculateUnitInFormationSystem : ISystem
{
    private EntityQuery squadQuery;
    private EntityQuery unitsQuery;

    private ComponentTypeHandle<FormationUnit> _formationUnitHandle;
    private ComponentTypeHandle<UnitTargetPosition> _targetPositionHandle;

    public void OnCreate(ref SystemState state)
    {
        squadQuery = state.GetEntityQuery(ComponentType.ReadOnly<SquadData>(), ComponentType.ReadOnly<LocalTransform>());
        unitsQuery = state.GetEntityQuery(ComponentType.ReadOnly<FormationUnit>(), ComponentType.ReadOnly<UnitTargetPosition>());

        _formationUnitHandle = state.GetComponentTypeHandle<FormationUnit>(true);
        _targetPositionHandle = state.GetComponentTypeHandle<UnitTargetPosition>();
    }

    [BurstCompile(CompileSynchronously = true)]//отключил журнал для джоба
    public void OnUpdate(ref SystemState state)
    {
        if (squadQuery.IsEmptyIgnoreFilter) return;

        var squad = squadQuery.ToEntityArray(Allocator.Temp)[0];
        var squadTransform = state.EntityManager.GetComponentData<LocalTransform>(squad);
        var squadData = state.EntityManager.GetComponentData<SquadData>(squad);

        _formationUnitHandle.Update(ref state);
        _targetPositionHandle.Update(ref state);

        var unitsInRow = unitsQuery.CalculateEntityCount() / squadData.RowCount + 1;
        
        var job = new ProcessUnitsJob
        {
            FormationUnitType = _formationUnitHandle,
            TargetPositionType = _targetPositionHandle,
            UnitsInRow = unitsInRow,
            SquadTransform = squadTransform,
        };

        state.Dependency = job.ScheduleParallel(unitsQuery, state.Dependency);
    }

    [BurstCompile]
    private partial struct ProcessUnitsJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<FormationUnit> FormationUnitType;
        public ComponentTypeHandle<UnitTargetPosition> TargetPositionType;
        public int UnitsCount;
        public LocalTransform SquadTransform;

        public int UnitsInRow;

        const float spacing = 2f;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var formationUnits = chunk.GetNativeArray(ref FormationUnitType);
            var targetPositions = chunk.GetNativeArray(ref TargetPositionType);
            for (int i = 0; i < chunk.Count; i++)
            {
                var unit = formationUnits[i];
                UnitTargetPosition unitTargetPos = targetPositions[i];

                int row = i / UnitsInRow;
                int col = i % UnitsInRow;

                float3 offset = new float3((col - UnitsInRow / 2f) * spacing, 0, -row * spacing);
                
                float3 rotatedVector = math.rotate(SquadTransform.Rotation, offset);

                unitTargetPos.Value = SquadTransform.Position + rotatedVector;
                targetPositions[i] = unitTargetPos;
            }
        }
    }
}