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
    private ComponentTypeHandle<TeamComp> _unitTeamHandle;

    public void OnCreate(ref SystemState state)
    {
        squadQuery = state.GetEntityQuery(ComponentType.ReadOnly<SquadData>(), ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<TeamComp>());
        unitsQuery = state.GetEntityQuery(ComponentType.ReadOnly<FormationUnit>(), ComponentType.ReadOnly<UnitTargetPosition>(), ComponentType.ReadOnly<TeamComp>());

        _formationUnitHandle = state.GetComponentTypeHandle<FormationUnit>(true);
        _targetPositionHandle = state.GetComponentTypeHandle<UnitTargetPosition>();
        _unitTeamHandle = state.GetComponentTypeHandle<TeamComp>();
    }

    [BurstCompile(CompileSynchronously = true)] //отключил журнал для джоба
    public void OnUpdate(ref SystemState state)
    {
        if (squadQuery.IsEmptyIgnoreFilter) return;
        
        _formationUnitHandle.Update(ref state);
        _targetPositionHandle.Update(ref state);
        _unitTeamHandle.Update(ref state);

        var job = new ProcessUnitsJob
        {
            FormationUnitType = _formationUnitHandle,
            TargetPositionType = _targetPositionHandle,
            UnitTeamComp = _unitTeamHandle,

            SquadsTransform = squadQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob),
            SquadTeamComp = squadQuery.ToComponentDataArray<TeamComp>(Allocator.TempJob),
            SquadsData = squadQuery.ToComponentDataArray<SquadData>(Allocator.TempJob)
        };

        state.Dependency = job.ScheduleParallel(unitsQuery, state.Dependency);
    }

    [BurstCompile]
    private partial struct ProcessUnitsJob : IJobChunk
    {
        const float spacing = 1.2f;

        [ReadOnly] public NativeArray<LocalTransform> SquadsTransform;
        [ReadOnly] public NativeArray<TeamComp> SquadTeamComp;
        [ReadOnly] public NativeArray<SquadData> SquadsData;
        
        [ReadOnly] public ComponentTypeHandle<FormationUnit> FormationUnitType;
        public ComponentTypeHandle<UnitTargetPosition> TargetPositionType;
        public ComponentTypeHandle<TeamComp> UnitTeamComp;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var formationUnits = chunk.GetNativeArray(ref FormationUnitType);
            var targetPositions = chunk.GetNativeArray(ref TargetPositionType);
            var unitTeam = chunk.GetNativeArray(ref UnitTeamComp);

            int unitsInRow = 0;
            
            for (int i = 0; i < chunk.Count; i++)
            {
                LocalTransform squadTransform = default;
                for (int playerIndex = 0; playerIndex < SquadTeamComp.Length; playerIndex++)
                {
                    if (SquadTeamComp[playerIndex].PlayerId == unitTeam[i].PlayerId)
                    {
                        squadTransform = SquadsTransform[playerIndex];
                        unitsInRow = SquadsData[playerIndex].StartUnitsCount / SquadsData[playerIndex].RowCount + 1;
                    }
                }

                var index = formationUnits[i].Index;
                UnitTargetPosition unitTargetPos = targetPositions[i];
                
                int row = index / unitsInRow;
                int col = index % unitsInRow;
                
                float3 offset = new float3((col - unitsInRow / 2f) * spacing, 0, -row * spacing);
                float3 rotatedVector = math.rotate(squadTransform.Rotation, offset);
                
                unitTargetPos.Value = squadTransform.Position + rotatedVector;
                targetPositions[i] = unitTargetPos;
            }
        }
    }
}