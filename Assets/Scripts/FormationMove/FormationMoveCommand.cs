using Unity.Entities;
using Unity.Mathematics;

public struct FormationMoveCommand : IComponentData, IEnableableComponent
{
    public float3 Target;
    public int RowCount; // Например, 3 или 5 — ширина построения
}