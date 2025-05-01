using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct FormationMoveSystem : ISystem
{
    EntityQuery squadQuery;
    private EntityQuery unitsQuery;

    private ComponentTypeHandle<FormationUnit> _formationUnitHandle;
    private ComponentTypeHandle<TargetPosition> _targetPositionHandle;

    public void OnCreate(ref SystemState state)
    {
        squadQuery = state.GetEntityQuery(ComponentType.ReadOnly<SquadData>(), ComponentType.ReadOnly<LocalTransform>());
        unitsQuery = state.GetEntityQuery(ComponentType.ReadOnly<FormationUnit>(), ComponentType.ReadOnly<TargetPosition>());

        _formationUnitHandle = state.GetComponentTypeHandle<FormationUnit>(true);
        _targetPositionHandle = state.GetComponentTypeHandle<TargetPosition>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (squadQuery.IsEmptyIgnoreFilter) return;

        var squad = squadQuery.ToEntityArray(Allocator.Temp)[0];
        var squadTransform = state.EntityManager.GetComponentData<LocalTransform>(squad);
        var squadData = state.EntityManager.GetComponentData<SquadData>(squad);

        _formationUnitHandle.Update(ref state);
        _targetPositionHandle.Update(ref state);

        var job = new ProcessUnitsJob
        {
            FormationUnitType = _formationUnitHandle,
            TargetPositionType = _targetPositionHandle,
            DeltaTime = SystemAPI.Time.DeltaTime,
            UnitsCount = unitsQuery.CalculateEntityCount(),
            SquadTransform = squadTransform,
            SquadRowsCount = squadData.RowCount,
        };

        state.Dependency = job.ScheduleParallel(unitsQuery, state.Dependency);
    }

    //[BurstCompile]
    private partial struct ProcessUnitsJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<FormationUnit> FormationUnitType;
        public ComponentTypeHandle<TargetPosition> TargetPositionType;
        public float DeltaTime;
        public int UnitsCount;
        public LocalTransform SquadTransform;
        public int SquadRowsCount;

        const float spacing = 2f;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var formationUnits = chunk.GetNativeArray(ref FormationUnitType);
            var targetPositions = chunk.GetNativeArray(ref TargetPositionType);
            for (int i = 0; i < chunk.Count; i++)
            {
                var unit = formationUnits[i];
                TargetPosition targetPos = targetPositions[i];

                int row = i / SquadRowsCount;
                int col = i % SquadRowsCount;

                float3 offset = new float3((col - SquadRowsCount / 2f) * spacing, 0, -row * spacing);

                targetPos.Value = SquadTransform.Position + offset;
                targetPositions[i] = targetPos;
            }
        }
    }
}